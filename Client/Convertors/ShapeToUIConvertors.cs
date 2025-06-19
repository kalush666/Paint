using Client.Models;
using Client.UIModels;
using Common.Models;

namespace Client.Convertors
{
    public class ShapeToUIConvertors
    {
        public class LineToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Line;

            public UIBaseShape Convert(ShapeBase shape)
            {
                return new UILine((Line)shape);
            }
        }

        public class RectangleToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Rectangle;

            public UIBaseShape Convert(ShapeBase shape)
            {
                return new UIRectangle((Rectangle)shape);
            }
        }

        public class CircleToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Circle;

            public UIBaseShape Convert(ShapeBase shape)
            {
                return new UiCircle((Circle)shape);
            }
        }

    }
}