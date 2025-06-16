using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Server.Events
{
    public class SketchEventBus<T>
    {
        private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

        public ValueTask PublishAsync(T evt) => _channel.Writer.WriteAsync(evt);

        public IAsyncEnumerable<T> Events => _channel.Reader.ReadAllAsync();
    }
}