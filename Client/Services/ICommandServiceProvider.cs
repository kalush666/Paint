using System;

namespace Client.Services
{
    public interface ICommandServiceProvider
    {
        T GetService<T>();
        object GetService(Type serviceType);
    }
}