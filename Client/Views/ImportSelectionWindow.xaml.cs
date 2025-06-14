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

        public ImportSelectionWindow()
        {
            InitializeComponent();
            LoadAllSketches();
        }

        private void LoadAllSketches()
        {
            PopulateSketchNames();
        }

        private async Task PopulateSketchNames()
        {
            SketchImportList.Children.Add(new TextBlock
            {
                Text = "Loading sketches...",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            });
            var sketchNames = await communicationService.GetAllSketchNames();
            if (sketchNames.Value == null || sketchNames.Value.Count == 0)
            {
                SketchImportList.Children.Clear();
                SketchImportList.Children.Add(new TextBlock
                {
                    Text = "No sketches available for import.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10)
                });
                return;
            }
            else
            {
                SketchImportList.Children.Clear();
                SketchImportList.Children.Add(new TextBlock
                {
                    Text = "Select a sketch to import:",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10)
                });
            }

            foreach (var sketchName in sketchNames.Value)
            {
                var selectImport = new Button
                {
                    Content = sketchName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(10, 5, 10, 5),
                    MinWidth = 150,
                    Margin = new Thickness(0, 0, 10, 10),
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