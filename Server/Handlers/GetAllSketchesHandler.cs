using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public class GetAllSketchesHandler : IRequestHandler
    {
        public bool CanHandle(string request) => request.Equals("GET:ALL", StringComparison.OrdinalIgnoreCase);

        public async Task<Result<String>> HandleAsync(RequestContext context)
        {
            try
            {
                var allSketches = await context.MongoStore.GetAllJsonAsync();
                var resultJson = JsonConvert.SerializeObject(allSketches.Value ?? new List<string>(), Formatting.None);
                await ResponseHelper.SendAsync(context.Stream, resultJson, context.CancellationToken);
                return Result<string>.Success(resultJson);
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.ReadError, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.ReadError);
            }
        }
    }
}