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
using Server.Events;
using Server.Enums;

namespace Server.Repositories
{
    public class MongoSketchStore
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly SketchEventBus<SketchEvent> _eventBus;

        public MongoSketchStore(SketchEventBus<SketchEvent> eventBus)
        {
            this._eventBus = eventBus;
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

        public async Task<Result<string>> InsertJsonAsync(string json)
        {
            var doc = BsonDocument.Parse(json);
            if (!doc.Contains(SketchFields.Name) || string.IsNullOrWhiteSpace(doc[SketchFields.Name].AsString))
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);

            var name = doc[SketchFields.Name].AsString;
            var filter = Builders<BsonDocument>.Filter.Eq(SketchFields.Name, name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                return Result<string>.Success(AppErrors.Mongo.AlreadyExists);

            await _collection.InsertOneAsync(doc);
            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Inserted, name));
            return Result<string>.Success(doc.ToJson());
        }

        public async Task<Result<string>> DeleteSketchAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(SketchFields.Name, name);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);
            }
            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Deleted, name));
            return Result<string>.Success($"{name} deleted successfully.");
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