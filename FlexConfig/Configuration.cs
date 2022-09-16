using System;
using System.Collections.Generic;
using System.IO;

using FlexConfig.Interfaces;
using Newtonsoft.Json;

namespace FlexConfig;

/// <summary>
/// Configuration alternative to Dalamud's built in-class.
/// </summary>
public class Configuration
{
    private readonly string configFilePath;

    private readonly JsonSerializerSettings jsonSerializerSettings = new ()
    {
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        TypeNameHandling = TypeNameHandling.Objects,
    };

    private Dictionary<string, IFlex> dictionary = new ();

    public Configuration(string configFilePath, bool autoSave = true)
    {
        if (string.IsNullOrEmpty(configFilePath))
        {
            throw new Exception("configFilePath is null or empty.");
        }

        this.AutoSave = autoSave;
        this.configFilePath = configFilePath;
    }

    public bool AutoSave { get; set; }

    public Flex<T> Get<T>(string key) => !this.dictionary.TryGetValue(key, out var value) ? default! : (Flex<T>)value;

    public void Set<T>(string key, Flex<T> value)
    {
        if (this.dictionary.ContainsKey(key) && this.dictionary[key].Type == value.Type)
        {
            this.dictionary[key].Value = value.Value;
        }
        else
        {
            this.dictionary[key] = value;
        }
    }

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

    public void Save()
    {
        File.WriteAllText(this.configFilePath, this.SerializeConfig());
    }

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

    internal Dictionary<string, IFlex> DeserializeConfig(string serializedData)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, IFlex>>(
                   serializedData, this.jsonSerializerSettings) ?? new Dictionary<string, IFlex>();
    }

    internal string SerializeConfig()
    {
        return JsonConvert.SerializeObject(
            this.dictionary, Formatting.Indented, this.jsonSerializerSettings);
    }
}
