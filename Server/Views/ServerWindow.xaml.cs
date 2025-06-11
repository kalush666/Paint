using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Server.Helpers;
using Server.Repositories;
using Server.Services;

namespace Server.Views
{
    public partial class ServerWindow : Window
    {
        private TcpSketchServer _server = new();
        private CancellationTokenSource suspendToken = new CancellationTokenSource();
        private MongoSketchStore _mongoStore = new MongoSketchStore();
        private bool _isSuspended = false;

        public ServerWindow()
        {

            InitializeComponent();
            _server.StartListener();
            LoadSketchList();

            SketchStoreNotifier.SketchInserted += name => Dispatcher.Invoke( () =>  AddToSketchList(name));
            SketchStoreNotifier.SketchDeleted += name => Dispatcher.Invoke( () =>  RemoveFromSketchList(name));
        }

        private void RemoveFromSketchList(string sketchName)
        {
            var displayName = sketchName + ".json";
            StackPanel toRemove = null;

            foreach (var child in SketchList.Children)
            {
                if (child is StackPanel panel && panel.Children.Count > 0 && panel.Children[0] is Button btn)
                {
                    if (btn.Content?.ToString() == displayName)
                    {
                        toRemove = panel;
                        break;
                    }
                }
            }

            if (toRemove != null)
            {
                SketchList.Children.Remove(toRemove);
            }
        }

        private void AddToSketchList(string sketchName)
        {
            var displayName = sketchName + ".json";
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
            deleteButton.Click += async (s, e) => await DeleteSketch(sketchName);
            stackPanel.Children.Add(nameButton);
            stackPanel.Children.Add(deleteButton);
            SketchList.Children.Add(stackPanel);
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
                        var displayName = obj["Name"] + ".json";

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

                        var sketchName = obj["Name"]?.ToString();
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
            }
            else
            {
                _server.Resume();
                _isSuspended = false;
                SuspendButton.Content = "Suspend";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _server.Suspend();
            base.OnClosed(e);
        }
    }
}