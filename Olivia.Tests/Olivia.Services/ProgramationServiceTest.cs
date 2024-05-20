using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class ProgramationServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDatabase> _mockDatabase;

    public ProgramationServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        _mockDatabase = new Mock<IDatabase>();
        var mockLoggerProgramationService = new Mock<ILogger<ProgramationService>>();

        serviceCollection.AddTransient(_ => _mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerProgramationService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task CreateAppointment_Should_Create()
    {
        //Arrage
        var ProgramationService = new ProgramationService(_mockDatabase.Object, serviceProvider.GetService<ILogger<ProgramationService>>()!);

        //Act
        var appointmentId = await ProgramationService.CreateAppointment(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Reason");

        //Assert
        _mockDatabase.Verify(x => x.Add(It.IsAny<Appointment>()), Times.Once);
    }

    [Fact]
    public async Task Find_Should_Find()
    {
        //Arrage
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() });
        var ProgramationService = new ProgramationService(_mockDatabase.Object, serviceProvider.GetService<ILogger<ProgramationService>>()!);

        //Act
        var appointment = await ProgramationService.Find(Guid.NewGuid(), Guid.NewGuid());

        //Assert
        Assert.NotNull(appointment);
    }
}
