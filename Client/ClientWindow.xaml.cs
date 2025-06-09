using System;
using System.Collections.Generic;
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
        }

        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = new Position(e.GetPosition(Canvas));
        }

        private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var endPoint = new Position(e.GetPosition(Canvas));

            var shape = ShapeFactory.Create(currentShape, startPoint, endPoint);
            if (shape == null) return;

            shapes.Add(shape);
            shapeAdded.Invoke(this,currentShape.ToString());

            Canvas.Children.Add(shape.ToUI(currentColor,currentStrokeThikness));
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Line;
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Rectangle;
        }

        private void CircleButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentShape = BasicShapeType.Circle;
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
    }
}