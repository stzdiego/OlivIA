using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class ProgramationServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDatabase> _mockDatabase;
    private Appointment _appointment;
    private Patient _patient;
    private Doctor _doctor;

    public ProgramationServiceTest()
    {
        _doctor = new Doctor() { Id = Guid.NewGuid(), Name = "Mike", LastName = "Wazowski", Email = "email@email.com", Phone = 123456, Available = true, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 0, 0), Identification = 123456, Information = "Information", Speciality = "Speciality" };
        _patient = new Patient() { Identification = 123456, Name = "John", LastName = "Doe", Email = "email@email.com", Phone = 123456, Reason = "Reason", Status = Shared.Enums.PatientStatusEnum.Programmed, Id = Guid.NewGuid() };
        _appointment = new Appointment() { Id = Guid.NewGuid(), Date = DateTime.Now, Observations = "Observations", DoctorId = _doctor.Id, PatientId = _patient.Id, Reason = "Reason", Time = new TimeSpan(1, 0, 0) };
        var serviceCollection = new ServiceCollection();

        _mockDatabase = new Mock<IDatabase>();
        var mockLoggerProgramationService = new Mock<ILogger<ProgramationService>>();

        serviceCollection.AddTransient(_ => _mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerProgramationService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task GetDoctorId_Should_Get_Doctor_Id()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        var doctorId = await programationService.GetDoctorId(_doctor.Name);

        // Assert
        Assert.Equal(_doctor.Id, doctorId);
    }

    [Fact]
    public async Task GetDoctorId_Should_Throw_Exception_When_Doctor_Not_Found()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync((Doctor?)null);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => programationService.GetDoctorId(_doctor.Name));

        // Assert
        Assert.Equal("Doctor not found", exception.Message);
    }

    [Fact]
    public async Task GetAvailableHours_Should_Get_Available_Hours()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>());

        // Act
        var availableHours = await programationService.GetAvailableHours(_doctor.Id, DateTime.Now);

        // Assert
        Assert.Equal(_doctor.End.Hours - _doctor.Start.Hours, availableHours.Count());
    }

    [Fact]
    public async Task GetAvailableHours_Should_Throw_Exception_When_Doctor_Not_Found()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync((Doctor?)null);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => programationService.GetAvailableHours(_doctor.Id, DateTime.Now));

        // Assert
        Assert.Equal("Doctor not found", exception.Message);
    }

    [Fact]
    public async Task GetAvailableHours_Should_Throw_Exception_When_Appointments_Not_Found()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        var availableHours = await programationService.GetAvailableHours(_doctor.Id, DateTime.Now);

        // Assert
        Assert.Equal(_doctor.End.Hours - _doctor.Start.Hours, availableHours.Count());
    }

    [Fact]
    public async Task CreateAppointment_Should_Create_Appointment()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(_patient);
        _mockDatabase.Setup(x => x.Add(It.IsAny<Appointment>())).ReturnsAsync(_appointment);

        // Act
        var appointment = await programationService.CreateAppointment(_doctor.Id, _patient.Id, DateTime.Now, "Reason");

        // Assert
        Assert.NotEqual(Guid.Empty, appointment);
    }

    [Fact]
    public async Task GetAppointmentsListDay_Should_Get_Appointments_List_Day()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment> { _appointment });

        // Act
        var appointments = await programationService.GetAppointmentsListDay(_doctor.Id, DateTime.Now);

        // Assert
        Assert.NotEmpty(appointments!);
    }

    [Fact]
    public async Task GetAppointmentsListDay_Should_Return_Empty_List_When_Appointments_Not_Found()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>());

        // Act
        var appointments = await programationService.GetAppointmentsListDay(_doctor.Id, DateTime.Now);

        // Assert
        Assert.Empty(appointments!);
    }

    [Fact]
    public async Task GetAppointmentsListRange_Should_Get_Appointments_List_Range()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment> { _appointment });

        // Act
        var appointments = await programationService.GetAppointmentsListRange(_doctor.Id, DateTime.Now, DateTime.Now);

        // Assert
        Assert.NotEmpty(appointments!);
    }

    [Fact]
    public async Task GetAppointmentsListRange_Should_Return_Empty_List_When_Appointments_Not_Found()
    {
        // Arrange
        var programationService = new ProgramationService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ProgramationService>>()!);
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>());

        // Act
        var appointments = await programationService.GetAppointmentsListRange(_doctor.Id, DateTime.Now, DateTime.Now);

        // Assert
        Assert.Empty(appointments!);
    }
}
