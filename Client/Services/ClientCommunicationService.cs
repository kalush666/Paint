using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client.Convertors;
using Newtonsoft.Json;
using Client.Factories;
using Client.Models;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using Common.Models;
using Newtonsoft.Json.Linq;

namespace Client.Services
{
    public class ClientCommunicationService
    {
        private readonly string _serverHost;
        private readonly int _serverPort;
        private readonly int _timeoutMs;

        public ClientCommunicationService(string serverHost = "localhost", int serverPort = 5000, int timeoutMs = 10000)
        {
            _serverHost = serverHost;
            _serverPort = serverPort;
            _timeoutMs = timeoutMs;
        }

        public async Task<Result<string>> UploadSketchAsync(Sketch sketch)
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = _timeoutMs;
            client.SendTimeout = _timeoutMs;
            var json = JsonConvert.SerializeObject(sketch, Formatting.Indented);
            var request = $"POST:{json}";
            var data = Encoding.UTF8.GetBytes(request);
            using var responseStream = new MemoryStream();
            var responseChunk = new byte[4096];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();

                await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                int byteRead;
                while ((byteRead =
                           await stream.ReadAsync(responseChunk, 0, responseChunk.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(responseChunk, 0, byteRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());
            if (response.StartsWith("ERROR:"))
            {
                return Result<string>.Failure(AppErrors.Generic.OperationFailed);
            }

            return Result<string>.Success(response);
        }

        public async Task<Result<Sketch>?> DownloadSketchAsync(string sketchName)
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = _timeoutMs;
            client.SendTimeout = _timeoutMs;
            var request = $"GET:SPECIFIC:{sketchName}";
            using var responseStream = new MemoryStream();
            var requestBuffer = new byte[4096];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();

                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, 0, requestData.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                int bytesRead;
                while ((bytesRead =
                           await stream.ReadAsync(requestBuffer, 0, requestBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(requestBuffer, 0, bytesRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (Exception ex)
            {
                return Result<Sketch>.Failure(AppErrors.Mongo.ReadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());

            if (response.StartsWith("ERROR:"))
            {
                return Result<Sketch>.Failure(AppErrors.Generic.OperationFailed);
            }

            var json = JObject.Parse(response);
            var sketch = new Sketch
            {
                Name = json[SketchFields.Name]?.ToString() ?? ""
            };

            if (json[SketchFields.Shapes] is not JArray shapesArray) return Result<Sketch>.Success(sketch);

            foreach (var shapeJson in shapesArray)
            {
                var shape = JsonToShapeConvertor.ConvertToShape(shapeJson as JObject);
                if (shape != null)
                {
                    sketch.Shapes.Add(shape);
                }
            }

            return Result<Sketch>.Success(sketch);
        }

        public async Task<Result<List<string>>> GetAllSketchNames()
        {
            try
            {
                using var client = new TcpClient();
                client.ReceiveTimeout = _timeoutMs;
                client.SendTimeout = _timeoutMs;

                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                using var stream = client.GetStream();

                var request = "GET:ALL:NAMES";
                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, 0, requestData.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                using var responseStream = new MemoryStream();
                var responseBuffer = new byte[4096];
                int bytesRead;
                while ((bytesRead =
                           await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(responseBuffer, 0, bytesRead);
                    if (!stream.DataAvailable) break;
                }

                var response = Encoding.UTF8.GetString(responseStream.ToArray());
                if (response.StartsWith("ERROR:") || response.Equals(AppErrors.Generic.OperationFailed))
                    return Result<List<string>>.Failure(response);

                var jsonArray = JArray.Parse(response);
                var names = jsonArray.Select(j => j.ToString()).ToList();

                return Result<List<string>>.Success(names);
            }
            catch (Exception)
            {
                return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
            }
        }
    }
}