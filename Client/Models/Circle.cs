using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Enums;
using Newtonsoft.Json;

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


        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            var minX = this.Radius;
            var maxX = canvasWidth - this.Radius;
            var minY = this.Radius;
            var maxY = canvasHeight - this.Radius;

            this.Center = new Position(
                Math.Max(minX, Math.Min(this.Center.X, maxX)),
                Math.Max(minY, Math.Min(this.Center.Y, maxY))
            );

            var maxRadius = Math.Min(
                Math.Min(this.Center.X, canvasWidth - this.Center.X),
                Math.Min(this.Center.Y, canvasHeight - this.Center.Y)
            );
            this.Radius = Math.Min(this.Radius, maxRadius);
        }
    }
}