using System;
using System.Linq;
using Client.Models;
using Common.DTO;
using System.Collections.Generic;

namespace Client.Mappers
{
    public static class SketchMapper
    {
        public static Sketch ToDomain(this SketchDto dto)
        {
            return new Sketch
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name,
                Shapes = dto.Shapes.Select(s => s.ToDomain()).Where(s => s != null).ToList()!
            };
        }

        public static SketchDto ToDto(this Sketch sketch)
        {
            return new SketchDto
            {
                Id = sketch.Id,
                Name = sketch.Name,
                Shapes = sketch.Shapes.Select(s => s.ToDto()).Where(s => s != null).ToList()!
            };
        }

        public static ShapeBase? ToDomain(this ShapeDto dto)
        {
            return dto.ShapeType switch
            {
                0 => CreateLine(dto),
                1 => CreateRectangle(dto),
                2 => CreateCircle(dto),
                _ => null
            };
        }

        public static ShapeDto? ToDto(this ShapeBase shape)
        {
            return shape switch
            {
                Line line => ConvertLine(line),
                Rectangle rect => ConvertRectangle(rect),
                Circle circle => ConvertCircle(circle),
                _ => null
            };
        }

        private static Line CreateLine(ShapeDto dto) => new Line
        {
            Start = dto.StartPosition,
            End = dto.EndPosition,
            Id = dto.Id
        };

        private static Rectangle CreateRectangle(ShapeDto dto) => new Rectangle
        {
            StartPosition = dto.StartPosition,
            EndPosition = dto.EndPosition,
            Width = dto.Width,
            Height = dto.Height,
            Id = dto.Id
        };

        private static Circle CreateCircle(ShapeDto dto) => new Circle
        {
            Center = dto.StartPosition,
            Radius = dto.Width,
            Id = dto.Id
        };

        private static ShapeDto ConvertLine(Line line) => new ShapeDto
        {
            StartPosition = line.Start,
            EndPosition = line.End,
            ShapeType = (int)line.shapeType,
            Id = line.Id
        };

        private static ShapeDto ConvertRectangle(Rectangle rect) => new ShapeDto
        {
            StartPosition = rect.StartPosition,
            EndPosition = rect.EndPosition,
            Width = rect.Width,
            Height = rect.Height,
            ShapeType = (int)rect.shapeType,
            Id = rect.Id
        };

        private static ShapeDto ConvertCircle(Circle circle) => new ShapeDto
        {
            StartPosition = circle.Center,
            Width = circle.Radius,
            ShapeType = (int)circle.shapeType,
            Id = circle.Id
        };
    }
}
