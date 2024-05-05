// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Olivia.Shared.Interfaces;

namespace Olivia.Services;
public class DatabaseService : IDatabase
{
    private readonly DbContext context;

    public DatabaseService(DbContext context)
    {
        this.context = context;
    }

    public async Task<bool> Exist<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        try
        {
            return await this.context.Set<T>().AnyAsync(condition);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking existence: {ex.Message}");
            throw;
        }
    }

    public async Task<T> Add<T>(T entity)
        where T : class
    {
        try
        {
            await this.context.Set<T>().AddAsync(entity);
            await this.context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding entity: {ex.Message}");
            throw;
        }
    }

    public async Task<T?> Find<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        try
        {
            return await this.context.Set<T>().SingleOrDefaultAsync(condition);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting entity: {ex.Message}");
            throw;
        }
    }

    public async Task<List<T>> Get<T>()
        where T : class
    {
        try
        {
            return await this.context.Set<T>().ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting entities: {ex.Message}");
            throw;
        }
    }

    public async Task<List<T>> Get<T>(Expression<Func<T, bool>> condition)
        where T : class
    {
        try
        {
            return await this.context.Set<T>().Where(condition as Expression<Func<T, bool>>).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting entities: {ex.Message}");
            throw;
        }
    }

    public async Task<T> Update<T>(T entity)
        where T : class
    {
        try
        {
            this.context.Set<T>().Update(entity);
            await this.context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating entity: {ex.Message}");
            throw;
        }
    }

    public async Task Delete<T>(T entity)
        where T : class
    {
        try
        {
            this.context.Set<T>().Remove(entity);
            await this.context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting entity: {ex.Message}");
            throw;
        }
    }
}