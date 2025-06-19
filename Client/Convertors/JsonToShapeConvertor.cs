using System;
using Client.Models;
using Newtonsoft.Json.Linq;
using Client.Enums;
using Common.Enums;
using Common.Models;

namespace Client.Convertors
{
    public static class JsonToShapeConvertor
    {
        public static ShapeBase? ConvertToShape(JObject json)
        {
            BasicShapeType typeToken = json["shapeType"].ToObject<BasicShapeType>();


            return typeToken switch
            { 
                BasicShapeType.Line => json.ToObject<Line>(),
                BasicShapeType.Rectangle => json.ToObject<Rectangle>(),
                BasicShapeType.Circle => json.ToObject<Circle>(),
                _ => null
            };
        }
    }
}