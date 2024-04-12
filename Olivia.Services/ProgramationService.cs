using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Services
{
    public class ProgramationService
    {
        private readonly IDatabase _database;
        private readonly ILogger<ProgramationService> _logger;

        public ProgramationService(IDatabase database, ILogger<ProgramationService> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<Guid> GetDoctorId(string name)
        {
            try
            {
                _logger.LogInformation("Getting doctor id");
                var doctor = await _database.Find<Doctor>(x => x.Name == name);

                if (doctor == null)
                {
                    _logger.LogInformation("Doctor not found");
                    throw new Exception("Doctor not found");
                }

                _logger.LogInformation("Doctor found");
                return doctor.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableHours(Guid doctorId, DateTime date)
        {
            try
            {
                _logger.LogInformation("Getting available hours");
                var doctor = await _database.Find<Doctor>(x => x.Id == doctorId);
                if (doctor == null)
                {
                    _logger.LogInformation("Doctor not found");
                    throw new Exception("Doctor not found");
                }

                var appointments = await _database
                .Get<Appointment>(x => x.DoctorId == doctorId && x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);

                var hours = new List<TimeSpan>();
                var current = doctor.Start;
                while (current < doctor.End)
                {
                    hours.Add(current);
                    current = current.Add(TimeSpan.FromHours(1));
                }

                if (appointments != null)
                {
                    foreach (var appointment in appointments)
                    {
                        hours.Remove(appointment.Date.TimeOfDay);
                    }
                }

                _logger.LogInformation("Available hours found");
                return hours;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Guid> CreateAppointment(Guid doctorId, Guid patientId, DateTime date, string reason)
        {
            try
            {
                _logger.LogInformation("Creating appointment");
                var appointment = new Appointment
                {
                    DoctorId = doctorId,
                    PatientId = patientId,
                    Date = date,
                    Reason = reason
                };

                await _database.Add(appointment);
                _logger.LogInformation("Appointment created");
                return appointment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>?> GetAppointmentsListDay(Guid doctorId, DateTime date)
        {
            try
            {
                _logger.LogInformation("Getting appointment");
                var appointments = await _database.Get<Appointment>(x => x.DoctorId == doctorId && x.Date == date);

                if (appointments == null)
                {
                    _logger.LogInformation("Appointment not found");
                    return null;
                }

                _logger.LogInformation("Appointment found");
                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>?> GetAppointmentsListRange(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting appointment");
                var appointments = await _database.Get<Appointment>(x => x.DoctorId == doctorId && x.Date >= startDate && x.Date <= endDate);

                if (appointments == null)
                {
                    _logger.LogInformation("Appointment not found");
                    return null;
                }

                _logger.LogInformation("Appointment found");
                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}