// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Extensions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// MigrationExtensions class.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// ApplyMigrations method.
    /// </summary>
    /// <typeparam name="T">Context.</typeparam>
    /// <param name="app">ApplicationBuilder.</param>
    public static void ApplyMigrations<T>(this IApplicationBuilder app)
        where T : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using T context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}
