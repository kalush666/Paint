using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Newtonsoft.Json;

namespace Common.Helpers
{
    public static class ResponseHelper
    {
        public static async Task<Result<string>> SendAsync<T>(NetworkStream stream, T data, CancellationToken token)
        {
            try
            {
                string message = JsonConvert.SerializeObject(data);
                var responseBytes = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length, token);
                await stream.FlushAsync(token);
                return Result<string>.Success("Sent Successfully");
            }
            catch
            {
                return Result<string>.Failure(AppErrors.Response.UnableToSend);
            }
            finally
            {
                try { stream.Close(); } catch { }
            }
        }
    }
}