using Client.Models;
using Client.UIModels;

namespace Client.Convertors
{
    public class ShapeToUIConvertors
    {
        public class LineToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Line;

            public UiBaseShape Convert(ShapeBase shape)
            {
                return new UILine((Line)shape);
            }
        }

        public class RectangleToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Rectangle;

            public UiBaseShape Convert(ShapeBase shape)
            {
                return new UiRectangle((Rectangle)shape);
            }
        }

        public class CircleToUIConverter : IUiShapeConvertor
        {
            public bool CanConvert(ShapeBase shape) => shape is Circle;

            public UiBaseShape Convert(ShapeBase shape)
            {
                return new UiCircle((Circle)shape);
            }
        }

    }
}