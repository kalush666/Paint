using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PainterServer.Services;

namespace Server
{
    public partial class ServerWindow : Window
    {
        private TcpSketchServer _server = new();
        private CancellationTokenSource suspendToken = new CancellationTokenSource();
        private MongoSketchStore _mongoStore = new MongoSketchStore();

        public ServerWindow()
        {
            InitializeComponent();
            _server.StartListener();
            LoadSketchList();
        }

        private async void LoadSketchList()
        {
            SketchList.Children.Clear();
            var sketchFiles = await _mongoStore.GetAllJsonAsync();

            foreach (var json in sketchFiles)
            {
                var button = new Button
                {
                    Content = json,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };


                SketchList.Children.Add(button);
            }
        }

        private void SuspendButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!suspendToken.IsCancellationRequested)
            {
                _server.Suspend();
                SuspendButton.Content = "Resume";
            }
            else
            {
                suspendToken = new CancellationTokenSource();
                _server.Resume();
                SuspendButton.Content = "Suspend";
            }
        }
    }
}