using Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models
{
    public class ServerBaseShape
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("StartPosition")]
        public Position StartPosition { get; set; }

        [BsonElement("EndPosition")]
        public Position EndPosition { get; set; }

        [BsonElement("Width")]
        public double Width { get; set; }

        [BsonElement("Height")]
        public double Height { get; set; }

        [BsonElement("ShapeType")]
        public int ShapeType { get; set; }
    }
}