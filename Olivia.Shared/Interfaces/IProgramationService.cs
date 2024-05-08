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
    /// Get the doctor id by name.
    /// </summary>
    /// <param name="name">Doctor name.</param>
    /// <returns>Id of the doctor.</returns>
    Task<Guid> GetDoctorId(string name);

    /// <summary>
    /// Get the available hours for a doctor.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="date">Date.</param>
    /// <returns>Available hours.</returns>
    Task<IEnumerable<TimeSpan>> GetAvailableHours(Guid doctorId, DateTime date);

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
}
