// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

/// <summary>
/// Doctor service.
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDatabase database;
    private readonly ILogger<DoctorService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoctorService"/> class.
    /// </summary>
    /// <param name="database">Database.</param>
    /// <param name="logger">Logger.</param>
    public DoctorService(IDatabase database, ILogger<DoctorService> logger)
    {
        this.database = database;
        this.logger = logger;
    }

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
    public virtual async Task<Guid> Create(long identification, string name, string lastName, string email, long phone, string speciality, string information, TimeSpan start, TimeSpan end)
    {
        this.logger.LogInformation("Creating doctor");
        Doctor doctor = new ()
        {
            Identification = identification,
            Name = name,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Speciality = speciality,
            Information = information,
            Start = start,
            End = end,
            Available = true,
        };

        await this.database.Add(doctor);
        this.logger.LogInformation("Doctor created");

        return doctor.Id;
    }

    /// <summary>
    /// Updates a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="speciality">Speciality.</param>
    /// <param name="information">Information.</param>
    /// <returns>Task.</returns>
    public virtual async Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string speciality, string information)
    {
        this.logger.LogInformation("Updating doctor");
        Doctor? doctor = await this.database.Find<Doctor>(x => x.Id == id);

        if (doctor == null)
        {
            this.logger.LogError("Doctor not found");
            return;
        }

        doctor.Identification = identification;
        doctor.Name = name;
        doctor.LastName = lastName;
        doctor.Email = email;
        doctor.Phone = phone;
        doctor.Speciality = speciality;
        doctor.Information = information;

        await this.database.Update(doctor);
        this.logger.LogInformation("Doctor updated");
    }

    /// <summary>
    /// Updates the availability of a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    public virtual async Task<bool> Exists(int identification)
    {
        this.logger.LogInformation("Checking if patient exists");
        return await this.database.Exist<Doctor>(p => p.Identification == identification);
    }

    /// <summary>
    /// Gets all doctors.
    /// </summary>
    /// <returns>Doctors.</returns>
    public virtual async Task<IEnumerable<Doctor>> Get()
    {
        this.logger.LogInformation("Getting doctors");
        return await this.database.Get<Doctor>();
    }

    /// <summary>
    /// Gets available doctors.
    /// </summary>
    /// <returns>Doctors.</returns>
    public virtual async Task<IEnumerable<Doctor>?> GetAvailable()
    {
        this.logger.LogInformation("Getting available doctors");
        var doctors = await this.database.Get<Doctor>(x => x.Available == true);

        if (doctors == null)
        {
            this.logger.LogWarning("No doctors available");
            return null;
        }

        return doctors;
    }

    /// <summary>
    /// Finds a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <returns>Doctor.</returns>
    public virtual async Task<Doctor?> Find(Guid id)
    {
        this.logger.LogInformation("Finding doctor");
        var doctor = await this.database.Find<Doctor>(x => x.Id == id);

        if (doctor == null)
        {
            this.logger.LogError("Doctor not found");
            throw new Exception("Doctor not found");
        }

        return doctor;
    }

    /// <summary>
    /// Finds a doctor.
    /// </summary>
    /// <param name="identification">Doctor identification.</param>
    /// <returns>Doctor.</returns>
    public virtual async Task<Doctor?> Find(long identification)
    {
        this.logger.LogInformation("Finding doctor");
        var doctor = await this.database.Find<Doctor>(x => x.Identification == identification);

        if (doctor == null)
        {
            this.logger.LogError("Doctor not found");
            throw new Exception("Doctor not found");
        }

        return doctor;
    }

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <returns>Task.</returns>
    public virtual async Task Delete(Guid id)
    {
        this.logger.LogInformation("Deleting doctor");
        Doctor? doctor = await this.database.Find<Doctor>(x => x.Id == id);

        if (doctor == null)
        {
            this.logger.LogError("Doctor not found");
            return;
        }

        await this.database.Delete(doctor);
        this.logger.LogInformation("Doctor deleted");
    }

    /// <summary>
    /// Gets the most recent available appointment.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <returns>Task.</returns>
    public virtual async Task<DateTime> GetMostRecentAvailableAppointmentAsync(Guid doctorId)
    {
        var appointments = (await this.database.Get<Appointment>(x => x.DoctorId == doctorId))
            .Select(a => a.Date)
            .ToHashSet();
        var doctor = await this.Find(doctorId);

        TimeZoneInfo doctorTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); // Zona horaria de Bogotá, Colombia
        var now = DateTime.UtcNow;
        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(now, doctorTimeZone);

        var start = new DateTime(localTime.Year, localTime.Month, localTime.Day, doctor!.Start.Hours, doctor.Start.Minutes, doctor.Start.Seconds);
        var end = new DateTime(localTime.Year, localTime.Month, localTime.Day, doctor.End.Hours, doctor.End.Minutes, doctor.End.Seconds);

        // Si la hora actual es después de la hora de inicio del médico, comienza a buscar desde la próxima hora completa.
        if (localTime > start)
        {
            var minutesToNextHour = 60 - localTime.Minute;
            start = localTime.AddMinutes(minutesToNextHour);
        }

        for (var nextAppointment = start; nextAppointment <= end; nextAppointment = nextAppointment.AddHours(1))
        {
            if (nextAppointment.DayOfWeek == DayOfWeek.Sunday || nextAppointment >= end)
            {
                start = start.AddDays(1);
                end = end.AddDays(1);
                nextAppointment = start;
            }

            if (!appointments.Contains(nextAppointment))
            {
                return nextAppointment;
            }
        }

        throw new Exception("No available appointments");
    }

    /// <summary>
    /// Gets the available appointment by date.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="date">Date.</param>
    /// <returns>Task.</returns>
    public virtual async Task<IList<DateTime>> GetAvailableAppointmentsByDate(Guid id, DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            return new List<DateTime>();
        }

        var doctor = await this.Find(id);
        var appointments = (await this.database.Get<Appointment>(x => x.DoctorId == id && x.Date.Date == date.Date))
            .Select(a => a.Date)
            .ToHashSet();
        var availableAppointments = new List<DateTime>();

        TimeZoneInfo doctorTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); // Zona horaria de Bogotá, Colombia
        DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(date, doctorTimeZone);

        var start = new DateTime(localDate.Year, localDate.Month, localDate.Day, doctor!.Start.Hours, doctor.Start.Minutes, doctor.Start.Seconds);
        var end = new DateTime(localDate.Year, localDate.Month, localDate.Day, doctor.End.Hours, doctor.End.Minutes, doctor.End.Seconds);

        for (var i = start; i < end; i = i.AddHours(1))
        {
            if (!appointments.Contains(i))
            {
                availableAppointments.Add(i);
            }
        }

        return availableAppointments;
    }

    /// <summary>
    /// Gets the available appointment by date.
    /// </summary>
    /// <param name="doctorId">Doctor id.</param>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <param name="status">Status.</param>
    /// <returns>Task.</returns>
    public virtual async Task<IEnumerable<PatientAppointmentDto>> GetPatientsPendingByDoctorByDate(Guid doctorId, DateTime start, DateTime end, AppointmentStateEnum status)
    {
        if (start > end)
        {
            throw new Exception("Start date must be less than end date.");
        }

        var appointments = await this.database.Get<Appointment>(x => x.DoctorId == doctorId && x.Date >= start && x.Date <= end);
        var patients = new List<PatientAppointmentDto>();

        foreach (var appointment in appointments)
        {
            var patient = await this.database.Find<Patient>(x => x.Id == appointment.PatientId);

            if (appointment!.State == status)
            {
                patients.Add(new PatientAppointmentDto
                {
                    PatientId = patient!.Id.ToString(),
                    FullName = patient.Name + " " + patient.LastName,
                    Datetime = appointment.Date.ToString(),
                    Status = appointment!.State.ToString(),
                    Reason = appointment.Reason,
                });
            }
        }

        return patients;
    }

    /// <summary>
    /// Approves a patient.
    /// </summary>
    /// <param name="appointmentId">Appintment id.</param>
    /// <returns>Task.</returns>
    /// <exception cref="Exception">Patient not found.</exception>
    public virtual async Task<bool> ApprovePatient(Guid appointmentId)
    {
        var appointment = await this.database.Find<Appointment>(x => x.Id == appointmentId);

        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }

        if (appointment!.State != AppointmentStateEnum.PendingApproval)
        {
            throw new Exception("Appointment is not approval pending.");
        }

        appointment.State = AppointmentStateEnum.PendingPayment;
        await this.database.Update(appointment);

        return true;
    }

    /// <summary>
    /// Refuses a patient.
    /// </summary>
    /// <param name="appointmentId">Appointment id.</param>
    /// <returns>Task.</returns>
    public async Task<bool> RefusedPatient(Guid appointmentId)
    {
        var appointment = await this.database.Find<Appointment>(x => x.Id == appointmentId);

        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }

        appointment!.State = AppointmentStateEnum.Rejected;
        await this.database.Update(appointment);

        return true;
    }

    /// <summary>
    /// Refuses a patient.
    /// </summary>
    /// <param name="appointmentId">Patient id.</param>
    /// <returns>Task.</returns>
    /// <exception cref="Exception">Patient not found.</exception>
    public virtual async Task<bool> PayPatient(Guid appointmentId)
    {
        var appointment = await this.database.Find<Appointment>(x => x.Id == appointmentId);

        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }

        if (appointment!.State != AppointmentStateEnum.PendingPayment)
        {
            throw new Exception("Appointment is not pending.");
        }

        appointment!.State = AppointmentStateEnum.Confirmed;
        await this.database.Update(appointment);

        return true;
    }
}
