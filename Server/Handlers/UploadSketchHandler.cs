using System;
using System.Threading.Tasks;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Server.Handlers
{
    public class UploadSketchHandler : IRequestHandler
    {
        private const int LengthOfRequestPrefix = 5;

        public bool CanHandle(string request)
            => request.StartsWith("POST:", StringComparison.OrdinalIgnoreCase);


        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            var sketchJson = context.Request.Substring(LengthOfRequestPrefix);
            string? sketchName = JObject.Parse(sketchJson)
                [SketchFields.Name]?.ToString();
            if (string.IsNullOrWhiteSpace(sketchName))
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Mongo.InvalidJson,
                    context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);
            }

            try
            {
                await context.MongoStore.InsertJsonAsync(sketchJson);
                await ResponseHelper.SendAsync(context.Stream,$"{sketchName} uploaded successfully", context.CancellationToken);
                return Result<string>.Success($"{sketchName} uploaded successfully.");
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, sketchName, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }
        }
    }
}