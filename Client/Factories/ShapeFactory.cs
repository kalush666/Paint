using System;
using System.Windows;
using Client.Enums;
using Client.Models;
using Common.Errors;
using Common.Utils;
using Newtonsoft.Json.Linq;
using Shared_Models.Models;

namespace Client.Factories
{
    public static class ShapeFactory
    {
        public static ShapeBase Create(BasicShapeType type)
        {
            return type switch
            {
                BasicShapeType.Line => new Line(),
                BasicShapeType.Rectangle => new Rectangle(),
                BasicShapeType.Circle => new Circle(),
                _ => throw new NotSupportedException(AppErrors.Shapes.NotAShape)
            };
        }

        public static ShapeBase Create(BasicShapeType type, Position startPoint, Position endPoint)
        {
            return type switch
            {
                BasicShapeType.Line => new Line(startPoint,endPoint),
                BasicShapeType.Rectangle => new Rectangle(startPoint,endPoint),
                BasicShapeType.Circle => new Circle(startPoint.MidPosition(endPoint),startPoint.Distance(endPoint)),
                _ => throw new NotSupportedException(AppErrors.Shapes.NotAShape)
            };
    }
    }
}
