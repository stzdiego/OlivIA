// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;

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
    /// Gets the available doctors.
    /// </summary>
    /// <returns>Task.</returns>
    Task<IEnumerable<Doctor>?> GetAvailable();

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Task.</returns>
    Task<Doctor?> Find(Guid id);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    Task<Doctor?> Find(long identification);

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Task.</returns>
    Task Delete(Guid id);

    /// <summary>
    /// Gets the most recent available appointment.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <returns>Task.</returns>
    Task<DateTime> GetMostRecentAvailableAppointmentAsync(Guid id);

    /// <summary>
    /// Gets the available appointment by date.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="date">Date.</param>
    /// <returns>Task.</returns>
    Task<IList<DateTime>> GetAvailableAppointmentsByDate(Guid id, DateTime date);

    /// <summary>
    /// Gets the available appointment by date.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <param name="status">Status.</param>
    /// <returns>Task.</returns>
    Task<IEnumerable<PatientAppointmentDto>> GetPatientsPendingByDoctorByDate(Guid doctorId, DateTime start, DateTime end, PatientStatusEnum status);

    /// <summary>
    /// Approves a patient.
    /// </summary>
    /// <param name="patientId">Patient id.</param>
    /// <returns>Task.</returns>
    /// <exception cref="Exception">Patient not found.</exception>
    Task<bool> ApprovePatient(Guid patientId);

    /// <summary>
    /// Refuses a patient.
    /// </summary>
    /// <param name="patientId">Patient id.</param>
    /// <returns>Task.</returns>
    Task<bool> RefusedPatient(Guid patientId);

    /// <summary>
    /// Refuses a patient.
    /// </summary>
    /// <param name="patientId">Patient id.</param>
    /// <returns>Task.</returns>
    /// <exception cref="Exception">Patient not found.</exception>
    Task<bool> PayPatient(Guid patientId);
}
