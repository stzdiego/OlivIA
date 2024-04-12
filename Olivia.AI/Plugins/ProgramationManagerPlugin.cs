using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Olivia.Services;
using Olivia.Shared.Entities;

namespace Olivia.AI.Plugins;
public class ProgramationManagerPlugin
{
    private readonly ProgramationService _programations;
    private readonly DoctorService _doctors;
    private readonly ChatService _chats;

    public ProgramationManagerPlugin(ProgramationService programations, ChatService chats, DoctorService doctors)
    {
        _programations = programations;
        _chats = chats;
        _doctors = doctors;
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
            var doctor = await _doctors.Find(identification);
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
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorToday(Kernel kernel,
        Guid chatId,
        Guid doctorId)
    {
        try
        {
            return await _programations.GetAppointmentsListDay(doctorId, DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en una fecha especifica.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByDate(Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de la cita")] DateTime date)
    {
        try
        {
            return await _programations.GetAppointmentsListDay(doctorId, date);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [KernelFunction]
    [Description("Obtiene la lista de citas de un doctor en un rango de fechas.")]
    public async Task<IEnumerable<Appointment>?> GetAppointmentsByDoctorByRange(Kernel kernel,
        Guid chatId,
        Guid doctorId,
        [Description("Fecha de inicio del rango")] DateTime startDate,
        [Description("Fecha de fin del rango")] DateTime endDate)
    {
        try
        {
            return await _programations.GetAppointmentsListRange(doctorId, startDate, endDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

}