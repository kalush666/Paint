using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Server.Models
{
    public class ServerSketch
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("Shapes")] public List<ServerBaseShape> Shapes { get; set; }
    }
}