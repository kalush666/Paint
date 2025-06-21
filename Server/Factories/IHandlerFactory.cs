using System;
using Server.Handlers;

namespace Server.Factories
{
    public interface IHandlerFactory
    {
        IRequestHandler? GetHandler(string request);
    }
    
}