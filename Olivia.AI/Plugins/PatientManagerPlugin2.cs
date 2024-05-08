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
public class PatientManagerPlugin2
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
    /// Gets the doctors.
    /// </summary>
    /// <returns>The doctors.</returns>
    [KernelFunction("GetDoctors")]
    [Description("Gets the doctors available for appointments.")]
    public async Task<IEnumerable<Doctor>?> GetDoctorsAsync()
    {
        if (this.Doctors is null)
        {
            this.Doctors = await this.doctorService.GetAvailable();
        }

        return this.Doctors;
    }
}