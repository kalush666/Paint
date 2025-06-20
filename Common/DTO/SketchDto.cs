using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Common.DTO
{
    public class SketchDto
    {
        [BsonIgnoreIfNull]
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id { get; set; }

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

        public SketchDto(Guid id, string name, List<ShapeDto> shapes)
        {
            Id = id;
            Name = name;
            Shapes = shapes ?? new List<ShapeDto>();
        }
    }
}