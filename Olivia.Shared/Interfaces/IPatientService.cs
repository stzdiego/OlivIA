// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using Olivia.Shared.Entities;

/// <summary>
/// Patient service interface.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Creates a patient.
    /// </summary>
    /// <param name="patient">Patient.</param>
    /// <returns>Patient id.</returns>
    Task<Patient> Create(Patient patient);

    /// <summary>
    /// Creates a patient.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>Patient id.</returns>
    Task<Guid> Create(long identification, string name, string lastName, string email, long phone, string reason);

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
    Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string reason);

    /// <summary>
    /// Deletes a patient.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    Task<bool> Exists(long identification);

    /// <summary>
    /// Finds a patient.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Patient.</returns>
    Task<Patient?> Find(Guid id);
}
