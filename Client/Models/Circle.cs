using Shared_Models.Models;

namespace Client.Models
{
    class Circle : ShapeBase
    {
        public Position Center { get; set; }
        public double Radius { get; set; }
        public override string shapeType => "Circle";
    }
}
