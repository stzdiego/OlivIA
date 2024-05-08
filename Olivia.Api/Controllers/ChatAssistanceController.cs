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
    private readonly IAgent agent;
    private readonly OliviaDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAssistanceController"/> class.
    /// </summary>
    /// <param name="chatService">Chat service.</param>
    /// <param name="agent">Agent.</param>
    /// <param name="context">Context.</param>
    public ChatAssistanceController(IChatService chatService, IAgent agent, OliviaDbContext context)
    {
        this.chatService = chatService;
        this.agent = agent;
        this.context = context;

        this.agent.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agent.AddScoped<IDatabase, DatabaseService>();
        this.agent.AddScoped<IPatientService, PatientService>();
        this.agent.AddScoped<IDoctorService, DoctorService>();
        this.agent.AddPlugin<GeneralPlugin>();
        this.agent.AddPlugin<RegisterPatientPlanner>();
        this.agent.AddPlugin<PatientManagerPlugin2>();
        this.agent.Initialize();
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
            else if (messages.Length == 0)
            {
                await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.Prompt, this.GetPersonality());
            }

            await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.User, chatMessage.Content);
            messages = await this.chatService.GetSummary(chatMessage.ChatId);
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agent.Send(messages) };
            await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.Agent, response.Content);

            return this.Ok(response);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private string GetPersonality()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append("""
        You are a friendly assistant who likes to follow the rules. You will complete required steps
        and request approval before taking any consequential actions. If the user doesn't provide
        enough information for you to complete a task, you will keep asking questions until you have
        enough information to complete the task. You will respond in Spanish.
        """);

        return strBuilder.ToString();
    }
}
