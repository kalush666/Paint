using System.Windows;
using System.Windows.Media;
using Client.Enums;
using Newtonsoft.Json;

namespace Client.Models
{
    public class Line : ShapeBase
    {
        public Position Start { get; set; }

        public Position End { get; set; }

        public override BasicShapeType shapeType => BasicShapeType.Line;

        public Line() { }

        public Line(Position start, Position end)
        {
            this.Start = start;
            this.End = end;
        }

        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            this.Start = ensureFitsPosition(this.Start, canvasWidth, canvasHeight);
            this.End = ensureFitsPosition(this.End, canvasWidth, canvasHeight);
        }
    }
}