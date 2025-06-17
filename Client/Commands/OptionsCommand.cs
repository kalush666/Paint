using System.Windows;
using Client.Handlers;
using Client.Helpers;

namespace Client.Commands
{
    public class OptionsCommand : DrawingCommand
    {
        private readonly DrawingHandler _handler;

        public OptionsCommand(DrawingHandler handler) => _handler = handler;

        public override void Execute()
        {
            var optionsWindow = new OptionsWindow
            {
                ColorPicker = { SelectedIndex = BrushMappingHelper.GetIndex(_handler.CurrentColor) },
                StrokeSlider = { Value = _handler.CurrentStrokeThickness }
            };

            if (optionsWindow.ShowDialog() != true) return;

            _handler.CurrentColor = optionsWindow.SelectedColor;
            _handler.CurrentStrokeThickness = optionsWindow.SelectedThickness;

            MessageBox.Show(
                $"Settings updated:\nColor: {BrushMappingHelper.GetName(_handler.CurrentColor)}\nStroke Thickness: {_handler.CurrentStrokeThickness}",
                "Settings Applied");
        }
    }
}