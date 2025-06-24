using System;
using Client.Commands;

namespace Client.Factories
{
    public interface IDrawingCommandFactory
    {
        IDrawingCommand? Create(Enum key);
    }
}