using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olivia.AI.Agents;
using Olivia.AI.Plugins;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Dtos;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

namespace Olivia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsAsistenceController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PatientsAsistenceController> _logger;
        private readonly OpenAIAgent _agentRegister;
        private readonly OpenAIAgent _agentPlanner;
        private readonly ChatService _chats;
        private readonly OliviaDbContext _context;

        public PatientsAsistenceController(IConfiguration configuration, ILogger<PatientsAsistenceController> logger,
                                           ChatService chats, OliviaDbContext context)
        {
            _config = configuration;
            _logger = logger;
            _chats = chats;
            _context = context;
            _agentRegister = GetRegisterAgent();
            _agentPlanner = GetPlannerAgent();
        }

        private OpenAIAgent GetRegisterAgent()
        {
            OpenAIAgent agent = new OpenAIAgent();
            agent.AddScoped<ChatService>();
            agent.AddScoped<PatientService>();
            agent.AddScoped<DoctorService>();
            agent.AddScoped<ProgramationService>();
            agent.AddScoped<IDatabase, DatabaseService>();
            agent.AddDbContext<DbContext, OliviaDbContext>(_context);
            agent.AddPlugin<PatientsManagerPlugin>();

            agent.Initialize(_config.GetValue<string>("Agents:Reception:Model")!, _config.GetValue<string>("Agents:Reception:Key")!,
                _config.GetValue<int>("Agents:Reception:MaxTokens"), _config.GetValue<double>("Agents:Reception:Temperature"),
                _config.GetValue<double>("Agents:Reception:Penalty"));

            return agent;
        }

        private OpenAIAgent GetPlannerAgent()
        {
            OpenAIAgent agent = new OpenAIAgent();
            agent.AddScoped<ChatService>();
            agent.AddScoped<PatientService>();
            agent.AddScoped<DoctorService>();
            agent.AddScoped<ProgramationService>();
            agent.AddScoped<IDatabase, DatabaseService>();
            agent.AddDbContext<DbContext, OliviaDbContext>(_context);
            agent.AddPlugin<PatientsManagerPlugin>();

            agent.Initialize(_config.GetValue<string>("Agents:Reception:Model")!, _config.GetValue<string>("Agents:Reception:Key")!,
                _config.GetValue<int>("Agents:Reception:MaxTokens"), _config.GetValue<double>("Agents:Reception:Temperature"),
                _config.GetValue<double>("Agents:Reception:Penalty"));

            return agent;
        }


        private string GetPromptRegister()
        {
            return @"
            Eres Olivia, una cordial asistente, tu objetivo es solicitar la información necesaria
            para registrar un paciente en la base de datos.
            Inicia presentandote cordialmente y continua con la solicitud de parametros.
            Los parametros son: Identificación, Nombre, Apellido, Correo electronico, Telefono celular y razón de consulta.
            Debes pedir todos los parametros en el orden indicado antes de registrar al paciente.
            No vas a enviar información vacia o null y no vas a inventar información de los pacientes.
            Solicita los parametros de la siguiente forma (Ejemplo): Ingresa tu número de identificación.
            Una vez registrado el paciente, finaliza con un mensaje satisfactorio.
            En tus respuestas no agregues información adicional, como Agent> o Olivia> o RegisterAgent: u otros.
            chatId: {0}
            ";
        }

        private string GetPromptPlanner()
        {
            return @"
            Eres Olivia, una cordial asistente, tu objetivo es brindar información de los doctores y permitir al paciente programar una cita con uno de ellos.
            No registraras pacientes en la base de datos, solo programaras citas.
            Consulta el día actual si es necesario, consulta la información del paciente si es necesario y consulta la información de los doctores disponibles.
            Comparte al paciente la información de los doctores disponibles resumida.
            Una vez el paciente elija el doctor, pregunta por la fecha y con esto consulta el horario disponible del doctor.
            Una vez el paciente te confirme la hora, programa la cita y finaliza con un mensaje satisfactorio.
            
            No respondas a nada diferente de tu objetivo principal.
            En tus respuestas no agregues Agent> o Olivia> o PlannerAgent: u otros.
            chatId: {0}
            patientId: {1}
            ";
        }

        [HttpPost("Initialize")]
        public async Task<IActionResult> Post()
        {
            try
            {
                var id = await _chats.Create();
                await _chats.NewMessage(id, MessageTypeEnum.Prompt, string.Format(GetPromptRegister(), id));
                var summary = await _chats.GetSummary(id);
                var response = await _agentRegister.Send(summary);
                await _chats.NewMessage(id, MessageTypeEnum.Agent, response);

                return Ok(new AgentMessageDto
                {
                    Id = id,
                    Content = "RegisterAgent: " + response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("NewMessage")]
        public async Task<IActionResult> Post([FromBody] NewMessageDto dto)
        {
            try
            {
                string response = string.Empty;
                Guid chatId = dto.ChatId;

                var chat = await _chats.Get(chatId);
                if (chat == null) return BadRequest("Chat not found");

                await _chats.NewMessage(chatId, MessageTypeEnum.User, dto.Content);
                var summary = await _chats.GetSummary(chatId);

                if (chat.PatientId is null)
                {
                    response = await _agentRegister!.Send(summary);
                    response = "RegisterAgent: " + response;

                    if(chat.PatientId != null)
                    {
                        var newChatId = await _chats.Create();
                        await _chats.AsociatePatient(newChatId, chat.PatientId.Value);
                        await _chats.NewMessage(newChatId, MessageTypeEnum.Prompt, string.Format(GetPromptPlanner(), newChatId, chat.PatientId));
                        var summaryPlanner = await _chats.GetSummary(newChatId);
                        response = await _agentPlanner!.Send(summaryPlanner);
                        response = "PlannerAgent: " + response;
                        chatId = newChatId;
                    }
                }
                else
                {
                    response = await _agentPlanner!.Send(summary);
                    response = "PlannerAgent: " + response;
                }

                await _chats.NewMessage(dto.ChatId, MessageTypeEnum.Agent, response);

                return Ok(new AgentMessageDto
                {
                    Id = chatId,
                    Content = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Resume")]
        public async Task<IActionResult> Post([FromBody] IdDto dto)
        {
            try
            {
                _logger.LogInformation("Getting chat");
                var chat = await _chats.Get(dto.Id);

                _logger.LogInformation("Getting chat messages");
                var messages = await _chats.GetMessages(dto.Id);

                return Ok(new ResumeDto
                {
                    ChatId = chat.Id,
                    Chat = chat,
                    Messages = messages.OrderBy(x => x.CreatedAt)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}