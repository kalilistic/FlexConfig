namespace FlexConfig.Interfaces;

/// <summary>
/// Configuration alternative to Dalamud's built in-class.
/// </summary>
public interface IConfiguration
{
    bool AutoSave { get; set; }

    void Set<T>(string key, T value) where T : class;

    T Get<T>(string key) where T : class;

    void Save();

    void Load();
}
