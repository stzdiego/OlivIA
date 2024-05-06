// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Controllers
{
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

    /// <summary>
    /// PatientsAsistenceController class.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsAsistenceController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly ILogger<PatientsAsistenceController> logger;
        private readonly IAgent agentRegister;
        private readonly IAgent agentPlanner;
        private readonly IChatService chats;
        private readonly OliviaDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientsAsistenceController"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="chats">Chat service.</param>
        /// <param name="context">Database context.</param>
        /// <param name="agent1">Agent 1.</param>
        /// <param name="agent2">Agent 2.</param>
        public PatientsAsistenceController(IConfiguration configuration, ILogger<PatientsAsistenceController> logger, IChatService chats, OliviaDbContext context, IAgent agent1, IAgent agent2)
        {
            this.config = configuration;
            this.logger = logger;
            this.chats = chats;
            this.context = context;
            this.agentRegister = agent1;
            this.agentPlanner = agent2;

            this.agentRegister.AddScoped<ChatService>();
            this.agentRegister.AddScoped<PatientService>();
            this.agentRegister.AddScoped<DoctorService>();
            this.agentRegister.AddScoped<ProgramationService>();
            this.agentRegister.AddScoped<IDatabase, DatabaseService>();
            this.agentRegister.AddDbContext<DbContext, OliviaDbContext>(this.context);
            this.agentRegister.AddPlugin<PatientsManagerPlugin>();
            this.agentRegister.Initialize(this.config.GetValue<string>("Agents:Reception:Model") !, this.config.GetValue<string>("Agents:Reception:Key") !, this.config.GetValue<int>("Agents:Reception:MaxTokens"), this.config.GetValue<double>("Agents:Reception:Temperature"), this.config.GetValue<double>("Agents:Reception:Penalty"));

            this.agentPlanner.AddScoped<ChatService>();
            this.agentPlanner.AddScoped<PatientService>();
            this.agentPlanner.AddScoped<DoctorService>();
            this.agentPlanner.AddScoped<ProgramationService>();
            this.agentPlanner.AddScoped<IDatabase, DatabaseService>();
            this.agentPlanner.AddDbContext<DbContext, OliviaDbContext>(this.context);
            this.agentPlanner.AddPlugin<PatientsManagerPlugin>();
            this.agentPlanner.Initialize(this.config.GetValue<string>("Agents:Reception:Model") !, this.config.GetValue<string>("Agents:Reception:Key") !, this.config.GetValue<int>("Agents:Reception:MaxTokens"), this.config.GetValue<double>("Agents:Reception:Temperature"), this.config.GetValue<double>("Agents:Reception:Penalty"));
        }

        /// <summary>
        /// Initializes a new chat.
        /// </summary>
        /// <returns>Chat id.</returns>
        [HttpPost("Initialize")]
        public async Task<IActionResult> Post()
        {
            try
            {
                var id = await this.chats.Create();
                await this.chats.NewMessage(id, MessageTypeEnum.Prompt, string.Format(this.GetPromptRegister(), id));
                var summary = await this.chats.GetSummary(id);
                var response = await this.agentRegister.Send(summary);
                await this.chats.NewMessage(id, MessageTypeEnum.Agent, response);

                return this.Ok(new AgentMessageDto
                {
                    Id = id,
                    Content = "RegisterAgent: " + response,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="dto">New message dto.</param>
        /// <returns>Chat id.</returns>
        [HttpPost("NewMessage")]
        public async Task<IActionResult> Post([FromBody] NewMessageDto dto)
        {
            try
            {
                string response = string.Empty;
                Guid chatId = dto.ChatId;

                var chat = await this.chats.Get(chatId);
                if (chat == null)
                {
                    return this.BadRequest("Chat not found");
                }

                await this.chats.NewMessage(chatId, MessageTypeEnum.User, dto.Content);
                var summary = await this.chats.GetSummary(chatId);

                if (chat.PatientId is null)
                {
                    response = await this.agentRegister!.Send(summary);
                    response = "RegisterAgent: " + response;

                    if (chat.PatientId != null)
                    {
                        var newChatId = await this.chats.Create();
                        await this.chats.AsociatePatient(newChatId, chat.PatientId.Value);
                        await this.chats.NewMessage(newChatId, MessageTypeEnum.Prompt, string.Format(this.GetPromptPlanner(), newChatId, chat.PatientId));
                        var summaryPlanner = await this.chats.GetSummary(newChatId);
                        response = await this.agentPlanner!.Send(summaryPlanner);
                        response = "PlannerAgent: " + response;
                        chatId = newChatId;
                    }
                }
                else
                {
                    response = await this.agentPlanner!.Send(summary);
                    response = "PlannerAgent: " + response;
                }

                await this.chats.NewMessage(dto.ChatId, MessageTypeEnum.Agent, response);

                return this.Ok(new AgentMessageDto
                {
                    Id = chatId,
                    Content = response,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Resumes a chat.
        /// </summary>
        /// <param name="dto">Id dto.</param>
        /// <returns>Chat id.</returns>
        [HttpPost("Resume")]
        public async Task<IActionResult> Post([FromBody] IdDto dto)
        {
            try
            {
                this.logger.LogInformation("Getting chat");
                var chat = await this.chats.Get(dto.Id);

                this.logger.LogInformation("Getting chat messages");
                var messages = await this.chats.GetMessages(dto.Id);

                return this.Ok(new ResumeDto
                {
                    ChatId = chat.Id,
                    Chat = chat,
                    Messages = messages.OrderBy(x => x.CreatedAt),
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.BadRequest(ex.Message);
            }
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
    }
}