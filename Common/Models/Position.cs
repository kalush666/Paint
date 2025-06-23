using System;
using System.Drawing;

namespace Common.Models
{
    public struct Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public double Distance(Position p1)
        {
            return Math.Sqrt(Math.Pow(this.X - p1.X, 2) + Math.Pow(this.Y - p1.Y, 2));
        }

    }
}