using System;
using System.Windows;
using System.Windows.Shapes;
using Client.Models;
using Common.Models;

namespace Client.UIModels
{
    public class UiCircle : UiBaseShape
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
                Margin = new Thickness(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius,
                    0, 0)
            };
        }

        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            if (LogicShape is not Circle circle) return;

            circle.StartPosition = Clamp(circle.StartPosition, canvasWidth, canvasHeight);
            circle.EndPosition = Clamp(circle.EndPosition, canvasWidth, canvasHeight);
        }
        
    }
}