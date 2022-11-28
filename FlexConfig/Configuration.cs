using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using FlexConfig.Converters;
using FlexConfig.Interfaces;

namespace FlexConfig;

/// <summary>
/// Configuration class.
/// </summary>
public class Configuration : IConfiguration
{
    private readonly string configFilePath;

    private readonly JsonSerializerOptions jsonSerializerSettings = new ()
    {
        WriteIndented = true,
    };

    private Dictionary<string, IFlex> dictionary = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="configFilePath">Path to config file (including name).</param>
    /// <param name="autoSave">
    /// Whether or not automatic save should happen with every call to <see cref="this[string]"/>, <see cref="Set{T}(string)"/> or <see cref="Set{T}(string, T)"/>.
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

        // Set up converters
        this.jsonSerializerSettings.Converters.Add(new FlexJsonConverter());
        this.jsonSerializerSettings.Converters.Add(new GenericJsonConverter());
        this.jsonSerializerSettings.Converters.Add(new IDictionaryJsonConverter());
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

    /// <summary>
    /// Constructs <see cref="Flex{T}"/> from value.
    /// </summary>
    /// <param name="value">Value to be assigned.</param>
    /// <typeparam name="T">Requested type (Can be implicit).</typeparam>
    /// <returns>Instance of <see cref="Flex{T}"/>.</returns>
    public static Flex<T> Create<T>(T value) => new (value);

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">The key does not exist in the dictionary.</exception>
    public dynamic Get(string key) => this.dictionary.TryGetValue(key, out var value)
                                          ? value.Value
                                          : throw new KeyNotFoundException(key);

    /// <inheritdoc />
    public dynamic Get(string key, dynamic defaultValue) =>
        this.dictionary.TryGetValue(key, out var value) ? value.Value : defaultValue;

    /// <inheritdoc/>
    /// <exception cref="KeyNotFoundException">The key does not exist in the dictionary.</exception>
    public Flex<T> Get<T>(string key) => this.dictionary.TryGetValue(key, out var value)
                                             ? (Flex<T>)value
                                             : throw new KeyNotFoundException(key);

    /// <inheritdoc />
    public Flex<T> Get<T>(string key, T defaultValue) =>
        this.dictionary.TryGetValue(key, out var value) ? (Flex<T>)value : defaultValue;

    /// <inheritdoc/>
    public void Set<T>(string key) => this.Set<T>(key, default!);

    /// <inheritdoc/>
    public void Set<T>(string key, T value)
    {
        if (value is IFlex wrappedValue)
        {
            this[key] = wrappedValue;
        }
        else if (this.dictionary.ContainsKey(key) && this.dictionary[key].Type == typeof(T))
        {
            this.dictionary[key].Value = value!;
        }
        else
        {
            var refInstance = Activator.CreateInstance(typeof(Flex<>).MakeGenericType(typeof(T)), value);
            this.dictionary[key] = (IFlex)refInstance!;
        }

        if (this.AutoSave)
        {
            this.Save();
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(string key) => this.dictionary.ContainsKey(key);

    /// <inheritdoc/>
    public void Remove(string key) => this.dictionary.Remove(key);

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
        return JsonSerializer.Deserialize<Dictionary<string, IFlex>>(serializedData, this.jsonSerializerSettings) ??
               new Dictionary<string, IFlex>();
    }

    /// <summary>
    /// Serializes configuration dictionary to string.
    /// </summary>
    /// <returns>JSON string.</returns>
    internal string SerializeConfig()
    {
        return JsonSerializer.Serialize(this.dictionary, this.jsonSerializerSettings);
    }
}
