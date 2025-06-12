using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Server.Handlers;
using Server.Repositories;

namespace Server.Services
{
    public class TcpSketchServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _listenerTask;
        private readonly MongoSketchStore _mongoStore = new();
        private bool _isSuspended = false;
        private readonly object _suspendLock = new object();

        public TcpSketchServer()
        {
            _port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var envPort) ? envPort : 5000;
            _listener = new TcpListener(IPAddress.Any, _port);
        }


        public void StartListener()
        {
            lock (_suspendLock)
            {
                if (_isSuspended) return;
            }

            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"TCP Sketch Server listening on port {_port}");

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
                Console.WriteLine("Suspending server...");
                _isSuspended = true;
                _cancellationTokenSource.Cancel();
                _listener.Stop();
            }
        }

        public void Resume()
        {
            lock (_suspendLock)
            {
                Console.WriteLine("Resuming server...");
                _isSuspended = false;
                _cancellationTokenSource = new CancellationTokenSource();
                StartListener();
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken token)
        {
            try
            {
                lock (_suspendLock)
                {
                    if (_isSuspended)
                    {
                        _ = ResponseHelper.SendAsync(client.GetStream(), AppErrors.Server.Suspended, token);
                        return;
                    }
                }

                await using var stream = client.GetStream();

                using var responseStream = new MemoryStream();
                var responseChunk = new byte[4096];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(responseChunk, 0, responseChunk.Length)) > 0)
                {
                    responseStream.Write(responseChunk, 0, bytesRead);
                    if (!stream.DataAvailable) break;
                }

                var clientRequest = Encoding.UTF8.GetString(responseStream.ToArray()).Trim();
                if (clientRequest.StartsWith("GET:"))
                {
                    var handler = new DownloadHandler(_mongoStore, stream, token, clientRequest.Trim());
                    await handler.HandleAsync();
                }
                else
                {
                    var handler = new UploadHandler(_mongoStore, stream, token, clientRequest);
                    await handler.HandleAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(AppErrors.Server.ClientHandlingCancelled);
            }
            catch (Exception)
            {
                Console.WriteLine(AppErrors.Generic.OperationFailed);
                await ResponseHelper.SendAsync(client.GetStream(), AppErrors.Generic.OperationFailed, token);
            }
            finally
            {
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(AppErrors.Generic.OperationFailed);
                }
            }
        }
    }
}