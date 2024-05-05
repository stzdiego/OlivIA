// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IDatabase
    {
        Task<T> Add<T>(T entity)
            where T : class;

        Task<T?> Find<T>(Expression<Func<T, bool>> condition)
            where T : class;

        Task<List<T>> Get<T>()
            where T : class;

        Task<List<T>> Get<T>(Expression<Func<T, bool>> condition)
            where T : class;

        Task<bool> Exist<T>(Expression<Func<T, bool>> condition)
            where T : class;

        Task<T> Update<T>(T entity)
            where T : class;

        Task Delete<T>(T entity)
            where T : class;
    }
}