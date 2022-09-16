using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace FlexConfig.Configuration;

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

    public Dictionary<string, IRef> Dictionary = new ();

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

    // public T Get<T>(string key) where T : class
    // {
    //     if (!this.dictionary.TryGetValue(key, out var value)) return default!;
    //     return (T)value;
    // }
    //
    // public void Set(string key, IRef value)
    // {
    //     if (this.dictionary.ContainsKey(key)) this.dictionary.Remove(key);
    //     this.dictionary.Add(key, value);
    //     if (this.AutoSave)
    //     {
    //         this.Save();
    //     }
    // }

    public void Save()
    {
        File.WriteAllText(this.configFilePath, this.SerializeConfig());
    }

    public void Load()
    {
        FileInfo config = new (this.configFilePath);
        if (!config.Exists)
        {
            this.Dictionary = new Dictionary<string, IRef>();
            this.Save();
        }
        else
        {
            var serializedData = File.ReadAllText(this.configFilePath);
            this.Dictionary = this.DeserializeConfig(serializedData);
        }
    }

    internal Dictionary<string, IRef> DeserializeConfig(string serializedData)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, IRef>>(
                   serializedData, this.jsonSerializerSettings) ?? new Dictionary<string, IRef>();
    }

    internal string SerializeConfig()
    {
        return JsonConvert.SerializeObject(
            this.Dictionary, Formatting.Indented, this.jsonSerializerSettings);
    }
}
