using System;
using Common.Models;

namespace Client.Helpers
{
    public static class GeometryHelper
    {
        public static Position Clamp(Position position, double canvasWidth, double canvasHeight)
        {
            return new Position(
                Math.Max(0, Math.Min(position.X, canvasWidth)),
                Math.Max(0, Math.Min(position.Y, canvasHeight))
            );
        }
    }
}