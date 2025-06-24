#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Constants;
using Common.DTO;
using Common.Errors;
using Common.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Server.Config;
using Server.Events;
using Server.Enums;
using Server.Models;
using Server.Mappers;

namespace Server.Repositories
{
    public class MongoSketchStore
    {
        private static MongoSketchStore? _instance;
        private static readonly object _lock = new();
        private readonly IMongoCollection<ServerSketch> _collection;
        private readonly SketchEventBus<SketchEvent> _eventBus;

        public static MongoSketchStore GetInstance(SketchEventBus<SketchEvent> eventBus)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new MongoSketchStore(eventBus);
                }
            }

            return _instance;
        }

        private MongoSketchStore(SketchEventBus<SketchEvent> eventBus)
        {
            _eventBus = eventBus;
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            var client = new MongoClient(MongoConfig.ConnectionString);
            var database = client.GetDatabase(MongoConfig.DatabaseName);
            _collection = database.GetCollection<ServerSketch>(MongoConfig.CollectionName);
        }

        public async Task<Result<SketchDto>> GetByNameAsync(string name)
        {
            var filter = Builders<ServerSketch>.Filter.Eq(d => d.Name, name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();

            if (document == null)
                return Result<SketchDto>.Failure(AppErrors.Mongo.SketchNotFound);


            return Result<SketchDto>.Success(document.ToDto());
        }

        public async Task<Result<string>> InsertSketchAsync(SketchDto sketchDto)
        {
            if (string.IsNullOrWhiteSpace(sketchDto.Name))
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);

            var serverSketch = sketchDto.ToServer();

            if (serverSketch.Shapes == null || serverSketch.Shapes.Count == 0)
            {
                return Result<string>.Failure("Sketch must contain at least one shape.");
            }

            var filter = Builders<ServerSketch>.Filter.Eq(d => d.Name, serverSketch.Name);
            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
                return Result<string>.Failure(AppErrors.Mongo.AlreadyExists);

            try
            {
                if (serverSketch.Shapes.Any(s =>
                        s.StartPosition.X == 0 && s.StartPosition.Y == 0 && s.EndPosition.X == 0 &&
                        s.EndPosition.Y == 0))
                {
                    return Result<string>.Failure("Shapes must have valid start and end positions.");
                }

                await _collection.InsertOneAsync(serverSketch);
                await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Inserted, serverSketch.Name));
                return Result<string>.Success($"{serverSketch.Name} inserted successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure("Exception during MongoDB insert: " + ex.Message);
            }
        }

        public async Task<Result<string>> DeleteSketchByIdAsync(ObjectId id)
        {
            var filter = Builders<ServerSketch>.Filter.Eq(s => s.Id, id);
            var sketch = await _collection.Find(filter).FirstOrDefaultAsync();

            if (sketch == null)
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);

            var result = await _collection.DeleteOneAsync(filter);
            if (result.DeletedCount == 0)
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);

            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Deleted, sketch.Name));
            return Result<string>.Success($"Sketch {id} deleted successfully.");
        }

        public async Task<Result<List<SketchDto>>> GetAllSketchesAsync()
        {
            try
            {
                var documents = await _collection.Find(_ => true).ToListAsync();
                var dtos = documents.Select(d => d.ToDto()).ToList();
                return Result<List<SketchDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[MongoSketchStore] Exception during fetch all: " + ex.Message);
                return Result<List<SketchDto>>.Failure(AppErrors.Mongo.ReadError);
            }
        }

        public async Task<Result<IEnumerable<string>>> GetAllSketchNamesAsync()
        {
            try
            {
                var projection = Builders<ServerSketch>.Projection.Include(s => s.Name);
                var cursor = await _collection.Find(_ => true).Project(projection).ToCursorAsync();

                var names = cursor
                    .ToEnumerable()
                    .Select(doc => doc.GetValue(nameof(ServerSketch.Name), "").AsString)
                    .Where(n => !string.IsNullOrWhiteSpace(n));

                return Result<IEnumerable<string>>.Success(names);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[MongoSketchStore] Exception during name fetch: " + ex.Message);
                return Result<IEnumerable<string>>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}