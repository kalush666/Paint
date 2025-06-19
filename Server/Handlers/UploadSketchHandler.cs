using System;
using System.Threading.Tasks;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using Common.Models;
using Newtonsoft.Json;
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
            Sketch? sketch;
            try
            {
                sketch = JsonConvert.DeserializeObject<Sketch>(sketchJson);
                if (sketch == null || string.IsNullOrWhiteSpace(sketch.Name))
                {
                    return Result<string>.Failure(AppErrors.Mongo.InvalidJson);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            try
            {
                await context.MongoStore.InsertSketchAsync(sketch);
                await ResponseHelper.SendAsync(context.Stream,$"{sketch.Name} uploaded successfully", context.CancellationToken);
                return Result<string>.Success($"{sketch.Name} uploaded successfully.");
            }
            catch (Exception e)
            {
                await ResponseHelper.SendAsync(context.Stream, sketch.Name, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Mongo.UploadError);
            }
        }
    }
}