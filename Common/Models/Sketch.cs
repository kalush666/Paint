using System.Collections.Generic;

namespace Common.Models
{
    public class Sketch
    {

        public string Name { get; set; }
        public List<ShapeBase> Shapes { get; set; } = new List<ShapeBase>();

        public Sketch() { }

        public Sketch(string name,List<ShapeBase> shapes) {
            Name = name;
            Shapes = shapes ?? new List<ShapeBase>();
        }

        public void AddShape(ShapeBase shape) {
            Shapes.Add(shape);
        }

        public void Clear() {
            Shapes.Clear();
        }
    }
}
