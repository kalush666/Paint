using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
    public class SketchDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("Name")]
        public string SketchName { get; set; }
        
        [BsonElement("Shapes")]
        public List<ShapeBase> Shapes { get; set; }
    }
}