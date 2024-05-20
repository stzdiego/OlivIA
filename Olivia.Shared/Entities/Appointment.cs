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

    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateTime Date { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Observations { get; set; } = string.Empty;

        public AppointmentStateEnum State { get; set; } = AppointmentStateEnum.PendingApproval;
    }
}