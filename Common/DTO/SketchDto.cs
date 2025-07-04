using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Common.DTO
{
    public class SketchDto
    {
        [BsonId]
        [JsonConverter(typeof(Convertors.ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }

        [JsonProperty("Name")] public string Name { get; set; } = string.Empty;

        [JsonProperty("Shapes")] public List<ShapeDto> Shapes { get; set; } = new List<ShapeDto>();

        public SketchDto()
        {
        }

        public SketchDto(string name, List<ShapeDto> shapes)
        {
            Name = name;
            Shapes = shapes ?? new List<ShapeDto>();
        }

        public SketchDto(ObjectId id, string name, List<ShapeDto> shapes)
        {
            Id = id;
            Name = name;
            Shapes = shapes ?? new List<ShapeDto>();
        }
    }
}