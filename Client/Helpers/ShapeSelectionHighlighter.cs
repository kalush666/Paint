using System.Windows;
using System.Windows.Controls;
using Client.Enums;
using Common.Enums;

namespace Client.Helpers
{
    public class ShapeSelectionHighlighter
    {
        private readonly StackPanel _buttonPanel;

        public ShapeSelectionHighlighter(StackPanel rootPanel)
        {
            _buttonPanel = rootPanel.Children[1] as StackPanel ?? rootPanel;
        }

        public void Highlight(BasicShapeType selectedShape)
        {
            foreach (var child in _buttonPanel.Children)
            {
                if (child is not Button button) continue;
                if (button.Tag is string tag && tag == selectedShape.ToString())
                    button.FontWeight = FontWeights.Bold;
                else
                    button.FontWeight = FontWeights.Normal;
            }
        }
    }
}