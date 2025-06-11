using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Services;

namespace Client.Views
{
    public partial class ImportSelectionWindow : Window
    {
        public string? SelectedSketch { get; private set; }
        public ClientCommunicationService communicationService = new ClientCommunicationService();

        public ImportSelectionWindow(List<string> sketchNames)
        {
            InitializeComponent();
            PopulateSketchNames(sketchNames);
        }
        
        private void PopulateSketchNames(List<string> sketchNames)
        {
            foreach (var sketchName in sketchNames)
            {
                var selectImport = new Button
                {
                    Content = sketchName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(10, 5, 10, 5),
                    MinWidth = 150,
                    Margin = new Thickness(0,0,10,10),
                    
                };
                selectImport.Click += (sender, e) =>
                {
                    SelectedSketch = sketchName;
                    DialogResult = true;
                    Close();
                };
                SketchImportList.Children.Add(selectImport);
            }
        }

    }
}