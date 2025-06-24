using System.Linq;
using Client.Models;
using Common.DTO;
using MongoDB.Bson;

namespace Client.Mappers
{
    public static class SketchMapper
    {
        public static Sketch ToDomain(this SketchDto dto)
        {
            return new Sketch
            {
                Id = dto.Id,
                Name = dto.Name,
                Shapes = dto.Shapes.Select(s => s.ToDomain()).Where(s => s != null).ToList()!
            };
        }

        public static SketchDto ToDto(this Sketch sketch)
        {
            return new SketchDto
            {
                Id = sketch.Id == ObjectId.Empty ? ObjectId.Empty : sketch.Id,
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
        };

        private static Rectangle CreateRectangle(ShapeDto dto) => new Rectangle
        {
            StartPosition = dto.StartPosition,
            EndPosition = dto.EndPosition,
            Width = dto.Width,
            Height = dto.Height,
        };

        private static Circle CreateCircle(ShapeDto dto) => new Circle
        {
            StartPosition = dto.StartPosition,
            EndPosition = dto.EndPosition,
        };

        private static ShapeDto ConvertLine(Line line) => new ShapeDto
        {
            StartPosition = line.Start,
            EndPosition = line.End,
            ShapeType = (int)line.shapeType,
        };

        private static ShapeDto ConvertRectangle(Rectangle rect) => new ShapeDto
        {
            StartPosition = rect.StartPosition,
            EndPosition = rect.EndPosition,
            Width = rect.Width,
            Height = rect.Height,
            ShapeType = (int)rect.shapeType,
        };

        private static ShapeDto ConvertCircle(Circle circle) => new ShapeDto
        {
            StartPosition = circle.StartPosition,
            EndPosition = circle.EndPosition,
            ShapeType = (int)circle.shapeType,
        };
    }
}