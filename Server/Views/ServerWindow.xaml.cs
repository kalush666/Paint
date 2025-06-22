using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common.Errors;
using Server.Events;
using Server.Models;
using Server.Repositories;
using Server.Services;
using Server.ViewModel;

namespace Server.Views
{
    public partial class ServerWindow : Window
    {
        private readonly TcpSketchServer _server;
        private readonly CancellationTokenSource suspendToken = new CancellationTokenSource();
        private readonly MongoSketchStore _mongoStore;
        private bool _isSuspended = false;
        private readonly SketchListViewModel _viewModel;
        private readonly SketchEventBus<SketchEvent> _eventBus;

        public ServerWindow()
        {
            InitializeComponent();
            _eventBus = new SketchEventBus<SketchEvent>();
            MongoSketchStore.Initialize(_eventBus);
            _mongoStore = MongoSketchStore.Instance;
            _viewModel = new SketchListViewModel(_mongoStore);
            DataContext = _viewModel;
            _server = new TcpSketchServer(_mongoStore);

            _server.StartListener();
            LoadSketchList();
        }

        protected override async void OnContentRendered(EventArgs eventArgs)
        {
            base.OnContentRendered(eventArgs);
            await _viewModel.ListenAsync(_eventBus, suspendToken.Token);
        }


        private async void LoadSketchList()
        {
            await RefreshSketchListAsync();
        }

        private async Task RefreshSketchListAsync()
        {
            try
            {
                var result = await _mongoStore.GetAllSketchesAsync();
                if (!result.IsSuccess)
                {
                    MessageBox.Show(AppErrors.Mongo.ReadError, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (var sketch in result.Value)
                {
                    var entry = new SketchEntry { Id = sketch.Id, Name = sketch.Name };
                    if (_viewModel.Sketches.All(s => s.Id != entry.Id))
                        _viewModel.Sketches.Add(entry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing sketches: {ex.Message}", "Error",
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