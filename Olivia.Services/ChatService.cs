using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Entities;
using System.Text;
using Olivia.Shared.Enums;

namespace Olivia.Services
{
    public class ChatService
    {
        private readonly IDatabase _database;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IDatabase database, ILogger<ChatService> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<Guid> Create()
        {
            try
            {
                _logger.LogInformation("Creating chat service");
                Chat chat = new Chat();
                await _database.Add(chat);
                _logger.LogInformation("Chat service created");
                return chat.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Chat> Get(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting chat service");
                Chat chat = await _database.Find<Chat>(x => x.Id == id)
                    ?? throw new Exception("Chat service not found");
                _logger.LogInformation("Chat service retrieved");
                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task NewMessage(Guid chatId, MessageTypeEnum type, string content)
        {
            try
            {
                _logger.LogInformation("Adding message to chat");
                var message = new Message { ChatId = chatId, Type = type, Content = content};
                await _database.Add(message);
                _logger.LogInformation("Message added to chat");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }


        public async Task<StringBuilder> GetSummary(Guid chatId)
        {
            try
            {
                _logger.LogInformation("Getting chat summary");
                Chat chat = await _database.Find<Chat>(x => x.Id == chatId)
                    ?? throw new Exception("Chat service not found");
                    
                List<Message> messages = await _database.Get<Message>(x => x.ChatId == chatId);
                StringBuilder summary = new StringBuilder();
                foreach (var message in messages)
                {
                    summary.AppendLine($"{message.Type.ToString()}> {message.Content}");
                }
                _logger.LogInformation("Chat summary retrieved");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        public async Task Update(Chat chat)
        {
            try
            {
                _logger.LogInformation("Updating chat service");
                await _database.Update(chat);
                _logger.LogInformation("Chat service updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting chat service");
                Chat chat = await _database.Find<Chat>(x => x.Id == id)
                    ?? throw new Exception("Chat service not found");
                await _database.Delete(chat);
                _logger.LogInformation("Chat service deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessages(Guid chatId)
        {
            try
            {
                _logger.LogInformation("Getting chat messages");
                IEnumerable<Message> messages = await _database
                    .Get<Message>(x => x.ChatId == chatId);
                _logger.LogInformation("Chat messages retrieved");
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task AsociatePatient(Guid chatId, Guid patientId)
        {
            try
            {
                _logger.LogInformation("Asociating patient to chat");
                Chat chat = await _database.Find<Chat>(x => x.Id == chatId)
                    ?? throw new Exception("Chat service not found");
                    
                chat.PatientId = patientId;
                await _database.Update(chat);
                _logger.LogInformation("Patient asociated to chat");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
    }
}