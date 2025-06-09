using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Models.Models
{
    class Circle : ShapeBase
    {
        public Position Center { get; set; }
        public double Radius { get; set; }
        public override string shapeType => "Circle";
    }
}
