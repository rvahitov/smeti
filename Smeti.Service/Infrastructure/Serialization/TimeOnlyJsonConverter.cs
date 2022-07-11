using LanguageExt;
using Newtonsoft.Json;

namespace Smeti.Service.Infrastructure.Serialization;

public sealed class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("O"));
    }

    public override TimeOnly ReadJson(
        JsonReader reader,
        Type objectType,
        TimeOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        return Prelude
              .Optional(reader.ReadAsString())
              .Where(s => !string.IsNullOrEmpty(s))
              .Map(TimeOnly.Parse)
              .IfNone(TimeOnly.MinValue);
    }
}