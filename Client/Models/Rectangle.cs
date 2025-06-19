using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Enums;
using Common.Enums;
using Common.Models;
using Newtonsoft.Json;

namespace Client.Models
{
    public class Rectangle : ShapeBase
    {
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        
        public override BasicShapeType shapeType => BasicShapeType.Rectangle;

        public Rectangle() { }

        public Rectangle(Position startPosition, Position endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            Width = Math.Abs(endPosition.X - startPosition.X);
            Height = Math.Abs(endPosition.Y - startPosition.Y);
        }

    }
}