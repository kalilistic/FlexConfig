using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using FlexConfig.Interfaces;

namespace FlexConfig.Converters;

/// <inheritdoc />
public class FlexJsonConverter : JsonConverter<IFlex>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IFlex).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override IFlex? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var jsonNode = JsonNode.Parse(ref reader);

        if (jsonNode == null)
        {
            throw new JsonException("JsonNode");
        }

        if (jsonNode["Type"] == null)
        {
            throw new JsonException("Type");
        }

        if (jsonNode["Value"] == null)
        {
            throw new JsonException("Value");
        }

        var valueType = Type.GetType(jsonNode["Type"] !.ToString());

        if (valueType == null)
        {
            return null;
        }

        var value = jsonNode["Value"].Deserialize(valueType, options);
        var ret = Activator.CreateInstance(typeof(Flex<>).MakeGenericType(valueType), value);

        return (IFlex?)ret;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IFlex? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        JsonNode obj = new JsonObject();

        obj["Type"] = JsonSerializer.SerializeToNode(
            $"{value.Type.FullName}, {value.Type.Assembly.GetName().Name}",
            typeof(string),
            options);
        obj["Value"] = JsonSerializer.SerializeToNode(value.Value, value.Type, options);

        obj.WriteTo(writer);
    }
}
