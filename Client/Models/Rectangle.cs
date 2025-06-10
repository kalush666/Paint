using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Client.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    class Rectangle : ShapeBase
    {
        [JsonProperty]
        public Position StartPosition { get; set; }

        [JsonProperty]
        public Position EndPosition { get; set; }

        [JsonProperty]
        public override string shapeType => "Rectangle";

        public Rectangle() { }

        public Rectangle(Position startPosition, Position endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public override UIElement ToUI(Brush color, double strokeThickness)
        {
            double x = Math.Min(StartPosition.X, EndPosition.X);
            double y = Math.Min(StartPosition.Y, EndPosition.Y);
            double width = Math.Abs(EndPosition.X - StartPosition.X);
            double height = Math.Abs(EndPosition.Y - StartPosition.Y);

            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Stroke = color,
                StrokeThickness = strokeThickness
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }
    }
}