using System;

namespace FlexConfig.Configuration;

/// <summary>
/// Reference wrapper interface.
/// </summary>
public interface IRef
{
    /// <summary>
    /// Gets the type of <see cref="IRef"/> property.
    /// </summary>
    public Type Type { get; }
}

/// <summary>
/// Reference wrapper class.
/// </summary>
/// <typeparam name="T">Any generic type.</typeparam>
public sealed class Ref<T> : IRef
{
    /// <summary>
    /// Stored value.
    /// </summary>
    public T Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ref{T}"/> class.
    /// <see cref="Ref{T}"/> constructor.
    /// </summary>
    /// <param name="value">Value to be assigned.</param>
    public Ref(T value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the type of <see cref="Ref{T}"/> property.
    /// </summary>
    public Type Type => typeof(T);

    /// <summary>
    /// Gets a reference to the stored value.
    /// </summary>
    /// <returns>A reference to <see cref="Value"/>.</returns>
    public ref T Reference => ref this.Value;

    /// <summary>
    /// Implicit conversion operator from <see cref="Ref{T}"/> to the underlying value type.
    /// </summary>
    /// <param name="value">An instance of <see cref="Ref{T}"/>.</param>
    /// <returns>Value stored in <see cref="Value"/>.</returns>
    public static implicit operator T(Ref<T> value) => value.Value;

    /// <summary>
    /// Implicit conversion from underlying value type to a new instance of <see cref="Ref{T}"/>.
    /// </summary>
    /// <param name="value">Newly assigned value.</param>
    /// <returns>New instance of <see cref="Ref{T}"/> containing newly assigned value.</returns>
    public static implicit operator Ref<T>(T value) => new (value);
}
