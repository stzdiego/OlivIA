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
    /// Creates an event.
    /// </summary>
    /// <param name="summary">Summary.</param>
    /// <param name="description">Description.</param>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task CreateEvent(string summary, string description, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
