using Microsoft.Extensions.Logging;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Services;

public class DoctorService
{
    private readonly IDatabase _database;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDatabase database, ILogger<DoctorService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<Guid> Create(long identification, string name, string lastName, string email, long phone, 
    string speciality, string information, TimeSpan start, TimeSpan end)
    {
        try
        {
            _logger.LogInformation("Creating doctor");
            Doctor doctor = new()
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
                Available = true
            };

            await _database.Add(doctor);
            _logger.LogInformation("Doctor created");

            return doctor.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string speciality, string information)
    {
        try
        {
            _logger.LogInformation("Updating doctor");
            Doctor? doctor = await _database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                _logger.LogError("Doctor not found");
                return;
            }

            doctor.Identification = identification;
            doctor.Name = name;
            doctor.LastName = lastName;
            doctor.Email = email;
            doctor.Phone = phone;
            doctor.Speciality = speciality;
            doctor.Information = information;

            await _database.Update(doctor);
            _logger.LogInformation("Doctor updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> Exists(int Identification)
    {
        try
        {
            _logger.LogInformation("Checking if patient exists");
            return await _database.Exist<Patient>(p => p.Identification == Identification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Doctor>> Get()
    {
        try
        {
            _logger.LogInformation("Getting doctors");
            return await _database.Get<Doctor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Doctor>?> GetAvailable()
    {
        try
        {
            _logger.LogInformation("Getting available doctors");
            var doctors = await _database.Get<Doctor>(x => x.Available == true);
            
            if (doctors == null)
            {
                _logger.LogWarning("No doctors available");
                return null;
            }

            return doctors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Doctor> Find(Guid id)
    {
        try
        {
            _logger.LogInformation("Finding doctor");
            var doctor = await _database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                _logger.LogError("Doctor not found");
                throw new Exception("Doctor not found");
            }

            return doctor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Doctor> Find(long identification)
    {
        try
        {
            _logger.LogInformation("Finding doctor");
            var doctor = await _database.Find<Doctor>(x => x.Identification == identification);

            if (doctor == null)
            {
                _logger.LogError("Doctor not found");
                throw new Exception("Doctor not found");
            }

            return doctor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting doctor");
            Doctor? doctor = await _database.Find<Doctor>(x => x.Id == id);

            if (doctor == null)
            {
                _logger.LogError("Doctor not found");
                return;
            }

            await _database.Delete(doctor);
            _logger.LogInformation("Doctor deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
