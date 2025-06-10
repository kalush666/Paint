using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Enums;
using Newtonsoft.Json;

namespace Client.Models
{
    public class Rectangle : ShapeBase
    {
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }

        public override BasicShapeType shapeType => BasicShapeType.Rectangle;

        public Rectangle() { }

        public Rectangle(Position startPosition, Position endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

    }
}