#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Config;
using Server.Events;
using Server.Enums;
using Server.Models;

namespace Server.Repositories
{
    public class MongoSketchStore
    {
        private readonly IMongoCollection<SketchDocument> _collection;
        private readonly SketchEventBus<SketchEvent> _eventBus;

        public MongoSketchStore(SketchEventBus<SketchEvent> eventBus)
        {
            _eventBus = eventBus;
            var client = new MongoClient(MongoConfig.ConnectionString);
            var database = client.GetDatabase(MongoConfig.DatabaseName);
            _collection = database.GetCollection<SketchDocument>(MongoConfig.CollectionName);
        }


        public async Task<Result<Sketch>> GetByNameAsync(string name)
        {
            var filter = Builders<SketchDocument>.Filter.Eq(d => d.SketchName,name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            if (document == null)
                return Result<Sketch>.Failure(AppErrors.Mongo.SketchNotFound);

            var sketch = new Sketch(document.SketchName,document.Shapes);
            return Result<Sketch>.Success(sketch!);
        }
        
        public async Task<Result<string>> InsertSketchAsync(Sketch sketch)
        {
            if (string.IsNullOrWhiteSpace(sketch.Name))
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);

            var filter = Builders<SketchDocument>.Filter.Eq(d => d.SketchName, sketch.Name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                return Result<string>.Failure(AppErrors.Mongo.AlreadyExists);

            var doc = new SketchDocument
            {
                SketchName = sketch.Name,
                Shapes = sketch.Shapes
            };
            
            await _collection.InsertOneAsync(doc);
            sketch.Id = doc.Id;
            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Inserted, sketch.Name));

            return Result<string>.Success("Sketch inserted successfully.");
        }

        public async Task<Result<string>> DeleteSketchAsync(string name)
        {
            var filter = Builders<SketchDocument>.Filter.Eq(d => d.SketchName,name);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);
            }

            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Deleted, name));
            return Result<string>.Success($"{name} deleted successfully.");
        }

        public async Task<Result<List<Sketch>>> GetAllSketchesAsync()
        {
            try
            {
                var documents = await _collection.Find(_ => true).ToListAsync();
                var sketches = documents.Select(doc => new Sketch(doc.SketchName, doc.Shapes)).ToList();
                return Result<List<Sketch>>.Success(sketches);
            }
            catch (Exception)
            {
                return Result<List<Sketch>>.Failure(AppErrors.Mongo.ReadError);
            }
        }

        public async Task<Result<List<string>>> GetAllSketchNamesAsync()
        {
            try
            {
                var projection = Builders<SketchDocument>.Projection.Include(d => d.SketchName);

                var namesCursor = await _collection
                    .Find(_ => true)
                    .Project(projection)
                    .ToCursorAsync();

                var result = namesCursor
                    .ToEnumerable()
                    .Select(doc => doc.GetValue(SketchFields.Name,"").AsString)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();

                return Result<List<string>>.Success(result);
            }
            catch (Exception)
            {
                return Result<List<string>>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}