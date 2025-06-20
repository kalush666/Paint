using Client.UIModels;
using Common.Models;

namespace Client.Convertors
{
    public interface IUiShapeConvertor
    {
        bool CanConvert(ShapeBase shape);
        UIBaseShape Convert(ShapeBase shape);
    }
}