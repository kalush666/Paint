using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Server.Repositories;
using Server.Services;

namespace Server.Handlers
{
    class DownloadHandler : ISketchHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;
        private readonly string _request;
        private readonly LockManager _lockManager;

        public DownloadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken,
            string request, LockManager lockManager)
        {
            _mongoStore = mongo;
            _stream = stream;
            _token = cancellationToken;
            _request = request;
            _lockManager = lockManager;
        }

        public async Task HandleAsync()
        {
            try
            {
                if (_request.Equals("GET:ALL", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Download request for all sketches");
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

                if (_request.Equals("GET:ALL:NAMES",StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Download request for all sketches");
                    try
                    {
                        var allSketchNames = await _mongoStore.GetAllSketchNamesAsync();
                        if (allSketchNames.Error != null) 
                        {
                            await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.ReadError, _token);
                        }
                        else
                        {
                            var resultJson = JsonConvert.SerializeObject(allSketchNames.Value ?? new List<string>(), Formatting.None);
                            await ResponseHelper.SendAsync(_stream, resultJson, _token);
                        }
                    }
                    catch (Exception ex)
                    {
                        await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.ReadError, _token);
                    }
                    return;
                }

                var sketchName = _request.Substring(4);
                Console.WriteLine($"Download request for: {sketchName}");
                CancellationToken lockToken = _token;

                if (!_lockManager.TryLock(sketchName,out lockToken))
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.File.Locked, _token);
                    return;
                }

                try
                {
                    var sketchJson = await _mongoStore.GetJsonByNameAsync(sketchName);
                    if (sketchJson == null)
                    {
                        await ResponseHelper.SendAsync(_stream, AppErrors.Mongo.SketchNotFound, _token);
                    }
                    else
                    {
                        await ResponseHelper.SendAsync(_stream, sketchJson, _token);
                    }
                }
                catch (OperationCanceledException)
                {
                    await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationCancelled, _token);
                }
            }
            catch (Exception ex)
            {
                await ResponseHelper.SendAsync(_stream, AppErrors.Generic.OperationFailed, _token);
            }
        }
    }
}
