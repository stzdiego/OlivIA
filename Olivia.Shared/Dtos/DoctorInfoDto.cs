// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Dtos;

/// <summary>
/// Doctor info DTO.
/// </summary>
public class DoctorInfoDto
{
    /// <summary>
    /// Gets or sets the doctor's ID.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the doctor's name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the doctor's speciality.
    /// </summary>
    public string Speciality { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the doctor's information.
    /// </summary>
    public string Information { get; set; } = string.Empty;
}