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
        private readonly ChatService _chats;

        public PatientsManagerPlugin(PatientService patients, ChatService chats)
        {
            _patients = patients;
            _chats = chats;
        }

        [KernelFunction]
        [Description("Verificar si un paciente ya esta registrado, retorna un booleano.")]
        public async Task<bool> CheckPatient(Kernel kernel,
        [Description("Numero de identificación")] int Identification)
        {
            try
            {
                return await _patients.Exists(Identification);
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
        [Description("Actualizar la información de un paciente, retorna el objeto paciente. Todos los parametros son requeridos y no pueden ser nulos, vacios o 0.")]
        public async Task UpdatePatient(
            Kernel kernel,
            Guid patientId,
            [Description("Numero de identificación")] long identification,
            [Description("Numero de telefono celular")] long phone,
            [Description("Nombre")] string name,
            [Description("Apellido")] string lastName,
            [Description("Correo electronico real")] string email,
            [Description("Motivo de consulta")] string reason)
        {
            try
            {
                await _patients.Update(patientId, identification, name, lastName, email, phone, reason);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}