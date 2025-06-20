using System;
using System.Collections.Generic;
using System.Linq;
using Client.Convertors;
using Client.UIModels;
using Common.Models;

namespace Client.Factories
{
    public class UiShapeFactory
    {
        private readonly List<IUiShapeConvertor> _convertors;

        public UiShapeFactory()
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