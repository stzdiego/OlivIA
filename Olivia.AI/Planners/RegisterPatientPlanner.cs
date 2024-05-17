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
    /// <returns>The list of steps required to register a patient in the system.</returns>
    [KernelFunction("GenerateRequiredStepsAsync")]
    [Description("Devuelve los pasos necesarios para registrar un paciente en el sistema.")]
    [return: Description("La lista de pasos necesarios para registrar un paciente en el sistema.")]
    public async Task<string> GenerateRequiredStepsAsync(
        Kernel kernel)
    {
        var result = await kernel.InvokePromptAsync(
            """
            Vas a registrar un paciente con la siguiente información {patient} en el sistema.
            Antes de hacerlo, ¿puedes recomendarme de forma sucinta los pasos que debo seguir en una lista numerada?
            Quiero asegurarme de no olvidar nada que pueda ayudar a que mi registro de paciente sea más eficiente.
            """,
            new () { { "Patient", new Patient() } });

        return result.ToString();
    }
}