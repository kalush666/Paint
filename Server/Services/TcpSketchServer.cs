using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Shared_Models.Models;
using Newtonsoft.Json;
using Server;

namespace PainterServer.Services
{
    public class TcpSketchServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly MongoSketchStore _mongoStore = new();

        public TcpSketchServer()
        {
            _port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var envPort) ? envPort : 5000;
            _listener = new TcpListener(IPAddress.Any, _port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"TCP Sketch Server listening on port {_port}");

            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        TcpClient client = await _listener.AcceptTcpClientAsync();
                        _ = Task.Run(() => HandleClient(client));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accepting client: {ex.Message}");
                    }
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }

        private async Task HandleClient(TcpClient client)
        {
            try
            {
                await _semaphore.WaitAsync();

                using var stream = client.GetStream();
                var buffer = new byte[8192];
                var requestData = new List<byte>();
                int totalBytes = 0;

                while (stream.DataAvailable || totalBytes == 0)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    requestData.AddRange(buffer[..bytesRead]);
                    totalBytes += bytesRead;

                    if (stream.DataAvailable)
                        await Task.Delay(10);
                    else
                        break;
                }

                string jsonInput = Encoding.UTF8.GetString(requestData.ToArray());
                Console.WriteLine($"Received data: {jsonInput}");
                Console.WriteLine($"Received bytes: {totalBytes}");

                Sketch? sketch = JsonConvert.DeserializeObject<Sketch>(jsonInput);

                if (sketch == null || sketch.Shapes == null)
                {
                    Console.WriteLine("Invalid sketch received.");
                    await SendResponse(stream, "ERROR: Invalid sketch");
                    return;
                }

                await _mongoStore.InsetSketchAsync(sketch);
                Console.WriteLine($"Sketch '{sketch.Name}' added with {sketch.Shapes.Count} shapes.");

                await SendResponse(stream, $"Sketch '{sketch.Name}' uploaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in client handler: {ex.Message}");
                await SendResponse(client.GetStream(), "ERROR: Exception occurred");
            }
            finally
            {
                _semaphore.Release();
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
