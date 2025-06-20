using System;
using Common.Enums;
using Common.Models;

namespace Client.Models
{
    public class Circle : ShapeBase
    {
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }

        public override BasicShapeType shapeType => BasicShapeType.Circle;

        // radius is half the smaller side of the rectangle
        public double Radius
            => Math.Min(Math.Abs(EndPosition.X - StartPosition.X),
                Math.Abs(EndPosition.Y - StartPosition.Y)) / 2;

        public Position Center
            => new Position(
                (StartPosition.X + EndPosition.X) / 2,
                (StartPosition.Y + EndPosition.Y) / 2
            );

        public Circle()
        {
        }

       
    }
}