// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a general plugin.
/// </summary>
public sealed class GeneralPlugin : IPlugin
{
    /// <summary>
    /// Gets the current time.
    /// </summary>
    /// <returns>The current time.</returns>
    [KernelFunction("GetTime")]
    [Description("Obtiene la fecha y hora actual.")]
    public static string GetTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}