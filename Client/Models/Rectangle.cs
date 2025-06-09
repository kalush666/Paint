using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;

namespace Shared_Models.Models
{
    class Rectangle : ShapeBase
    {
        public Position TopLeft { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public override string shapeType => "Rectangle";
    }
}
