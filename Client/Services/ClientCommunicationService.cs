using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared_Models.Models;
using Client.Factories;
using Newtonsoft.Json.Linq;

namespace Client.Services
{
    public class ClientCommunicationService
    {
        private readonly string _serverHost;
        private readonly int _serverPort;

        public ClientCommunicationService(string serverHost = "localhost", int serverPort = 5000)
        {
            _serverHost = serverHost;
            _serverPort = serverPort;
        }

        public async Task<string> UploadSketchAsync(Sketch sketch)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_serverHost, _serverPort);

                using var stream = client.GetStream();

                var json = JsonConvert.SerializeObject(sketch, Formatting.Indented);
                var data = Encoding.UTF8.GetBytes(json);

                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();

                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload sketch: {ex.Message}", ex);
            }
        }

        public async Task<Sketch?> DownloadSketchAsync(string sketchName)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_serverHost, _serverPort);

                using var stream = client.GetStream();

                var request = $"GET:{sketchName}";
                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, 0, requestData.Length);
                await stream.FlushAsync();

                var buffer = new byte[4096];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (response.StartsWith("ERROR:"))
                {
                    throw new Exception(response);
                }

                var json = JObject.Parse(response);
                var sketch = new Sketch();
                sketch.Name = json["Name"]?.ToString() ?? "";

                var shapesArray = json["Shapes"] as JArray;
                if (shapesArray != null)
                {
                    foreach (var shapeJson in shapesArray)
                    {
                        var shape = ShapeFactory.createShapeFromJson(shapeJson as JObject);
                        if (shape != null)
                        {
                            sketch.Shapes.Add(shape);
                        }
                    }
                }

                return sketch;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download sketch: {ex.Message}", ex);
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_serverHost, _serverPort);
                return client.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}