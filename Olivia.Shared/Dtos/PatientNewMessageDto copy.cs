// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;
using Olivia.Shared.Enums;

/// <summary>
/// Represents a new message data transfer object.
/// </summary>
public class PatientNewMessageDto
{
    /// <summary>
    /// Gets or sets the chat identifier.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
