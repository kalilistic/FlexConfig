using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FlexConfig.Converters;

/// <inheritdoc />
public class GenericJsonConverter : JsonConverter<object>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsPrimitive && typeToConvert != typeof(string) &&
               !typeof(IDictionary).IsAssignableFrom(typeToConvert) && !typeof(IList).IsAssignableFrom(typeToConvert) &&
               !typeof(ISet<>).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        if (jsonNode["$type"] == null)
        {
            throw new JsonException("Type");
        }

        var valueType = Type.GetType(jsonNode["$type"] !.ToString());

        if (valueType == null)
        {
            return null;
        }

        var value = Activator.CreateInstance(valueType);

        foreach (var field in valueType.GetFields())
        {
            if (jsonNode[field.Name] == null)
            {
                continue;
            }

            field.SetValue(value, jsonNode[field.Name].Deserialize(field.FieldType, options));
        }

        foreach (var property in valueType.GetProperties())
        {
            if (jsonNode[property.Name] == null)
            {
                continue;
            }

            property.SetValue(value, jsonNode[property.Name].Deserialize(property.PropertyType, options));
        }

        return value;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        var type = value?.GetType();

        if (type == null)
        {
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("$type", $"{type.FullName}, {type.Assembly.GetName().Name}");

        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                JsonSerializer.Serialize(writer, value, type, options);
            }
            else
            {
                foreach (var field in type.GetFields())
                {
                    writer.WritePropertyName(field.Name);
                    JsonSerializer.Serialize(writer, field.GetValue(value), field.FieldType, options);
                }

                foreach (var property in type.GetProperties())
                {
                    writer.WritePropertyName(property.Name);
                    JsonSerializer.Serialize(writer, property.GetValue(value), property.PropertyType, options);
                }
            }

            writer.WriteEndObject();
        }
    }
}
