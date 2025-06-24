using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Handlers;

namespace Server.Factories
{
    public class SketchRequestFactory : ISketchHandlerFactory
    {
        private const string NameOfInstanceMethod = "GetInstance";
        private readonly List<Type> _handlerTypes = new()
        {
            typeof(UploadSketchHandler),
            typeof(GetAllNamesHandler),
            typeof(GetAllSketchesHandler),
            typeof(GetSpecificSketchHandler)
        };

        public SketchRequestFactory()
        {
        }

        public ISketchRequestHandler? GetHandler(string request)
        {
            return (from handlerType in _handlerTypes
                    select handlerType.GetMethod(NameOfInstanceMethod, BindingFlags.Public | BindingFlags.Static)
                    into getInstanceMethod
                    where getInstanceMethod != null
                    select getInstanceMethod.Invoke(null, null)).OfType<ISketchRequestHandler>()
                .FirstOrDefault(handler => handler.CanHandle(request));
        }
    }
}