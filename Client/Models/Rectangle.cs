using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Models
{
    class Rectangle : ShapeBase
    {
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }
        public override string shapeType => "Rectangle";
        public override UIElement ToUI(Brush color,double strokeThikness)
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
                StrokeThickness = strokeThikness
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }


        public Rectangle()
        {
        }

        public Rectangle(Position startPosition, Position endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}
