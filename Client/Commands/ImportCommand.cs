using System;
using System.Data;
using System.Windows;
using Client.Enums;
using Client.Handlers;
using Client.Services;
using Client.Views;
using Client.Views.Service_Windows;
using Common.Errors;
using Common.Events;

namespace Client.Commands
{
    public class ImportCommand : IDrawingCommand
    {
        private readonly DrawingHandler _handler;
        private readonly ClientCommunicationService _service;

        public ImportCommand(DrawingHandler handler, ClientCommunicationService service)
        {
            _handler = handler;
            _service = service;
        }

        public Enum Key => CommandTypes.Import;

        public async void Execute()
        {
            var importWindow = new ImportSelectionWindow();
            if (importWindow.ShowDialog() != true || string.IsNullOrWhiteSpace(importWindow.SelectedSketch)) return;
            try
            {
                var response = await _service.DownloadSketchAsync(importWindow.SelectedSketch);

                if (response is not { Error: null })
                {
                    MessageBox.Show(AppErrors.Mongo.ReadError, "Import Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (response.Value == null)
                {
                    MessageBox.Show(AppErrors.File.AccessDenied, "Import Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                LockHub.TriggerUnlock(_handler.CurrentSketch?.Name ?? string.Empty);
                _handler.ImportSketch(response.Value);
                LockHub.TriggerLock(_handler.CurrentSketch?.Name ?? string.Empty);

                MessageBox.Show("Import Success", "Import Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppErrors.File.Locked, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
