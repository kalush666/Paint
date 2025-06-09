using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Models.Models
{
    class Line : ShapeBase
    {
        public Position Start { get; set; }
        public Position End { get; set; }
        public override string shapeType => "Line";
    }
}
