using System.Windows;
using Client.Helpers;
using Client.Models;

namespace Client.UIModels
{
    public class UILine : UiBaseShape
    {
        public UILine(Line shape) : base(shape){}

        public override UIElement Render()
        {
            var line = (Line)LogicShape;

            return new System.Windows.Shapes.Line
            {
                X1 = line.Start.X,
                Y1 = line.Start.Y,
                X2 = line.End.X,
                Y2 = line.End.Y,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };
        }
        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            var line = (Line)LogicShape;
            line.Start = GeometryHelper.Clamp(line.Start, canvasWidth, canvasHeight);
            line.End = GeometryHelper.Clamp(line.End, canvasWidth, canvasHeight);
        }

    }
}