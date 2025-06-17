using System.Windows;
using Client.Handlers;
using Client.Services;
using Common.Errors;

namespace Client.Commands
{
    public class UploadCommand : DrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly ClientCommunicationService _service;

        public UploadCommand(DrawingHandler handler, ClientCommunicationService service)
        {
            _handler = handler;
            _service = service;
        }

        public override bool CanExecute() => _handler.CurrentSketch.Shapes.Count > 0;

        public override string Key => "Upload";

        public override void Execute()
        {
            if (!CanExecute())
            {
                MessageBox.Show(AppErrors.Shapes.NoShapes, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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