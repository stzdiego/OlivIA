// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Olivia.Services;
    using Olivia.Shared.Dtos;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Doctors controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> logger;
        private readonly IDoctorService doctors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsController"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="doctors">Doctors service.</param>
        public DoctorsController(ILogger<DoctorsController> logger, IDoctorService doctors)
        {
            this.logger = logger;
            this.doctors = doctors;
        }

        /// <summary>
        /// Gets doctors.
        /// </summary>
        /// <returns>Doctors.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                this.logger.LogInformation("Getting doctors");
                var doctors = await this.doctors.Get();
                return this.Ok(doctors);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting doctors");
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Gets doctor.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Doctor.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                this.logger.LogInformation("Getting doctor");
                var doctor = await this.doctors.Find(id);
                return this.Ok(doctor);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting doctor");
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Creates doctor.
        /// </summary>
        /// <param name="doctor">Doctor.</param>
        /// <returns>Doctor id.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DoctorDto doctor)
        {
            try
            {
                this.logger.LogInformation("Creating doctor");
                var id = await this.doctors.Create(doctor.Identification, doctor.Name, doctor.LastName, doctor.Email, doctor.Phone, doctor.Speciality, doctor.Information, doctor.Start, doctor.End);
                return this.Ok(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating doctor");
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Updates doctor.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="doctor">Doctor.</param>
        /// <returns>Task.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] DoctorDto doctor)
        {
            try
            {
                this.logger.LogInformation("Updating doctor");
                await this.doctors.Update(id, doctor.Identification, doctor.Name, doctor.LastName, doctor.Email, doctor.Phone, doctor.Speciality, doctor.Information);
                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating doctor");
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Deletes doctor.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Task.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                this.logger.LogInformation("Deleting doctor");
                await this.doctors.Delete(id);
                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting doctor");
                return this.StatusCode(500);
            }
        }
    }
}