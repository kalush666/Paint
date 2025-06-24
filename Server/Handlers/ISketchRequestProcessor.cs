using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;

namespace Server.Handlers
{
    public interface ISketchRequestProcessor
    {
        Task<Result<string>> ProcessAsync(string request, NetworkStream stream, CancellationToken token);
    }
    
}