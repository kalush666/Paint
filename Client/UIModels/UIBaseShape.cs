
using System.Windows;
using System.Windows.Media;
using Client.Models;

namespace Client.UIModels
{
    public abstract class UIBaseShape
    {
        public bool IsSelected { get; set; }
        public Brush StrokeColor { get; set; } = Brushes.Black;
        public double StrokeThickness { get; set; } = 2;
        protected ShapeBase LogicShape { get; }

        protected UIBaseShape(ShapeBase logicShape)
        {
            LogicShape = logicShape;
        }

        public abstract UIElement Render();
        //public abstract void EnsureFitsCanvas(double canvasWidth, double canvasHeight, ShapeBase shape);
    }
}