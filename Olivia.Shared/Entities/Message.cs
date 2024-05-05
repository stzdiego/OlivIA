// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using Olivia.Shared.Enums;

namespace Olivia.Shared.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public MessageTypeEnum Type { get; set; }

    public string Content { get; set; } = string.Empty;

    [ForeignKey("Chat")]
    public Guid ChatId { get; set; }

    public virtual Chat Chat { get; set; } = null!;
}
