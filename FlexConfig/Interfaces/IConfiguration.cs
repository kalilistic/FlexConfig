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
    /// Gets the immediate value by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <returns>Existing or default constructed instance of value from dictionary.</returns>
    dynamic Get(string key);

    /// <summary>
    /// Gets the immediate value by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>Existing or default constructed instance of value from dictionary.</returns>
    dynamic Get(string key, dynamic defaultValue);

    /// <summary>
    /// Gets an instance of <see cref="Flex{T}"/> by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <typeparam name="T">Requested type.</typeparam>
    /// <returns>Existing or default constructed instance of <see cref="Flex{T}"/> from dictionary.</returns>
    Flex<T> Get<T>(string key);

    /// <summary>
    /// Gets an instance of <see cref="Flex{T}"/> by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <typeparam name="T">Requested type.</typeparam>
    /// <returns>Existing or default constructed instance of <see cref="Flex{T}"/> from dictionary.</returns>
    Flex<T> Get<T>(string key, T defaultValue);

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
    /// Sets the value in the dictionary by key if doesn't exist.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <param name="value">Value to be stored.</param>
    /// <typeparam name="T">Requested type (Can be implicit).</typeparam>
    /// <returns>True if key is already contained in dictionary.</returns>
    bool SetIfNew<T>(string key, T value);

    /// <summary>
    /// Checks if key is contained in dictionary.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    /// <returns>True if key is contained in dictionary.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Removes entry from dictionary by key.
    /// </summary>
    /// <param name="key">Configuration key.</param>
    void Remove(string key);

    /// <summary>
    /// Removes all entries from dictionary.
    /// </summary>
    void RemoveAll();

    /// <summary>
    /// Serializes configuration dictionary to provided file path.
    /// </summary>
    void Save();

    /// <summary>
    /// Deserializes configuration dictionary from provided file path.
    /// </summary>
    void Load();
}
