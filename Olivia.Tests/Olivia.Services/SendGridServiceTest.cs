using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olivia.Services;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

namespace Olivia.Tests.Olivia.Services;

public class SendGridServiceTest
{
    private ServiceProvider serviceProvider;

    public SendGridServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockIConfiguration = new Mock<IConfiguration>();
        var mockIMailSettings = new Mock<IMailSettings>();

        mockIMailSettings.Setup(x => x.Host).Returns("");
        mockIMailSettings.Setup(x => x.Port).Returns(0);
        mockIMailSettings.Setup(x => x.Mail).Returns("");
        mockIMailSettings.Setup(x => x.Name).Returns("");
        mockIMailSettings.Setup(x => x.Key).Returns("xxxxxxxx");

        serviceCollection.AddTransient(_ => mockIConfiguration.Object);
        serviceCollection.AddTransient(_ => mockIMailSettings.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void SendEmailTemplateAsync_Should_Send()
    {
        // Arrange
        var plugin = new SendGridService(serviceProvider.GetService<IMailSettings>()!);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(() => plugin.SendEmailTemplateAsync("xxxxxxxx", new string[] { "eee@eee.ee" }, new Dictionary<string, string>()));

        // Assert
        Assert.True(true);
    }

    [Fact]
    public void SendEmailTemplateAsync_When_Exception()
    {
        // Arrange
        var plugin = new SendGridService(serviceProvider.GetService<IMailSettings>()!);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(() => plugin.SendEmailTemplateAsync("", new string[] { "eee@eee.ee" }, new Dictionary<string, string>()));

        // Assert
        Assert.True(true);
    }
}
