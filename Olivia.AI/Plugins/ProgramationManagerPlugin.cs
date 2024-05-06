// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Plugin para la gestión de programaciones.
/// </summary>
public class ProgramationManagerPlugin : IPlugin
{
    private readonly ProgramationService programations;
    private readonly DoctorService doctors;
    private readonly ChatService chats;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramationManagerPlugin"/> class.
    /// </summary>
    /// <param name="programations">ProgramationService.</param>
    /// <param name="chats">ChatService.</param>
    /// <param name="doctors">DoctorService.</param>
    public ProgramationManagerPlugin(ProgramationService programations, ChatService chats, DoctorService doctors)
    {
        this.programations = programations;
        this.chats = chats;
        this.doctors = doctors;
    }

    /// <summary>
    /// Obtiene la fecha actual.
    /// </summary>
    /// <returns>Fecha actual.</returns>
    [KernelFunction]
    [Description("Obtiene la fecha actual.")]
    public DateTime GetDate()
    {
        return DateTime.Now;
    }

    /// <summary>
    /// Get the doctor id by identification.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="identification">Número de identificación del doctor.</param>
    /// <returns>Id del doctor.</returns>
    [KernelFunction]
    [Description("Obtiene el id del doctor solicitando su numero de identificación.")]
    public async Task<Guid> GetDoctorId(Kernel kernel, long identification)
    {
        var doctor = await this.doctors.Find(identification);
        return doctor.Id;
    }

    /// <summary>
    /// Get appointments by doctor today.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Id del chat.</param>
    /// <param name="doctorId">Id del doctor.</param>
    /// <returns>Lista de citas.</returns>
    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en el dia actual.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorToday(
        Kernel kernel,
        Guid chatId,
        Guid doctorId)
    {
        return await this.programations.GetAppointmentsListDay(doctorId, DateTime.Now);
    }

    /// <summary>
    /// Get appointments by doctor by date.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Id del chat.</param>
    /// <param name="doctorId">Id del doctor.</param>
    /// <param name="date">Fecha de la cita.</param>
    /// <returns>Lista de citas.</returns>
    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en una fecha especifica.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByDate(
        Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de la cita")] DateTime date)
    {
        return await this.programations.GetAppointmentsListDay(doctorId, date);
    }

    /// <summary>
    /// Get appointments by doctor by range.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">ChatId.</param>
    /// <param name="doctorId">DoctorId.</param>
    /// <param name="startDate">StartDate.</param>
    /// <param name="endDate">EndDate.</param>
    /// <returns>Lista de citas.</returns>
    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en un rango de fechas.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByRange(
        Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de inicio del rango")] DateTime startDate,
        [Description("Fecha de fin del rango")] DateTime endDate)
    {
        return await this.programations.GetAppointmentsListRange(doctorId, startDate, endDate);
    }
}