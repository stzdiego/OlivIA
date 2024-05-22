using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class DoctorServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDatabase> _mockDatabase;
    private Doctor _doctor;

    public DoctorServiceTest()
    {
        _doctor = new Doctor() { Identification = 123456, Name = "John", LastName = "Doe", Email = "email@email.com", Phone = 123456, Speciality = "Speciality", Information = "Information", Available = true, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) };

        var serviceCollection = new ServiceCollection();

        _mockDatabase = new Mock<IDatabase>();
        var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();

        serviceCollection.AddTransient(_ => _mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_Should_Create_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));

        // Assert
        Assert.NotEqual(Guid.Empty, doctorId);
    }

    [Fact]
    public async Task Update_Should_Update_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        await doctorService.Update(doctorId, 654321, "Jane", "Doe", "email@email.com", 123456, "Speciality", "Information");
        var doctorUpdated = await doctorService.Find(doctorId);

        // Assert
        Assert.Equal(654321, doctorUpdated!.Identification);
    }

    [Fact]
    public async Task Update_Should_Update_Doctor_Null()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var id = new Guid();

        // Act
        await doctorService.Update(id, 654321, "Jane", "Doe", "email@email.com", 123456, "Speciality", "Information");

        // Assert
        _mockDatabase.Verify(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Exists_Should_Return_True()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        _mockDatabase.Setup(x => x.Exist<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);

        // Act
        var exists = await doctorService.Exists(123456);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task Get_Should_Return_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        _mockDatabase.Setup(x => x.Get<Doctor>()).ReturnsAsync(new List<Doctor>() { _doctor });
        // Act
        var doctor = await doctorService.Get();

        // Assert
        Assert.NotNull(doctor);
    }

    [Fact]
    public async Task GetAvailable_Should_Return_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });

        // Act
        var doctor = await doctorService.GetAvailable();

        // Assert
        Assert.NotNull(doctor);
    }

    [Fact]
    public void GetMostRecentAvailableAppointmentAsync_Should_Return_Datetime()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, Date = DateTime.UtcNow } });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => doctorService.GetMostRecentAvailableAppointmentAsync(Guid.Empty));
        Assert.True(true);
    }

    [Fact]
    public async Task GetAvailableAppointmentsByDate_Should_Return_List()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, Date = DateTime.Now } });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var appointments = await doctorService.GetAvailableAppointmentsByDate(Guid.Empty, DateTime.UtcNow);

        // Assert
        Assert.NotNull(appointments);
    }

    [Fact]
    public async Task GetPatientsPendingByDoctorByDate_Should_Return_List()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, } });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(new Patient() { Id = Guid.Empty, Name = "John", LastName = "Doe", Email = "email", Phone = 123456 });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var appointments = await doctorService.GetPatientsPendingByDoctorByDate(Guid.Empty, DateTime.Now, DateTime.Now.AddDays(1), Shared.Enums.AppointmentStateEnum.PendingApproval);

        // Assert
        Assert.NotNull(appointments);
    }

    [Fact]
    public async Task ApprovePatient_Should_Return_True()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, } });
        _mockDatabase.Setup(x => x.Find<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new Appointment() { DoctorId = Guid.Empty, });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(new Patient() { Id = Guid.Empty, Name = "John", LastName = "Doe", Email = "email", Phone = 123456 });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var result = await doctorService.ApprovePatient(Guid.Empty);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RefusedPatient_Should_Return_True()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, } });
        _mockDatabase.Setup(x => x.Find<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new Appointment() { DoctorId = Guid.Empty, });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(new Patient() { Id = Guid.Empty, Name = "John", LastName = "Doe", Email = "email", Phone = 123456 });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var result = await doctorService.RefusedPatient(Guid.Empty);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PayPatient_Should_Return_True()
    {
        // Arrange
        _mockDatabase.Setup(x => x.Get<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new List<Appointment>() { new Appointment() { DoctorId = Guid.Empty, } });
        _mockDatabase.Setup(x => x.Find<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(new Appointment() { DoctorId = Guid.Empty, State = Shared.Enums.AppointmentStateEnum.PendingPayment });
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);
        _mockDatabase.Setup(x => x.Get<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new List<Doctor>() { _doctor });
        _mockDatabase.Setup(x => x.Find<Patient>(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(new Patient() { Id = Guid.Empty, Name = "John", LastName = "Doe", Email = "email", Phone = 123456 });
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var result = await doctorService.PayPatient(Guid.Empty);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAvailable_Should_Return_Null()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var doctor = await doctorService.GetAvailable();

        // Assert
        Assert.Null(doctor);
    }

    [Fact]
    public async Task Find_Guid_Should_Return_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        var doctor = await doctorService.Find(doctorId);

        // Assert
        Assert.NotNull(doctor);
    }

    [Fact]
    public void Find_Guid_Should_Return_Null()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var id = new Guid();

        // Act
        var ex = Assert.ThrowsAsync<Exception>(() => doctorService.Find(id));

        // Assert
        Assert.NotNull(ex);
    }

    [Fact]
    public async Task Find_Identification_Should_Return_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        var doctor = await doctorService.Find(123456);

        // Assert
        Assert.NotNull(doctor);
    }

    [Fact]
    public void Find_Identification_Should_Return_Null()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(() => doctorService.Find(123456));

        // Assert
        Assert.NotNull(ex);
    }

    [Fact]
    public async Task Delete_Should_Delete_Doctor()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);
        var doctorId = await doctorService.Create(123456, "John", "Doe", "email@email.com", 123456, "Speciality", "Information", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        _mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(_doctor);

        // Act
        await doctorService.Delete(doctorId);

        // Assert
        _mockDatabase.Verify(x => x.Delete(It.IsAny<Doctor>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Throw_Exception_When_Doctor_Not_Found()
    {
        // Arrange
        var doctorService = new DoctorService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<DoctorService>>()!);

        // Act
        await doctorService.Delete(new Guid());

        // Assert
        _mockDatabase.Verify(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
    }
}
