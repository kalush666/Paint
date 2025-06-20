using System;
using System.ComponentModel;
using System.Windows;
using Client.Enums;
using Client.Factories;
using Client.Handlers;
using Client.Helpers;
using Client.Models;
using Client.Services;
using Common.Enums;
using Common.Events;

namespace Client.Views.Main_Window
{
    public partial class ClientWindow : Window
    {
        private const string DefaultSketchName = "Untitled Sketch";

        private readonly DrawingHandler _drawingHandler;
        private readonly IDrawingCommandFactory _commandFactory;
        private readonly ShapeSelectionHighlighter _shapeHighlighter;

        public ClientWindow()
        {
            InitializeComponent();

            var sketch = new Sketch { Name = DefaultSketchName };
            var communicationService = new ClientCommunicationService();

            _drawingHandler = new DrawingHandler(Canvas, sketch);
            _drawingHandler.ShapeAdded += (_, type) => Console.WriteLine($"Shape added: {type}");

            _shapeHighlighter = new ShapeSelectionHighlighter(ShapesPanel);

            var serviceProvider = new CommandServiceProvider();
            serviceProvider.RegisterService(_drawingHandler);
            serviceProvider.RegisterService(_shapeHighlighter);
            serviceProvider.RegisterService(communicationService);

            _commandFactory = new DrawingCommandFactory(serviceProvider);

            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_drawingHandler.CurrentSketch.Name))
            {
                LockHub.TriggerUnlock(_drawingHandler.CurrentSketch.Name);
            }
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            _shapeHighlighter.Highlight(BasicShapeType.Line);
            _commandFactory.Create(BasicShapeType.Line)?.Execute();
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _shapeHighlighter.Highlight(BasicShapeType.Rectangle);
            _commandFactory.Create(BasicShapeType.Rectangle)?.Execute();
        }

        private void CircleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _shapeHighlighter.Highlight(BasicShapeType.Circle);
            _commandFactory.Create(BasicShapeType.Circle)?.Execute();
        }

        private void UploadButton_OnClick(object sender, RoutedEventArgs e) => _commandFactory.Create(CommandTypes.Upload)?.Execute();
        private void ClearButton_OnClick(object sender, RoutedEventArgs e) => _commandFactory.Create(CommandTypes.Clear)?.Execute();
        private void ImportButton_OnClick(object sender, RoutedEventArgs e) => _commandFactory.Create(CommandTypes.Import)?.Execute();
        private void OptionsButton_OnClick(object sender, RoutedEventArgs e) => _commandFactory.Create(CommandTypes.Options)?.Execute();
    }
}