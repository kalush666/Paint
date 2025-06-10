using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Shared_Models.Models;

namespace Client.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Circle : ShapeBase
    {
        [JsonProperty]
        public Position Center { get; set; }

        [JsonProperty]
        public double Radius { get; set; }

        [JsonProperty]
        public override string shapeType => "Circle";

        public Circle() { }

        public Circle(Position center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public override UIElement ToUI(Brush color, double strokeThickness)
        {
            var ellipse = new System.Windows.Shapes.Ellipse
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Stroke = color,
                StrokeThickness = strokeThickness
            };

            Canvas.SetLeft(ellipse, Center.X - Radius);
            Canvas.SetTop(ellipse, Center.Y - Radius);
            return ellipse;
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