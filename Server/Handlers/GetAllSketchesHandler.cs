using System;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public class GetAllSketchesHandler : IRequestHandler
    {
        public static readonly GetAllSketchesHandler _instance = new GetAllSketchesHandler();
        public static GetAllSketchesHandler Instance => _instance;
        
        public bool CanHandle(string request) => request.Equals("GET:ALL", StringComparison.OrdinalIgnoreCase);
        
        private GetAllSketchesHandler() { }
        
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