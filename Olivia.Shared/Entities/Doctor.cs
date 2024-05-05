// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

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

        public required TimeSpan Start { get; set; } = new TimeSpan(8, 0, 0);

        public required TimeSpan End { get; set; } = new TimeSpan(18, 0, 0);

        public required bool Available { get; set; } = true;
    }
}