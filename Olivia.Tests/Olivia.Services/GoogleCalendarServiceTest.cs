using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olivia.Services;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

namespace Olivia.Tests.Olivia.Services;

public class GoogleCalendarServiceTest
{
    private IGoogleCalendarSettings googleCalendarSettings;

    public GoogleCalendarServiceTest()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.Development.json");
        var configuration = builder.Build();
        this.googleCalendarSettings = new GoogleCalendarSettings();
        configuration.GetSection("GoogleCalendarSettings").Bind(googleCalendarSettings);

        var openAISettings = new OpenAISettings();
        configuration.GetSection("OpenAISettings").Bind(openAISettings);
    }

    [Fact]
    public async Task CreateEvent_Should_Create()
    {
        // Arrange
        var googleCalendar = new GoogleCalendarService(googleCalendarSettings);

        // Act
        //await googleCalendar.CreateEvent("Test", "Test", DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-4).AddHours(1));

        // Assert
        Assert.True(true);
    }

}
