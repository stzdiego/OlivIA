// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins;

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a plugin that manages the patient.
/// </summary>
public class PatientManagerPlugin2 : IPlugin
{
    /// <summary>
    /// Gets or sets the patient.
    /// </summary>
    public Patient? Patient { get; set; }

    /// <summary>
    /// Gets or sets the doctors.
    /// </summary>
    public IEnumerable<Doctor>? Doctors { get; set; }

    private readonly IPatientService patientService;
    private readonly IDoctorService doctorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientManagerPlugin2"/> class.
    /// </summary>
    /// <param name="patientService">Patient service.</param>
    /// <param name="doctorService">Doctor service.</param>
    public PatientManagerPlugin2(IPatientService patientService, IDoctorService doctorService)
    {
        this.patientService = patientService;
        this.doctorService = doctorService;
    }

    /// <summary>
    /// Registers a patient in the system.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="patient">The patient to register.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("RegisterPatient")]
    [Description("Register a patient in the system.")]
    public async Task RegisterPatientAsync(
        Kernel kernel,
        [Description("The patient to register.")] Patient patient)
    {
        await this.patientService.Create(patient);
    }
}