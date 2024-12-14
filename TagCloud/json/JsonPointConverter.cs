using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TagCloud.json;

internal class JsonPointConverter : JsonConverter<Point>
{
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        var xPropName = reader.GetString();
        reader.Read();
        var x = 0;
        if (xPropName == "X")
            x = reader.GetInt32();
        
        reader.Read();
        var yPropName = reader.GetString();
        reader.Read();
        var y = 0;
        if (yPropName == "Y")
            y = reader.GetInt32();
        
        reader.Read();
        
        if (y <= 0 || x <= 0)
            return Point.Empty;
        return new Point(x, y);
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteNumberValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteNumberValue(value.Y);
        writer.WriteEndObject();
    }
}