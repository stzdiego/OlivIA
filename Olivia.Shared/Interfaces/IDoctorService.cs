// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using Olivia.Shared.Entities;

/// <summary>
/// Doctor service.
/// </summary>
public interface IDoctorService
{
    /// <summary>
    /// Creates a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="speciality">Speciality.</param>
    /// <param name="information">Information.</param>
    /// <param name="start">Start hour.</param>
    /// <param name="end">End hour.</param>
    /// <returns>Doctor id.</returns>
    Task<Guid> Create(long identification, string name, string lastName, string email, long phone, string speciality, string information, TimeSpan start, TimeSpan end);

    /// <summary>
    /// Updates a doctor.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="speciality">Speciality.</param>
    /// <param name="information">Information.</param>
    /// <returns>Task.</returns>
    Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string speciality, string information);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    Task<bool> Exists(int identification);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <returns>Task.</returns>
    Task<IEnumerable<Doctor>> Get();

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <returns>Task.</returns>
    Task<IEnumerable<Doctor>?> GetAvailable();

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Task.</returns>
    Task<Doctor> Find(Guid id);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    Task<Doctor> Find(long identification);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Task.</returns>
    Task Delete(Guid id);
}
