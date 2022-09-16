using System;
using System.Collections.Generic;
using System.IO;

using FlexConfig.Interfaces;
using Newtonsoft.Json;

namespace FlexConfig;

/// <summary>
/// Configuration class.
/// </summary>
public class Configuration : IConfiguration
{
    private readonly string configFilePath;

    private readonly JsonSerializerSettings jsonSerializerSettings = new ()
    {
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        TypeNameHandling = TypeNameHandling.Objects,
    };

    private Dictionary<string, IFlex> dictionary = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="configFilePath">Path to config file (including name).</param>
    /// <param name="autoSave">
    /// Whether or not automatic save should happen with every call to <see cref="Set{T}"/>.
    /// Default: true.
    /// </param>
    public Configuration(string configFilePath, bool autoSave = true)
    {
        if (string.IsNullOrEmpty(configFilePath))
        {
            throw new Exception("configFilePath is null or empty.");
        }

        this.AutoSave = autoSave;
        this.configFilePath = configFilePath;
    }

    /// <inheritdoc/>
    public bool AutoSave { get; set; }

    /// <inheritdoc/>
    public IFlex this[string key]
    {
        get => !this.dictionary.TryGetValue(key, out var value) ? default! : value;

        set
        {
            if (this.dictionary.ContainsKey(key))
            {
                this.dictionary[key].Value = value.Value;
            }
            else
            {
                this.dictionary[key] = value;
            }

            if (this.AutoSave)
            {
                this.Save();
            }
        }
    }

    /// <inheritdoc/>
    public Flex<T> Get<T>(string key) => !this.dictionary.TryGetValue(key, out var value) ? default! : (Flex<T>)value;

    /// <inheritdoc/>
    public void Set<T>(string key, T value)
    {
        if (this.dictionary.ContainsKey(key) && this.dictionary[key].Type == typeof(T))
        {
            this.dictionary[key].Value = value!;
        }
        else
        {
            var refInstance = Activator.CreateInstance(typeof(Flex<>).MakeGenericType(typeof(T)), value);
            this.dictionary[key] = (IFlex)refInstance!;
        }
    }

    /// <inheritdoc/>
    public void Save()
    {
        File.WriteAllText(this.configFilePath, this.SerializeConfig());
    }

    /// <inheritdoc/>
    public void Load()
    {
        FileInfo config = new (this.configFilePath);
        if (!config.Exists)
        {
            this.dictionary = new Dictionary<string, IFlex>();
            this.Save();
        }
        else
        {
            var serializedData = File.ReadAllText(this.configFilePath);
            this.dictionary = this.DeserializeConfig(serializedData);
        }
    }

    /// <summary>
    /// Deserializes configuration dictionary from string.
    /// </summary>
    /// <param name="serializedData">JSON string.</param>
    /// <returns>Instance of configuration dictionary.</returns>
    internal Dictionary<string, IFlex> DeserializeConfig(string serializedData)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, IFlex>>(
                   serializedData, this.jsonSerializerSettings) ?? new Dictionary<string, IFlex>();
    }

    /// <summary>
    /// Serializes configuration dictionary to string.
    /// </summary>
    /// <returns>JSON string.</returns>
    internal string SerializeConfig()
    {
        return JsonConvert.SerializeObject(
            this.dictionary, Formatting.Indented, this.jsonSerializerSettings);
    }
}
