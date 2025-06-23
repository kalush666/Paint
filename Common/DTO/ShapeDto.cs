using Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DTO
{
    public class ShapeDto
    {
        [BsonId]
        [Newtonsoft.Json.JsonConverter(typeof(Convertors.ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ShapeType {get; set; }
    }
}