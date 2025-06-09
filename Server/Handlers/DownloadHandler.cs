using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Shared_Models.Models;
using Newtonsoft.Json;

namespace Server.Handlers
{
    class DownloadHandler
    {
        private readonly MongoSketchStore _mongoStore;
        private readonly NetworkStream _stream;
        private readonly CancellationToken _token;

        public DownloadHandler(MongoSketchStore mongo, NetworkStream stream, CancellationToken cancellationToken)
        {
            this._mongoStore = mongo;
            this._stream = stream;
            this._token = cancellationToken;
        }

        public async Task HandleAsync()
        {
            try
            {
                var buffer = new byte[1024];
                var byteRead = await _stream.ReadAsync(buffer,0, buffer.Length,  _token);
                var sketchName = Encoding.UTF8.GetString(buffer, 0, byteRead).Trim();

                if (!FileLockManager.tryLock(sketchName))
                {
                    await SendResponseAcync($"sketch {sketchName} is locked");
                    return;
                }
            }
        }
    }
}
