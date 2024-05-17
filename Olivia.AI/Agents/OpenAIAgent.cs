// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// The OpenAI agent.
/// </summary>
public class OpenAIAgent : IAgent
{
    /// <summary>
    /// Gets the plugins.
    /// </summary>
    public List<Type> Plugins { get; } = new List<Type>();

    /// <summary>
    /// Gets the services.
    /// </summary>
    public List<Type> Services { get; } = new List<Type>();

    private readonly IAgentSettings settings;
    private readonly IKernelBuilder builder;
    private OpenAIPromptExecutionSettings? prompSettings;
    private Kernel? kernel;
    private IChatCompletionService? chatCompletionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAgent"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public OpenAIAgent(IAgentSettings settings)
    {
        this.settings = settings;
        this.builder = Kernel.CreateBuilder();
    }

    /// <summary>
    /// Adds the plugin.
    /// </summary>
    /// <typeparam name="T">The plugin type.</typeparam>
    public void AddPlugin<T>()
        where T : class
    {
        if (!typeof(IPlugin).IsAssignableFrom(typeof(T)))
        {
            throw new Exception("T is not a plugin");
        }

        if (this.Plugins.Contains(typeof(T)))
        {
            return;
        }

        this.builder!.Plugins.AddFromType<T>();
        this.Plugins.Add(typeof(T));
    }

    /// <summary>
    /// Adds the plugin.
    /// </summary>
    /// <typeparam name="TInterface">TIhe interface type.</typeparam>
    /// <typeparam name="TClass">The class type.</typeparam>
    /// <exception cref="Exception">T is not a plugin.</exception>
    public void AddPlugin<TInterface, TClass>()
        where TInterface : class
        where TClass : class, TInterface
    {
        if (this.Plugins.Contains(typeof(TInterface)))
        {
            return;
        }

        this.builder!.Plugins.AddFromType<TInterface>().AddFromType<TClass>();
        this.Plugins.Add(typeof(TInterface));
    }

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    public void AddSingleton<T>()
        where T : class
    {
        if (this.Services.Contains(typeof(T)))
        {
            return;
        }

        this.builder!.Services.AddSingleton<T>();
        this.Services.Add(typeof(T));
    }

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    public void AddScoped<T>()
        where T : class
    {
        if (this.Services.Contains(typeof(T)))
        {
            return;
        }

        this.builder!.Services.AddScoped<T>();
        this.Services.Add(typeof(T));
    }

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TClass">The class type.</typeparam>
    public void AddScoped<TInterface, TClass>()
        where TInterface : class
        where TClass : class, TInterface
    {
        if (this.Services.Contains(typeof(TInterface)))
        {
            return;
        }

        this.builder!.Services.AddScoped<TInterface, TClass>();
        this.Services.Add(typeof(TInterface));
    }

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <typeparam name="TContextService">The context service type.</typeparam>
    /// <typeparam name="TContextImplementation">The context implementation type.</typeparam>
    /// <param name="context">Context instance.</param>
    public void AddDbContext<TContextService, TContextImplementation>(TContextImplementation context)
        where TContextService : DbContext
        where TContextImplementation : class, TContextService
    {
        if (this.Services.Contains(typeof(TContextService)))
        {
            return;
        }

        this.builder!.Services.AddSingleton<TContextService>(context);
        this.Services.Add(typeof(TContextService));
    }

    /// <summary>
    /// Initializes the OpenAI agent.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="apiKey">The API key.</param>
    /// <param name="maxTokens">The maximum tokens.</param>
    /// <param name="temperature">The temperature.</param>
    /// <param name="presencePenalty">The presence penalty.</param>
    public void Initialize()
    {
        this.builder!.Services.AddLogging();
        this.builder!.Services.AddOpenAIChatCompletion(this.settings.Model, this.settings.Key);
        this.kernel = this.builder.Build();
        this.chatCompletionService = this.kernel.GetRequiredService<IChatCompletionService>();
        this.prompSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            MaxTokens = this.settings.MaxTokens,
            Temperature = this.settings.Temperature,
            PresencePenalty = this.settings.PresencePenalty,
            FrequencyPenalty = this.settings.FrequencyPenalty,
            StopSequences = this.settings.StopSequences,
            TopP = this.settings.TopP,
        };
    }

    /// <summary>
    /// Sends the specified string builder.
    /// </summary>
    /// <param name="summary">The string builder.</param>
    /// <returns>The response.</returns>
    public async Task<string> Send(List<Message> summary)
    {
        var response = await this.chatCompletionService!.GetChatMessageContentAsync(
            this.ConvertToChatHistory(summary),
            this.prompSettings!,
            this.kernel!);

        return response.ToString();
    }

    private ChatHistory ConvertToChatHistory(List<Message> messages)
    {
        var chatHistory = new ChatHistory();

        foreach (var message in messages)
        {
            if (message.Type is Shared.Enums.MessageTypeEnum.Prompt)
            {
                chatHistory.AddSystemMessage(message.Content);
            }
            else if (message.Type is Shared.Enums.MessageTypeEnum.User)
            {
                chatHistory.AddUserMessage(message.Content);
            }
            else if (message.Type is Shared.Enums.MessageTypeEnum.Agent)
            {
                chatHistory.AddAssistantMessage(message.Content);
            }
        }

        return chatHistory;
    }
}