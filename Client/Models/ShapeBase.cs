using System;
using System.Windows;
using System.Windows.Media;
using Client.Enums;
using Newtonsoft.Json;

namespace Client.Models
{
    public abstract class ShapeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public abstract BasicShapeType shapeType { get; }
        

    }
}