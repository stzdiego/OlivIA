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
using Olivia.Shared.Interfaces;

/// <summary>
/// The OpenAI agent.
/// </summary>
public class OpenAIAgent
{
    /// <summary>
    /// Gets the plugins.
    /// </summary>
    public List<Type> Plugins { get; } =[];

    /// <summary>
    /// Gets the services.
    /// </summary>
    public List<Type> Services { get; } =[];

    private readonly IKernelBuilder? builder;
    private OpenAIPromptExecutionSettings? settings;
    private Kernel? kernel;
    private IChatCompletionService? chatCompletionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAgent"/> class.
    /// </summary>
    public OpenAIAgent()
    {
        this.builder = Kernel.CreateBuilder();
    }

    /// <summary>
    /// Adds the plugin.
    /// </summary>
    /// <typeparam name="T">The plugin type.</typeparam>
    public void AddPlugin<T>()
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
        try
        {
            if (this.Services.Contains(typeof(TInterface)))
            {
                return;
            }

            this.builder!.Services.AddScoped<TInterface, TClass>();
            this.Services.Add(typeof(TInterface));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
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
    public void Initialize(string modelId, string apiKey, int maxTokens, double temperature = 0.5, double presencePenalty = 0.0)
    {
        this.builder!.Services.AddLogging();
        this.builder!.Services.AddOpenAIChatCompletion(modelId, apiKey);
        this.kernel = this.builder.Build();
        this.chatCompletionService = this.kernel.GetRequiredService<IChatCompletionService>();
        this.settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            MaxTokens = maxTokens,
            Temperature = temperature,
            PresencePenalty = presencePenalty,
        };
    }

    /// <summary>
    /// Sends the specified string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <returns>The response.</returns>
    public async Task<string> Send(StringBuilder stringBuilder)
    {
        var response = await this.chatCompletionService!.GetChatMessageContentAsync(
            stringBuilder.ToString(),
            this.settings!,
            this.kernel!);

        return response.ToString();
    }
}