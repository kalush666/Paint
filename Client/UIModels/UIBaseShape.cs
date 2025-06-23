
using System;
using System.Windows;
using System.Windows.Media;
using Client.Models;
using Common.Models;

namespace Client.UIModels
{
    public abstract class UiBaseShape
    {
        public Brush StrokeColor { get; set; } = Brushes.Black;
        public double StrokeThickness { get; set; } = 2;
        protected ShapeBase LogicShape { get; }

        protected UiBaseShape(ShapeBase logicShape)
        {
            LogicShape = logicShape;
        }
        
        
        protected Position Clamp(Position p, double maxWidth, double maxHeight)
        {
            return new Position(
                Math.Max(0, Math.Min(p.X, maxWidth)),
                Math.Max(0, Math.Min(p.Y, maxHeight))
            );
        }

        
        public abstract UIElement Render();
        public abstract void EnsureFitsCanvas(double canvasWidth, double canvasHeight);
    }
}