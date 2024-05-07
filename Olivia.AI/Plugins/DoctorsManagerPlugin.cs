// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.SemanticKernel;
    using Olivia.Services;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Plugin for managing doctors.
    /// </summary>
    public class DoctorsManagerPlugin : IPlugin
    {
        private readonly IDoctorService doctors;
        private readonly IChatService chats;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsManagerPlugin"/> class.
        /// </summary>
        /// <param name="doctors">Doctor service.</param>
        /// <param name="chats">Chat service.</param>
        public DoctorsManagerPlugin(IDoctorService doctors, IChatService chats)
        {
            this.doctors = doctors;
            this.chats = chats;
        }

        /// <summary>
        /// Gets the information of the doctors.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <returns>Doctors information.</returns>
        [KernelFunction]
        [Description("Obtiene la información de los doctores")]
        public async Task<IEnumerable<Doctor>> GetInformation(Kernel kernel)
        {
            return await this.doctors.Get();
        }
    }
}