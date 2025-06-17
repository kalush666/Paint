using Client.Handlers;

namespace Client.Commands
{
    public class ClearCommand : DrawingCommand
    {
        private readonly DrawingHandler _handler;
        public ClearCommand(DrawingHandler handler) => _handler = handler;
        public override string Key => "Clear";
        public override void Execute() => _handler.Clear();
    }
}