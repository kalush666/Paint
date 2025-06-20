using System.Drawing;

namespace Common.DTO
{
    public class ShapeDto
    {
        public string Id { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ShapeType {get; set; }
    }
}