using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shared_Models.Models;
using Shared_Models.Enums;

namespace Server.Patterns
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
                _ => throw new NotSupportedException($"{type} is not a basic type");
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
