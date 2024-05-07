// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;
using Google.Apis.Calendar.v3.Data;

/// <summary>
/// Calendar service.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Adds an event to the calendar.
    /// </summary>
    /// <param name="newEvent">Event to add.</param>
    /// <returns>Task.</returns>
    Task AddEvent(Event newEvent);
}
