// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.AI.Plugins;
public class ProgramationManagerPlugin : IPlugin
{
    private readonly ProgramationService programations;
    private readonly DoctorService doctors;
    private readonly ChatService chats;

    public ProgramationManagerPlugin(ProgramationService programations, ChatService chats, DoctorService doctors)
    {
        this.programations = programations;
        this.chats = chats;
        this.doctors = doctors;
    }

    [KernelFunction]
    [Description("Obtiene la fecha actual.")]
    public DateTime GetDate()
    {
        return DateTime.Now;
    }

    [KernelFunction]
    [Description("Obtiene el id del doctor solicitando su numero de identificación.")]
    public async Task<Guid> GetDoctorId(Kernel kernel, long identification)
    {
        try
        {
            var doctor = await this.doctors.Find(identification);
            return doctor.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en el dia actual.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorToday(
        Kernel kernel,
        Guid chatId,
        Guid doctorId)
    {
        try
        {
            return await this.programations.GetAppointmentsListDay(doctorId, DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en una fecha especifica.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByDate(
        Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de la cita")] DateTime date)
    {
        try
        {
            return await this.programations.GetAppointmentsListDay(doctorId, date);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en un rango de fechas.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByRange(
        Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de inicio del rango")] DateTime startDate,
        [Description("Fecha de fin del rango")] DateTime endDate)
    {
        try
        {
            return await this.programations.GetAppointmentsListRange(doctorId, startDate, endDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}