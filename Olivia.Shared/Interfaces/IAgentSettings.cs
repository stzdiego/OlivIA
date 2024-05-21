// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

/// <summary>
/// Agent settings interface.
/// </summary>
public interface IAgentSettings
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    string Type { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    string Model { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    string Key { get; set; }

    /// <summary>
    /// Gets or sets the max tokens.
    /// </summary>
    int MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    double Temperature { get; set; }

    /// <summary>
    /// Gets or sets the presence penalty.
    /// </summary>
    double PresencePenalty { get; set; }

    /// <summary>
    /// Gets or sets the frequency penalty.
    /// </summary>
    double FrequencyPenalty { get; set; }

    /// <summary>
    /// Gets or sets the stop sequences.
    /// </summary>
    IList<string>? StopSequences { get; set; }

    /// <summary>
    /// Gets or sets the TopP.
    /// </summary>
    double TopP { get; set; }
}
