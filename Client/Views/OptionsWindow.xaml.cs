using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Views.Service_Windows
{
    public partial class OptionsWindow : Window
    {
        public Brush SelectedColor { get; private set; } = Brushes.Black;
        public double SelectedThickness { get; private set; } = 2;

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)ColorPicker.SelectedItem;
            var colorName = selectedItem.Content.ToString();

            SelectedColor = colorName switch
            {
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                _ => Brushes.Black
            };

            SelectedThickness = StrokeSlider.Value;
            DialogResult = true;
            Close();
        }

        private void StrokeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}