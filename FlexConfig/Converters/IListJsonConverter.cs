using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FlexConfig.Converters;

/// <inheritdoc />
public class IListJsonConverter : JsonConverter<IList>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IList).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override IList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Malformed JSON.");
        }

        var result = new List<object?>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            var node = JsonNode.Parse(ref reader);

            if (node == null)
            {
                throw new JsonException("Malformed JSON.");
            }

            object? value;

            if (node["$type"] != null)
            {
                var valueType = Type.GetType(node["$type"] !.ToString());

                if (valueType == null)
                {
                    throw new JsonException("Malformed JSON.");
                }

                value = node.Deserialize(valueType, options);
            }
            else
            {
                value = node.Deserialize<object?>(options);
            }

            result.Add(value);
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IList? value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        var enumerator = value?.GetEnumerator();

        if (enumerator != null)
        {
            while (enumerator.MoveNext())
            {
                var val = enumerator.Current;
                var valType = val?.GetType();

                if (valType == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, val, valType, options);
                }
            }
        }

        writer.WriteEndArray();
    }
}
