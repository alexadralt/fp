using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TagCloud.json;

internal class JsonColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Color.FromName(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}