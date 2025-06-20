using System.Collections.Generic;
using Common.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models
{
    public class SketchDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("Name")]
        public string SketchName { get; set; }
        
        [BsonElement("Shapes")]
        public List<ShapeDto> Shapes { get; set; }
    }
}