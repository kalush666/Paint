using System.Windows;
using Shared_Models.Models;

namespace Client.Models
{
    class Line : ShapeBase
    {
        public Position Start { get; set; }
        public Position End { get; set; }
        public override string shapeType => "Line";

        public Line() { }


        public Line(Position start, Position end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
