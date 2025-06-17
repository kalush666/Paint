using System;
using System.Collections.Generic;
using System.Linq;
using Server.Handlers;

namespace Server.Factories
{
    public interface IHandlerFactory
    {
        IRequestHandler? getHandler(string request);
        void RegisterHandler<T>() where T : IRequestHandler, new();
        void RegisterHandler(IRequestHandler handler);
    }

    public class HandlerFactory : IHandlerFactory
    {
        private readonly List<IRequestHandler> _handlers = new();

        public HandlerFactory()
        {
            var handlerTypes = typeof(IRequestHandler).Assembly
                .GetTypes()
                .Where(t => typeof(IRequestHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in handlerTypes)
            {
                var handler = (IRequestHandler)Activator.CreateInstance(type)!;
                RegisterHandler(handler);
            }
        }

        public IRequestHandler getHandler(string request)
            => _handlers.FirstOrDefault(h => h.CanHandle(request));


        public void RegisterHandler<T>() where T : IRequestHandler, new()
            => _handlers.Add(new T());

        public void RegisterHandler(IRequestHandler handler)
            => _handlers.Add(handler);
    }
}