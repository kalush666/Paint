using System;
using System.Collections.Generic;
using System.Linq;
using Client.Convertors;
using Client.Models;
using Client.UIModels;

namespace Client.Factories
{
    public class UIShapeFactory
    {
        private readonly List<IUiShapeConvertor> _convertors;

        public UIShapeFactory()
        {
            _convertors = typeof(IUiShapeConvertor).Assembly
                .GetTypes()
                .Where(t => typeof(IUiShapeConvertor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IUiShapeConvertor)Activator.CreateInstance(t)!)
                .ToList();
        }

        public UIBaseShape? Create(ShapeBase shape) =>
            _convertors.FirstOrDefault(c => c.CanConvert(shape))?.Convert(shape);
    }
}