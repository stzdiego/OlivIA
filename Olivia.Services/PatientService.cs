// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Patient service.
/// </summary>
public class PatientService
{
    private readonly IDatabase database;
    private readonly ILogger<PatientService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientService"/> class.
    /// </summary>
    /// <param name="database">Database.</param>
    /// <param name="logger">Logger.</param>
    public PatientService(IDatabase database, ILogger<PatientService> logger)
    {
        this.database = database;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a patient.
    /// </summary>
    /// /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>Patient id.</returns>
    public virtual async Task<Guid> Create(long identification, string name, string lastName, string email, long phone, string reason)
    {
        this.logger.LogInformation("Creating chat service");
        Patient patient = new ()
        {
            Identification = identification,
            Name = name,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Reason = reason,
        };

        await this.database.Add(patient);
        this.logger.LogInformation("Patient created");
        return patient.Id;
    }

    /// <summary>
    /// Updates a patient.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>Task.</returns>
    public virtual async Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string reason)
    {
        this.logger.LogInformation("Updating patient");
        Patient patient = await this.database.Find<Patient>(x => x.Id == id)
            ?? throw new Exception("Patient not found");

        patient.Identification = identification;
        patient.Name = name;
        patient.LastName = lastName;
        patient.Email = email;
        patient.Phone = phone;
        patient.Reason = reason;

        await this.database.Update(patient);
        this.logger.LogInformation("Patient updated");
    }

    /// <summary>
    /// Checks if a patient exists.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>True if exists, false otherwise.</returns>
    public virtual async Task<bool> Exists(long identification)
    {
        this.logger.LogInformation("Checking if patient exists");
        return await this.database.Exist<Patient>(p => p.Identification == identification);
    }

    /// <summary>
    /// Finds a patient.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Patient.</returns>
    public virtual async Task<Patient?> Find(Guid id)
    {
        this.logger.LogInformation("Finding patient");
        return await this.database.Find<Patient>(x => x.Id == id);
    }
}
