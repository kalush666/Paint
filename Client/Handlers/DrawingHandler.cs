using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Factories;
using Client.Models;
using Client.UIModels;
using Common.Enums;
using Common.Models;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace Client.Handlers
{
    public class DrawingHandler
    {
        private const BasicShapeType DefaultShapeType = BasicShapeType.None;
        private static readonly Brush DefaultColor = Brushes.Black;
        private const double DefaultStrokeThickness = 2.0;
        private const double MouseMoveThreshold = 2.0;
        private const int ThrottleMs = 16;

        private readonly Canvas _canvas;
        private readonly UiShapeFactory _uiShapeFactory;
        private BasicShapeType _currentShape = DefaultShapeType;
        private Position _startPosition;
        private Position _lastMousePosition;
        private Sketch _currentSketch = new Sketch();
        private UiBaseShape? _previewShape;
        private UIElement? _previewElement;
        private bool _isDrawing;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public Brush CurrentColor { get; set; } = DefaultColor;
        public double CurrentStrokeThickness { get; set; } = DefaultStrokeThickness;
        public Sketch CurrentSketch => _currentSketch;
        public event EventHandler<string>? ShapeAdded;

        public DrawingHandler(Canvas canvas, Sketch sketch)
        {
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            _currentSketch = sketch ?? new Sketch();
            _uiShapeFactory = new UiShapeFactory();

            AttachEvents();
        }

        private void AttachEvents()
        {
            _canvas.MouseDown += OnMouseDown;
            _canvas.MouseMove += OnMouseMove;
            _canvas.MouseUp += OnMouseUp;
            _canvas.MouseLeave += OnMouseLeave;
            _canvas.CaptureMouse();
        }

        public void SetCurrentShape(BasicShapeType shapeType) => _currentShape = shapeType;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentShape == BasicShapeType.None || e.ChangedButton != MouseButton.Left) return;

            _isDrawing = true;
            Point mousePoint = e.GetPosition(_canvas);
            _startPosition = new Position(mousePoint.X, mousePoint.Y);
            _lastMousePosition = _startPosition;
            _canvas.CaptureMouse();

            CreatePreview(_startPosition, _startPosition);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            Point mousePoint = e.GetPosition(_canvas);
            var currentPosition = new Position(mousePoint.X, mousePoint.Y);

            var now = DateTime.Now;
            if ((now - _lastUpdateTime).TotalMilliseconds < ThrottleMs)
                return;

            if (_lastMousePosition.Distance(currentPosition) < MouseMoveThreshold)
                return;

            _lastUpdateTime = now;
            _lastMousePosition = currentPosition;

            currentPosition = ClampToCanvas(currentPosition);
            UpdatePreview(_startPosition, currentPosition);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDrawing || e.ChangedButton != MouseButton.Left) return;
            CompleteDrawing(e);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;
            CompleteDrawing(null);
        }

        private void CompleteDrawing(MouseButtonEventArgs? e)
        {
            _isDrawing = false;
            _canvas.ReleaseMouseCapture();

            Point mousePoint = e?.GetPosition(_canvas) ?? new Point(_lastMousePosition.X, _lastMousePosition.Y);
            var endPosition = ClampToCanvas(new Position(mousePoint.X, mousePoint.Y));
            FinalizeShape(_startPosition, endPosition);
        }

        private Position ClampToCanvas(Position position)
        {
            var clampedX = Math.Max(0, Math.Min(_canvas.ActualWidth, position.X));
            var clampedY = Math.Max(0, Math.Min(_canvas.ActualHeight, position.Y));
            return new Position(clampedX, clampedY);
        }

        private void CreatePreview(Position start, Position end)
        {
            RemovePreview();
            var logicShape = ShapeFactory.Create(_currentShape, start, end);
            if (logicShape == null) return;

            _previewShape = _uiShapeFactory.Create(logicShape);
            if (_previewShape == null) return;

            _previewShape.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight);
            ApplyStyle(_previewShape);
            _previewElement = _previewShape.Render();

            if (_previewElement != null)
                _canvas.Children.Add(_previewElement);
        }

        private void UpdatePreview(Position start, Position end)
        {
            CreatePreview(start, end);
        }

        private void RemovePreview()
        {
            if (_previewElement != null)
            {
                _canvas.Children.Remove(_previewElement);
                _previewElement = null;
                _previewShape = null;
            }
        }

        private void FinalizeShape(Position start, Position end)
        {
            var shape = ShapeFactory.Create(_currentShape, start, end);
            if (shape == null)
            {
                RemovePreview();
                return;
            }

            var uiShape = _uiShapeFactory.Create(shape);
            if (uiShape == null) return;

            uiShape.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight);
            ApplyStyle(uiShape);

            _currentSketch.AddShape(shape);
            ShapeAdded?.Invoke(this, _currentShape.ToString());

            RemovePreview();

            var element = uiShape.Render();
            if (element != null)
                _canvas.Children.Add(element);
        }

        private void ApplyStyle(UiBaseShape? uiShape)
        {
            if (uiShape == null) return;
            uiShape.StrokeColor = CurrentColor;
            uiShape.StrokeThickness = CurrentStrokeThickness;
        }

        public void Clear()
        {
            _canvas.Children.Clear();
            _currentSketch.Clear();
            RemovePreview();
        }

        public void ImportSketch(Sketch importedSketch)
        {
            Clear();
            _currentSketch = importedSketch;

            foreach (var shape in _currentSketch.Shapes)
            {
                var uiShape = _uiShapeFactory.Create(shape);
                if (uiShape == null) continue;

                uiShape.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight);
                ApplyStyle(uiShape);

                var finalElement = uiShape.Render();
                if (finalElement != null)
                    _canvas.Children.Add(finalElement);
            }
        }
    }
}