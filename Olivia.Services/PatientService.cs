using Microsoft.Extensions.Logging;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Services;

public class PatientService
{
    private readonly IDatabase _database;
    private readonly ILogger<ChatService> _logger;

    public PatientService(IDatabase database, ILogger<ChatService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<Guid> Create(string name, string lastName, string email, long phone, string reason)
    {
        try
        {
            _logger.LogInformation("Creating chat service");
            Patient patient = new()
            {
                Name = name,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Reason = reason
            };

            await _database.Add(patient);
            _logger.LogInformation("Patient created");
            return patient.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Update(Guid id, long identification, string name, string lastName, string email, long phone, string reason)
    {
        try
        {
            _logger.LogInformation("Updating patient");
            Patient patient = await _database.Find<Patient>(x => x.Id == id)
                ?? throw new Exception("Patient not found");

            patient.Identification = identification;
            patient.Name = name;
            patient.LastName = lastName;
            patient.Email = email;
            patient.Phone = phone;
            patient.Reason = reason;

            await _database.Update(patient);
            _logger.LogInformation("Patient updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> Exists(long Identification)
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

    public async Task<Patient?> Find(Guid id)
    {
        try
        {
            _logger.LogInformation("Finding patient");
            return await _database.Find<Patient>(x => x.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
