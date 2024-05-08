// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Settings;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents the OpenAI settings.
/// </summary>
public class OpenAISettings : IAgentSettings
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the max tokens.
    /// </summary>
    public int MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// Gets or sets the presence penalty.
    /// </summary>
    public double PresencePenalty { get; set; }

    /// <summary>
    /// Gets or sets the frequency penalty.
    /// </summary>
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// Gets or sets the stop sequences.
    /// </summary>
    public IList<string>? StopSequences { get; set; }

    /// <summary>
    /// Gets or sets the TopP.
    /// </summary>
    public double TopP { get; set; }
}
