using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Client.Factories;
using Client.Helpers;
using Client.Models;
using Client.UIModels;
using Common.Enums;

namespace Client.Handlers
{
    public class DrawingHandler
    {
        private const BasicShapeType DefaultShapeType = BasicShapeType.None;
        private static readonly Brush DefaultColor = Brushes.Black;
        private const double DefaultStrokeThickness = 2.0;

        private readonly Canvas _canvas;
        private readonly UiShapeFactory _uiShapeFactory;
        private BasicShapeType _currentShape = DefaultShapeType;
        private Position _startPosition;
        private Sketch _currentSketch = new Sketch();
        private UIElement? _previewElement;
        private bool _isDrawing;

        public Brush CurrentColor { get; set; } = DefaultColor;
        public double CurrentStrokeThickness { get; set; } = DefaultStrokeThickness;
        public Sketch CurrentSketch => _currentSketch;
        public event EventHandler<string>? ShapeAdded;

        public DrawingHandler(Canvas canvas, Sketch sketch)
        {
            _canvas = canvas;
            _currentSketch = sketch;
            _uiShapeFactory = new UiShapeFactory();

            _canvas.MouseDown += OnMouseDown;
            _canvas.MouseMove += OnMouseMove;
            _canvas.MouseUp += OnMouseUp;
        }


        public void SetCurrentShape(BasicShapeType shapeType) => _currentShape = shapeType;

        private void OnMouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (_currentShape == BasicShapeType.None) return;

            _isDrawing = true;
            Point mousePoint = e.GetPosition(_canvas);
            _startPosition = new  Position(mousePoint.X, mousePoint.Y);
            CreatePreview(_startPosition, _startPosition);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;
            Point mousePoint = e.GetPosition(_canvas);
            var currentPosition = new Position(mousePoint.X, mousePoint.Y);
            UpdatePreview(_startPosition, currentPosition);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            _isDrawing = false;
            Point mousePoint = e.GetPosition(_canvas);
            var endPosition = new Position(mousePoint.X, mousePoint.Y);
            FinalizeShape(_startPosition, endPosition);
        }

        private void CreatePreview(Position start, Position end)
        {
            var shape = ShapeFactory.Create(_currentShape, start, end);
            CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);

            var uiShape = _uiShapeFactory.Create(shape);
            if (uiShape == null) return;

            ApplyStyle(uiShape);
            _previewElement = uiShape.Render();
            if (_previewElement != null)
                _canvas.Children.Add(_previewElement);
        }

        private void UpdatePreview(Position start, Position end)
        {
            if (_previewElement != null)
                _canvas.Children.Remove(_previewElement);

            CreatePreview(start, end);
        }

        private void FinalizeShape(Position start, Position end)
        {
            var shape = ShapeFactory.Create(_currentShape, start, end);
            CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);

            _currentSketch.AddShape(shape);
            ShapeAdded?.Invoke(this, _currentShape.ToString());

            if (_previewElement != null)
            {
                _canvas.Children.Remove(_previewElement);
                _previewElement = null;
            }

            var uiShape = _uiShapeFactory.Create(shape);
            if (uiShape == null) return;

            ApplyStyle(uiShape);
            var finalElement = uiShape.Render();
            if (finalElement != null)
                _canvas.Children.Add(finalElement);
        }

        private void ApplyStyle(UIBaseShape uiShape)
        {
            uiShape.StrokeColor = CurrentColor;
            uiShape.StrokeThickness = CurrentStrokeThickness;
        }

        public void Clear()
        {
            _canvas.Children.Clear();
            _currentSketch.Clear();
        }

        public void ImportSketch(Sketch importedSketch)
        {
            Clear();
            _currentSketch = importedSketch;
            foreach (var shape in _currentSketch.Shapes)
            {
                CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);
                var uiShape = _uiShapeFactory.Create(shape);
                if (uiShape == null) continue;

                ApplyStyle(uiShape);
                var element = uiShape.Render();
                if (element != null)
                    _canvas.Children.Add(element);
            }
        }
        private ShapeSelectionHighlighter? _shapeHighlighter;

        public void RegisterHighlighter(ShapeSelectionHighlighter highlighter)
        {
            _shapeHighlighter = highlighter;
        }

        public void HighlightShape(BasicShapeType shapeType)
        {
            _shapeHighlighter?.Highlight(shapeType);
        }

    }
}