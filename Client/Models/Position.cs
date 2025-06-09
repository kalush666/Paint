using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Models.Models
{
    public struct Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
