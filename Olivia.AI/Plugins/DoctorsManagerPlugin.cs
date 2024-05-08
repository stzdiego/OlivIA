// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.SemanticKernel;
    using Olivia.Services;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Plugin for managing doctors.
    /// </summary>
    public class DoctorsManagerPlugin : IPlugin
    {
        private readonly IDoctorService doctors;
        private readonly IChatService chats;
        private readonly ICalendarService calendarService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsManagerPlugin"/> class.
        /// </summary>
        /// <param name="doctors">Doctor service.</param>
        /// <param name="chats">Chat service.</param>
        /// <param name="calendarService">Calendar service.</param>
        public DoctorsManagerPlugin(IDoctorService doctors, IChatService chats, ICalendarService calendarService)
        {
            this.doctors = doctors;
            this.chats = chats;
            this.calendarService = calendarService;
        }

        /// <summary>
        /// Gets the information of the doctors.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <returns>Doctors information.</returns>
        [KernelFunction]
        [Description("Obtiene la información de los doctores")]
        public async Task<IEnumerable<Doctor>> GetInformation(Kernel kernel)
        {
            return await this.doctors.Get();
        }

        /// <summary>
        /// Creates a doctor.
        /// </summary>
        /// <param name="patientName">Patient name.</param>
        /// <param name="patientLastName">Patient last name.</param>
        /// <param name="reason">Reason for the appointment.</param>
        /// <param name="date">Date of the appointment.</param>
        /// <returns>Task.</returns>
        public async Task CreateEventCalendar(
            [Description("Patient name")] string patientName,
            [Description("Patient last name")] string patientLastName,
            [Description("Reason for the appointment")] string reason,
            [Description("Date of the appointment")] DateTime date)
        {
            await this.calendarService.CreateEvent($"{patientName} {patientLastName}", reason, date, date.AddHours(1));
        }
    }
}