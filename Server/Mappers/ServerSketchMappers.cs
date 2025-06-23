using System.Linq;
using Common.DTO;
using Server.Models;

namespace Server.Mappers
{
    public static class ServerSketchMappers
    {
        public static SketchDto ToDto(this ServerSketch model) => new()
        {
            Id = model.Id,
            Name = model.Name,
            Shapes = model.Shapes.Select(s => new ShapeDto
            {
                Id = s.Id,
                StartPosition = s.StartPosition,
                EndPosition = s.EndPosition,
                Width = s.Width,
                Height = s.Height,
                ShapeType = s.ShapeType
            }).ToList()
        };

        public static ServerSketch ToServer(this SketchDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Shapes = dto.Shapes.Select(s => new ServerBaseShape
            {
                Id = s.Id,
                StartPosition = s.StartPosition,
                EndPosition = s.EndPosition,
                Width = s.Width,
                Height = s.Height,
                ShapeType = s.ShapeType
            }).ToList()
        };
    }
}