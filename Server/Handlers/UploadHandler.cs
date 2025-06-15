using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json.Linq;
using Server.Repositories;

namespace Server.Handlers
{
    public class UploadHandler
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
                var jsonSketch = JObject.Parse(_jsonInput);
                sketchName = jsonSketch["Name"]?.ToString();

                if (string.IsNullOrWhiteSpace(sketchName))
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.AlreadyExists, _token);
                    return;
                }
                

                Console.WriteLine($"Upload request for: {sketchName}");

                try
                {
                    await _mongoStore.InsertJsonAsync(_jsonInput);
                    Console.WriteLine($"Sketch '{sketchName}' uploaded successfully");
                    await ResponseHelper.SendAsync(_stream, "Sketch uploaded successfully", _token);
                }
                catch
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.ReadError, _token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed for '{sketchName}': {ex.Message}");
                await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
            }
        }
    }
}
