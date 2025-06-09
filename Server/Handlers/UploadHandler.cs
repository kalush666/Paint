using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Services;

namespace Server.Handlers
{
    class UploadHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;
        private readonly string _jsonInput;

        public UploadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken, string jsonInput)
        {
            _mongoStore = mongo;
            _stream = stream;
            _token = cancellationToken;
            _jsonInput = jsonInput;
        }

        public async Task HandleAsync()
        {
            try
            {
                await _mongoStore.InsertJsonAsync(_jsonInput);
                Console.WriteLine("Sketch Inserted");
                await SendResponseAsync("sketch Inserted");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insrtion failed: {ex.Message}");
                await SendResponseAsync($"error: {ex.Message}");
            }

        }
        private async Task SendResponseAsync(string response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            await _stream.WriteAsync(responseBytes, 0, responseBytes.Length, _token);
            await _stream.FlushAsync(_token);
            _stream.Close();
        }

    }
}
