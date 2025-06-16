using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly Dictionary<string, Brush> NameToBrush = BrushToName.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static int GetIndex(Brush brush) => Array.IndexOf(BrushesList, brush);
        public static Brush GetBrushByIndex(int index) => (index >= 0 && index < BrushesList.Length) ? BrushesList[index] : Brushes.Black;

        public static string GetName(Brush brush) => BrushToName.TryGetValue(brush, out var name) ? name : "Black";
        public static Brush GetBrushByName(string name) => NameToBrush.TryGetValue(name, out var brush) ? brush : Brushes.Black;
    }

}