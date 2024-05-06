// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Enums;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Chat service.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IDatabase database;
        private readonly ILogger<ChatService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatService"/> class.
        /// </summary>
        /// <param name="database">Database.</param>
        /// <param name="logger">Logger.</param>
        public ChatService(IDatabase database, ILogger<ChatService> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        /// <summary>
        /// Creates a new chat service.
        /// </summary>
        /// <returns>Chat service id.</returns>
        public async Task<Guid> Create()
        {
            this.logger.LogInformation("Creating chat service");
            Chat chat = new Chat();
            await this.database.Add(chat);
            this.logger.LogInformation("Chat service created");
            return chat.Id;
        }

        /// <summary>
        /// Gets a chat service.
        /// </summary>
        /// <param name="id">Chat service id.</param>
        /// <returns>Chat service.</returns>
        public async Task<Chat> Get(Guid id)
        {
            this.logger.LogInformation("Getting chat service");
            Chat chat = await this.database.Find<Chat>(x => x.Id == id)
                ?? throw new Exception("Chat service not found");
            this.logger.LogInformation("Chat service retrieved");
            return chat;
        }

        /// <summary>
        /// Adds a new message to a chat.
        /// </summary>
        /// <param name="chatId">Chat id.</param>
        /// <param name="type">Message type.</param>
        /// <param name="content">Message content.</param>
        /// <returns>Task.</returns>
        public async Task NewMessage(Guid chatId, MessageTypeEnum type, string content)
        {
            this.logger.LogInformation("Adding message to chat");
            var message = new Message { ChatId = chatId, Type = type, Content = content };
            await this.database.Add(message);
            this.logger.LogInformation("Message added to chat");
        }

        /// <summary>
        /// Gets the chat summary.
        /// </summary>
        /// <param name="chatId">Chat id.</param>
        /// <returns>Chat summary.</returns>
        public async Task<StringBuilder> GetSummary(Guid chatId)
        {
            this.logger.LogInformation("Getting chat summary");
            Chat chat = await this.database.Find<Chat>(x => x.Id == chatId)
                ?? throw new Exception("Chat service not found");

            List<Message> messages = await this.database.Get<Message>(x => x.ChatId == chatId);
            StringBuilder summary = new StringBuilder();
            foreach (var message in messages)
            {
                summary.AppendLine($"{message.Type.ToString()}> {message.Content}");
            }

            this.logger.LogInformation("Chat summary retrieved");
            return summary;
        }

        /// <summary>
        /// Updates a chat service.
        /// </summary>
        /// <param name="chat">Chat service.</param>
        /// <returns>Task.</returns>
        public async Task Update(Chat chat)
        {
            this.logger.LogInformation("Updating chat service");
            await this.database.Update(chat);
            this.logger.LogInformation("Chat service updated");
        }

        /// <summary>
        /// Deletes a chat service.
        /// </summary>
        /// <param name="id">Chat service id.</param>
        /// <returns>Task.</returns>
        public async Task Delete(Guid id)
        {
            this.logger.LogInformation("Deleting chat service");
            Chat chat = await this.database.Find<Chat>(x => x.Id == id)
                ?? throw new Exception("Chat service not found");
            await this.database.Delete(chat);
            this.logger.LogInformation("Chat service deleted");
        }

        /// <summary>
        /// Gets chat messages.
        /// </summary>
        /// <param name="chatId">Chat id.</param>
        /// <returns>Chat messages.</returns>
        public async Task<IEnumerable<Message>> GetMessages(Guid chatId)
        {
            this.logger.LogInformation("Getting chat messages");
            IEnumerable<Message> messages = await this.database
                .Get<Message>(x => x.ChatId == chatId);
            this.logger.LogInformation("Chat messages retrieved");
            return messages;
        }

        /// <summary>
        /// Asociates a patient to a chat.
        /// </summary>
        /// <param name="chatId">Chat id.</param>
        /// <param name="patientId">Patient id.</param>
        /// <returns>Task.</returns>
        public async Task AsociatePatient(Guid chatId, Guid patientId)
        {
            this.logger.LogInformation("Asociating patient to chat");
            Chat chat = await this.database.Find<Chat>(x => x.Id == chatId)
                ?? throw new Exception("Chat service not found");

            chat.PatientId = patientId;
            await this.database.Update(chat);
            this.logger.LogInformation("Patient asociated to chat");
        }
    }
}