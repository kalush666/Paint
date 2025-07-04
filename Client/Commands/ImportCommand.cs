using System;
using System.Windows;
using Client.Enums;
using Client.Handlers;
using Client.Models;
using Client.Services;
using Client.Views.Service_Windows.Import_Selection_Window;
using Common.Errors;
using Common.Events;
using Common.Helpers;

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
                Result<Sketch> response = await _service.DownloadSketchAsync(importWindow.SelectedSketch);

                if (response is not { Error: null })
                {
                    string errorMessage = response.Error ?? AppErrors.Mongo.ReadError;
                    if (errorMessage.StartsWith("\"") && errorMessage.EndsWith("\""))
                    {
                        errorMessage = errorMessage.Substring(1, errorMessage.Length - 2);
                    }
                    MessageBox.Show(errorMessage, "Import Error",
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
            catch (OperationCanceledException ex)
            {
                MessageBox.Show(AppErrors.Server.Suspended, "Import Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppErrors.File.Locked, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}