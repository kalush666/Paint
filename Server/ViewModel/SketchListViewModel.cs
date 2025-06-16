using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Server.Enums;
using Server.Events;
using Server.Helpers;
using Server.Repositories;

namespace Server.ViewModel
{
    public class SketchListViewModel : INotifyPropertyChanged
    {
        private readonly MongoSketchStore _store;

        public ObservableCollection<string> Sketches { get; } = new();

        public ICommand DeleteCommand { get; }

        public SketchListViewModel(MongoSketchStore sketchStore)
        {
            _store = sketchStore;

            DeleteCommand = new RelayCommand<string>(async name =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    await _store.DeleteSketchAsync(name);
                }
            });
        }

        public async Task ListenAsync(SketchEventBus<SketchEvent> bus, CancellationToken token)
        {
            await foreach (var evt in bus.Events.WithCancellation(token))
            {
                if (evt.EventType == SketchEventType.Inserted)
                {
                    if (!Sketches.Contains(evt.SketchName))
                        Sketches.Add(evt.SketchName);
                }
                else if (evt.EventType == SketchEventType.Deleted)
                {
                    if (Sketches.Contains(evt.SketchName))
                        Sketches.Remove(evt.SketchName);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}