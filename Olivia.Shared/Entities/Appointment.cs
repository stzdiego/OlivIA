// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Olivia.Shared.Enums;

    /// <summary>
    /// Represents an appointment entity.
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public Guid PatientId { get; set; }

        /// <summary>
        /// Gets or sets the doctor identifier.
        /// </summary>
        public Guid DoctorId { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the observations.
        /// </summary>
        public string Observations { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public AppointmentStateEnum State { get; set; } = AppointmentStateEnum.PendingApproval;
    }
}