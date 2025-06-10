using System;
using Client.Enums;

namespace Client.Models
{
    public class Circle : ShapeBase
    {
        public Position Center { get; set; }

        public double Radius { get; set; }

        public override BasicShapeType shapeType => BasicShapeType.Circle;

        public Circle() { }

        public Circle(Position center, double radius)
        {
            Center = center;
            Radius = radius;
        }

    }
}