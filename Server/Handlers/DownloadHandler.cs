using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Services;

namespace Server.Handlers
{
    class DownloadHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;

        public DownloadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken)
        {
            this._mongoStore = mongo;
            this._stream = stream;
            this._token = cancellationToken;
        }

        public async Task HandleAsync()
        {
            try
            {
                var buffer = new byte[1024];
                var byteRead = await _stream.ReadAsync(buffer, 0, buffer.Length, _token);
                var sketchName = Encoding.UTF8.GetString(buffer, 0, byteRead).Trim();

                if (!LockManager.TryLock(sketchName))
                {
                    await SendResponseAsync($"ERROR: sketch '{sketchName}' is locked");
                    return;
                }

                try
                {
                    var sketchJson = await _mongoStore.GetJsonByNameAsync(sketchName);
                    if (sketchJson == null)
                    {
                        await SendResponseAsync($"ERROR: sketch '{sketchName}' not found");
                    }
                    else
                    {
                        await SendResponseAsync(sketchJson);
                    }
                }
                finally
                {
                    LockManager.Unlock(sketchName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadHandler error: {ex.Message}");
                await SendResponseAsync("ERROR: Exception occurred while processing download");
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
