// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Olivia.Shared.Entities;

namespace Olivia.Shared.Dtos;

public class ResumeDto
{
    public required Guid ChatId { get; set; }

    public required Chat Chat { get; set; }

    public IEnumerable<Message>? Messages { get; set; }
}
