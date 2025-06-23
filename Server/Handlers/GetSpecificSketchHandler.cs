using System;
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

        private GetSpecificSketchHandler() { }

        public bool CanHandle(string request)
            => request.StartsWith("GET:SPECIFIC:", StringComparison.OrdinalIgnoreCase);

        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            var sketchName = context.Request.Substring(LengthOfRequestPrefix);
            Console.WriteLine("[SERVER] Handling GET:SPECIFIC for sketch: " + sketchName);

            if (context.CancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("[SERVER] Request canceled.");
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Server.Suspended, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }

            if (context.LockManager.IsLocked(sketchName))
            {
                Console.WriteLine($"[SERVER] Sketch '{sketchName}' is LOCKED.");
                await ResponseHelper.SendAsync(context.Stream, AppErrors.File.Locked, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.Locked);
            }

            try
            {
                var sketchResult = await context.MongoStore.GetByNameAsync(sketchName);

                if (sketchResult.Error != null || sketchResult.Value == null)
                {
                    Console.WriteLine($"[SERVER] Sketch '{sketchName}' not found or is null.");
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.SketchNotFound, context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.SketchNotFound);
                }

                await ResponseHelper.SendAsync(context.Stream, sketchResult, context.CancellationToken);
                Console.WriteLine($"[SERVER] Sent sketch '{sketchName}' successfully.");
                return Result<string>.Success("Sketch sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] Error while fetching sketch: {ex.Message}");
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}
