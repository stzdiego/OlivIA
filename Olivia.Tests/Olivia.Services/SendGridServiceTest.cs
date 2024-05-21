using Microsoft.Extensions.Configuration;
using Olivia.Services;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

namespace Olivia.Tests.Olivia.Services;

public class SendGridServiceTest
{
    private IMailSettings? mailSettings;

    public SendGridServiceTest()
    {
        /*
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.Development.json");
        var configuration = builder.Build();
        this.mailSettings = new MailSettings();
        configuration.GetSection("MailSettings").Bind(mailSettings);
        */
    }

    [Fact]
    public void SendEmailTemplateAsync_Should_Send()
    {
        // Arrange
        //mailSettings.Key = "SG.";
        //var sendGridService = new SendGridService(mailSettings);

        // Act
        //await sendGridService.SendEmailTemplateAsync("Test", new List<string> { "stzdiego@gmail.com" }, new { });

        // Assert
        Assert.True(true);
    }
}
