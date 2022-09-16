namespace FlexConfig.Configuration;

/// <summary>
/// Wrapper class to use primitives with Configuration.
/// </summary>
/// <typeparam name="T">Configuration field type.</typeparam>
public class PrimitiveContainer<T> where T : unmanaged
{
    private T value;

    public PrimitiveContainer(T value)
    {
        this.value = value;
    }

    public static implicit operator T(PrimitiveContainer<T> container) => container.value;

    public static implicit operator PrimitiveContainer<T>(T value) => new (value);

    public override string ToString() => this.value.ToString() !;
}
