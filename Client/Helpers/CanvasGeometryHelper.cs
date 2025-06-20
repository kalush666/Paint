using System;
using Client.Models;
using Common.Errors;
using Common.Models;

namespace Client.Helpers
{
    public static class CanvasGeometryHelper
    {
        private static Position EnsureFitsPosition(Position p, double maxWidth, double maxHeight)
        {
            return new Position(Math.Max(0, Math.Min(p.X, maxWidth)),
                Math.Max(0, Math.Min(p.Y, maxHeight)));
        }

        public static void EnsureFitsCanvas(double canvasWidth, double canvasHeight, ShapeBase shape)
        {
            switch (shape)
            {
                case Line line:
                    line.Start = EnsureFitsPosition(line.Start, canvasWidth, canvasHeight);
                    line.End = EnsureFitsPosition(line.End, canvasWidth, canvasHeight);
                    break;

                case Rectangle rectangle:
                    rectangle.StartPosition = EnsureFitsPosition(rectangle.StartPosition, canvasWidth, canvasHeight);
                    rectangle.EndPosition = EnsureFitsPosition(rectangle.EndPosition, canvasWidth, canvasHeight);
                    break;

                case Circle circle:
                    circle.StartPosition = EnsureFitsPosition(circle.StartPosition, canvasWidth, canvasHeight);
                    circle.EndPosition = EnsureFitsPosition(circle.EndPosition, canvasWidth, canvasHeight);
                    break;

                default:
                    throw new NotSupportedException(AppErrors.Shapes.NotAShape);
            }
        }
    }
}