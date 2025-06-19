using Common.Models;

namespace Common.Mappers
{
    public class SketchMapper
    {
        public static Sketch ToSketch(SketchDocument document)
            => new(document.SketchName, document.Shapes);
        
        public static SketchDocument ToDocument(Sketch sketch)
            => new() {SketchName = sketch.Name, Shapes = sketch.Shapes};
    }
}