// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Planners;

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a planner that registers the patient.
/// </summary>
public class RegisterPatientPlanner : IPlugin
{
    /// <summary>
    /// Generates the steps necessary to register a patient in the system.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="patient">The patient to register.</param>
    /// <returns>The list of steps required to register a patient in the system.</returns>
    [KernelFunction("GenerateRequiredStepsAsync")]
    [Description("Returns the steps necessary to register a patient in the system.")]
    [return: Description("The list of steps required to register a patient in the system.")]
    public async Task<string> GenerateRequiredStepsAsync(
        Kernel kernel,
        [Description("The patient to register.")] Patient patient)
    {
        var result = await kernel.InvokePromptAsync(
            """
        I am going to register a patient with the following information {patient} in the system.
        Before I do that, can you succinctly recommend the steps I should follow in a numbered list?
        I want to make sure I don't forget anything that could help make my patient record more efficient.
        """,
            new () { { "Patient", patient }, });

        return result.ToString();
    }
}