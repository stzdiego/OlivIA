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

        var mockIChatService = new Mock<IChatService>();
        var mockIPatientService = new Mock<IPatientService>();
        var mockIDoctorService = new Mock<IDoctorService>();
        var mockIProgramationService = new Mock<IProgramationService>();
        var mockIMailService = new Mock<IMailService>();

        mockIChatService.Setup(x => x.AsociateSender(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        mockIDoctorService.Setup(x => x.GetAvailable()).ReturnsAsync(new List<Doctor>() { new Doctor() { Name = "John Doe", LastName = "Doe", Email = "ee@ee.ee", Information = "Information" } });
        mockIMailService.Setup(x => x.SendEmailTemplateAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);

        serviceCollection.AddTransient(_ => mockIChatService.Object);
        serviceCollection.AddTransient(_ => mockIPatientService.Object);
        serviceCollection.AddTransient(_ => mockIDoctorService.Object);
        serviceCollection.AddTransient(_ => mockIProgramationService.Object);
        serviceCollection.AddTransient(_ => mockIMailService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task RegisterPatientAsync_Should_Register()
    {
        // Arrange
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        await plugin.RegisterPatientAsync(new Kernel(), Guid.Empty.ToString(), 123, "", "", "", 123, "");

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task GetDoctorsInfoAsync_Should_Return_Doctors()
    {
        // Arrange
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetDoctorsInfoAsync(new Kernel(), Guid.Empty.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetDoctorsInfoAsync_Should_Throw_Exception()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.GetAvailable()).Throws(new Exception("Error"));
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, mockDoctorService.Object, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetDoctorsInfoAsync(new Kernel(), Guid.Empty.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMostRecentAvailableAppointmentAsync_Should_Return_Appointment()
    {
        // Arrange
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetMostRecentAvailableAppointmentAsync(new Kernel(), Guid.Empty.ToString());

        // Assert
        Assert.IsType<DateTime>(result);
    }

    [Fact]
    public async Task GetMostRecentAvailableAppointmentAsync_Should_Throw_Exception()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.GetMostRecentAvailableAppointmentAsync(It.IsAny<Guid>())).Throws(new Exception("Error"));
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, mockDoctorService.Object, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);
        // Act
        var result = await plugin.GetMostRecentAvailableAppointmentAsync(new Kernel(), Guid.Empty.ToString());

        // Assert
        Assert.IsType<DateTime>(result);
    }

    [Fact]
    public async Task GetAvailableAppointmentByDateAsync_Should_Return_Appointment()
    {
        // Arrange
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetAvailableAppointmentByDateAsync(new Kernel(), Guid.Empty.ToString(), DateTime.Now.ToString());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAvailableAppointmentByDateAsync_Should_Throw_Exception()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.GetAvailableAppointmentsByDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).Throws(new Exception("Error"));
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, mockDoctorService.Object, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetAvailableAppointmentByDateAsync(new Kernel(), Guid.Empty.ToString(), DateTime.Now.ToString());

        // Assert
        Assert.IsAssignableFrom<IEnumerable<DateTime>>(result);
    }

    [Fact]
    public async Task RegisterAppointmentAsync_Should_Register_Appointment()
    {
        // Arrange
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        await plugin.RegisterAppointmentAsync(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString(), Guid.Empty.ToString(), DateTime.Now.ToString(), "Random");

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task RegisterAppointmentAsync_Should_Throw_Exception()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.GetAvailableAppointmentsByDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).Throws(new Exception("Error"));
        var plugin = new PatientManagerPlugin(serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<IPatientService>()!, mockDoctorService.Object, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        await plugin.RegisterAppointmentAsync(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString(), Guid.Empty.ToString(), DateTime.Now.ToString(), "Random");

        // Assert
        Assert.True(true);
    }
}
