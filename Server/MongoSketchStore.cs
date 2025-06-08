using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared_Models.Models;
using PainterServer.Utils;

namespace Server
{
    public class MongoSketchStore
    {
        private readonly IMongoCollection<Sketch> _collection;

        public MongoSketchStore() {
            var client = new MongoClient(MongoConfig.ConnectionString);
            var database = client.GetDatabase(MongoConfig.DatabaseName);
            _collection = database.GetCollection<Sketch>(MongoConfig.CollectionName);
        }

        public async Task<Sketch?> GetSketchByNameAsync(string name)
        {
            return await _collection.Find(s => s.Name == name).FirstOrDefaultAsync();
        }

        public async Task InsetSketchAsync(Sketch sketch) {
            if (sketch == null || string.IsNullOrWhiteSpace(sketch.Name)) throw new ArgumentException("invalid Sketch name");

            var existing = await GetSketchByNameAsync(sketch.Name);
            if (existing != null) throw new InvalidOperationException($"Sketch with the name {sketch.Name} already exists");

            await _collection.InsertOneAsync(sketch);
            Console.WriteLine($"Sketch {sketch.Name} insert to MongoDB");
        }


        public async Task DeleteSketchAsync(string name) {
            var found = await GetSketchByNameAsync(name);
            if (found == null) throw new ArgumentException("invalid Sketch name");

            var result = await _collection.DeleteOneAsync(s => s.Name == name);
            Console.WriteLine(result.DeletedCount > 0
                ? $"Sketch {name} deleted from db"
                : $"error while deleting {name} from db");
        }
    }
}
