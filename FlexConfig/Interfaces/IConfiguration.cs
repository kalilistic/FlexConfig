namespace FlexConfig.Interfaces;

/// <summary>
/// Configuration interface.
/// </summary>
public interface IConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether automatic saving is enabled.
    /// </summary>
    bool AutoSave { get; set; }

    /// <summary>
    /// Indexer operator for storing or retrieving instance of <see cref="IFlex"/>.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    IFlex this[string key] { get; set; }

    /// <summary>
    /// Gets an instance of <see cref="Flex{T}"/> by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <typeparam name="T">Requested type.</typeparam>
    /// <returns>Existing or default constructed instance of <see cref="Flex{T}"/> from dictionary.</returns>
    Flex<T> Get<T>(string key);

    /// <summary>
    /// Sets the value in the dictionary by key to a default state.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <typeparam name="T">Requested type (Can be implicit).</typeparam>
    void Set<T>(string key);

    /// <summary>
    /// Sets the value in the dictionary by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <param name="value">Value to be stored.</param>
    /// <typeparam name="T">Requested type (Can be implicit).</typeparam>
    void Set<T>(string key, T value);

    /// <summary>
    /// Serializes configuration dictionary to provided file path.
    /// </summary>
    void Save();

    /// <summary>
    /// Deserializes configuration dictionary from provided file path.
    /// </summary>
    void Load();
}
