using System;
using System.Windows;
using Client.Enums;
using Client.Models;
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
                _ => throw new NotSupportedException($"{type} is not a basic type")
            };
        }

        public static ShapeBase Create(BasicShapeType type, Position startPoint, Position endPoint)
        {
            return type switch
            {
                BasicShapeType.Line => new Line(startPoint,endPoint),
                BasicShapeType.Rectangle => new Rectangle(startPoint,endPoint),
                BasicShapeType.Circle => new Circle(startPoint.MidPosition(endPoint),startPoint.Distance(endPoint)),
                _ => throw new NotSupportedException($"{type} is not a basic type")
            };
    }

        public static ShapeBase? createShapeFromString(string typeString)
        {
            if (Enum.TryParse<BasicShapeType>(typeString, true, out var type))
            {
                return Create(type);
            }

            return null;
        }

        public static ShapeBase? createShapeFromJson(JObject json)
        {
            var typeToken = json["Type"] ?? json["type"];
            if (typeToken==null)
            {
                return null;
            }

            var typeString = typeToken.ToString().ToLower();

            return typeString switch
            {
                "line" => json.ToObject<Line>(),
                "rectangle" => json.ToObject<Rectangle>(),
                "circle" => json.ToObject<Circle>(),
                _ => null
            };
        }
    }
}
