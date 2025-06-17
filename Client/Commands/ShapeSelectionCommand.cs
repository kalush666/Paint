using System;
using System.Windows.Input;
using Client.Enums;
using Client.Handlers;

namespace Client.Commands
{
    public class ShapeSelectionCommand : DrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly BasicShapeType _shapeType;
        private readonly Action<string> _updateUI;

        public ShapeSelectionCommand(DrawingHandler handler, BasicShapeType shapeType, Action<string> updateUi)
        {
            _handler = handler;
            _shapeType = shapeType;
            _updateUI = updateUi;
        }
        
        public override void Execute()
        {
            _handler.SetCurrentShape(_shapeType);
            _updateUI.Invoke(_shapeType.ToString());
        }
    }
}