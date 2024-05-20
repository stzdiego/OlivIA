using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olivia.Services;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class GoogleCalendarServiceTest
{
    private ServiceProvider serviceProvider;

    public GoogleCalendarServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockIGoogleCalendarSettings = new Mock<IGoogleCalendarSettings>();

        mockIGoogleCalendarSettings.Setup(x => x.ClientId).Returns("clientId");
        mockIGoogleCalendarSettings.Setup(x => x.ClientSecret).Returns("clientSecret");
        mockIGoogleCalendarSettings.Setup(x => x.Scope).Returns(new[] { "scope" });
        mockIGoogleCalendarSettings.Setup(x => x.User).Returns("user");
        mockIGoogleCalendarSettings.Setup(x => x.ApplicationName).Returns("applicationName");
        mockIGoogleCalendarSettings.Setup(x => x.CalendarId).Returns("calendarId");

        serviceCollection.AddTransient(_ => mockIGoogleCalendarSettings.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

}
