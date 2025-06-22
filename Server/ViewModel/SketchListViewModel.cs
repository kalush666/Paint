using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Helpers;
using Server.Enums;
using Server.Events;
using Server.Models;
using Server.Repositories;

namespace Server.ViewModel
{
    public class SketchListViewModel : INotifyPropertyChanged
    {
        private readonly MongoSketchStore _store;

        public ObservableCollection<SketchEntry> Sketches { get; } = new();

        public ICommand DeleteCommand { get; }

        public SketchListViewModel(MongoSketchStore sketchStore)
        {
            _store = sketchStore;

            DeleteCommand = new RelayCommand<SketchEntry>(async entry =>
            {
                if (entry?.Id == Guid.Empty) return;
                if (entry != null)
                    await _store.DeleteSketchByIdAsync(entry.Id);
            });
        }

        public async Task ListenAsync(SketchEventBus<SketchEvent> bus, CancellationToken token)
        {
            await foreach (var evt in bus.Events.WithCancellation(token))
            {
                if (evt.EventType == SketchEventType.Inserted)
                {
                    if (Sketches.All(s => s.Name != evt.SketchName))
                    {
                        var result = await _store.GetByNameAsync(evt.SketchName);
                        if (!result.IsSuccess) continue;

                        var dto = result.Value;
                        Sketches.Add(new SketchEntry { Id = dto.Id, Name = dto.Name });
                    }
                }
                else if (evt.EventType == SketchEventType.Deleted)
                {
                    var item = Sketches.FirstOrDefault(s => s.Name == evt.SketchName);
                    if (item != null)
                        Sketches.Remove(item);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}