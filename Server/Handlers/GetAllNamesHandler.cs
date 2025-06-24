using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public sealed class GetAllNamesHandler : ISketchRequestHandler
    {
        private const string GetAllNamesRequest = "GET:ALL:NAMES";
        private static GetAllNamesHandler _instance;
        private GetAllNamesHandler() { }
        
        public static GetAllNamesHandler GetInstance()
        {
            return _instance ??= new GetAllNamesHandler();
        }
        
        public bool CanHandle(string request)
            => request.Equals(GetAllNamesRequest, StringComparison.OrdinalIgnoreCase);

        
        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            try
            {
                var allSketchNames = await context.MongoStore.GetAllSketchNamesAsync();
                
                if (allSketchNames.Error is not null)
                {
                    await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError,
                        context.CancellationToken);
                    return Result<string>.Failure(AppErrors.Mongo.ReadError);
                }
                
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