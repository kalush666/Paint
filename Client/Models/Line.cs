﻿using Common.Enums;
using Common.Models;

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
            Start = start;
            End = end;
        }

    }
}