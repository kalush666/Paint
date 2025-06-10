using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Common.Errors;

namespace Client.Utils
{
    public static class CanvasGeometryHelper
    {
        private static Position EnsureFitsPosition(Position p, double maxWidth, double maxHeight)
        {
            return new Position(Math.Max(0, Math.Min(p.X, maxWidth)),
                Math.Max(0, Math.Min(p.Y, maxHeight)));
        }

        public static void EnsureFitsCanvas(double canvasWidth, double canvasHeight,ShapeBase shape)
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
                    var minX = circle.Radius;
                    var maxX = canvasWidth - circle.Radius;
                    var minY = circle.Radius;
                    var maxY = canvasHeight - circle.Radius;

                    circle.Center = new Position(
                        Math.Max(minX, Math.Min(circle.Center.X, maxX)),
                        Math.Max(minY, Math.Min(circle.Center.Y, maxY))
                    );

                    var maxRadius = Math.Min(
                        Math.Min(circle.Center.X, canvasWidth - circle.Center.X),
                        Math.Min(circle.Center.Y, canvasHeight - circle.Center.Y)
                    );
                    circle.Radius = Math.Min(circle.Radius, maxRadius);
                    break;
                default:
                    throw new NotSupportedException(AppErrors.Shapes.NotAShape);
            }
        }
    }
}
