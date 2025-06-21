using System;
using System.Threading.Tasks;
using Common.DTO;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public class UploadSketchHandler : IRequestHandler
    {
        private const int LengthOfRequestPrefix = 5;

        private static readonly Lazy<UploadSketchHandler> _instance = new(() => new UploadSketchHandler());
        public static UploadSketchHandler Instance => _instance.Value;

        public bool CanHandle(string request)
            => request.StartsWith("POST:", StringComparison.OrdinalIgnoreCase);

        private UploadSketchHandler()
        {
        }

        public async Task<Result<string>> HandleAsync(RequestContext context)
        {
            var sketchJson = context.Request.Substring(LengthOfRequestPrefix);
            SketchDto? sketchDto;
            try
            {
                sketchDto = JsonConvert.DeserializeObject<SketchDto>(sketchJson);
                if (sketchDto == null || string.IsNullOrWhiteSpace(sketchDto.Name))
                {
                    return Result<string>.Failure(AppErrors.Mongo.InvalidJson);
                }

                sketchDto.Id = Guid.NewGuid();
            }
            catch
            {
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);
            }

            try
            {
                await context.MongoStore.InsertSketchAsync(sketchDto);
                var result = Result<string>.Success($"{sketchDto.Name} uploaded successfully.");
                var json = JsonConvert.SerializeObject(result);
                await ResponseHelper.SendAsync(context.Stream, json, context.CancellationToken);
                return result;
            }
            catch
            {
                var errorResult = Result<string>.Failure(AppErrors.Mongo.UploadError);
                var errorJson = JsonConvert.SerializeObject(errorResult);
                await ResponseHelper.SendAsync(context.Stream, errorJson, context.CancellationToken);
                return errorResult;
            }
        }
    }
}