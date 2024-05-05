// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;

public class AgentMessageDto
{
    public required Guid Id { get; set; }

    public required string Content { get; set; }
}
