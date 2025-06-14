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
        private BasicShapeType currentShape = BasicShapeType.None;
        private Position startPoint;
        private Sketch currentSketch = new Sketch();
        public event EventHandler<String>? shapeAdded;
        private Brush currentColor = Brushes.Black;
        private double currentStrokeThikness = 2;
        private ClientCommunicationService communicationService;

        public ClientWindow()
        {
            InitializeComponent();
            shapeAdded += OnShapeAdded;
            communicationService = new ClientCommunicationService();
            currentSketch.Name = "Untitled Sketch";

            this.Closing += OnClientWindowClosing;
        }

        private void OnClientWindowClosing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentSketch.Name))
            {
                LockHub.TriggerUnlock(currentSketch.Name);
            }
        }

        private static void OnShapeAdded(object sender, string type) => Console.WriteLine($"shape added :{type}");

        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentShape == BasicShapeType.None) return;

            startPoint = new Position(e.GetPosition(Canvas));
        }

        private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentShape == BasicShapeType.None) return;

            var endPoint = new Position(e.GetPosition(Canvas));

            var shape = ShapeFactory.Create(currentShape, startPoint, endPoint);
            if (shape == null) return;

            shape.SetColor(currentColor);
            shape.StrokeThikness = currentStrokeThikness;

            CanvasGeometryHelper.EnsureFitsCanvas(Canvas.ActualWidth, Canvas.ActualHeight, shape);

            currentSketch.addShape(shape);
            shapeAdded?.Invoke(this, currentShape.ToString());

            var uiElement = ShapeToUiElementConvertor.ConvertToUiElement(shape);
            Canvas.Children.Add(uiElement);
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Line;
            UpdateButtonSelection("Line");
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Rectangle;
            UpdateButtonSelection("Rectangle");
        }

        private void CircleButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Circle;
            UpdateButtonSelection("Circle");
        }

        private async void UploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentSketch.Shapes.Count == 0)
            {
                MessageBox.Show("No shapes to upload. Please draw something first.", "Nothing to Upload");
                return;
            }

            var sketchName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter sketch name:", "Upload Sketch", currentSketch.Name);

            if (string.IsNullOrWhiteSpace(sketchName)) return;

            try
            {
                currentSketch.Name = sketchName;

                var result = await communicationService.UploadSketchAsync(currentSketch);
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
            currentSketch.clear();
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
                var importedSketch = await communicationService.DownloadSketchAsync(selectedImport);
                if (importedSketch == null) return;

                Canvas.Children.Clear();

                LockHub.TriggerUnlock(currentSketch.Name);
                currentSketch = importedSketch;
                LockHub.TriggerLock(currentSketch.Name);

                foreach (var shape in currentSketch.Shapes)
                {
                    CanvasGeometryHelper.EnsureFitsCanvas(Canvas.ActualWidth, Canvas.ActualHeight, shape);

                    shape.Color = shape.GetBrushFromName(shape.ColorName);

                    var uiElement = ShapeToUiElementConvertor.ConvertToUiElement(shape);
                    Canvas.Children.Add(uiElement);
                }

                MessageBox.Show(
                    $"Sketch '{currentSketch.Name}' imported successfully with {currentSketch.Shapes.Count} shapes!",
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
                    SelectedIndex = GetColorIndex(currentColor)
                },
                StrokeSlider =
                {
                    Value = currentStrokeThikness
                }
            };

            if (optionsWindow.ShowDialog() != true) return;
            currentColor = optionsWindow.SelectedColor;
            currentStrokeThikness = optionsWindow.SelectedThickness;

            MessageBox.Show(
                $"Settings updated:\nColor: {GetColorName(currentColor)}\nStroke Thickness: {currentStrokeThikness}",
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