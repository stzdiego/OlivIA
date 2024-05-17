// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Controllers;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olivia.AI.Planners;
using Olivia.AI.Plugins;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a controller that manages patient assistance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ChatAssistanceController : ControllerBase
{
    private readonly IChatService chatService;
    private readonly IAgent agentRegisterPatient;
    private readonly OliviaDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAssistanceController"/> class.
    /// </summary>
    /// <param name="chatService">Chat service.</param>
    /// <param name="agentRegisterPatient">Agent.</param>
    /// <param name="context">Context.</param>
    public ChatAssistanceController(IChatService chatService, IAgent agentRegisterPatient, OliviaDbContext context)
    {
        this.chatService = chatService;
        this.agentRegisterPatient = agentRegisterPatient;
        this.context = context;

        this.agentRegisterPatient.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agentRegisterPatient.AddScoped<IDatabase, DatabaseService>();
        this.agentRegisterPatient.AddScoped<IChatService, ChatService>();
        this.agentRegisterPatient.AddScoped<IPatientService, PatientService>();
        this.agentRegisterPatient.AddScoped<IDoctorService, DoctorService>();
        this.agentRegisterPatient.AddPlugin<GeneralPlugin>();
        ////this.agentRegisterPatient.AddPlugin<RegisterPatientPlanner>();
        this.agentRegisterPatient.AddPlugin<PatientManagerPlugin>();
        this.agentRegisterPatient.Initialize();
    }

    /// <summary>
    /// Registers a new patient in the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpGet("NewChat")]
    public async Task<IActionResult> NewChat()
    {
        try
        {
            IdDto chatId = new IdDto { Id = await this.chatService.Create() };

            return this.Ok(chatId);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Registers a new patient in the system.
    /// </summary>
    /// <param name="chatMessage">The chat message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpPost("NewMessage")]
    public async Task<IActionResult> NewMessage(NewMessageDto chatMessage)
    {
        try
        {
            var messages = await this.chatService.GetSummary(chatMessage.ChatId);

            if (messages is null)
            {
                return this.NotFound("Chat not found.");
            }
            else if (messages.Count == 0)
            {
                var prompt = string.Format(this.GetRegisterPersonality(), chatMessage.ChatId);
                await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.Prompt, prompt);
            }

            await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.User, chatMessage.Content);
            messages = await this.chatService.GetSummary(chatMessage.ChatId);
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agentRegisterPatient.Send(messages) };
            await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.Agent, response.Content);

            var chat = await this.chatService.Get(chatMessage.ChatId);
            if (chat.SenderId.HasValue)
            {
                response.SenderId = chat.SenderId.Value;
            }

            return this.Ok(response);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private string GetRegisterPersonality()
    {
        return """
        Eres un asistente amigable al que le gusta seguir las reglas. Completarás los pasos requeridos
        y solicitaras aprobación antes de tomar cualquier acción consiguiente. Si el usuario no proporciona
        suficiente información para completar una tarea, seguirá haciendo preguntas hasta que haya
        suficiente información para completar la tarea.
        La tarea principal es programar una cita con un médico.
        Tu te encargarás de recopilar la información necesaria para registrar al paciente en el sistema.
        Una vez registres el paciente exitosamente, responde con un mensaje de confirmación de registro.
        ChatId: {0}

        [Patient]
        - Identificación
        - Nombre
        - Apellido
        - Correo electrónico
        - Teléfono
        - Razón de la cita
        """;
    }
}
