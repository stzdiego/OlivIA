// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

/// <summary>
/// Google Calendar settings interface.
/// </summary>
public interface IGoogleCalendarSettings
{
    /// <summary>
    /// Gets or sets the client id.
    /// </summary>
    string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    string[] Scope { get; set; }

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    string User { get; set; }

    /// <summary>
    /// Gets the calendar id.
    /// </summary>
    string CalendarId { get; }
}
