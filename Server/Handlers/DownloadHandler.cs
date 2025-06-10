using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Server.Repositories;

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

                try
                {
                    var sketchJson = await _mongoStore.GetJsonByNameAsync(_sketchName);
                    if (sketchJson == null)
                    {
                        await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.SketchNotFound, _token);
                    }
                    else
                    {
                        Console.WriteLine($"Sending sketch: {_sketchName}");
                        await ResponseHelper.SendAsync(_stream, sketchJson, _token);
                    }
                }
                catch (Exception ex)
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadHandler error: {ex.Message}");
                await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
            }
        }
    }
}