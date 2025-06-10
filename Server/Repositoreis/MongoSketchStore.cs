using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PainterServer.Utils;

namespace Server
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

            string name = doc["Name"].AsString;
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                throw new InvalidOperationException($"Sketch with the name {name} already exists");

            await _collection.InsertOneAsync(doc);
            Console.WriteLine($"Sketch {name} inserted to MongoDB");
        }

        public async Task DeleteSketchAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var result = await _collection.DeleteOneAsync(filter);
            Console.WriteLine(result.DeletedCount > 0
                ? $"Sketch {name} deleted from db"
                : $"Error while deleting {name} from db");
        }

        public async Task<List<string>> GetAllJsonAsync()
        {
            var documents = await _collection.Find(new BsonDocument()).ToListAsync();
            return documents.Select(doc => doc.ToJson()).ToList();
        }
    }
}
