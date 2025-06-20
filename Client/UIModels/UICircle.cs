using System;
using System.Windows;
using System.Windows.Shapes;
using Client.Models;

namespace Client.UIModels
{
    public class UiCircle : UIBaseShape
    {
        public UiCircle(Circle circle) : base(circle)
        {
        }

        public override UIElement Render()
        {
            var circle = (Circle)LogicShape;
            double diameter = Math.Max(0, circle.Radius * 2);
            return new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Margin = new Thickness(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, 0, 0)
            };
        }
    }
}