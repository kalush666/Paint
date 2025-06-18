using System;
using System.Collections.Generic;

namespace Client.Services
{
    public class CommandServiceProvider : ICommandServiceProvider
    {
        private readonly Dictionary<Type,object> _services = new();
        public T GetService<T>() => (T)_services[typeof(T)];
        public object GetService(Type serviceType) => _services[serviceType];
        public void RegisterService<T>(T service) => _services[typeof(T)] = service!;
    }
}