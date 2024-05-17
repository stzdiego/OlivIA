// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins;

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a plugin that manages the patient.
/// </summary>
public class PatientManagerPlugin : IPlugin
{
    /// <summary>
    /// Gets or sets the patient.
    /// </summary>
    public Patient? Patient { get; set; }

    /// <summary>
    /// Gets or sets the doctors.
    /// </summary>
    public IEnumerable<Doctor>? Doctors { get; set; }

    private readonly IChatService chatService;
    private readonly IPatientService patientService;
    private readonly IDoctorService doctorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientManagerPlugin"/> class.
    /// </summary>
    /// <param name="chatService">Chat service.</param>
    /// <param name="patientService">Patient service.</param>
    /// <param name="doctorService">Doctor service.</param>
    public PatientManagerPlugin(IChatService chatService, IPatientService patientService, IDoctorService doctorService)
    {
        this.chatService = chatService;
        this.patientService = patientService;
        this.doctorService = doctorService;
    }

    /// <summary>
    /// Registers a patient in the system.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Chat identifier.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastname">Lastname.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("RegisterPatient")]
    [Description("Register a patient in the system.")]
    public async Task RegisterPatientAsync(
        Kernel kernel,
        [Description("Chat identifier")] string chatId,
        [Description("Identificación")] long identification,
        [Description("Nombre")] string name,
        [Description("Apellido")] string lastname,
        [Description("Correo electronico")] string email,
        [Description("Telefono")] long phone,
        [Description("Razón")] string reason)
    {
        var patient = new Patient
        {
            Identification = identification,
            Name = name,
            LastName = lastname,
            Email = email,
            Phone = phone,
            Reason = reason,
            Status = PatientStatusEnum.Created,
        };

        await this.patientService.Create(patient);
        await this.chatService.AsociateSender(Guid.Parse(chatId), patient.Id);
    }
}