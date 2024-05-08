// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// IAgent interface.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Gets the plugins.
    /// </summary>
    List<Type> Plugins { get; }

    /// <summary>
    /// Gets the services.
    /// </summary>
    List<Type> Services { get; }

    /// <summary>
    /// Adds the plugin.
    /// </summary>
    /// <typeparam name="T">The plugin type.</typeparam>
    void AddPlugin<T>()
        where T : class;

    /// <summary>
    /// Adds the plugin.
    /// </summary>
    /// <typeparam name="TInterface">Interface type.</typeparam>
    /// <typeparam name="TClass">Class type.</typeparam>
    void AddPlugin<TInterface, TClass>()
        where TInterface : class
        where TClass : class, TInterface;

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    void AddSingleton<T>()
        where T : class;

    /// <summary>
    /// Adds the scoped.
    /// </summary>
    /// <typeparam name="T">The scoped type.</typeparam>
    void AddScoped<T>()
        where T : class;

    /// <summary>
    /// Adds the scoped.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TClass">The class type.</typeparam>
    void AddScoped<TInterface, TClass>()
        where TInterface : class
        where TClass : class, TInterface;

    /// <summary>
    /// Adds the transient.
    /// </summary>
    /// <typeparam name="TContextService">The context service type.</typeparam>
    /// <typeparam name="TContextImplementation">The context implementation type.</typeparam>
    /// <param name="context">Context instance.</param>
    void AddDbContext<TContextService, TContextImplementation>(TContextImplementation context)
        where TContextService : DbContext
        where TContextImplementation : class, TContextService;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Sends the specified string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <returns>Task.</returns>
    Task<string> Send(StringBuilder stringBuilder);
}
