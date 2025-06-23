using System;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public sealed class GetAllSketchesHandler : IRequestHandler
    {
        private static GetAllSketchesHandler _instance;

        public static GetAllSketchesHandler GetInstance()
        {
            return _instance ??= new GetAllSketchesHandler();
        }

        private GetAllSketchesHandler()
        {
        }

        public bool CanHandle(string request) => request.Equals("GET:ALL", StringComparison.OrdinalIgnoreCase);


        public async Task<Result<String>> HandleAsync(RequestContext context)
        {
            try
            {
                var allSketches = await context.MongoStore.GetAllSketchesAsync();
                await ResponseHelper.SendAsync(context.Stream, allSketches, context.CancellationToken);
                return Result<string>.Success("All sketches retrieved successfully");
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}