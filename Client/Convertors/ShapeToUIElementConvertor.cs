using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Models;
using Common.Errors;
using Common.Utils;

namespace Client.Convertors
{
    public static class ShapeToUiElementConvertor
    {
        public static UIElement ConvertToUiElement(Brush color, double strokeThickness, ShapeBase shape)
        {
            switch (shape)
            {
                case Circle circle:
                    var ellipse = new System.Windows.Shapes.Ellipse
                    {
                        Width = circle.Radius*2,
                        Height = circle.Radius*2,
                        Stroke = color,
                        StrokeThickness = strokeThickness
                    };
                    Canvas.SetLeft(ellipse,circle.Center.X-circle.Radius);
                    Canvas.SetTop(ellipse,circle.Center.Y-circle.Radius);
                    return ellipse;
                case Line line:
                    return new System.Windows.Shapes.Line
                    {
                        X1 = line.Start.X,
                        Y1 = line.Start.Y,
                        X2 = line.End.X,
                        Y2 = line.End.Y,
                        Stroke = color,
                        StrokeThickness = strokeThickness
                    };
                case Rectangle rectangle:
                    double x = Math.Min(rectangle.StartPosition.X, rectangle.EndPosition.X);
                    double y = Math.Min(rectangle.StartPosition.Y, rectangle.EndPosition.Y);
                    double width = Math.Abs(rectangle.EndPosition.X - rectangle.StartPosition.X);
                    double height = Math.Abs(rectangle.EndPosition.Y - rectangle.StartPosition.Y);

                    var rect = new System.Windows.Shapes.Rectangle
                    {
                        Width = width,
                        Height = height,
                        Stroke = color,
                        StrokeThickness = strokeThickness
                    };

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                    return rect;
                default:
                    throw new NotSupportedException(AppErrors.Shapes.NotAShape);
            }
        }
    }

}