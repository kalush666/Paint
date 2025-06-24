using System;
using System.Threading.Tasks;
using Common.DTO;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public class GetSpecificSketchHandler : ISketchRequestHandler
    {
        private const int LengthOfRequestPrefix = 13;
        private const string GetSpecificRequest = "GET:SPECIFIC:";

        private static GetSpecificSketchHandler _instance;

        public static GetSpecificSketchHandler GetInstance()
        {
            return _instance ??= new GetSpecificSketchHandler();
        }

        private GetSpecificSketchHandler()
        {
        }

        public bool CanHandle(string request)
            => request.StartsWith(GetSpecificRequest, StringComparison.OrdinalIgnoreCase);

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
                Result<SketchDto> sketchResult = await context.MongoStore.GetByNameAsync(sketchName);

                if (sketchResult.Error != null || sketchResult.Value == null)
                {
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.SketchNotFound,
                        context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.SketchNotFound);
                }

                await ResponseHelper.SendAsync(context.Stream, sketchResult, context.CancellationToken);
                return Result<string>.Success("Sketch sent successfully");
            }
            catch (Exception ex)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}