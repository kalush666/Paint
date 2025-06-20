using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client.Mappers;
using Newtonsoft.Json;
using Common.Errors;
using Common.Helpers;
using Client.Models;
using Common.Constants;
using Common.DTO;

namespace Client.Services
{
    public class ClientCommunicationService
    {
        private const int DefaultChunkSize = 4096;
        private readonly string _serverHost;
        private readonly int _serverPort;
        private readonly int _timeoutMs;

        public ClientCommunicationService(string serverHost = Ports.DefaultHost, int serverPort = Ports.DefaultPort, int timeoutMs = Ports.DefaultTimeout)
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

            var sketchDto = sketch.ToDto();
            var json = JsonConvert.SerializeObject(sketchDto, Formatting.Indented);
            var request = $"POST:{json}";
            var data = Encoding.UTF8.GetBytes(request);

            Console.WriteLine("[UploadSketchAsync] Starting upload...");
            Console.WriteLine("[UploadSketchAsync] Serialized DTO:");
            Console.WriteLine(json);
            Console.WriteLine("[UploadSketchAsync] Encoded byte length: " + data.Length);

            using var responseStream = new MemoryStream();
            var responseChunk = new byte[DefaultChunkSize];

            try
            {
                Console.WriteLine("[UploadSketchAsync] Connecting to server...");
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();
                Console.WriteLine("[UploadSketchAsync] Connected. Sending data...");

                await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                Console.WriteLine("[UploadSketchAsync] Data sent and flushed.");

                int byteRead;
                while ((byteRead = await stream.ReadAsync(responseChunk, 0, responseChunk.Length).ConfigureAwait(false)) > 0)
                {
                    Console.WriteLine("[UploadSketchAsync] Received bytes: " + byteRead);
                    responseStream.Write(responseChunk, 0, byteRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UploadSketchAsync] Error during upload: " + ex.Message);
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());
            Console.WriteLine("[UploadSketchAsync] Server response:");
            Console.WriteLine(response);

            if (response.StartsWith("ERROR:"))
                return Result<string>.Failure(AppErrors.Generic.OperationFailed);
            
            Console.WriteLine("[UploadSketchAsync] Treating response as raw sketch name.");
            return Result<string>.Success(response.Trim('"'));
        }

        public async Task<Result<Sketch>?> DownloadSketchAsync(string sketchName)
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = _timeoutMs;
            client.SendTimeout = _timeoutMs;
            var request = $"GET:SPECIFIC:{sketchName}";
            using var responseStream = new MemoryStream();
            var requestBuffer = new byte[DefaultChunkSize];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();

                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, 0, requestData.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(requestBuffer, 0, requestBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(requestBuffer, 0, bytesRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (Exception)
            {
                return Result<Sketch>.Failure(AppErrors.Mongo.ReadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());
            if (response.StartsWith("ERROR:"))
                return Result<Sketch>.Failure(AppErrors.Generic.OperationFailed);

            var dto = JsonConvert.DeserializeObject<SketchDto>(response);
            if (dto == null)
                return Result<Sketch>.Failure("Failed to deserialize SketchDto");

            var sketch = dto.ToDomain();
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
                var responseBuffer = new byte[DefaultChunkSize];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(responseBuffer, 0, bytesRead);
                    if (!stream.DataAvailable) break;
                }

                var response = Encoding.UTF8.GetString(responseStream.ToArray());
                if (response.StartsWith("ERROR:") || response.Equals(AppErrors.Generic.OperationFailed))
                    return Result<List<string>>.Failure(response);

                var result = JsonConvert.DeserializeObject<Result<List<string>>>(response);
                if (result == null || result.IsSuccess == false || result.Value == null)
                    return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);

                return Result<List<string>>.Success(result.Value);
            }
            catch (Exception)
            {
                return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
            }
        }
    }
}
