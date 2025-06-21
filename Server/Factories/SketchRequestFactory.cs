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
        private readonly List<IRequestHandler> _handlers = new()
        {
            UploadSketchHandler.Instance,
            GetAllNamesHandler.Instance,
            GetAllSketchesHandler.Instance,
            GetSpecificSketchHandler.Instance
        };

        public SketchRequestFactory()
        {
        }

        public IRequestHandler? GetHandler(string request)
        {
            return _handlers.FirstOrDefault(h => h.CanHandle(request));
        }
    }
}