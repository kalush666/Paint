using System;
using Shared_Models.Models;

namespace Shared_Models.Models
{
    public abstract class ShapeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Color Color { get; set; } = new Color(0, 0, 0);
        public double StrokeThikness { get; set; }
        public Position Position { get; set; }

        public abstract string shapeType { get; }
    }
}
