// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Settings;
using Olivia.Shared.Interfaces;

/// <summary>
/// Olivia API settings.
/// </summary>
public class OliviaApiSettings : IOliviaApiSettings
{
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets the new chat endpoint.
    /// </summary>
    public string NewChatEndpoint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the chat endpoint.
    /// </summary>
    public string NewMessagePatientEndpoint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the doctor endpoint.
    /// </summary>
    public string NewMessageDoctorEndpoint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the patients endpoint.
    /// </summary>
    public string DoctorsEndpoint { get; set; } = null!;
}
