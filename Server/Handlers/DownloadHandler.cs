using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Repositories;
using Server.Services;

namespace Server.Handlers
{
    class DownloadHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;
        private readonly string _sketchName;

        public DownloadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken, string sketchName)
        {
            this._mongoStore = mongo;
            this._stream = stream;
            this._token = cancellationToken;
            this._sketchName = sketchName;
        }

        public async Task HandleAsync()
        {
            try
            {
                Console.WriteLine($"Download request for: {_sketchName}");

                if (!LockManager.TryLock(_sketchName,out var lockToken))
                {
                    await SendResponseAsync($"ERROR: sketch '{_sketchName}' is locked");
                    return;
                }

                try
                {
                    var sketchJson = await _mongoStore.GetJsonByNameAsync(_sketchName);
                    if (sketchJson == null)
                    {
                        await SendResponseAsync($"ERROR: sketch '{_sketchName}' not found");
                    }
                    else
                    {
                        Console.WriteLine($"Sending sketch: {_sketchName}");
                        await SendResponseAsync(sketchJson);
                    }
                }
                catch(Exception ex)
                {
                    await SendResponseAsync("ERROR: Exception occurred while processing download");
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
            try
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await _stream.WriteAsync(responseBytes, 0, responseBytes.Length, _token);
                await _stream.FlushAsync(_token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending response: {ex.Message}");
            }
            finally
            {
                try { _stream.Close(); } catch { }
            }
        }
    }
}