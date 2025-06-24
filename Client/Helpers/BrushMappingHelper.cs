using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Client.Helpers
{
    public static class BrushMappingHelper
    {
        private static readonly Brush[] BrushesList =
        {
            Brushes.Black,
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue
        };

        private static readonly Dictionary<Brush, string> BrushToName = new()
        {
            { Brushes.Black, "Black" },
            { Brushes.Red, "Red" },
            { Brushes.Green, "Green" },
            { Brushes.Blue, "Blue" }
        };

        public static int GetIndex(Brush brush) => Array.IndexOf(BrushesList, brush);

        public static string GetName(Brush brush) => BrushToName.TryGetValue(brush, out var name) ? name : "Black";
    }

}