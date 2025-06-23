using Client.Models;
using Client.UIModels;

namespace Client.Convertors
{
    public interface IUiShapeConvertor
    {
        bool CanConvert(ShapeBase shape);
        UiBaseShape Convert(ShapeBase shape);
    }
}