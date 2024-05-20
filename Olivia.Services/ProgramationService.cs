// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Google.Apis.Calendar.v3.Data;
    using Microsoft.Extensions.Logging;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Service for programation.
    /// </summary>
    public class ProgramationService : IProgramationService
    {
        private readonly IDatabase database;
        private readonly ILogger<ProgramationService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramationService"/> class.
        /// </summary>
        /// <param name="database">Database.</param>
        /// <param name="logger">Logger.</param>
        public ProgramationService(IDatabase database, ILogger<ProgramationService> logger) // , ICalendarService calendarService)
        {
            this.database = database;
            this.logger = logger;
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
        /// Update an appointment.
        /// </summary>
        /// <param name="patientId">Patient id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <returns>Appointment.</returns>
        public virtual async Task<Appointment> Find(Guid patientId, Guid doctorId)
        {
            this.logger.LogInformation("Finding appointment");

            var appointments = await this.database.Get<Appointment>(x => x.PatientId == patientId && x.DoctorId == doctorId);
            var appointment = appointments?.First();

            this.logger.LogInformation("Appointment found");
            return appointment!;
        }
    }
}