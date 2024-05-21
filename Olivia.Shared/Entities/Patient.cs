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
    public class Patient
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the identification.
        /// </summary>
        public long Identification { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public long Phone { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Reason { get; set; } = null!;
    }
}