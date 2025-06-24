using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DTO;
using Common.Errors;
using Common.Helpers;

namespace Server.Handlers
{
    public sealed class GetAllSketchesHandler : ISketchRequestHandler
    {
        private static GetAllSketchesHandler _instance;
        private const string GetAllRequest = "GET:ALL";

        public static GetAllSketchesHandler GetInstance()
        {
            return _instance ??= new GetAllSketchesHandler();
        }

        private GetAllSketchesHandler()
        {
        }

        public bool CanHandle(string request) => request.Equals(GetAllRequest, StringComparison.OrdinalIgnoreCase);


        public async Task<Result<String>> HandleAsync(RequestContext context)
        {
            try
            {
                Result<List<SketchDto>> allSketches = await context.MongoStore.GetAllSketchesAsync();
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