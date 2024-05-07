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
    /// Patients manager plugin.
    /// </summary>
    public class PatientsManagerPlugin : IPlugin
    {
        private readonly PatientService patients;
        private readonly IDoctorService doctors;
        private readonly IChatService chats;
        private readonly ProgramationService programations;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientsManagerPlugin"/> class.
        /// </summary>
        /// <param name="patients">Patient service.</param>
        /// <param name="chats">Chat service.</param>
        /// <param name="programations">Programation service.</param>
        /// <param name="doctors">Doctor service.</param>
        public PatientsManagerPlugin(PatientService patients, IChatService chats, ProgramationService programations, IDoctorService doctors)
        {
            this.patients = patients;
            this.chats = chats;
            this.programations = programations;
            this.doctors = doctors;
        }

        /// <summary>
        /// Gets the date and time of the system.
        /// </summary>
        /// <returns>Current date.</returns>
        [KernelFunction]
        [Description("Obtiene la fecha actual.")]
        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Get the patient information.
        /// </summary>
        /// <param name="patientId">Patient id.</param>
        /// <returns>Patient information.</returns>
        [KernelFunction]
        [Description("Obtiene la información del paciente, requiere el patientId, retorna un objeto paciente.")]
        public async Task<Patient?> GetPatient(Guid patientId)
        {
            return await this.patients.Find(patientId);
        }

        /// <summary>
        /// Get the patient information.
        /// </summary>
        /// <returns>Patient information.</returns>
        [KernelFunction]
        [Description("Obtiene información de los doctores disponibles, retorna una lista de doctores.")]
        public async Task<IEnumerable<Doctor>?> GetDoctors()
        {
            return await this.doctors.GetAvailable();
        }

        /// <summary>
        /// Register a new patient.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="chatId">Chat id.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="name">Name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="email">Email.</param>
        /// <param name="phone">Phone.</param>
        /// <param name="reason">Reason.</param>
        /// <returns>Patient Guid.</returns>
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
            if (await this.patients.Exists(patientId))
            {
                throw new Exception("El paciente ya esta registrado");
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(reason))
            {
                throw new Exception("Todos los campos son requeridos");
            }

            var guid = await this.patients.Create(patientId, name, lastName, email, phone, reason);
            await this.chats.AsociatePatient(chatId, guid);
            return guid;
        }

        /// <summary>
        /// Get the available hours of a doctor.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="date">Date.</param>
        /// <returns>Available hours.</returns>
        [KernelFunction]
        [Description("Obtener disponibilidad de horas de un doctor, retorna una lista de horas disponibles.")]
        public async Task<IEnumerable<TimeSpan>> GetAvailableHours(
            Kernel kernel,
            Guid doctorId,
            [Description("Fecha de la cita")] DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                throw new Exception("La fecha de la cita no puede ser menor a la fecha actual");
            }

            return await this.programations.GetAvailableHours(doctorId, date);
        }

        /// <summary>
        /// Register an appointment.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="date">Date.</param>
        /// <param name="reason">Reason.</param>
        /// <returns>Appointment Guid.</returns>
        [KernelFunction]
        [Description("Registra cita para un paciente, retorna Guid de la cita. Todos los parametros son requeridos y no pueden ser nulos, vacios o 0.")]
        public async Task<Guid> RegisterAppointment(
            Kernel kernel,
            Guid patientId,
            Guid doctorId,
            [Description("Fecha y hora de la cita")] DateTime date,
            [Description("Motivo de la cita")] string reason)
        {
            return await this.programations.CreateAppointment(doctorId, patientId, date, reason);
        }
    }
}