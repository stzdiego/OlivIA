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
    /// Update an appointment.
    /// </summary>
    /// <param name="patientId">Patient id.</param>
    /// <param name="doctorId">Doctor id.</param>
    /// <returns>Appointment.</returns>
    Task<Appointment> Find(Guid patientId, Guid doctorId);
}
