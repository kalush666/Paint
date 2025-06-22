using System;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public class GetSpecificSketchHandler : IRequestHandler
    {
        private const int LengthOfRequestPrefix = 13;
        public static readonly Lazy<GetSpecificSketchHandler> _instance = new(() => new GetSpecificSketchHandler());
        public static GetSpecificSketchHandler Instance => _instance.Value;

        private GetSpecificSketchHandler()
        {
        }

        public bool CanHandle(string request)
            => request.StartsWith("GET:SPECIFIC:", StringComparison.OrdinalIgnoreCase);

        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            var sketchName = context.Request.Substring(LengthOfRequestPrefix);

            if (context.CancellationToken.IsCancellationRequested)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Server.Suspended, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }

            if (context.LockManager.IsLocked(sketchName))
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.File.Locked, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.Locked);
            }

            try
            {
                var sketch = await context.MongoStore.GetByNameAsync(sketchName);
                if (sketch.Error != null)
                {
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.SketchNotFound, context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.SketchNotFound);
                }

                var json = JsonSerializer.Serialize(sketch.Value);
                await ResponseHelper.SendAsync(context.Stream, json, context.CancellationToken);
                return Result<string>.Success(json);
            }
            catch
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}
