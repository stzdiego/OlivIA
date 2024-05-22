// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

/// <summary>
/// Interface for Olivia API settings.
/// </summary>
public interface IOliviaApiSettings
{
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    string Url { get; set; }

    /// <summary>
    /// Gets or sets the new chat endpoint.
    /// </summary>
    string NewChatEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the chat endpoint.
    /// </summary>
    string NewMessagePatientEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the doctor endpoint.
    /// </summary>
    string NewMessageDoctorEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the patients endpoint.
    /// </summary>
    string DoctorsEndpoint { get; set; }
}
