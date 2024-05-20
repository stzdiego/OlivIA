// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;

/// <summary>
/// Represents a patient appointment data transfer object.
/// </summary>
public class PatientAppointmentDto
{
    /// <summary>
    /// Gets or sets the patient identifier.
    /// </summary>
    public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the datetime.
    /// </summary>
    public string Datetime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
