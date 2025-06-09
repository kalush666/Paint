using System.Windows;
using System.Windows.Media;
using Shared_Models.Models;

namespace Client.Models
{
    class Line : ShapeBase
    {
        public Position Start { get; set; }
        public Position End { get; set; }
        public override string shapeType => "Line";
        public override UIElement ToUI(Brush color,double strokeThikness)
        {
            return new System.Windows.Shapes.Line
            {
                X1 = Start.X,
                Y1 = Start.Y,
                X2 = End.X,
                Y2 = End.Y,
                Stroke = color,
                StrokeThickness = strokeThikness
            };
        }

        public Line() { }


        public Line(Position start, Position end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
