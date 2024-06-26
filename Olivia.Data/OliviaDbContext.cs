// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#pragma warning disable SA1201 // ElementsMustAppearInTheCorrectOrder
namespace Olivia.Data;
using Microsoft.EntityFrameworkCore;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// OliviaDbContext class.
/// </summary>
public class OliviaDbContext : DbContext, IOliviaDbContext
{
    /// <summary>
    /// Gets or sets the Doctors DbSet.
    /// </summary>
    public DbSet<Doctor> Doctors { get; set; }

    /// <summary>
    /// Gets or sets the Patients DbSet.
    /// </summary>
    public DbSet<Patient> Patients { get; set; }

    /// <summary>
    /// Gets or sets the Chats DbSet.
    /// </summary>
    public DbSet<Chat> Chats { get; set; }

    /// <summary>
    /// Gets or sets the Messages DbSet.
    /// </summary>
    public DbSet<Message> Messages { get; set; }

    /// <summary>
    /// Gets or sets the Appointments DbSet.
    /// </summary>
    public DbSet<Appointment> Appointments { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OliviaDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContextOptions.</param>
    public OliviaDbContext(DbContextOptions<OliviaDbContext> options)
        : base(options)
    {
    }
}
#pragma warning restore SA1201 // ElementsMustAppearInTheCorrectOrder