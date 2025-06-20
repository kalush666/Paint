using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Factories;
using Client.Helpers;
using Client.Models;
using Client.UIModels;
using Common.Enums;
using Common.Models;
using Line = Client.Models.Line;

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
            if (_currentShape == BasicShapeType.None || e.ChangedButton != MouseButton.Left) 
                return;

            _isDrawing = true;
            Point mousePoint = e.GetPosition(_canvas);
            _startPosition = new Position(mousePoint.X, mousePoint.Y);
            _lastMousePosition = _startPosition;
            
            _canvas.CaptureMouse();
            
            CreateOptimizedPreview(_startPosition, _startPosition);
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
            
            UpdateOptimizedPreview(_startPosition, currentPosition);
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

        private void CreateOptimizedPreview(Position start, Position end)
        {
            RemovePreview();

            var shape = ShapeFactory.Create(_currentShape, start, end);
            if (shape == null) return;

            CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);

            _previewElement = CreatePreviewElement(shape);
            if (_previewElement != null)
            {
                _previewElement.Opacity = 0.7;
                _canvas.Children.Add(_previewElement);
            }
        }

        private void UpdateOptimizedPreview(Position start, Position end)
        {
            if (_previewElement == null) return;

            switch (_currentShape)
            {
                case BasicShapeType.Line when _previewElement is System.Windows.Shapes.Line line:
                    line.X1 = start.X;
                    line.Y1 = start.Y;
                    line.X2 = end.X;
                    line.Y2 = end.Y;
                    break;

                case BasicShapeType.Rectangle when _previewElement is System.Windows.Shapes.Rectangle rect:
                    UpdateRectanglePreview(rect, start, end);
                    break;

                case BasicShapeType.Circle when _previewElement is Ellipse ellipse:
                    UpdateCirclePreview(ellipse, start, end);
                    break;

                default:
                    CreateOptimizedPreview(start, end);
                    break;
            }
        }

        private void UpdateRectanglePreview(System.Windows.Shapes.Rectangle rect, Position start, Position end)
        {
            var x = Math.Min(start.X, end.X);
            var y = Math.Min(start.Y, end.Y);
            var width = Math.Abs(end.X - start.X);
            var height = Math.Abs(end.Y - start.Y);

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            rect.Width = width;
            rect.Height = height;
        }

        private void UpdateCirclePreview(Ellipse ellipse, Position start, Position end)
        {
            var width = Math.Abs(end.X - start.X);
            var height = Math.Abs(end.Y - start.Y);
            var size = Math.Min(width, height);

            var x = Math.Min(start.X, end.X);
            var y = Math.Min(start.Y, end.Y);

            var centerX = x + width / 2;
            var centerY = y + height / 2;

            Canvas.SetLeft(ellipse, centerX - size / 2);
            Canvas.SetTop(ellipse, centerY - size / 2);
            ellipse.Width = size;
            ellipse.Height = size;
        }



        private UIElement? CreatePreviewElement(ShapeBase shape)
        {
            return shape switch
            {
                Line line => new System.Windows.Shapes.Line
                {
                    X1 = line.Start.X,
                    Y1 = line.Start.Y,
                    X2 = line.End.X,
                    Y2 = line.End.Y,
                    Stroke = CurrentColor,
                },

                Models.Rectangle rectangle => new System.Windows.Shapes.Rectangle
                {
                    Width = rectangle.Width,
                    Height = rectangle.Height,
                    Stroke = CurrentColor,
                    Fill = Brushes.Transparent
                }.Apply(r => {
                    var x = Math.Min(rectangle.StartPosition.X, rectangle.EndPosition.X);
                    var y = Math.Min(rectangle.StartPosition.Y, rectangle.EndPosition.Y);
                    Canvas.SetLeft(r, x);
                    Canvas.SetTop(r, y);
                }),

                Circle circle => new Ellipse
                {
                    Width = circle.Radius * 2,
                    Height = circle.Radius * 2,
                    Stroke = CurrentColor,
                    Fill = Brushes.Transparent
                }.Apply(e => {
                    Canvas.SetLeft(e, circle.Center.X - circle.Radius);
                    Canvas.SetTop(e, circle.Center.Y - circle.Radius);
                }),

                _ => null
            };
        }

        private void RemovePreview()
        {
            if (_previewElement != null)
            {
                _canvas.Children.Remove(_previewElement);
                _previewElement = null;
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

            CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);
            _currentSketch.AddShape(shape);
            ShapeAdded?.Invoke(this, _currentShape.ToString());

            RemovePreview();
            AddFinalShape(shape);
        }

        private void AddFinalShape(ShapeBase shape)
        {
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
            RemovePreview();
        }

        public void ImportSketch(Sketch importedSketch)
        {
            Clear();
            _currentSketch = importedSketch;
            
            foreach (var shape in _currentSketch.Shapes)
            {
                CanvasGeometryHelper.EnsureFitsCanvas(_canvas.ActualWidth, _canvas.ActualHeight, shape);
                AddFinalShape(shape);
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

        public void Dispose()
        {
            _canvas.MouseDown -= OnMouseDown;
            _canvas.MouseMove -= OnMouseMove;
            _canvas.MouseUp -= OnMouseUp;
            _canvas.MouseLeave -= OnMouseLeave;
            
            RemovePreview();
        }
    }
}

public static class UIElementExtensions
{
    public static T Apply<T>(this T element, Action<T> action) where T : UIElement
    {
        action(element);
        return element;
    }
}