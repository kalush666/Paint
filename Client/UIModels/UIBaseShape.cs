
using System;
using System.Windows;
using System.Windows.Media;
using Client.Models;
using Common.Models;

namespace Client.UIModels
{
    public abstract class UiBaseShape
    {
        private static readonly Brush DefaultColor = Brushes.Black;
        private const double DefaultStrokeThickness = 2;
        
        public Brush StrokeColor { get; set; } = DefaultColor;
        public double StrokeThickness { get; set; } = DefaultStrokeThickness;
        protected ShapeBase LogicShape { get; }

        protected UiBaseShape(ShapeBase logicShape)
        {
            LogicShape = logicShape;
        }
        
      
        
        
        public abstract UIElement Render();
        public abstract void EnsureFitsCanvas(double canvasWidth, double canvasHeight);
    }
}