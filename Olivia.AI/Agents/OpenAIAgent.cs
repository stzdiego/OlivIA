using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Olivia.AI.Agents;
public class OpenAIAgent
{
    private readonly ILogger<OpenAIAgent> _logger;
    private IKernelBuilder? _builder;
    private OpenAIPromptExecutionSettings? _settings;
    private Kernel? _kernel;
    private IChatCompletionService? _chatCompletionService;

    public OpenAIAgent(ILogger<OpenAIAgent> logger)
    {
        _logger = logger;
        _builder = Kernel.CreateBuilder();
    }

    public void AddPlugin<T>()
    {
        try
        {
            _builder!.Plugins.AddFromType<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void AddSingleton<T>() where T : class
    {
        try
        {
            _builder!.Services.AddSingleton<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void AddScoped<T>() where T : class
    {
        try
        {
            _builder!.Services.AddScoped<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void AddScoped<TInterface, TClass>() where TInterface : class where TClass : class, TInterface
    {
        try
        {
            _builder!.Services.AddScoped<TInterface, TClass>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void AddDbContext<TContextService, TContextImplementation>(TContextImplementation context)
    where TContextService : DbContext
    where TContextImplementation : class, TContextService
    {
        try
        {
            _builder!.Services.AddSingleton<TContextService>(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void Initialize(string modelId, string apiKey, int maxTokens, double temperature = 0.5, double presencePenalty = 0.0)
    {
        try
        {
            _builder!.Services.AddLogging();
            _builder!.Services.AddOpenAIChatCompletion(modelId, apiKey);
            _kernel = _builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                MaxTokens = maxTokens,
                Temperature = temperature,
                PresencePenalty = presencePenalty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<string> Send(StringBuilder stringBuilder)
    {
        try
        {
            var response = await _chatCompletionService!.GetChatMessageContentAsync(
                stringBuilder.ToString(),
                _settings!,
                _kernel!
            );

            return response.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}