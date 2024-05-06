using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class PatientServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDatabase> _mockDatabase;
    private Patient _patient;

    public PatientServiceTest()
    {
        _patient = new Patient() { Identification = 123456, Name = "John", LastName = "Doe", Email = "email@email.com", Phone = 123456, Reason = "Reason" };
        var serviceCollection = new ServiceCollection();

        _mockDatabase = new Mock<IDatabase>();
        var mockLoggerPatientService = new Mock<ILogger<PatientService>>();

        serviceCollection.AddTransient(_ => _mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerPatientService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_Should_Create_Patient()
    {
        // Arrange
        var patientService = new PatientService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<PatientService>>()!);

        // Act
        var patientId = await patientService.Create(123456, "John", "Doe", "email@email.com", 123456, "Reason");

        // Assert
        Assert.NotEqual(Guid.Empty, patientId);
    }

    [Fact]
    public async Task Update_Should_Update_Patient()
    {
        // Arrange
        var patientService = new PatientService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<PatientService>>()!);
        var patientId = await patientService.Create(123456, "John", "Doe", "email@email.com", 123456, "Reason");
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(_patient);

        // Act
        await patientService.Update(patientId, 654321, "Jane", "Doe", "email@email.com", 123456, "Reason");
        var patientUpdated = await patientService.Find(patientId);

        // Assert
        Assert.Equal(654321, patientUpdated!.Identification);
    }

    [Fact]
    public async Task Exists_Should_Return_True()
    {
        // Arrange
        var patientService = new PatientService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<PatientService>>()!);
        var patientId = await patientService.Create(123456, "John", "Doe", "email@email.com", 123456, "Reason");
        _mockDatabase.Setup(x => x.Exist<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);

        // Act
        var exists = await patientService.Exists(123456);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task Find_Should_Return_Patient()
    {
        // Arrange
        var patientService = new PatientService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<PatientService>>()!);
        var patientId = await patientService.Create(123456, "John", "Doe", "email@email.com", 123456, "Reason");
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(_patient);

        // Act
        var patient = await patientService.Find(patientId);

        // Assert
        Assert.NotNull(patient);
    }
}
