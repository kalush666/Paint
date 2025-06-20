using System;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public class GetSpecificSketchHandler : IRequestHandler
    {
        private const int LengthOfRequestPrefix = 13;

        public bool CanHandle(string request)
            => request.StartsWith("GET:SPECIFIC:", StringComparison.OrdinalIgnoreCase);

        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            var sketchName = context.Request.Substring(LengthOfRequestPrefix);
            if (!context.LockManager.TryLock(sketchName, out var lockToken))
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.File.Locked, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.Locked);
            }

            try
            {
                var sketch = await context.MongoStore.GetByNameAsync(sketchName);
                if (sketch.Error != null)
                {
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.SketchNotFound,
                        context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.SketchNotFound);
                }

                await ResponseHelper.SendAsync(context.Stream, sketch, context.CancellationToken);
                return Result<string>.Success(sketch.Value.ToString());
            }
            catch (OperationCanceledException)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.File.AccessDenied, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.AccessDenied);
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.Locked);
            }
        }
    }
}