// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Olivia.Shared.Enums;

/// <summary>
/// Represents a message.
/// </summary>
public class Message
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the updated at.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public MessageTypeEnum Type { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [ForeignKey("Chat")]
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the chat.
    /// </summary>
    public virtual Chat Chat { get; set; } = null!;
}
