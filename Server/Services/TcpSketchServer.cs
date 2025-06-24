using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using Server.Factories;
using Server.Handlers;
using Server.Repositories;

namespace Server.Services
{
    public class TcpSketchServer
    {
        private const int SizeOfChunk = 4096;
        private const int Offset = 0;

        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _listenerTask;
        private readonly ISketchRequestProcessor _sketchRequestProcessor;
        private readonly ISketchHandlerFactory _sketchHandlerFactory;
        private readonly MongoSketchStore _mongoStore;
        private bool _isSuspended = false;
        private readonly object _suspendLock = new object();
        private readonly LockManager _lockManager = new();

        public TcpSketchServer(MongoSketchStore sketchStore,ISketchRequestProcessor? requestProcessor = null, ISketchHandlerFactory? handlerFactory = null)
        {
            _port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var envPort) ? envPort : Ports.DefaultPort;
            _listener = new TcpListener(IPAddress.Any, _port);
            _mongoStore = sketchStore;
            _sketchHandlerFactory = handlerFactory ?? new SketchRequestFactory();
            _sketchRequestProcessor = requestProcessor ?? new SketchSketchRequestProcessor(_sketchHandlerFactory, _mongoStore, _lockManager);
        }


        public void StartListener()
        {
            lock (_suspendLock)
            {
                if (_isSuspended) return;
            }

            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();

            _listenerTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        _ = Task.Run(() => HandleClient(client, _cancellationTokenSource.Token));
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine(AppErrors.Server.ListenerCancelled);
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(AppErrors.Server.AcceptClientError);
                    }
                }
            });
        }

        public void Suspend()
        {
            lock (_suspendLock)
            {
                _isSuspended = true;
                _cancellationTokenSource.Cancel();
                _listener.Stop();
            }
        }

        public void Resume()
        {
            lock (_suspendLock)
            {
                _isSuspended = false;
                _cancellationTokenSource = new CancellationTokenSource();
                StartListener();
            }
        }

        private async Task<Result<string>> HandleClient(TcpClient client, CancellationToken token)
        {
            bool isSuspended;
            lock (_suspendLock)
            {
                isSuspended = _isSuspended;
            }

            if (isSuspended)
            {
                await ResponseHelper.SendAsync(client.GetStream(), AppErrors.Server.Suspended, token);
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }

            try
            {
                await using var stream = client.GetStream();
                using var requestStream = new MemoryStream();
                var requestChunk = new byte[SizeOfChunk];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(requestChunk, Offset, requestChunk.Length, token)) > 0)
                {
                    requestStream.Write(requestChunk, Offset, bytesRead);
                    if (!stream.DataAvailable) break;
                }

                var clientRequest = Encoding.UTF8.GetString(requestStream.ToArray());
                Result<string> precessingResult = await _sketchRequestProcessor.ProcessAsync(clientRequest, stream, token);

                return precessingResult.Error != null
                    ? Result<string>.Failure(precessingResult.Error)
                    : Result<string>.Success(Encoding.UTF8.GetString(requestStream.ToArray()));
            }
            catch (OperationCanceledException)
            {
                await ResponseHelper.SendAsync(client.GetStream(), AppErrors.Server.Suspended, token);
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(client.GetStream(), AppErrors.Generic.OperationFailed, token);
                return Result<string>.Failure(AppErrors.Generic.OperationFailed);
            }
        }
    }
}