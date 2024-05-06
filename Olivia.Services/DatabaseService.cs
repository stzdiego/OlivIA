// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Olivia.Shared.Interfaces;

/// <summary>
/// Database service.
/// </summary>
public class DatabaseService : IDatabase
{
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public DatabaseService(DbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Check if entity exists.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="condition">Condition.</param>
    /// <returns>True if entity exists, false otherwise.</returns>
    public async Task<bool> Exist<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        return await this.context.Set<T>().AnyAsync(condition);
    }

    /// <summary>
    /// Add entity.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="entity">Entity.</param>
    /// <returns>Entity .</returns>
    public async Task<T> Add<T>(T entity)
        where T : class
    {
        await this.context.Set<T>().AddAsync(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Find entity.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="condition">Condition.</param>
    /// <returns>Entity.</returns>
    public async Task<T?> Find<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        return await this.context.Set<T>().SingleOrDefaultAsync(condition);
    }

    /// <summary>
    /// Get all entities.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <returns>List of entities.</returns>
    public async Task<List<T>> Get<T>()
        where T : class
    {
        return await this.context.Set<T>().ToListAsync();
    }

    /// <summary>
    /// Get entities based on condition.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="condition">Condition.</param>
    /// <returns>List of entities.</returns>
    public async Task<List<T>> Get<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        return await this.context.Set<T>().Where(condition as Expression<Func<T, bool>>).ToListAsync();
    }

    /// <summary>
    /// Update entity.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="entity">Entity.</param>
    /// <returns>Entity updated.</returns>
    public async Task<T> Update<T>(T entity)
        where T : class
    {
        this.context.Set<T>().Update(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Delete entity.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="entity">Entity.</param>
    /// <returns>Task.</returns>
    public async Task Delete<T>(T entity)
        where T : class
    {
        this.context.Set<T>().Remove(entity);
        await this.context.SaveChangesAsync();
    }
}