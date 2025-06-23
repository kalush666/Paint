using Client.Models;
using Client.UIModels;
using Common.Enums;
using Common.Models;

namespace Client.Factories
{
    public class UiShapeFactory
    {
        public UiBaseShape? Create(ShapeBase shape)
        {
            return shape switch
            {
                Line line => new UILine(line),
                Rectangle rect => new UIRectangle(rect),
                Circle circle => new UiCircle(circle),
                _ => null
            };
        }

        public UiBaseShape? Create(BasicShapeType type, Position start, Position end)
        {
            return type switch
            {
                BasicShapeType.Line => new UILine(new Line(start, end)),
                BasicShapeType.Rectangle => new UIRectangle(new Rectangle(start, end)),
                BasicShapeType.Circle => new UiCircle(new Circle
                {
                    StartPosition = start,
                    EndPosition = end
                }),
                _ => null
            };
        }
    }
}