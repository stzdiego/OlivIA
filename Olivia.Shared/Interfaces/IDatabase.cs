// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// IDatabase interface.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Add an entity to the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The added entity.</returns>
        Task<T> Add<T>(T entity)
            where T : class;

        /// <summary>
        /// Find an entity in the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <returns>The entity.</returns>
        Task<T?> Find<T>(Expression<Func<T, bool>> condition)
            where T : class;

        /// <summary>
        /// Get all entities of a type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The entities.</returns>
        Task<List<T>> Get<T>()
            where T : class;

        /// <summary>
        /// Get entities of a type that match a condition.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <returns>The entities.</returns>
        Task<List<T>> Get<T>(Expression<Func<T, bool>> condition)
            where T : class;

        /// <summary>
        /// Check if an entity exists in the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <returns>True if the entity exists, false otherwise.</returns>
        Task<bool> Exist<T>(Expression<Func<T, bool>> condition)
            where T : class;

        /// <summary>
        /// Update an entity in the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        Task<T> Update<T>(T entity)
            where T : class;

        /// <summary>
        /// Delete an entity from the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Delete<T>(T entity)
            where T : class;
    }
}