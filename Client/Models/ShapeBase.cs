using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace Client.Models
{
    public abstract class ShapeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Brush Color { get; set; } = Brushes.Black;
        public double StrokeThikness { get; set; }
        public Position Position { get; set; }

        public abstract string shapeType { get; }
        public abstract UIElement ToUI(Brush color,double stokeThikness);
    }
}
