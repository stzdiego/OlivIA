using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Olivia.Shared.Entities
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required long Identification { get; set; }

        public required string Name { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required long Phone { get; set; }

        public required string Speciality { get; set; }

        public required string Information { get; set; }
    }
}