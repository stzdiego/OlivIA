// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Settings;
using Olivia.Shared.Interfaces;

/// <summary>
/// Google Calendar settings.
/// </summary>
public class GoogleCalendarSettings : IGoogleCalendarSettings
{
    /// <summary>
    /// Gets or sets the client id.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    public string[] Scope { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the calendar id.
    /// </summary>
    public string CalendarId { get; set; } = string.Empty;
}
