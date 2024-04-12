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

namespace Olivia.AI.Plugins
{
    public class PatientsManagerPlugin
    {
        private readonly PatientService _patients;
        private readonly DoctorService _doctors;
        private readonly ChatService _chats;
        private readonly ProgramationService _programations;

        public PatientsManagerPlugin(PatientService patients, ChatService chats, ProgramationService programations, DoctorService doctors)
        {
            _patients = patients;
            _chats = chats;
            _programations = programations;
            _doctors = doctors;
        }

        [KernelFunction]
        [Description("Obtiene la fecha actual.")]
        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        [KernelFunction]
        [Description("Obtiene la información del paciente, requiere el patientId, retorna un objeto paciente.")]
        public async Task<Patient?> GetPatient(Guid patientId)
        {
            try
            {
                return await _patients.Find(patientId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [KernelFunction]
        [Description("Obtiene información de los doctores disponibles, retorna una lista de doctores.")]
        public async Task<IEnumerable<Doctor>?> GetDoctors()
        {
            try
            {
                return await _doctors.GetAvailable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        [KernelFunction]
        [Description("Registrar un nuevo paciente, retorna Guid del paciente registrado. Todos los parametros son requeridos y no pueden ser nulos, vacios o 0.")]
        public async Task<Guid> RegisterPatient(
            Kernel kernel,
            Guid chatId,
            [Description("Numero de identificación, requerido, mayor a 0")] long patientId,
            [Description("Nombre")] string name,
            [Description("Apellido")] string lastName,
            [Description("Correo electronico, requerido")] string email,
            [Description("Numero de telefono celular, requerido, mayor a 0")] long phone,
            [Description("Motivo de consulta, requerido, mayor a 10 caracteres")] string reason)
        {
            try
            {
                if (await _patients.Exists(patientId))
                {
                    throw new Exception("El paciente ya esta registrado");
                }

                if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(reason))
                {
                    throw new Exception("Todos los campos son requeridos");
                }

                var guid = await _patients.Create(name, lastName, email, phone, reason);
                await _chats.AsociatePatient(chatId, guid);
                return guid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        [KernelFunction]
        [Description("Obtener disponibilidad de horas de un doctor, retorna una lista de horas disponibles.")]
        public async Task<IEnumerable<TimeSpan>> GetAvailableHours(
            Kernel kernel,
            Guid doctorId,
            [Description("Fecha de la cita")] DateTime date)
        {
            try
            {
                if(date.Date < DateTime.Now.Date)
                {
                    throw new Exception("La fecha de la cita no puede ser menor a la fecha actual");
                }

                return await _programations.GetAvailableHours(doctorId, date);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [KernelFunction]
        [Description("Registra cita para un paciente, retorna Guid de la cita. Todos los parametros son requeridos y no pueden ser nulos, vacios o 0.")]
        public async Task<Guid> RegisterAppointment(
            Kernel kernel,
            Guid patientId,
            Guid doctorId,
            [Description("Fecha y hora de la cita")] DateTime date,
            [Description("Motivo de la cita")] string reason)
        {
            try
            {
                return await _programations.CreateAppointment(doctorId, patientId, date, reason);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}