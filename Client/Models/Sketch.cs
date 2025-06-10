using System.Collections.Generic;

namespace Client.Models
{
    public class Sketch
    {
        public string Name { get; set; }
        public List<ShapeBase> Shapes { get; set; } = new List<ShapeBase>();

        public Sketch() { }

        public Sketch(string name,List<ShapeBase> shapes) {
            this.Name = name;
            this.Shapes = shapes ?? new List<ShapeBase>();
        }

        public void addShape(ShapeBase shape) {
            this.Shapes.Add(shape);
        }

        public void clear() {
            this.Shapes.Clear();
        }
    }
}
