// <copyright file="Flex.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

using FlexConfig.Interfaces;
using Newtonsoft.Json;

namespace FlexConfig;

/// <summary>
/// Reference wrapper class.
/// </summary>
/// <typeparam name="T">Any generic type.</typeparam>
public sealed class Flex<T> : IFlex
{
    /// <summary>
    /// Stored value.
    /// </summary>
    private T storedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Flex{T}"/> class.
    /// </summary>
    /// <param name="value">Value to be assigned.</param>
    public Flex(T value)
    {
        this.storedValue = value;
    }

    /// <summary>
    /// Gets a reference to the stored value.
    /// </summary>
    /// <returns>A reference to <see cref="Value"/>.</returns>
    [JsonIgnore]
    public ref T Reference => ref this.storedValue!;

    /// <summary>
    /// Gets the type of <see cref="Flex{T}"/> property.
    /// </summary>
    [JsonIgnore]
    public Type Type => typeof(T);

    /// <inheritdoc />
    public dynamic Value
    {
        get => this.storedValue!;
        set
        {
            if (value.GetType() != typeof(T))
            {
                throw new Exception($"{value.GetType()} does not match {typeof(T)}.");
            }

            this.storedValue = value;
        }
    }

    /// <summary>
    /// Implicit conversion operator from <see cref="Flex{T}"/> to the underlying value type.
    /// </summary>
    /// <param name="value">An instance of <see cref="Flex{T}"/>.</param>
    /// <returns>Value stored in <see cref="Value"/>.</returns>
    public static implicit operator T?(Flex<T> value) => value.storedValue;

    /// <summary>
    /// Implicit conversion from underlying value type to a new instance of <see cref="Flex{T}"/>.
    /// </summary>
    /// <param name="value">Newly assigned value.</param>
    /// <returns>New instance of <see cref="Flex{T}"/> containing newly assigned value.</returns>
    public static implicit operator Flex<T>(T value) => new (value);

    /// <inheritdoc/>
    public override string? ToString() => this.storedValue?.ToString();
}
