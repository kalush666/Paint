using System.Windows;
using Client.Handlers;
using Client.Services;
using Client.Views;
using Common.Errors;
using Common.Events;

namespace Client.Commands
{
    public class ImportCommand : DrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly ClientCommunicationService _service;

        public ImportCommand(DrawingHandler handler, ClientCommunicationService service)
        {
            _handler = handler;
            _service = service;
        }

        public override async void Execute()
        {
            var importWindow = new ImportSelectionWindow();
            if (importWindow.ShowDialog() != true || string.IsNullOrWhiteSpace(importWindow.SelectedSketch)) return;
            
            var response = await _service.DownloadSketchAsync(importWindow.SelectedSketch);
            if(response is not { Error: null })
            {
                MessageBox.Show(AppErrors.Mongo.ReadError, "Import Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LockHub.TriggerUnlock(_handler.CurrentSketch.Name);
            _handler.ImportSketch(response.Value.Value);
            LockHub.TriggerLock(_handler.CurrentSketch.Name);
            MessageBox.Show("Import Success", "Import Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}