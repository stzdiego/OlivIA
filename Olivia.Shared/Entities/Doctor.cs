// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Doctor entity.
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// Gets or sets the doctor's ID.
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the doctor's identification.
        /// </summary>
        public long Identification { get; set; }

        /// <summary>
        /// Gets or sets the doctor's name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the doctor's last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the doctor's email.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the doctor's phone.
        /// </summary>
        public long Phone { get; set; }

        /// <summary>
        /// Gets or sets the doctor's speciality.
        /// </summary>
        public string Speciality { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the doctor's information.
        /// </summary>
        public string Information { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the doctor's start schedule.
        /// </summary>
        public TimeSpan Start { get; set; } = new TimeSpan(8, 0, 0);

        /// <summary>
        /// Gets or sets the doctor's end schedule.
        /// </summary>
        public TimeSpan End { get; set; } = new TimeSpan(18, 0, 0);

        /// <summary>
        /// Gets or sets a value indicating whether the doctor is available.
        /// </summary>
        public bool Available { get; set; } = true;
    }
}