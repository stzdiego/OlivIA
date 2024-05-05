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

    public class ChatService
    {
        private readonly IDatabase database;
        private readonly ILogger<ChatService> logger;

        public ChatService(IDatabase database, ILogger<ChatService> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<Guid> Create()
        {
            try
            {
                this.logger.LogInformation("Creating chat service");
                Chat chat = new Chat();
                await this.database.Add(chat);
                this.logger.LogInformation("Chat service created");
                return chat.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Chat> Get(Guid id)
        {
            try
            {
                this.logger.LogInformation("Getting chat service");
                Chat chat = await this.database.Find<Chat>(x => x.Id == id)
                    ?? throw new Exception("Chat service not found");
                this.logger.LogInformation("Chat service retrieved");
                return chat;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task NewMessage(Guid chatId, MessageTypeEnum type, string content)
        {
            try
            {
                this.logger.LogInformation("Adding message to chat");
                var message = new Message { ChatId = chatId, Type = type, Content = content };
                await this.database.Add(message);
                this.logger.LogInformation("Message added to chat");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<StringBuilder> GetSummary(Guid chatId)
        {
            try
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
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Update(Chat chat)
        {
            try
            {
                this.logger.LogInformation("Updating chat service");
                await this.database.Update(chat);
                this.logger.LogInformation("Chat service updated");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                this.logger.LogInformation("Deleting chat service");
                Chat chat = await this.database.Find<Chat>(x => x.Id == id)
                    ?? throw new Exception("Chat service not found");
                await this.database.Delete(chat);
                this.logger.LogInformation("Chat service deleted");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessages(Guid chatId)
        {
            try
            {
                this.logger.LogInformation("Getting chat messages");
                IEnumerable<Message> messages = await this.database
                    .Get<Message>(x => x.ChatId == chatId);
                this.logger.LogInformation("Chat messages retrieved");
                return messages;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task AsociatePatient(Guid chatId, Guid patientId)
        {
            try
            {
                this.logger.LogInformation("Asociating patient to chat");
                Chat chat = await this.database.Find<Chat>(x => x.Id == chatId)
                    ?? throw new Exception("Chat service not found");

                chat.PatientId = patientId;
                await this.database.Update(chat);
                this.logger.LogInformation("Patient asociated to chat");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}