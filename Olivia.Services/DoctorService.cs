// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Doctor service.
/// </summary>
public class DoctorService
{
    private readonly IDatabase database;
    private readonly ILogger<DoctorService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoctorService"/> class.
    /// </summary>
    /// <param name="database">Database.</param>
    /// <param name="logger">Logger.</param>
    public DoctorService(IDatabase database, ILogger<DoctorService> logger)
    {
        this.database = database;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="speciality">Speciality.</param>
    /// <param name="information">Information.</param>
    /// <param name="start">Start hour.</param>
    /// <param name="end">End hour.</param>
    /// <returns>Doctor id.</returns>
    public virtual async Task<Guid> Create(long identification, string name, string lastName, string email, long phone, string speciality, string information, TimeSpan start, TimeSpan end)
    {
        try
        {
            this.logger.LogInformation("Creating doctor");
            Doctor doctor = new ()
            {
                Identification = identification,
                Name = name,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Speciality = speciality,
                Information = information,
                Start = start,
                End = end,
                Available = true,
            };

            await this.database.Add(doctor);
            this.logger.LogInformation("Doctor created");

            return doctor.Id;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Updates a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="speciality">Speciality.</param>
    /// <param name="information">Information.</param>
    /// <returns>Task.</returns>
    public virtual async Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string speciality, string information)
    {
        try
        {
            this.logger.LogInformation("Updating doctor");
            Doctor? doctor = await this.database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                this.logger.LogError("Doctor not found");
                return;
            }

            doctor.Identification = identification;
            doctor.Name = name;
            doctor.LastName = lastName;
            doctor.Email = email;
            doctor.Phone = phone;
            doctor.Speciality = speciality;
            doctor.Information = information;

            await this.database.Update(doctor);
            this.logger.LogInformation("Doctor updated");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Updates the availability of a doctor.
    /// </summary>
    /// <param name="identification">Identification.</param>
    /// <returns>Task.</returns>
    public virtual async Task<bool> Exists(int identification)
    {
        try
        {
            this.logger.LogInformation("Checking if patient exists");
            return await this.database.Exist<Patient>(p => p.Identification == identification);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets all doctors.
    /// </summary>
    /// <returns>Doctors.</returns>
    public virtual async Task<IEnumerable<Doctor>> Get()
    {
        try
        {
            this.logger.LogInformation("Getting doctors");
            return await this.database.Get<Doctor>();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets available doctors.
    /// </summary>
    /// <returns>Doctors.</returns>
    public virtual async Task<IEnumerable<Doctor>?> GetAvailable()
    {
        try
        {
            this.logger.LogInformation("Getting available doctors");
            var doctors = await this.database.Get<Doctor>(x => x.Available == true);

            if (doctors == null)
            {
                this.logger.LogWarning("No doctors available");
                return null;
            }

            return doctors;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Finds a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <returns>Doctor.</returns>
    public virtual async Task<Doctor> Find(Guid id)
    {
        try
        {
            this.logger.LogInformation("Finding doctor");
            var doctor = await this.database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                this.logger.LogError("Doctor not found");
                throw new Exception("Doctor not found");
            }

            return doctor;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Finds a doctor.
    /// </summary>
    /// <param name="identification">Doctor identification.</param>
    /// <returns>Doctor.</returns>
    public virtual async Task<Doctor> Find(long identification)
    {
        try
        {
            this.logger.LogInformation("Finding doctor");
            var doctor = await this.database.Find<Doctor>(x => x.Identification == identification);

            if (doctor == null)
            {
                this.logger.LogError("Doctor not found");
                throw new Exception("Doctor not found");
            }

            return doctor;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">Doctor id.</param>
    /// <returns>Task.</returns>
    public virtual async Task Delete(Guid id)
    {
        try
        {
            this.logger.LogInformation("Deleting doctor");
            Doctor? doctor = await this.database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                this.logger.LogError("Doctor not found");
                return;
            }

            await this.database.Delete(doctor);
            this.logger.LogInformation("Doctor deleted");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
