using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Server.Repositories;
using Server.Services;

namespace Server.Handlers
{
    public interface IRequestHandler
    {
        public bool CanHandle(string request);
        public Task<Result<string>> HandleAsync(RequestContext context);
    }

    public class RequestContext
    {
        public string Request { get; set; }
        public NetworkStream Stream { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public MongoSketchStore MongoStore { get; set; }
        public LockManager LockManager { get; set; }
    }
}