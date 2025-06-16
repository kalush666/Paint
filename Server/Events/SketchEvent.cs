using Server.Enums;

namespace Server.Events
{
    public class SketchEvent
    {
        public SketchEventType EventType { get; }
        public string SketchName { get; }
        
        public SketchEvent(SketchEventType eventType, string sketchName)
        {
            EventType = eventType;
            SketchName = sketchName;
        }
    }
}