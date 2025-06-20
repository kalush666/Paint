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

            Console.WriteLine("▶ UploadSketchAsync started...");
            Console.WriteLine($"📤 Serialized Sketch JSON:\n{json}");
            Console.WriteLine($"📨 Full Request:\n{request}");

            using var responseStream = new MemoryStream();
            var responseChunk = new byte[4096];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                Console.WriteLine("✅ Connected to server.");

                await using var stream = client.GetStream();

                await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                Console.WriteLine("📨 Request sent and flushed.");

                int byteRead;
                while ((byteRead =
                           await stream.ReadAsync(responseChunk, 0, responseChunk.Length).ConfigureAwait(false)) > 0)
                {
                    Console.WriteLine($"📦 Received {byteRead} bytes...");
                    responseStream.Write(responseChunk, 0, byteRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Upload failed: {ex.Message}");
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());
            Console.WriteLine($"📥 Raw server response:\n{response}");

            if (response.StartsWith("ERROR:"))
            {
                Console.WriteLine("❌ Server returned error.");
                return Result<string>.Failure(AppErrors.Generic.OperationFailed);
            }

            try
            {
                var result = JsonConvert.DeserializeObject<Result<string>>(response);
                Console.WriteLine(
                    $"🔍 Deserialized Result: IsSuccess={result?.IsSuccess}, Value={result?.Value}, Error={result?.Error}");

                if (result == null || !result.IsSuccess || string.IsNullOrWhiteSpace(result.Value))
                {
                    Console.WriteLine("❌ Deserialization failed or result not successful.");
                    return Result<string>.Failure(result?.Error ?? "Unknown error");
                }

                Console.WriteLine($"✅ Upload successful. Message: {result.Value}");
                return Result<string>.Success(result.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to deserialize response: {ex.Message}");
                return Result<string>.Failure("Failed to deserialize server response.");
            }
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
                    Console.WriteLine(responseBuffer);
                    if (!stream.DataAvailable) break;
                }

                var response = Encoding.UTF8.GetString(responseStream.ToArray());
                Console.WriteLine($"Response: {response}");
                if (response.StartsWith("ERROR:") || response.Equals(AppErrors.Generic.OperationFailed))
                    return Result<List<string>>.Failure(response);

                var result = JsonConvert.DeserializeObject<Result<List<string>>>(response);
                if (result == null)
                {
                    Console.WriteLine("Failed to deserialize response");
                    return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
                }

                if (result.IsSuccess == false || result.Value == null)
                    return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
                Console.WriteLine(string.Join(", ", result.Value));
                return Result<List<string>>.Success(result.Value);
            }
            catch (Exception)
            {
                Console.WriteLine("Error while fetching sketch names");
                return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
            }
        }
    }
}