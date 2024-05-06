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

public class ProgramationManagerPluginTest
{
    private ServiceProvider serviceProvider;
    private Mock<DoctorService> _mockDoctorService;
    private Mock<ProgramationService> _mockProgramationService;

    public ProgramationManagerPluginTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockDatabase = new Mock<IDatabase>();
        var mockLoggerProgramationService = new Mock<ILogger<ProgramationService>>();
        var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
        var mockLoggerChatService = new Mock<ILogger<ChatService>>();
        _mockProgramationService = new Mock<ProgramationService>(mockDatabase.Object, mockLoggerProgramationService.Object);
        _mockDoctorService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
        var mockChatService = new Mock<ChatService>(mockDatabase.Object, mockLoggerChatService.Object);

        serviceCollection.AddTransient(_ => mockDatabase.Object);
        serviceCollection.AddTransient(_ => _mockProgramationService.Object);
        serviceCollection.AddTransient(_ => _mockDoctorService.Object);
        serviceCollection.AddTransient(_ => mockChatService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void GetDate_Should_Return_Current_Date()
    {
        // Arrange
        var plugin = new ProgramationManagerPlugin(serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<DoctorService>()!);

        // Act
        var date = plugin.GetDate();

        // Assert
        Assert.NotEmpty(date.ToString());
    }

    [Fact]
    public async Task GetDoctorId_Should_Return_Doctor_Id()
    {
        // Arrange
        var plugin = new ProgramationManagerPlugin(serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<DoctorService>()!);
        _mockDoctorService.Setup(x => x.Find(It.IsAny<long>())).ReturnsAsync(new Doctor() { Id = Guid.NewGuid(), Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "doctor@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) });

        // Act
        var doctorId = await plugin.GetDoctorId(new Kernel(), 123456);

        // Assert
        Assert.NotEqual(Guid.Empty, doctorId);
    }

    [Fact]
    public async Task GetAppointmentsByDoctorToday_Should_Return_Appointments()
    {
        // Arrange
        var plugin = new ProgramationManagerPlugin(serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<DoctorService>()!);
        var doctor = new Doctor() { Id = Guid.NewGuid(), Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "doctor@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) };
        _mockProgramationService.Setup(x => x.GetAppointmentsListDay(doctor.Id, DateTime.Now)).ReturnsAsync(new List<Appointment>());

        // Act
        var appointments = await plugin.GetAppointmentsByDoctorToday(new Kernel(), Guid.NewGuid(), doctor.Id);

        // Assert
        Assert.NotNull(appointments);
    }

    [Fact]
    public async Task GetAppointmentsByDoctorByDate_Should_Return_Appointments()
    {
        // Arrange
        var plugin = new ProgramationManagerPlugin(serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<DoctorService>()!);
        var doctor = new Doctor() { Id = Guid.NewGuid(), Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "doctor@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) };
        _mockProgramationService.Setup(x => x.GetAppointmentsListDay(doctor.Id, DateTime.Now)).ReturnsAsync(new List<Appointment>());

        // Act
        var appointments = await plugin.GetAppointmentsByDoctorByDate(new Kernel(), Guid.NewGuid(), doctor.Id, DateTime.Now);

        // Assert
        Assert.NotNull(appointments);
    }

    [Fact]
    public async Task GetAppointmentsByDoctorByRange_Should_Return_Appointments()
    {
        // Arrange
        var plugin = new ProgramationManagerPlugin(serviceProvider.GetService<ProgramationService>()!, serviceProvider.GetService<ChatService>()!, serviceProvider.GetService<DoctorService>()!);
        var doctor = new Doctor() { Id = Guid.NewGuid(), Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "doctor@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) };
        _mockProgramationService.Setup(x => x.GetAppointmentsListRange(doctor.Id, DateTime.Now, DateTime.Now)).ReturnsAsync(new List<Appointment>());

        // Act
        var appointments = await plugin.GetAppointmentsByDoctorByRange(new Kernel(), Guid.NewGuid(), doctor.Id, DateTime.Now, DateTime.Now);

        // Assert
        Assert.NotNull(appointments);
    }
}
