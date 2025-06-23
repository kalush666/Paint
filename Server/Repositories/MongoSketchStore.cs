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

namespace Server.Repositories
{
    public class MongoSketchStore
    {
        private static MongoSketchStore? _instance;


        private readonly IMongoCollection<SketchDto> _collection;
        private readonly SketchEventBus<SketchEvent> _eventBus;

        public static MongoSketchStore Instance =>
            _instance ?? throw new InvalidOperationException("MongoSketchStore is not initialized.");

        public static void Initialize(SketchEventBus<SketchEvent> eventBus)
        {
            _instance ??= new MongoSketchStore(eventBus);
        }

        private MongoSketchStore(SketchEventBus<SketchEvent> eventBus)
        {
            _eventBus = eventBus;
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            var client = new MongoClient(MongoConfig.ConnectionString);
            var database = client.GetDatabase(MongoConfig.DatabaseName);
            _collection = database.GetCollection<SketchDto>(MongoConfig.CollectionName);
        }

        public async Task<Result<SketchDto>> GetByNameAsync(string name)
        {
            var filter = Builders<SketchDto>.Filter.Eq(d => d.Name, name);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            if (document == null)
                return Result<SketchDto>.Failure(AppErrors.Mongo.SketchNotFound);
            Console.WriteLine(
                $"[MongoSketchStore.Get] Retrieved - Name: {document.Name}, Shapes: {document.Shapes?.Count}");

            return Result<SketchDto>.Success(document);
        }

        public async Task<Result<string>> InsertSketchAsync(SketchDto sketchDto)
        {
            if (string.IsNullOrWhiteSpace(sketchDto.Name))
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);

            var filter = Builders<SketchDto>.Filter.Eq(d => d.Name, sketchDto.Name);

            var exists = await _collection.Find(filter).AnyAsync();
            if (exists)
            {
                return Result<string>.Failure(AppErrors.Mongo.AlreadyExists);
            }

            try
            {
                await _collection.InsertOneAsync(sketchDto);

                await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Inserted, sketchDto.Name));
                return Result<string>.Success($"{sketchDto.Name} inserted successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure("Exception during MongoDB insert: " + ex.Message);
            }
        }
        

        public async Task<Result<string>> DeleteSketchByIdAsync(ObjectId id)
        {
            var filter = Builders<SketchDto>.Filter.Eq("_id",id);
            var sketch = await _collection.Find(filter).FirstOrDefaultAsync();

            if (sketch == null)
            {
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);
            }

            var deleteFilter = Builders<SketchDto>.Filter.Eq(d => d.Id, id);
            var result = await _collection.DeleteOneAsync(deleteFilter);

            if (result.DeletedCount == 0)
            {
                return Result<string>.Failure(AppErrors.Mongo.DeleteError);
            }

            await _eventBus.PublishAsync(new SketchEvent(SketchEventType.Deleted, sketch.Name));
            return Result<string>.Success($"sketch:{id} was deleted successfully.");
        }

        public async Task<Result<List<SketchDto>>> GetAllSketchesAsync()
        {
            try
            {
                var documents = await _collection.Find(_ => true).ToListAsync();
                return Result<List<SketchDto>>.Success(documents);
            }
            catch (Exception)
            {
                return Result<List<SketchDto>>.Failure(AppErrors.Mongo.ReadError);
            }
        }

        public async Task<Result<IEnumerable<string>>> GetAllSketchNamesAsync()
        {
            try
            {
                var projection = Builders<SketchDto>.Projection.Include(d => d.Name);

                var namesCursor = await _collection
                    .Find(_ => true)
                    .Project(projection)
                    .ToCursorAsync();

                var result = namesCursor
                    .ToEnumerable()
                    .Select(doc => doc.GetValue(nameof(SketchDto.Name), "").AsString)
                    .Where(name => !string.IsNullOrWhiteSpace(name));

                return Result<IEnumerable<string>>.Success(result);
            }
            catch (Exception)
            {
                return Result<IEnumerable<string>>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}