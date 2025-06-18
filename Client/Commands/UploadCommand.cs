using System;
using System.Windows;
using Client.Enums;
using Client.Handlers;
using Client.Services;
using Common.Errors;

namespace Client.Commands
{
    public class UploadCommand : IDrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly ClientCommunicationService _service;

        public UploadCommand(DrawingHandler handler, ClientCommunicationService service)
        {
            _handler = handler;
            _service = service;
        }

        public bool CanExecute(string key) => _handler.CurrentSketch.Shapes.Count > 0 && key.Equals(Key);

        public Enum Key => CommandTypes.Upload;

        public void Execute()
        {
            var sketchName =
                Microsoft.VisualBasic.Interaction.InputBox("Enter sketch name:", "Upload Sketch", "My Sketch");

            if (string.IsNullOrWhiteSpace(sketchName)) return;

            _handler.CurrentSketch.Name = sketchName;
            var response = _service.UploadSketchAsync(_handler.CurrentSketch);
            if (response.Result.Error != null)
            {
                MessageBox.Show(response.Result.Error, "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(response.Result.Value, "Upload Result", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}