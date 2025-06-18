using System;
using Client.Enums;
using Client.Handlers;

namespace Client.Commands
{
    public class ShapeSelectionCommand : IDrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly BasicShapeType _shapeType;

        public ShapeSelectionCommand(DrawingHandler handler, BasicShapeType shapeType)
        {
            _handler = handler;
            _shapeType = shapeType;
        }

        public Enum Key => _shapeType;

        public void Execute()
        {
            _handler.SetCurrentShape(_shapeType);
            _handler.HighlightShape(_shapeType);
        }
    }
}