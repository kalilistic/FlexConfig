using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlexConfig.Converters;

/// <inheritdoc />
public class IDictionaryJsonConverter : JsonConverter<IDictionary?>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IDictionary).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override IDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Malformed JSON.");
        }

        IDictionary? result = null;
        Type? valueType = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (result == null)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Malformed JSON.");
                }

                var propertyName = reader.GetString();

                if (propertyName != "$type")
                {
                    throw new JsonException("Malformed JSON.");
                }

                reader.Read();
                valueType = Type.GetType(reader.GetString() !);

                if (valueType == null)
                {
                    return null;
                }

                result = (IDictionary?)Activator.CreateInstance(valueType);
                continue;
            }

            var key = valueType?.GenericTypeArguments[0] == typeof(string)
                          ? reader.GetString()
                          : JsonSerializer.Deserialize(ref reader, valueType?.GenericTypeArguments[0] !, options);
            reader.Read();
            var value = JsonSerializer.Deserialize(ref reader, valueType?.GenericTypeArguments[1] !, options);

            result.Add(key!, value);
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IDictionary? value, JsonSerializerOptions options)
    {
        var type = value?.GetType();

        if (type == null)
        {
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("$type", $"{type.FullName}, {type.Assembly.GetName().Name}");

        var enumerator = value?.GetEnumerator();

        if (enumerator == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key;
                var keyType = key.GetType();

                if (keyType == typeof(string))
                {
                    writer.WritePropertyName((string)key);
                }
                else
                {
                    writer.WritePropertyName(JsonSerializer.Serialize(key, keyType, options));
                }

                var val = enumerator.Value;
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

        writer.WriteEndObject();
    }
}
