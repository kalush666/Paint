using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views
{
    public partial class ImportSelectionWindow : Window
    {
        public string? SelectedSketch { get; private set; }

        public ImportSelectionWindow(List<string> sketchNames)
        {
            InitializeComponent();

            foreach (var sketchName in sketchNames )
            {
                var selectSketch = new Button
                {
                    Content = sketchName,
                    Margin = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                selectSketch.Click += (selectedImport, e) =>
                {
                    SelectedSketch = sketchName;
                    DialogResult = true;
                    Close();
                };
            }
        }

    }
}