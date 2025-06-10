using Client.Models;
using Common.Utils;
using Newtonsoft.Json.Linq;

namespace Client.Convertors
{
    public static class JsonToShapeConvertor
    {
        public static ShapeBase? ConvertToShape(JObject json)
        {
            var typeToken = json["Type"] ?? json["type"] ?? json["shapeType"];
            if (typeToken == null)
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