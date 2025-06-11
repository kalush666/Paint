using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using MongoDB.Bson;
using Newtonsoft.Json;
using Server.Repositories;

namespace Server.Handlers
{
    class DownloadHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;
        private readonly string _request;

        public DownloadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken, string request)
        {
            _mongoStore = mongo;
            _stream = stream;
            _token = cancellationToken;
            _request = request;
        }

        public async Task HandleAsync()
        {
            try
            {
                if (_request.Equals("GET:ALL", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Download request for all sketches Names");
                    try
                    {
                        var allJson = await _mongoStore.GetAllJsonAsync();
                        var names = allJson.Select(json =>
                        {
                            try
                            {
                                var doc = BsonDocument.Parse(json);
                                return doc.GetValue("Name", "").AsString;
                            }
                            catch
                            {
                                return AppErrors.Mongo.ReadError;
                            }
                        }).Where(name => !string.IsNullOrWhiteSpace(name)).ToList();

                        var resultJson = JsonConvert.SerializeObject(names, Formatting.None);
                        await ResponseHelper.SendAsync(_stream, resultJson, _token);
                    }
                    catch
                    {
                        await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.ReadError, _token);
                    }
                    return;
                }

                var sketchName = _request.Substring(4);
                Console.WriteLine($"Download request for: {sketchName}");

                try
                {
                    var sketchJson = await _mongoStore.GetJsonByNameAsync(sketchName);
                    if (sketchJson == null)
                    {
                        await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.SketchNotFound, _token);
                    }
                    else
                    {
                        Console.WriteLine($"Sending sketch: {sketchName}");
                        await ResponseHelper.SendAsync(_stream, sketchJson, _token);
                    }
                }
                catch
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(AppErrors.Generic.OperationFailed);
                await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
            }
        }
    }
}
