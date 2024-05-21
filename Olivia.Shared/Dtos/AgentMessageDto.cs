// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;

/// <summary>
/// Represents an agent message data transfer object.
/// </summary>
public class AgentMessageDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the chat identifier.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sender identifier.
    /// </summary>
    public Guid? SenderId { get; set; }
}
