using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Utils;
using 

namespace Server.Handlers
{
    class ResponseHandler
    {
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;

        public ResponseHandler(NetworkStream stream, CancellationToken token)
        {
            _stream = stream;
            _token = token;
        }

        public async Task<Result<string>> SendAsync(string message)
        {
            try
            {
                var responseBytes = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(responseBytes, 0, responseBytes.Length, _token);
                await _stream.FlushAsync(_token);
                return Result<string>.Success("Sent Successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure();
            }
            finally
            {
                try
                {
                    _stream.Close();
                }
                catch
                {
                }
            }
        }
    }
}
