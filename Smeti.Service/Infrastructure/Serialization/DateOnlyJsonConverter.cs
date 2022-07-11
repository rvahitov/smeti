using LanguageExt;
using Newtonsoft.Json;

namespace Smeti.Service.Infrastructure.Serialization;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        var stringValue = value.ToString("O");
        writer.WriteValue(stringValue);
    }

    public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue,
                                      JsonSerializer serializer)
    {
        return Prelude.Optional(reader.ReadAsString())
                      .Where(s => !string.IsNullOrEmpty(s))
                      .Map(DateOnly.Parse)
                      .IfNone(DateOnly.MinValue);
    }
}