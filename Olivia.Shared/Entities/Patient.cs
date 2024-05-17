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

    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public long Identification { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public long Phone { get; set; }

        public string Reason { get; set; }

        public PatientStatusEnum Status { get; set; } = PatientStatusEnum.Created;
    }
}