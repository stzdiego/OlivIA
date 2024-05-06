// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Service for programation.
    /// </summary>
    public class ProgramationService
    {
        private readonly IDatabase database;
        private readonly ILogger<ProgramationService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramationService"/> class.
        /// </summary>
        /// <param name="database">Database.</param>
        /// <param name="logger">Logger.</param>
        public ProgramationService(IDatabase database, ILogger<ProgramationService> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        /// <summary>
        /// Get the doctor id by name.
        /// </summary>
        /// <param name="name">Doctor name.</param>
        /// <returns>Id of the doctor.</returns>
        public virtual async Task<Guid> GetDoctorId(string name)
        {
            this.logger.LogInformation("Getting doctor id");
            var doctor = await this.database.Find<Doctor>(x => x.Name == name);

            if (doctor == null)
            {
                this.logger.LogInformation("Doctor not found");
                throw new Exception("Doctor not found");
            }

            this.logger.LogInformation("Doctor found");
            return doctor.Id;
        }

        /// <summary>
        /// Get the available hours for a doctor.
        /// </summary>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="date">Date.</param>
        /// <returns>Available hours.</returns>
        public virtual async Task<IEnumerable<TimeSpan>> GetAvailableHours(Guid doctorId, DateTime date)
        {
            this.logger.LogInformation("Getting available hours");
            var doctor = await this.database.Find<Doctor>(x => x.Id == doctorId);
            if (doctor == null)
            {
                this.logger.LogInformation("Doctor not found");
                throw new Exception("Doctor not found");
            }

            var appointments = await this.database
            .Get<Appointment>(x => x.DoctorId == doctorId && x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);

            var hours = new List<TimeSpan>();
            var current = doctor.Start;
            while (current < doctor.End)
            {
                hours.Add(current);
                current = current.Add(TimeSpan.FromHours(1));
            }

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    hours.Remove(appointment.Date.TimeOfDay);
                }
            }

            this.logger.LogInformation("Available hours found");
            return hours;
        }

        /// <summary>
        /// Create an appointment.
        /// </summary>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="date">Date.</param>
        /// <param name="reason">Reason.</param>
        /// <returns>Appointment id.</returns>
        public virtual async Task<Guid> CreateAppointment(Guid doctorId, Guid patientId, DateTime date, string reason)
        {
            this.logger.LogInformation("Creating appointment");
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Date = date,
                Reason = reason,
            };

            await this.database.Add(appointment);
            this.logger.LogInformation("Appointment created");
            return appointment.Id;
        }

        /// <summary>
        /// Get the appointments list by doctor.
        /// </summary>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="date">Date.</param>
        /// <returns>Appointments list.</returns>
        public virtual async Task<IEnumerable<Appointment>?> GetAppointmentsListDay(Guid doctorId, DateTime date)
        {
            this.logger.LogInformation("Getting appointment");
            var appointments = await this.database
            .Get<Appointment>(x => x.DoctorId == doctorId && x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);

            if (appointments == null)
            {
                this.logger.LogInformation("Appointment not found");
                return null;
            }

            this.logger.LogInformation("Appointment found");
            return appointments;
        }

        /// <summary>
        /// Get the appointments list by doctor.
        /// </summary>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns>Appointments list.</returns>
        public virtual async Task<IEnumerable<Appointment>?> GetAppointmentsListRange(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            this.logger.LogInformation("Getting appointment");
            var appointments = await this.database.Get<Appointment>(x => x.DoctorId == doctorId && x.Date >= startDate && x.Date <= endDate);

            if (appointments == null)
            {
                this.logger.LogInformation("Appointment not found");
                return null;
            }

            this.logger.LogInformation("Appointment found");
            return appointments;
        }
    }
}