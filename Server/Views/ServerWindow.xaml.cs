using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Server.Services;

namespace Server
{
    public partial class ServerWindow : Window
    {
        private TcpSketchServer _server = new();
        private CancellationTokenSource suspendToken = new CancellationTokenSource();
        private MongoSketchStore _mongoStore = new MongoSketchStore();
        private Timer _refreshTimer;
        private bool _isSuspended = false;

        public ServerWindow()
        {
            InitializeComponent();
            _server.StartListener();
            LoadSketchList();

            _refreshTimer = new Timer(async _ => await RefreshSketchListAsync(), null,
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private async void LoadSketchList()
        {
            await RefreshSketchListAsync();
        }

        private async Task RefreshSketchListAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    SketchList.Children.Clear();
                    var sketchFiles = await _mongoStore.GetAllJsonAsync();

                    foreach (var json in sketchFiles)
                    {
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                        string displayName = obj["Name"]?.ToString() + ".json";

                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 5)
                        };

                        var nameButton = new Button
                        {
                            Content = displayName,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            MinWidth = 150,
                            Margin = new Thickness(0, 0, 10, 0)
                        };

                        var deleteButton = new Button
                        {
                            Content = "Delete",
                            Background = System.Windows.Media.Brushes.LightCoral,
                            Padding = new Thickness(10, 5, 10, 5),
                            MinWidth = 80
                        };

                        string sketchName = obj["Name"]?.ToString();
                        deleteButton.Click += async (s, e) => await DeleteSketch(sketchName);

                        stackPanel.Children.Add(nameButton);
                        stackPanel.Children.Add(deleteButton);
                        SketchList.Children.Add(stackPanel);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing sketch list: {ex.Message}");
            }
        }

        private async Task DeleteSketch(string sketchName)
        {
            try
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{sketchName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _mongoStore.DeleteSketchAsync(sketchName);
                    await RefreshSketchListAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting sketch: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuspendButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_isSuspended)
            {
                _server.Suspend();
                _isSuspended = true;
                SuspendButton.Content = "Resume";
                _refreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                _server.Resume();
                _isSuspended = false;
                SuspendButton.Content = "Suspend";
                _refreshTimer?.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer?.Dispose();
            _server.Suspend();
            base.OnClosed(e);
        }
    }
}