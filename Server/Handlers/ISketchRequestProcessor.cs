using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;
using Server.Factories;
using Server.Repositories;
using Server.Services;

namespace Server.Handlers
{
    public interface ISketchRequestProcessor
    {
        Task<Result<string>> ProcessAsync(string request, NetworkStream stream, CancellationToken token);
    }

    public class SketchSketchRequestProcessor : ISketchRequestProcessor
    {
        private readonly ISketchHandlerFactory _sketchHandlerFactory;
        private readonly MongoSketchStore _mongoSketchStore;
        private readonly LockManager _lockManager;

        public SketchSketchRequestProcessor(ISketchHandlerFactory sketchHandlerFactory, MongoSketchStore mongoSketchStore,
            LockManager lockManager)
        {
            _sketchHandlerFactory = sketchHandlerFactory;
            _mongoSketchStore = mongoSketchStore;
            _lockManager = lockManager;
        }

        public async Task<Result<string>> ProcessAsync(string request, NetworkStream stream, CancellationToken token)
        {
            var handler = _sketchHandlerFactory.GetHandler(request);
            if (handler == null)
            {
                await ResponseHelper.SendAsync(stream, AppErrors.Request.UnsupportedType, token);
                return Result<string>.Failure(AppErrors.Request.UnsupportedType);
            }

            var context = new RequestContext
            {
                Request = request,
                Stream = stream,
                CancellationToken = token,
                MongoStore = _mongoSketchStore,
                LockManager = _lockManager
            };
            return await handler.HandleAsync(context);
        }
    }
}