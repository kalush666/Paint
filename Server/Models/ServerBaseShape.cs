using Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models
{
    public class ServerBaseShape
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Position StartPosition { get; set; }

        public Position EndPosition { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int ShapeType { get; set; }
    }
}