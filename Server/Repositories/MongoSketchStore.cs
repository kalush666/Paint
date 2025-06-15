#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Config;
using Server.Helpers;

namespace Server.Repositories
{
    public class MongoSketchStore
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoSketchStore()
        {
            var client = new MongoClient(MongoConfig.ConnectionString);
            var database = client.GetDatabase(MongoConfig.DatabaseName);
            _collection = database.GetCollection<BsonDocument>(MongoConfig.CollectionName);
        }

        public async Task<Result<string>> GetJsonByNameAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(SketchFields.Name, name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            if (document == null)
            {
                return Result<string>.Failure(AppErrors.Mongo.SketchNotFound);
            }
            return Result<string>.Success(document.ToJson());
        }

        public async Task InsertJsonAsync(string json)
        {
            var doc = BsonDocument.Parse(json);
            if (!doc.Contains("Name") || string.IsNullOrWhiteSpace(doc[SketchFields.Name].AsString))
                throw new ArgumentException(AppErrors.Mongo.InvalidJson);

            var name = doc[SketchFields.Name].AsString;
            var filter = Builders<BsonDocument>.Filter.Eq(SketchFields.Name, name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                throw new InvalidOperationException(AppErrors.Mongo.AlreadyExists);

            await _collection.InsertOneAsync(doc);
            Console.WriteLine($"Sketch {name} inserted to MongoDB");
            SketchStoreNotifier.NotifyInserted(name);
        }

        public async Task DeleteSketchAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(SketchFields.Name, name);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                Console.WriteLine($"sketch {name} deleted from db");
                SketchStoreNotifier.NotifyDeleted(name);
            }
            else
            {
                Console.WriteLine(AppErrors.Mongo.DeleteError);
            }
        }

        public async Task<Result<List<string>>> GetAllJsonAsync()
        {
            try
            {
                var documents = await _collection.Find(new BsonDocument()).ToListAsync();
                var jsonList = documents.Select(doc => doc.ToJson()).ToList();
                return Result<List<string>>.Success(jsonList);
            }
            catch (Exception)
            {
                return Result<List<string>>.Failure(AppErrors.Mongo.ReadError);
            }
        }

        public Task<Result<List<string>>> GetAllSketchNamesAsync()
        {
            try
            {
                var names = _collection.Find(new BsonDocument())
                    .ToEnumerable()
                    .Where(doc => doc.Contains(SketchFields.Name))
                    .Select(doc => doc[SketchFields.Name].AsString)
                    .ToList();
                return Task.FromResult(Result<List<string>>.Success(names));
            }
            catch (Exception)
            {
                return Task.FromResult(Result<List<string>>.Failure(AppErrors.Mongo.ReadError));
            }
        }
    }
}