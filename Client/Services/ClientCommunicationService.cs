using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MongoDB.Bson;

namespace Client.Services
{
    public class ClientCommunicationService
    {
        private const int DefaultChunkSize = 4096;
        private readonly string _serverHost;
        private readonly int _serverPort;
        private readonly int _timeoutMs;
        private const string UploadSketchRequestPrefix = "POST:";
        private const string DownloadSketchRequestPrefix = "GET:SPECIFIC:";
        private const string GetAllSketchNamesRequest = "GET:ALL:NAMES";
        private const int Offset = 0;

        public ClientCommunicationService(string serverHost = Ports.DefaultHost, int serverPort = Ports.DefaultPort,
            int timeoutMs = Ports.DefaultTimeout)
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
            var json = JsonConvert.SerializeObject(sketchDto, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var request = $"{UploadSketchRequestPrefix}{json}";
            var data = Encoding.UTF8.GetBytes(request);

            using var responseStream = new MemoryStream();
            var responseChunk = new byte[DefaultChunkSize];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();

                await stream.WriteAsync(data, Offset, data.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                int byteRead;
                while ((byteRead =
                           await stream.ReadAsync(responseChunk, Offset, responseChunk.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(responseChunk, Offset, byteRead);
                    if (!stream.DataAvailable) break;
                }

                var responseJson = Encoding.UTF8.GetString(responseStream.ToArray());
                return JsonConvert.DeserializeObject<Result<string>>(responseJson)
                       ?? Result<string>.Failure(AppErrors.Generic.OperationFailed);
            }
            catch (SocketException)
            {
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }
            catch
            {
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }
        }


        public async Task<Result<Sketch>?> DownloadSketchAsync(string sketchName)
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = _timeoutMs;
            client.SendTimeout = _timeoutMs;

            var request = $"{DownloadSketchRequestPrefix}{sketchName}";

            using var responseStream = new MemoryStream();
            var requestBuffer = new byte[DefaultChunkSize];

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                await using var stream = client.GetStream();

                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, Offset, requestData.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                int bytesRead;
                while ((bytesRead =
                           await stream.ReadAsync(requestBuffer, Offset, requestBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(requestBuffer,Offset, bytesRead);
                    if (!stream.DataAvailable) break;
                }
            }
            catch (SocketException)
            {
                return Result<Sketch>.Failure(AppErrors.Server.Suspended);
            }
            catch (Exception ex)
            {
                return Result<Sketch>.Failure(AppErrors.Mongo.ReadError);
            }

            var response = Encoding.UTF8.GetString(responseStream.ToArray());

            if (response.StartsWith("ERROR:") || response.Contains(AppErrors.File.Locked))
            {
                return Result<Sketch>.Failure(response);
            }

            try
            {
                var result = JsonConvert.DeserializeObject<Result<SketchDto>>(response);
                if (result?.Value == null)
                {
                    return Result<Sketch>.Failure("Failed to deserialize response");
                }

                Sketch sketch = result.Value.ToDomain();
                return Result<Sketch>.Success(sketch);
            }
            catch (Exception ex)
            {
                return Result<Sketch>.Failure("Deserialization failed");
            }
        }


        public async Task<Result<List<string>>> GetAllSketchNames()
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = _timeoutMs;
            client.SendTimeout = _timeoutMs;

            try
            {
                await client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);
                using var stream = client.GetStream();

                var request = GetAllSketchNamesRequest;
                var requestData = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestData, 0, requestData.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                using var responseStream = new MemoryStream();
                var responseBuffer = new byte[DefaultChunkSize];
                int bytesRead;
        
                while ((bytesRead =
                           await stream.ReadAsync(responseBuffer, Offset, responseBuffer.Length).ConfigureAwait(false)) > 0)
                {
                    responseStream.Write(responseBuffer, Offset, bytesRead);
                    if (!stream.DataAvailable) break;
                }

                var response = Encoding.UTF8.GetString(responseStream.ToArray());

                if (response.StartsWith("ERROR:") || response.Equals(AppErrors.Generic.OperationFailed))
                    return Result<List<string>>.Failure(response);

                var result = JsonConvert.DeserializeObject<Result<IEnumerable<string>>>(response);
                if (result == null || result.IsSuccess == false || result.Value == null)
                    return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);

                return Result<List<string>>.Success(result.Value.ToList());
            }
            catch (SocketException)
            {
                return Result<List<string>>.Failure(AppErrors.Server.Suspended);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Failure(AppErrors.Generic.OperationFailed);
            }
        }
    }
}