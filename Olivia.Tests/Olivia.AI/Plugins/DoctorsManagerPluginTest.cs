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
        var mockDoctorService = new Mock<IDoctorService>();
        var mockChatService = new Mock<IChatService>();
        var mockCalendarService = new Mock<ICalendarService>();

        mockDoctorService.Setup(x => x.Get()).ReturnsAsync(new List<Doctor>());

        serviceCollection.AddTransient(_ => mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockDoctorService.Object);
        serviceCollection.AddTransient(_ => mockChatService.Object);
        serviceCollection.AddTransient(_ => mockCalendarService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task GetInformation_Should_Return_Doctors_Information()
    {
        // Arrange
        var kernel = new Kernel();
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!);

        // Act
        var doctors = await plugin.GetInformation(kernel);

        // Assert
        Assert.NotNull(doctors);
    }
}
