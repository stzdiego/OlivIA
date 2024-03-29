using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Olivia.Shared.Enums;

namespace Olivia.Shared.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public long Identification { get; set; }

        public required string Name { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required long Phone { get; set; }

        public required string Reason { get; set; }

        public PatientStatusEnum Status { get; set; } = PatientStatusEnum.Created;
    }
}