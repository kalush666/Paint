using System.Windows;
using Client.Models;

namespace Client.UIModels
{
    public class UILine : UIBaseShape
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
    }
}