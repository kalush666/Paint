using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Client.Enums;
using Client.Factories;
using Client.Models;

namespace Client
{
    public partial class ClientWindow : Window
    {
        private BasicShapeType currentShape = BasicShapeType.None;
        private Position startPoint;
        private List<ShapeBase> shapes = new();
        public event EventHandler<String>? shapeAdded;
        private Brush currentColor = Brushes.Black;
        private double currentStrokeThikness = 2;

        public ClientWindow()
        {
            InitializeComponent();
            shapeAdded += OnShapeAdded;
        }

        private void OnShapeAdded(object sender, string type) => Console.WriteLine($"shape added :{type}");
        

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

            shape.Color = currentColor;
            shape.StrokeThikness = currentStrokeThikness;

            shapes.Add(shape);
            shapeAdded?.Invoke(this, currentShape.ToString());

            var UiElement = shape.ToUI(currentColor, currentStrokeThikness);
            Canvas.Children.Add(UiElement);
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

        private void UploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            shapes.Clear();
        }

        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            if (optionsWindow.ShowDialog() == true)
            {
                currentColor = optionsWindow.SelectedColor;
                currentStrokeThikness = optionsWindow.SelectedThickness;
            }
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
    }
}