using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TagCloud.json;

internal class JsonFontFamilyConverter : JsonConverter<FontFamily>
{
    public override FontFamily? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        var families = FontFamily.Families.Select(family => family.Name);
        if (families.Contains(value))
            return new Font(value, 8, FontStyle.Regular).FontFamily;
        return null;
    }

    public override void Write(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}