// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;
using Olivia.Shared.Entities;

/// <summary>
/// Represents a resume data transfer object.
/// </summary>
public class ResumeDto
{
    /// <summary>
    /// Gets or sets the chat identifier.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the messages.
    /// </summary>
    public IEnumerable<Message>? Messages { get; set; }
}
