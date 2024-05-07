// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Olivia.Shared.Interfaces;
using System;
using System.Threading;

/// <summary>
/// Google Calendar service.
/// </summary>
public class GoogleCalendarService : ICalendarService
{
    private readonly string[] scopes = { CalendarService.Scope.Calendar };
    private readonly string applicationName = "Olivia";
    private readonly string calendarId = "Olivia"; // Usa el ID del calendario que quieras

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleCalendarService"/> class.
    /// </summary>
    public GoogleCalendarService()
    {
    }

    /// <summary>
    /// Adds an event to the calendar.
    /// </summary>
    /// <param name="newEvent">Event to add.</param>
    /// <returns>Task.</returns>
    public async Task AddEvent(Event newEvent)
    {
        UserCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                this.scopes,
                "user",
                CancellationToken.None).Result;
        }

        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = this.applicationName,
        });

        EventsResource.InsertRequest request = service.Events.Insert(newEvent, this.calendarId);
        Event createdEvent = await request.ExecuteAsync();
        Console.WriteLine("Event created: " + createdEvent.HtmlLink);
    }
}