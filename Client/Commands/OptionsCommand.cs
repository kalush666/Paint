using System;
using System.Windows;
using Client.Enums;
using Client.Handlers;
using Client.Helpers;
using Client.Views.Service_Windows;
using Client.Views.Service_Windows.Import_Selection_Window;
using Client.Views.Service_Windows.Options_Window;

namespace Client.Commands
{
    public class OptionsCommand : IDrawingCommand
    {
        private readonly DrawingHandler _handler;

        public OptionsCommand(DrawingHandler handler) => _handler = handler;

        public Enum Key => CommandTypes.Options;

        public void Execute()
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