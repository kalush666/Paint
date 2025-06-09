using System;
using System.Windows;
using System.Windows.Input.Manipulations;

namespace Client.Models
{
    public struct Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public Position(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public double Distance(Position p1)
        {
            return Math.Sqrt(Math.Pow(this.X - p1.X, 2) + Math.Pow(this.Y - p1.Y, 2));
        }

        public Position MidPosition(Position p1)
        {
            return new Position((this.X + p1.X) / 2, (this.Y + p1.Y) / 2);
        }
    }
}
