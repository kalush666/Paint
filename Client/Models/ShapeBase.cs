using System;
using System.Windows;
using System.Windows.Media;
using Client.Enums;
using Newtonsoft.Json;

namespace Client.Models
{
    public abstract class ShapeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ColorName { get; set; } = "Black";

        public Brush Color { get; set; } = Brushes.Black;

        public double StrokeThikness { get; set; } = 2;

        public abstract BasicShapeType shapeType { get; }


        public Brush GetBrushFromName(string colorName)
        {
            return colorName switch
            {
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                _ => Brushes.Black
            };
        }

        public void SetColor(Brush brush)
        {
            Color = brush;
            ColorName = brush switch
            {
                var b when b == Brushes.Red => "Red",
                var b when b == Brushes.Green => "Green",
                var b when b == Brushes.Blue => "Blue",
                _ => "Black"
            };
        }
        protected Position ensureFitsPosition(Position p, double maxWidth, double maxHeight)
        {
            return new Position(Math.Max(0, Math.Min(p.X, maxWidth)),
                Math.Max(0, Math.Min(p.Y, maxHeight)));
        }

        public abstract void EnsureFitsCanvas(double canvasWidth, double canvasHeight);
    }
}