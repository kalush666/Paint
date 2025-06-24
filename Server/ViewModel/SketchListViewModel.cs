using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.DTO;
using Common.Helpers;
using MongoDB.Bson;
using Server.Enums;
using Server.Events;
using Server.Models;
using Server.Repositories;

namespace Server.ViewModel
{
    public class SketchListViewModel
    {
        private readonly MongoSketchStore _store;

        public ObservableCollection<SketchEntry> Sketches { get; } = new();

        public ICommand DeleteCommand { get; }

        public SketchListViewModel(MongoSketchStore sketchStore)
        {
            _store = sketchStore;

            DeleteCommand = new RelayCommand<SketchEntry>(async entry =>
            {
                if (entry is null || entry.Id == ObjectId.Empty) return;
                await _store.DeleteSketchByIdAsync(entry.Id);
            });
        }

        public async Task ListenAsync(SketchEventBus<SketchEvent> bus, CancellationToken token)
        {
            await foreach (SketchEvent evt in bus.Events.WithCancellation(token))
            {
                if (evt.EventType == SketchEventType.Inserted)
                {
                    if (Sketches.Any(s => s.Name == evt.SketchName)) continue;
                    Result<SketchDto> result = await _store.GetByNameAsync(evt.SketchName);
                    if (!result.IsSuccess) continue;

                    var dto = result.Value;
                    Sketches.Add(new SketchEntry { Id = dto.Id, Name = dto.Name });
                }
                else if (evt.EventType == SketchEventType.Deleted)
                {
                    SketchEntry item = Sketches.FirstOrDefault(s => s.Name == evt.SketchName);
                    if (item != null)
                        Sketches.Remove(item);
                }
            }
        }
    }
}