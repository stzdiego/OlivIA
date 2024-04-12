using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Olivia.Shared.Entities
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Observations { get; set; } = string.Empty;
    }
}