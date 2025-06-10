using System;

namespace Server.Helpers
{
    public static class SketchStoreNotifier
    {
        public static event Action<string>? SketchInserted;
        public static event Action<string>? SketchDeleted;

        public static void NotifyInserted(string name)
        {
            SketchInserted?.Invoke(name);
        }

        public static void NotifyDeleted(string name)
        {
            SketchDeleted?.Invoke(name);
        }
    }
}