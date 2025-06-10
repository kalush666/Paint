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

    }
}