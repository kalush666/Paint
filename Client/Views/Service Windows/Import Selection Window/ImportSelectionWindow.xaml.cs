using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Services;
using Common.Errors;
using Common.Helpers;

namespace Client.Views.Service_Windows.Import_Selection_Window
{
    public partial class ImportSelectionWindow : Window
    {
        public string? SelectedSketch { get; private set; }
        private readonly ClientCommunicationService _communicationService = new ClientCommunicationService();

        private readonly ProgressBar _loadingSpinner = new ProgressBar
        {
            IsIndeterminate = true,
            Width = 150,
            Height = 20,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };

        public ImportSelectionWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadSketchNames();
        }

        private async Task LoadSketchNames()
        {
            SketchImportList.Children.Clear();
            SketchImportList.Children.Add(_loadingSpinner);

            Result<List<string>> sketchNames = await _communicationService.GetAllSketchNames();


            if (sketchNames.Error is AppErrors.Server.Suspended)
            {
                SetMessage(AppErrors.Server.Suspended);
                return;
            }

            if (sketchNames.Value == null || sketchNames.Value.Count == 0)
            {
                SetMessage("No sketches available for import.");
                return;
            }

            SketchImportList.Children.Clear();
            PopulateSketchNames(sketchNames);
        }

        private void PopulateSketchNames(Result<List<string>> sketchNames)
        {

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


        private void SetMessage(string message)
        {
            SketchImportList.Children.Clear();
            SketchImportList.Children.Add(new TextBlock
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            });
        }
        
    }
}