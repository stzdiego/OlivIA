// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using System.Text;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;

/// <summary>
/// IChatService interface.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Create a new chat.
    /// </summary>
    /// <returns>Chat id.</returns>
    Task<Guid> Create();

    /// <summary>
    /// Get a chat by id.
    /// </summary>
    /// <param name="id">Chat id.</param>
    /// <returns>Chat.</returns>
    Task<Chat> Get(Guid id);

    /// <summary>
    /// Create a new message.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="type">Message type.</param>
    /// <param name="content">Message content.</param>
    /// <returns>Task.</returns>
    Task NewMessage(Guid chatId, MessageTypeEnum type, string content);

    /// <summary>
    /// Get chat summary.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <returns>Chat summary.</returns>
    Task<List<Message>> GetSummary(Guid chatId);

    /// <summary>
    /// Update chat.
    /// </summary>
    /// <param name="chat">Chat.</param>
    /// <returns>Task.</returns>
    Task Update(Chat chat);

    /// <summary>
    /// Delete chat.
    /// </summary>
    /// <param name="id">Chat id.</param>
    /// <returns>Task.</returns>
    Task Delete(Guid id);

    /// <summary>
    /// Get messages.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <returns>Messages.</returns>
    Task<IEnumerable<Message>> GetMessages(Guid chatId);

    /// <summary>
    /// Asociate patient to chat.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="patientId">Patient id.</param>
    /// <returns>Task.</returns>
    Task AsociateSender(Guid chatId, Guid patientId);
}
