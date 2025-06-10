using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Services;
using Newtonsoft.Json.Linq;

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
            string sketchName = null;
            try
            {
                var jsonObj = JObject.Parse(_jsonInput);
                sketchName = jsonObj["Name"]?.ToString();

                if (string.IsNullOrWhiteSpace(sketchName))
                {
                    await SendResponseAsync("ERROR: Missing sketch name");
                    return;
                }

                Console.WriteLine($"Upload request for: {sketchName}");

                if (!LockManager.TryLock(sketchName))
                {
                    await SendResponseAsync($"ERROR: sketch '{sketchName}' is currently being accessed by another client");
                    return;
                }

                try
                {
                    await _mongoStore.InsertJsonAsync(_jsonInput);
                    Console.WriteLine($"Sketch '{sketchName}' uploaded successfully");
                    await SendResponseAsync("Sketch uploaded successfully");
                }
                finally
                {
                    LockManager.Unlock(sketchName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed for '{sketchName}': {ex.Message}");
                await SendResponseAsync($"ERROR: {ex.Message}");

                if (!string.IsNullOrWhiteSpace(sketchName))
                {
                    LockManager.Unlock(sketchName);
                }
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