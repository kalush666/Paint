using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared_Models.Models;
using Shared_Models.Enums;

namespace Server.Patterns
{
    public static class ShapeFactory
    {
        public static ShapeBase Create(BasicShapeType type) { 
            return type switch
            {
                BasicShapeType.Line => new Line(),
                BasicShapeType.Rectangle => new Rectangle(),
                BasicShapeType.Circle => new Circle(),
                _ => throw new NotSupportedException($"{type} is not a basic type");
            }
        }


    }
}
