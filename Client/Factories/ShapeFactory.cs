using System;
using Client.Models;
using Common.Enums;
using Common.Errors;
using Common.Models;

namespace Client.Factories
{
    public static class ShapeFactory
    {
        public static ShapeBase Create(BasicShapeType type, Position startPoint, Position endPoint)
        {
            switch (type)
            {
                case BasicShapeType.Line:
                    return new Line(startPoint, endPoint);
                case BasicShapeType.Rectangle:
                    return new Rectangle(startPoint, endPoint);
                case BasicShapeType.Circle:
                    return new Circle { StartPosition = startPoint, EndPosition = endPoint };
                default:
                    throw new NotSupportedException(AppErrors.Shapes.NotAShape);
            }
        }
    }
}