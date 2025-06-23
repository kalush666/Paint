using System;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Common.Convertors
{
    public class ObjectIdJsonConverter : JsonConverter<ObjectId>
    {
        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            return ObjectId.TryParse(value, out var objectId)
                ? objectId
                : ObjectId.Empty;
        }
    }
}