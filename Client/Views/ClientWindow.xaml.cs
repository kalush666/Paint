#nullable enable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Client.Convertors;
using Client.Enums;
using Client.Factories;
using Client.Helpers;
using Client.Models;
using Client.Services;
using Common.Errors;
using Common.Events;

namespace Client.Views
{
    public partial class ClientWindow : Window
    {
        private BasicShapeType _currentShape = BasicShapeType.None;
        private Position _startPoint;
        private Sketch _currentSketch = new Sketch();
        public event EventHandler<String>? ShapeAdded;
        private Brush _currentColor = Brushes.Black;
        private double _currentStrokeThikness = 2;
        private readonly ClientCommunicationService _communicationService;
        private UIElement? _previewShape = null;
        private bool isDrawing = false;

        public ClientWindow()
        {
            InitializeComponent();
            ShapeAdded += OnShapeAdded;
            _communicationService = new ClientCommunicationService();
            _currentSketch.Name = "Untitled Sketch";

            this.Closing += OnClientWindowClosing;
        }

        private void OnClientWindowClosing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_currentSketch.Name))
            {
                LockHub.TriggerUnlock(_currentSketch.Name);
            }
        }

        private static void OnShapeAdded(object sender, string type) => Console.WriteLine($"shape added :{type}");

        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentShape == BasicShapeType.None) return;
            isDrawing = true;
            _startPoint = new Position(e.GetPosition(Canvas));
            _previewShape = ShapeToUiElementConvertor.ConvertToUiElement(
                ShapeFactory.Create(_currentShape, _startPoint, _startPoint));
            if (_previewShape != null)
                Canvas.Children.Add(_previewShape);
        }
        
        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || _currentShape == BasicShapeType.None || _previewShape == null) return;
            var currentPoint = new Position(e.GetPosition(Canvas));
            var shape = ShapeFactory.Create(_currentShape, _startPoint, currentPoint);
            if (shape == null) return;
            shape.SetColor(_currentColor);
            shape.StrokeThikness = _currentStrokeThikness;
            CanvasGeometryHelper.EnsureFitsCanvas(Canvas.ActualWidth, Canvas.ActualHeight, shape);

            Canvas.Children.Remove(_previewShape);
            _previewShape = ShapeToUiElementConvertor.ConvertToUiElement(shape);
            if (_previewShape != null)
                Canvas.Children.Add(_previewShape);
        }

        
        private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing || _currentShape == BasicShapeType.None) return;
            isDrawing = false;
            var endPoint = new Position(e.GetPosition(Canvas));
            var shape = ShapeFactory.Create(_currentShape, _startPoint, endPoint);
            if (shape == null) return;
            shape.SetColor(_currentColor);
            shape.StrokeThikness = _currentStrokeThikness;
            CanvasGeometryHelper.EnsureFitsCanvas(Canvas.ActualWidth, Canvas.ActualHeight, shape);

            _currentSketch.addShape(shape);
            ShapeAdded?.Invoke(this, _currentShape.ToString());

            if (_previewShape != null)
                Canvas.Children.Remove(_previewShape);
            _previewShape = null;
            var uiElement = ShapeToUiElementConvertor.ConvertToUiElement(shape);
            Canvas.Children.Add(uiElement);
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentShape = BasicShapeType.Line;
            UpdateButtonSelection("Line");
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentShape = BasicShapeType.Rectangle;
            UpdateButtonSelection("Rectangle");
        }

        private void CircleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentShape = BasicShapeType.Circle;
            UpdateButtonSelection("Circle");
        }

        private async void UploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentSketch.Shapes.Count == 0)
            {
                MessageBox.Show("No shapes to upload. Please draw something first.", "Nothing to Upload");
                return;
            }

            var sketchName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter sketch name:", "Upload Sketch", _currentSketch.Name);

            if (string.IsNullOrWhiteSpace(sketchName)) return;

            try
            {
                _currentSketch.Name = sketchName;

                var result = await _communicationService.UploadSketchAsync(_currentSketch);
                MessageBox.Show(result, "Upload Result");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Upload failed: {AppErrors.Mongo.UploadError}", "Error");
            }
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            _currentSketch.clear();
        }

        private async void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedImport ="";
            var importWindow = new ImportSelectionWindow();
            if (importWindow.ShowDialog() == true && !string.IsNullOrWhiteSpace(importWindow.SelectedSketch))
            {
                 selectedImport = importWindow.SelectedSketch;
                
            }

            if (string.IsNullOrEmpty(selectedImport))
            {
                return;
            }
            try
            {
                var importedSketch = await _communicationService.DownloadSketchAsync(selectedImport);
                if (importedSketch == null) return;

                Canvas.Children.Clear();

                LockHub.TriggerUnlock(_currentSketch.Name);
                _currentSketch = importedSketch;
                LockHub.TriggerLock(_currentSketch.Name);

                foreach (var shape in _currentSketch.Shapes)
                {
                    CanvasGeometryHelper.EnsureFitsCanvas(Canvas.ActualWidth, Canvas.ActualHeight, shape);

                    shape.Color = shape.GetBrushFromName(shape.ColorName);

                    var uiElement = ShapeToUiElementConvertor.ConvertToUiElement(shape);
                    Canvas.Children.Add(uiElement);
                }

                MessageBox.Show(
                    $"Sketch '{_currentSketch.Name}' imported successfully with {_currentSketch.Shapes.Count} shapes!",
                    "Import Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {AppErrors.Mongo.ReadError}", "Error");
            }
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow
            {
                ColorPicker =
                {
                    SelectedIndex = GetColorIndex(_currentColor)
                },
                StrokeSlider =
                {
                    Value = _currentStrokeThikness
                }
            };

            if (optionsWindow.ShowDialog() != true) return;
            _currentColor = optionsWindow.SelectedColor;
            _currentStrokeThikness = optionsWindow.SelectedThickness;

            MessageBox.Show(
                $"Settings updated:\nColor: {GetColorName(_currentColor)}\nStroke Thickness: {_currentStrokeThikness}",
                "Settings Applied");
        }

        private void UpdateButtonSelection(string selectedShape)
        {
            LineButton.FontWeight = FontWeights.Normal;
            RectangleButton.FontWeight = FontWeights.Normal;
            CircleButton.FontWeight = FontWeights.Normal;

            switch (selectedShape)
            {
                case "Line":
                    LineButton.FontWeight = FontWeights.Bold;
                    break;
                case "Rectangle":
                    RectangleButton.FontWeight = FontWeights.Bold;
                    break;
                case "Circle":
                    CircleButton.FontWeight = FontWeights.Bold;
                    break;
            }
        }

        private int GetColorIndex(Brush color)
        {
            return color switch
            {
                var c when c == Brushes.Black => 0,
                var c when c == Brushes.Red => 1,
                var c when c == Brushes.Green => 2,
                var c when c == Brushes.Blue => 3,
                _ => 0
            };
        }

        private string GetColorName(Brush color)
        {
            return color switch
            {
                var c when c == Brushes.Black => "Black",
                var c when c == Brushes.Red => "Red",
                var c when c == Brushes.Green => "Green",
                var c when c == Brushes.Blue => "Blue",
                _ => "Black"
            };
        }
    }
}