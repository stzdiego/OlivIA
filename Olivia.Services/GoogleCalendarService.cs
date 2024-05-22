// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using Olivia.Shared.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;

/// <summary>
/// Google Calendar service.
/// </summary>
public class GoogleCalendarService : ICalendarService
{
    private const string TIMEZONE = "America/Bogota";
    private readonly IGoogleCalendarSettings settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleCalendarService"/> class.
    /// </summary>
    /// <param name="settings">Settings.</param>
    public GoogleCalendarService(IGoogleCalendarSettings settings)
    {
        this.settings = settings;
    }

    /// <summary>
    /// Creates an event.
    /// </summary>
    /// <param name="summary">Summary.</param>
    /// <param name="description">Description.</param>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task CreateEvent(string summary, string description, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        UserCredential credential = null;

        if (!string.IsNullOrEmpty(this.settings.ClientId))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets()
                {
                    ClientId = this.settings.ClientId,
                    ClientSecret = this.settings.ClientSecret,
                },
                this.settings.Scope,
                this.settings.User,
                cancellationToken);
        }

        var services = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = this.settings.ApplicationName,
        });

        Event newEvent = new Event()
        {
            Summary = summary,
            Description = description,
            Start = new EventDateTime()
            {
                DateTimeDateTimeOffset = start,
                TimeZone = TIMEZONE,
            },
            End = new EventDateTime()
            {
                DateTimeDateTimeOffset = end,
                TimeZone = TIMEZONE,
            },
        };

        var eventRequest = services.Events.Insert(newEvent, this.settings.CalendarId);
        var requestCreate = await eventRequest.ExecuteAsync(cancellationToken);
    }
}