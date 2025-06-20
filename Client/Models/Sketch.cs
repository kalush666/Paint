using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Client.Models
{
    public class Sketch
    {
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id { get; set; }
        
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("Shapes")]
        public List<ShapeBase> Shapes { get; set; } = new List<ShapeBase>();

        public Sketch() { }

        public Sketch(string name, List<ShapeBase> shapes) 
        {
            Name = name;
            Shapes = shapes ?? new List<ShapeBase>();
        }

        public Sketch(Guid id, string name, List<ShapeBase> shapes) 
        {
            Id = id;
            Name = name;
            Shapes = shapes ?? new List<ShapeBase>();
        }

        public void AddShape(ShapeBase shape) 
        {
            Shapes.Add(shape);
        }

        public void Clear() 
        {
            Shapes.Clear();
        }
    }
}