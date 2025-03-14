using System.Text.Json;
using System.Text.Json.Serialization;
using ConsolePaint.Models;

namespace ConsolePaint.Helpers;

public class ShapeJsonConverter : JsonConverter<Shape>
{
    public override Shape? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            string type = root.GetProperty("Type").GetString();

            return type switch
            {
                "Circle" => JsonSerializer.Deserialize<Circle>(root.GetRawText(), options),
                "Rectangle" => JsonSerializer.Deserialize<Rectangle>(root.GetRawText(), options),
                "Triangle" => JsonSerializer.Deserialize<Triangle>(root.GetRawText(), options),
                _ => throw new JsonException($"Неизвестный тип фигуры: {type}")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions options)
    {
        var typeName = value.GetType().Name;
        using (JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(value, value.GetType(), options)))
        {
            var obj = doc.RootElement;
            writer.WriteStartObject();

            writer.WriteString("Type", typeName);
            foreach (var property in obj.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}