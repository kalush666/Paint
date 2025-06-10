using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Client.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    class Line : ShapeBase
    {
        [JsonProperty]
        public Position Start { get; set; }

        [JsonProperty]
        public Position End { get; set; }

        [JsonProperty]
        public override string shapeType => "Line";

        public Line() { }

        public Line(Position start, Position end)
        {
            this.Start = start;
            this.End = end;
        }

        public override UIElement ToUI(Brush color, double strokeThickness)
        {
            return new System.Windows.Shapes.Line
            {
                X1 = Start.X,
                Y1 = Start.Y,
                X2 = End.X,
                Y2 = End.Y,
                Stroke = color,
                StrokeThickness = strokeThickness
            };
        }

        public override void EnsureFitsCanvas(double canvasWidth, double canvasHeight)
        {
            this.Start = ensureFitsPosition(this.Start, canvasWidth, canvasHeight);
            this.End = ensureFitsPosition(this.End, canvasWidth, canvasHeight);
        }
    }
}