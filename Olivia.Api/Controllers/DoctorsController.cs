using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Olivia.Services;
using Olivia.Shared.Dtos;

namespace Olivia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly DoctorService _doctors;

        public DoctorsController(ILogger<DoctorsController> logger, DoctorService doctors)
        {
            _logger = logger;
            _doctors = doctors;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Getting doctors");
                var doctors = await _doctors.Get();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting doctor");
                var doctor = await _doctors.Find(id);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DoctorDto doctor)
        {
            try
            {
                _logger.LogInformation("Creating doctor");
                var id = await _doctors.Create(doctor.Identification, doctor.Name, doctor.LastName, doctor.Email, 
                doctor.Phone, doctor.Speciality, doctor.Information, doctor.Start, doctor.End);
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] DoctorDto doctor)
        {
            try
            {
                _logger.LogInformation("Updating doctor");
                await _doctors.Update(id, doctor.Identification, doctor.Name, doctor.LastName, doctor.Email, doctor.Phone, doctor.Speciality, doctor.Information);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting doctor");
                await _doctors.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor");
                return StatusCode(500);
            }
        }
    }
}