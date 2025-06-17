using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Client.Commands;
using Client.Handlers;
using Client.Models;
using Client.Services;
using Common.Events;

namespace Client.Views
{
    public partial class ClientWindow : Window
    {
        private readonly DrawingHandler _drawingHandler;
        private readonly Dictionary<string, DrawingCommand> _commands;

        public ClientWindow()
        {
            InitializeComponent();
            
            var sketch = new Sketch { Name = "Untitled Sketch" };
            var communicationService = new ClientCommunicationService();
            
            _drawingHandler = new DrawingHandler(Canvas, sketch);
            _drawingHandler.ShapeAdded += (_, type) => Console.WriteLine($"Shape added: {type}");
            
            _commands = new Dictionary<string, DrawingCommand>
            {
                ["Line"] = new ShapeSelectionCommand(_drawingHandler, Enums.BasicShapeType.Line, UpdateButtonSelection),
                ["Rectangle"] = new ShapeSelectionCommand(_drawingHandler, Enums.BasicShapeType.Rectangle, UpdateButtonSelection),
                ["Circle"] = new ShapeSelectionCommand(_drawingHandler, Enums.BasicShapeType.Circle, UpdateButtonSelection),
                ["Upload"] = new UploadCommand(_drawingHandler, communicationService),
                ["Clear"] = new ClearCommand(_drawingHandler),
                ["Import"] = new ImportCommand(_drawingHandler, communicationService),
                ["Options"] = new OptionsCommand(_drawingHandler)
            };

            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_drawingHandler.CurrentSketch.Name))
            {
                LockHub.TriggerUnlock(_drawingHandler.CurrentSketch.Name);
            }
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e) => _commands["Line"].Execute();
        private void RectangleButton_OnClick(object sender, RoutedEventArgs e) => _commands["Rectangle"].Execute();
        private void CircleButton_OnClick(object sender, RoutedEventArgs e) => _commands["Circle"].Execute();
        private void UploadButton_OnClick(object sender, RoutedEventArgs e) => _commands["Upload"].Execute();
        private void ClearButton_OnClick(object sender, RoutedEventArgs e) => _commands["Clear"].Execute();
        private void ImportButton_OnClick(object sender, RoutedEventArgs e) => _commands["Import"].Execute();
        private void OptionsButton_OnClick(object sender, RoutedEventArgs e) => _commands["Options"].Execute();

        private void UpdateButtonSelection(string selectedShape)
        {
            LineButton.FontWeight = FontWeights.Normal;
            RectangleButton.FontWeight = FontWeights.Normal;
            CircleButton.FontWeight = FontWeights.Normal;

            switch (selectedShape)
            {
                case "Line": LineButton.FontWeight = FontWeights.Bold; break;
                case "Rectangle": RectangleButton.FontWeight = FontWeights.Bold; break;
                case "Circle": CircleButton.FontWeight = FontWeights.Bold; break;
            }
        }
    }
}