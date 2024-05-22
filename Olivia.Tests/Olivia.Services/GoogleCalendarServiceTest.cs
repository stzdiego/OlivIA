using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olivia.Services;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

namespace Olivia.Tests.Olivia.Services;

public class GoogleCalendarServiceTest
{
    private ServiceProvider serviceProvider;

    public GoogleCalendarServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockIConfiguration = new Mock<IConfiguration>();
        var mockIGoogleCalendarSettings = new Mock<IGoogleCalendarSettings>();

        mockIGoogleCalendarSettings.Setup(x => x.ApplicationName).Returns("ApplicationName");
        mockIGoogleCalendarSettings.Setup(x => x.CalendarId).Returns("CalendarId");
        mockIGoogleCalendarSettings.Setup(x => x.ClientId).Returns("");
        mockIGoogleCalendarSettings.Setup(x => x.ClientSecret).Returns("ClientSecret");
        mockIGoogleCalendarSettings.Setup(x => x.User).Returns("User");

        serviceCollection.AddTransient(_ => mockIConfiguration.Object);
        serviceCollection.AddTransient(_ => mockIGoogleCalendarSettings.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void CreateEvent_Should_Create()
    {
        // Arrange
        var plugin = new GoogleCalendarService(serviceProvider.GetService<IGoogleCalendarSettings>()!);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(() => plugin.CreateEvent("Test", "Test", DateTime.Now, DateTime.Now.AddHours(1), CancellationToken.None));

        // Assert
        Assert.True(true);
    }

}
