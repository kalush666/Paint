using System.Windows;
using System.Windows.Shapes;
using Client.Models;

namespace Client.UIModels
{
    public class UICircle : UIBaseShape
    {
        public UICircle(Circle circle) : base(circle){}

        public override UIElement Render()
        {
            var circle = (Circle)LogicShape;
            return new Ellipse
            {
                Width = circle.Radius * 2,
                Height = circle.Radius * 2,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Margin = new Thickness(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, 0, 0)
            };
        }
    }
}