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

        public long Identification { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public long Phone { get; set; }

        public string Speciality { get; set; }

        public string Information { get; set; }

        public TimeSpan Start { get; set; } = new TimeSpan(8, 0, 0);

        public TimeSpan End { get; set; } = new TimeSpan(18, 0, 0);

        public bool Available { get; set; } = true;
    }
}