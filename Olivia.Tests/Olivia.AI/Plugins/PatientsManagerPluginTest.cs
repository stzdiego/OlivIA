using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Olivia.AI.Plugins;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.AI.Plugins;

public class PatientsManagerPluginTest
{
    private ServiceProvider serviceProvider;

    public PatientsManagerPluginTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockDatabase = new Mock<IDatabase>();
        var mockLoggerPatientService = new Mock<ILogger<PatientService>>();
        var mockLoggerChatService = new Mock<ILogger<ChatService>>();
        var mockLoggerProgramationService = new Mock<ILogger<ProgramationService>>();
        var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
        var mockPatientService = new Mock<PatientService>(mockDatabase.Object, mockLoggerPatientService.Object);
        var mockChatService = new Mock<ChatService>(mockDatabase.Object, mockLoggerChatService.Object);
        var mockDoctorService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
        var mockProgramationService = new Mock<ProgramationService>(mockDatabase.Object, mockLoggerProgramationService.Object);

        mockPatientService.Setup(x => x.Find(It.IsAny<Guid>())).ReturnsAsync(new Patient() { Name = "Mike", LastName = "Wazowski", Email = "MikeW@email.com", Phone = 123456789, Reason = "Reason" });
        mockDoctorService.Setup(x => x.Get()).ReturnsAsync(new List<Doctor>());
        mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat());
        mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new Doctor() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "maik@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) });

        serviceCollection.AddTransient(_ => mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerPatientService.Object);
        serviceCollection.AddTransient(_ => mockLoggerChatService.Object);
        serviceCollection.AddTransient(_ => mockLoggerProgramationService.Object);
        serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);
        serviceCollection.AddTransient(_ => mockPatientService.Object);
        serviceCollection.AddTransient(_ => mockChatService.Object);
        serviceCollection.AddTransient(_ => mockDoctorService.Object);
        serviceCollection.AddTransient(_ => mockProgramationService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void GetDate_Should_Return_Current_Date()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var date = plugin.GetDate();

        // Assert
        Assert.NotEmpty(date.ToString());
    }

    [Fact]
    public async void GetPatient_Should_Return_Patient_Information()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var patient = await plugin.GetPatient(Guid.NewGuid());

        // Assert
        Assert.NotNull(patient);
    }

    [Fact]
    public async void GetDoctors_Should_Return_Doctors_Information()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var doctors = await plugin.GetDoctors();

        // Assert
        Assert.NotNull(doctors);
    }

    [Fact]
    public async void RegisterPatient_Should_Return_Patient_Guid()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var patientGuid = await plugin.RegisterPatient(new Kernel(), Guid.NewGuid(), 123456, "Mike", "Wazowski", "maikee@email.com", 123456789, "Reason");

        // Assert
        Assert.NotEmpty(patientGuid.ToString());
    }

    [Fact]
    public async void GetAvailableHours_Should_Return_Available_Hours()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var availableHours = await plugin.GetAvailableHours(new Kernel(), Guid.NewGuid(), DateTime.Now);

        // Assert
        Assert.NotNull(availableHours);
    }

    [Fact]
    public async void GetAvailableHours_Should_DateTime_Is_Less_Than_Current_DateTime()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => plugin.GetAvailableHours(new Kernel(), Guid.NewGuid(), DateTime.Now.AddDays(-1)));

        // Assert
        Assert.IsType<Exception>(exception);
    }

    [Fact]
    public async void RegisterAppointment_Should_Return_Appointment_Guid()
    {
        // Arrange
        var plugin = new PatientsManagerPlugin(serviceProvider.GetService<PatientService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var appointmentGuid = await plugin.RegisterAppointment(new Kernel(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Reason");

        // Assert
        Assert.NotEmpty(appointmentGuid.ToString());
    }
}
