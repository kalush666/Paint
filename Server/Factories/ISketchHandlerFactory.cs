using System;
using Server.Handlers;

namespace Server.Factories
{
    public interface ISketchHandlerFactory
    {
        ISketchRequestHandler? GetHandler(string request);
    }
    
}