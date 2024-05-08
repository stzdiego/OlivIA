// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;

/// <summary>
/// Represents a new message data transfer object.
/// </summary>
public class NewMessageDto
{
    /// <summary>
    /// Gets or sets the chat identifier.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender identifier.
    /// </summary>
    public Guid? SenderId { get; set; }
}
