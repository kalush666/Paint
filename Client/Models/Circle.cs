using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shared_Models.Models;

namespace Client.Models
{
    class Circle : ShapeBase
    {
        public Position Center { get; set; }
        public double Radius { get; set; }
        public override string shapeType => "Circle";

        public Circle()
        {
        }

        public Circle(Position center, double radius)
        {
            Center = center;
            Radius = radius;
        }
        public override UIElement ToUI(Brush color,double strokeThikness)
        {
            var ellipse = new System.Windows.Shapes.Ellipse
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Stroke = color,
                StrokeThickness = strokeThikness
            };

            Canvas.SetLeft(ellipse, Center.X - Radius);
            Canvas.SetTop(ellipse, Center.Y - Radius);
            return ellipse;
        }

    }
}
