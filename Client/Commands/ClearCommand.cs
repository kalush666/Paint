using System;
using Client.Enums;
using Client.Handlers;

namespace Client.Commands
{
    public class ClearCommand : IDrawingCommand
    {
        private readonly DrawingHandler? _handler;


        public ClearCommand(DrawingHandler handler) => _handler = handler;

        public Enum Key => CommandTypes.Clear;

        public void Execute() =>
            _handler?.Clear();
    }
}