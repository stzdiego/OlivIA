using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olivia.Shared.Dtos
{
    public class DoctorDto
    {
        public required int Identification { get; set; }

        public required string Name { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required long Phone { get; set; }

        public required string Speciality { get; set; }

        public required string Information { get; set; }

        public required TimeSpan Start { get; set; }

        public required TimeSpan End { get; set; }
    }
}