using System;
using System.Windows;
using Client.Models;

namespace Client.UIModels
{
    public class UIRectangle : UIBaseShape
    {
        public UIRectangle(Rectangle rect) : base(rect) { }

        public override UIElement Render()
        {
            var rect = (Rectangle)LogicShape;
            var x = Math.Min(rect.StartPosition.X, rect.EndPosition.X);
            var y = Math.Min(rect.StartPosition.Y, rect.EndPosition.Y);

            return new System.Windows.Shapes.Rectangle
            {
                Width = rect.Width,
                Height = rect.Height,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Margin = new Thickness(x, y, 0, 0)
            };
        }

        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            var rect = (Rectangle)LogicShape;
            rect.StartPosition = Clamp(rect.StartPosition, canvasWidth, canvasHeight);
            rect.EndPosition = Clamp(rect.EndPosition, canvasWidth, canvasHeight);
        }

    }
}