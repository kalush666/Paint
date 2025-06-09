using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Server;

namespace PainterServer.Services
{
    public class TcpSketchServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _listenerTask;
        private readonly MongoSketchStore _mongoStore = new();

        public TcpSketchServer()
        {
            _port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var envPort) ? envPort : 5000;
            _listener = new TcpListener(IPAddress.Any, _port);
        }

        public void StartListener()
        {
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
                        Console.WriteLine("Listener task cancelled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accepting client: {ex.Message}");
                    }
                }
            });
        }

        public void Suspend()
        {
            Console.WriteLine("Suspending server...");
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }

        public void Resume()
        {
            Console.WriteLine("Resuming server...");
            _cancellationTokenSource = new CancellationTokenSource();
            StartListener();
        }

        private async Task HandleClient(TcpClient client, CancellationToken token)
        {
            try
            {
                using var stream = client.GetStream();
                var buffer = new byte[8192];
                var requestData = new List<byte>();
                int totalBytes = 0;

                while (stream.DataAvailable || totalBytes == 0)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break;

                    requestData.AddRange(buffer[..bytesRead]);
                    totalBytes += bytesRead;

                    if (stream.DataAvailable)
                        await Task.Delay(10, token);
                    else
                        break;
                }

                string jsonInput = Encoding.UTF8.GetString(requestData.ToArray()).Trim();
                Console.WriteLine($"Received data: {jsonInput}");

                if (jsonInput.StartsWith("GET:"))
                {
                    string sketchName = jsonInput[4..].Trim();
                    if (!FileLockManager.TryLock(sketchName))
                    {
                        await SendResponse(stream, $"ERROR: sketch '{sketchName}' is locked");
                        return;
                    }

                    try
                    {
                        string? sketchJson = await _mongoStore.GetJsonByNameAsync(sketchName);
                        if (sketchJson == null)
                        {
                            await SendResponse(stream, $"ERROR: sketch '{sketchName}' not found");
                        }
                        else
                        {
                            await SendResponse(stream, sketchJson);
                        }
                    }
                    finally
                    {
                        FileLockManager.Unlock(sketchName);
                    }
                }
                else
                {
                    try
                    {
                        await _mongoStore.InsertJsonAsync(jsonInput);
                        Console.WriteLine("Sketch inserted successfully.");
                        await SendResponse(stream, "Sketch uploaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Insert failed: {ex.Message}");
                        await SendResponse(stream, $"ERROR: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Client handling was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in client handler: {ex.Message}");
                await SendResponse(client.GetStream(), "ERROR: Exception occurred");
            }
            finally
            {
                try { client.Close(); } catch { }
            }
        }

        private async Task SendResponse(NetworkStream stream, string response)
        {
            try
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                await stream.FlushAsync();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending response: {ex.Message}");
            }
        }
    }
}
