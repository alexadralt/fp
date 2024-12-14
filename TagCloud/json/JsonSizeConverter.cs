using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TagCloud.json;

internal class JsonSizeConverter : JsonConverter<Size>
{
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        var widthPropName = reader.GetString();
        reader.Read();
        var width = 0;
        if (widthPropName == "Width")
            width = reader.GetInt32();
        
        reader.Read();
        var heightPropName = reader.GetString();
        reader.Read();
        var height = 0;
        if (heightPropName == "Height")
            height = reader.GetInt32();
        
        reader.Read();
        
        if (height <= 0 || width <= 0)
            return Size.Empty;
        return new Size(width, height);
    }

    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Width");
        writer.WriteNumberValue(value.Width);
        writer.WritePropertyName("Height");
        writer.WriteNumberValue(value.Height);
        writer.WriteEndObject();
    }
}