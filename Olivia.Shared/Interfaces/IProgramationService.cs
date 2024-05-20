// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using Olivia.Shared.Entities;

/// <summary>
/// Programation service interface.
/// </summary>
public interface IProgramationService
{
    /// <summary>
    /// Create an appointment.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="patientId">Patient id.</param>
    /// <param name="date">Date.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>Appointment id.</returns>
    Task<Guid> CreateAppointment(Guid doctorId, Guid patientId, DateTime date, string reason);

    /// <summary>
    /// Get the appointments list for a doctor.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="date">Date.</param>
    /// <returns>Appointments list.</returns>
    Task<IEnumerable<Appointment>?> GetAppointmentsListDay(Guid doctorId, DateTime date);

    /// <summary>
    /// Get the appointments list for a doctor in a range of dates.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <returns>Appointments list.</returns>
    Task<IEnumerable<Appointment>?> GetAppointmentsListRange(Guid doctorId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Update an appointment.
    /// </summary>
    /// <param name="patientId">Patient id.</param>
    /// <param name="doctorId">Doctor id.</param>
    /// <returns>Appointment.</returns>
    Task<Appointment> Find(Guid patientId, Guid doctorId);
}
