// <copyright file="IFlex.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace FlexConfig.Interfaces;

/// <summary>
/// Reference wrapper interface.
/// </summary>
public interface IFlex
{
    /// <summary>
    /// Gets or sets stored value.
    /// </summary>
    public dynamic Value { get; set; }

    /// <summary>
    /// Gets the type of stored value.
    /// </summary>
    public Type Type { get; }
}
