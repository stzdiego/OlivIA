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
            Eres Olivia, una asistente de doctores, tu objetivo es solicitar la información necesaria
            para registrar un paciente en la base de datos, pide los parametros uno por uno y valida
            que sean coherentemente correctos.
            Los parametros son: Identificación, Nombre, Apellido, Correo electronico, Telefono celular y razón de consulta.
            Solicita los parametros de la siguiente forma (Ejemplo): 1. Ingresa tu número de identificación.
            Solo registraras un paciente por lo cual habla con el en primera persona.
            No respondas a nada diferente de tu objetivo principal.
            Inicia presentandote con un saludo cordial y el objetivo de tu asistencia.
            Una vez registrado el paciente, finaliza con un mensaje satisfactorio.
            El paciente solo se comunicara contigo si no esta registrado en la base de datos, por lo cual 
            debes insistir en la información necesaria para registrar al paciente y ejecutar el proceso de registro.
            chatId: {0}
            ";
        }

        private string GetPromptPlanner()
        {
            return @"
            Eres Olivia, una asistente de doctores.
            Tu objetivo es programar una cita para un paciente registrado en la base de datos.
            Inicia consultando el paciente registrado en la base de datos utilizando el patientId.
            Continua consultando la información de los doctores disponibles.
            En base a la razón de consulta del paciente, recomienda un doctor.
            Consulta la confirmación del paciente para programar la cita con el doctor recomendado.
            Una vez confirmado el doctor, consulta la disponibilidad de fechas y horas, para esto debes solicitar la fecha de la cita.
            Solicita al paciente la fecha y hora de la cita y procede a programarla ejecutando el proceso de regristro de programación.
            No respondas a nada diferente de tu objetivo principal.
            Finaliza con un mensaje satisfactorio y el resumen de la cita programada.
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
                    Content = response
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

                var chat = await _chats.Get(dto.ChatId);
                if (chat == null) return BadRequest("Chat not found");

                await _chats.NewMessage(dto.ChatId, MessageTypeEnum.User, dto.Content);
                var summary = await _chats.GetSummary(dto.ChatId);

                if (chat.PatientId is null)
                {
                    response = await _agentRegister!.Send(summary);

                    if(chat.PatientId != null)
                    {
                        var chatId = await _chats.Create();
                        await _chats.AsociatePatient(chatId, chat.PatientId.Value);
                        await _chats.NewMessage(chatId, MessageTypeEnum.Prompt, string.Format(GetPromptPlanner(), chatId, chat.PatientId));
                        var summaryPlanner = await _chats.GetSummary(chatId);
                        response = await _agentPlanner!.Send(summaryPlanner);
                        dto.ChatId = chatId;
                        dto.Content = response;
                    }
                }
                else
                {
                    response = await _agentPlanner!.Send(summary);
                }

                await _chats.NewMessage(dto.ChatId, MessageTypeEnum.Agent, response);

                return Ok(new AgentMessageDto
                {
                    Id = dto.ChatId,
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