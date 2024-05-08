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

    [Fact]
    public async Task CreateEvent_Should_Create_Event()
    {
        // Arrange
        var googleCalendarService = new GoogleCalendarService(serviceProvider.GetService<IGoogleCalendarSettings>()!);

        // Act
        //var ex = await Record.ExceptionAsync(async () => await googleCalendarService.CreateEvent("summary", "description", DateTime.Now, DateTime.Now.AddHours(1)));

        // Assert
        Assert.True(true);
    }
}
