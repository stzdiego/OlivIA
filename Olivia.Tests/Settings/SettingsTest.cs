using Olivia.Shared.Settings;

namespace Olivia.Tests.Settings;

public class SettingsTest
{
    [Fact]
    public void OliviaApiSettings()
    {
        // Arrange
        var settings = new OliviaApiSettings()
        {
            DoctorsEndpoint = "https://doctors.com",
            NewChatEndpoint = "https://chat.com",
            NewMessageDoctorEndpoint = "https://message.com",
            NewMessagePatientEndpoint = "https://message.com",
            Url = "https://olivia.com"
        };

        var googleCalendarSettings = new GoogleCalendarSettings()
        {
            CalendarId = "calendarId",
            ApplicationName = "applicationName",
            ClientId = "clientId",
            ClientSecret = "clientSecret",
            Scope = ["scope"],
            User = "user"
        };

        var openAiSettings = new OpenAISettings()
        {
            Description = "description",
            Key = "key",
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            MaxTokens = 0,
            Temperature = 0,
            TopP = 0,
            Model = "model",
            StopSequences = ["stopSequences"],
            Type = "type"
        };

        var mailSetting = new MailSettings()
        {
            Host = "host",
            Port = 0,
            Mail = "mail",
            Name = "name",
            Key = "key",
            DisplayName = "displayName",
            EnableSsl = true
        };

        // Assert
        Assert.True(true);
    }
}

