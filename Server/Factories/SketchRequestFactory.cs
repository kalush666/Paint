using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Handlers;

namespace Server.Factories
{
    public class SketchRequestFactory : IHandlerFactory
    {
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

        public IRequestHandler? GetHandler(string request)
        {
            return (from handlerType in _handlerTypes
                    select handlerType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static)
                    into getInstanceMethod
                    where getInstanceMethod != null
                    select getInstanceMethod.Invoke(null, null)).OfType<IRequestHandler>()
                .FirstOrDefault(handler => handler.CanHandle(request));
        }
    }
}