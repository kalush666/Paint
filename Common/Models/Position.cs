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
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public double Distance(Position p1)
        {
            return Math.Sqrt(Math.Pow(X - p1.X, 2) + Math.Pow(Y - p1.Y, 2));
        }

    }
}