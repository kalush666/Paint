using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared_Models.Models;
using PainterServer.Utils;
using Newtonsoft.Json;

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

        public async Task<Sketch?> GetSketchByNameAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            return document != null ? JsonConvert.DeserializeObject<Sketch>(document.ToJson()) : null;
        }

        public async Task InsertSketchAsync(Sketch sketch)
        {
            if (sketch == null || string.IsNullOrWhiteSpace(sketch.Name)) throw new ArgumentException("invalid Sketch name");

            var existing = await GetSketchByNameAsync(sketch.Name);
            if (existing != null) throw new InvalidOperationException($"Sketch with the name {sketch.Name} already exists");

            var json = JsonConvert.SerializeObject(sketch);
            var document = BsonDocument.Parse(json);
            await _collection.InsertOneAsync(document);
            Console.WriteLine($"Sketch {sketch.Name} insert to MongoDB");
        }

        public async Task DeleteSketchAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var result = await _collection.DeleteOneAsync(filter);
            Console.WriteLine(result.DeletedCount > 0
                ? $"Sketch {name} deleted from db"
                : $"error while deleting {name} from db");
        }

        public async Task<List<Sketch>> GetAllAsync()
        {
            var documents = await _collection.Find(new BsonDocument()).ToListAsync();

            return documents.Select(doc => JsonConvert.DeserializeObject<Sketch>(doc.ToJson())).Where(sketch => sketch != null).ToList();
        }
    }
}
