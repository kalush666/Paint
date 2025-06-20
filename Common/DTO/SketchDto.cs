using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common.DTO
{
    public class SketchDto
    {
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }

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

        public SketchDto(string id, string name, List<ShapeDto> shapes)
        {
            Id = id;
            Name = name;
            Shapes = shapes ?? new List<ShapeDto>();
        }

        public void AddShape(ShapeDto shape)
        {
            Shapes.Add(shape);
        }

        public void Clear()
        {
            Shapes.Clear();
        }
    }
}