
using System.Windows;
using System.Windows.Media;
using Common.Models;

namespace Client.UIModels
{
    public abstract class UIBaseShape
    {
        public ShapeBase LogicShape { get; }
        public bool IsSelected { get; set; }
        public Brush StrokeColor { get; set; } = Brushes.Black;
        public double StrokeThickness { get; set; } = 2;

        protected UIBaseShape(ShapeBase logicShape)
        {
            LogicShape = logicShape;
        }

        public abstract UIElement Render();
    }
}