#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Errors;
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

        public async Task<string?> GetJsonByNameAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            return document?.ToJson();
        }

        public async Task InsertJsonAsync(string json)
        {
            var doc = BsonDocument.Parse(json);
            if (!doc.Contains("Name") || string.IsNullOrWhiteSpace(doc["Name"].AsString))
                throw new ArgumentException("Invalid or missing 'Name' in sketch JSON");

            var name = doc["Name"].AsString;
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                throw new InvalidOperationException(AppErrors.Mongo.AlreadyExists);

            await _collection.InsertOneAsync(doc);
            Console.WriteLine($"Sketch {name} inserted to MongoDB");
            SketchStoreNotifier.NotifyInserted(name);
        }

        public async Task DeleteSketchAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                Console.WriteLine($"sketch {name} deleted from db");
                SketchStoreNotifier.NotifyInserted(name);
            }
            else
            {
                Console.WriteLine(AppErrors.Mongo.DeleteError);
            }
        }

        public async Task<List<string>> GetAllJsonAsync()
        {
            var documents = await _collection.Find(new BsonDocument()).ToListAsync();
            return documents.Select(doc => doc.ToJson()).ToList();
        }
    }
}
