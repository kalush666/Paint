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

        public bool CanHandle(string request)
            => request.StartsWith("POST:", StringComparison.OrdinalIgnoreCase);


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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            try
            {
                await context.MongoStore.InsertSketchAsync(sketchDto);
                var result = Result<string>.Success($"{sketchDto.Name} uploaded successfully.");
                var json = JsonConvert.SerializeObject(result);
                await ResponseHelper.SendAsync(context.Stream, json, context.CancellationToken);
                return result;
            }
            catch (Exception e)
            {
                var errorResult = Result<string>.Failure(AppErrors.Mongo.UploadError);
                var errorJson = JsonConvert.SerializeObject(errorResult);
                await ResponseHelper.SendAsync(context.Stream, errorJson, context.CancellationToken);
                return errorResult;
            }
        }
    }
}