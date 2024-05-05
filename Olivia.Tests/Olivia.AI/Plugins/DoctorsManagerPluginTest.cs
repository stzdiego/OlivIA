using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Olivia.AI.Plugins;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.AI.Plugins;

public class DoctorsManagerPluginTest
{
    private ServiceProvider serviceProvider;

    public DoctorsManagerPluginTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockDatabase = new Mock<IDatabase>();
        var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
        var mockLoggerChatService = new Mock<ILogger<ChatService>>();
        var mockDoctorService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
        var mockChatService = new Mock<ChatService>(mockDatabase.Object, mockLoggerChatService.Object);

        mockDoctorService.Setup(x => x.Get()).ReturnsAsync(new List<Doctor>());

        serviceCollection.AddTransient(_ => mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);
        serviceCollection.AddTransient(_ => mockLoggerChatService.Object);
        serviceCollection.AddTransient(_ => mockDoctorService.Object);
        serviceCollection.AddTransient(_ => mockChatService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async void GetInformation_Should_Return_Doctors_Information()
    {
        // Arrange
        var kernel = new Kernel();
        var doctorService = serviceProvider.GetService<DoctorService>()!;
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<DoctorService>()!, serviceProvider.GetService<ChatService>()!);

        // Act
        var doctors = await plugin.GetInformation(kernel);

        // Assert
        Assert.NotNull(doctors);
    }
}
