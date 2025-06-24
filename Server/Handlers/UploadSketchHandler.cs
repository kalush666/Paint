using System;
using System.Threading.Tasks;
using Common.DTO;
using Common.Errors;
using Common.Helpers;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public class UploadSketchHandler : ISketchRequestHandler
    {
        private const int LengthOfRequestPrefix = 5;
        private const string UploadRequestPrefix = "POST:";
        
        private static UploadSketchHandler _instance;

        public static UploadSketchHandler GetInstance()
        {
            return _instance ??= new UploadSketchHandler();
        }

        public bool CanHandle(string request)
            => request.StartsWith(UploadRequestPrefix, StringComparison.OrdinalIgnoreCase);

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
            }
            catch
            {
                return Result<string>.Failure(AppErrors.Mongo.InvalidJson);
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.Server.Suspended, context.CancellationToken);
                return Result<string>.Failure(AppErrors.Server.Suspended);
            }

            if (context.LockManager.IsLocked(sketchDto.Name))
            {
                await ResponseHelper.SendAsync(context.Stream, AppErrors.File.Locked, context.CancellationToken);
                return Result<string>.Failure(AppErrors.File.Locked);
            }

            try
            {
                await context.MongoStore.InsertSketchAsync(sketchDto);
                var result = Result<string>.Success($"{sketchDto.Name} uploaded successfully.");
                await ResponseHelper.SendAsync(context.Stream, result, context.CancellationToken);
                return result;
            }
            catch
            {
                var errorResult = Result<string>.Failure(AppErrors.Mongo.UploadError);
                await ResponseHelper.SendAsync(context.Stream, errorResult, context.CancellationToken);
                return errorResult;
            }
        }
    }
}