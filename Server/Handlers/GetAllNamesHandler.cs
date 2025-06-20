using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public class GetAllNamesHandler : IRequestHandler
    {
        public bool CanHandle(string request)
            => request.Equals("GET:ALL:NAMES", StringComparison.OrdinalIgnoreCase);

        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            try
            {
                var allSketchNames = await context.MongoStore.GetAllSketchNamesAsync();
                /*
                if (allSketchNames != null)
                {
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError,
                        context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.ReadError);
                }
                */
                await ResponseHelper.SendAsync(context.Stream, allSketchNames, context.CancellationToken);
                return Result<string>.Success("Names retrieved successfully");
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}