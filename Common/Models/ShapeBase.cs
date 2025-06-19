using System;
using Common.Enums;

namespace Common.Models
{
    public abstract class ShapeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public abstract BasicShapeType shapeType { get; }
        
    }
}