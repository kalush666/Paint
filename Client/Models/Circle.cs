using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Shared_Models.Models;

namespace Client.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    class Circle : ShapeBase
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
    }
}