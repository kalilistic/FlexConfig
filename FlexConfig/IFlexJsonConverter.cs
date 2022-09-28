using System;

using FlexConfig.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexConfig;

/// <inheritdoc />
public class IFlexJsonConverter : JsonConverter<IFlex>
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, IFlex? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var obj = new JObject
        {
            ["Type"] = JToken.FromObject($"{value.Type.FullName}, {value.Type.Assembly.GetName().Name}", serializer),
            ["Value"] = JToken.FromObject(value.Value, serializer),
        };

        obj.WriteTo(writer);
    }

    /// <inheritdoc />
    public override IFlex? ReadJson(JsonReader reader, Type objectType, IFlex? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var jObject = JObject.Load(reader);

        if (!jObject.ContainsKey("Type"))
        {
            throw new JsonReaderException("Type");
        }

        if (!jObject.ContainsKey("Value"))
        {
            throw new JsonReaderException("Value");
        }

        var valueType = Type.GetType(jObject["Type"] !.ToString());

        if (valueType == null)
        {
            throw new NullReferenceException($"{valueType}");
        }

        var value = jObject["Value"] !.ToObject(valueType);
        var ret = Activator.CreateInstance(typeof(Flex<>).MakeGenericType(valueType), value);

        return (IFlex?)ret;
    }
}
