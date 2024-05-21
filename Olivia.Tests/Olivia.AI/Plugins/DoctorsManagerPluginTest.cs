using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Olivia.AI.Plugins;
using Olivia.Services;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.AI.Plugins;

public class DoctorsManagerPluginTest
{
    private ServiceProvider serviceProvider;

    public DoctorsManagerPluginTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockIPatientService = new Mock<IPatientService>();
        var mockIDoctorService = new Mock<IDoctorService>();
        var mockIChatService = new Mock<IChatService>();
        var mockCalendarService = new Mock<ICalendarService>();
        var mockIProgramationService = new Mock<IProgramationService>();
        var mockIMailService = new Mock<IMailService>();

        mockIDoctorService.Setup(x => x.GetPatientsPendingByDoctorByDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<AppointmentStateEnum>())).ReturnsAsync(new List<PatientAppointmentDto>() { new PatientAppointmentDto() { Datetime = DateTime.Now.ToString(), FullName = "John Doe", PatientId = Guid.Empty.ToString() } });
        mockIDoctorService.Setup(x => x.ApprovePatient(It.IsAny<Guid>())).ReturnsAsync(true);
        mockIProgramationService.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Appointment() { DoctorId = Guid.Empty, PatientId = Guid.Empty });
        mockIPatientService.Setup(x => x.Find(It.IsAny<Guid>())).ReturnsAsync(new Patient() { Name = "John Doe", LastName = "Doe", Email = "ee@ee.ee" });
        mockIDoctorService.Setup(x => x.Find(It.IsAny<Guid>())).ReturnsAsync(new Doctor() { Name = "John Doe", LastName = "Doe", Email = "ee@ee.ee" });
        mockIDoctorService.Setup(x => x.PayPatient(It.IsAny<Guid>())).ReturnsAsync(true);
        mockIMailService.Setup(x => x.SendEmailTemplateAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask); ;

        serviceCollection.AddTransient(_ => mockIPatientService.Object);
        serviceCollection.AddTransient(_ => mockIDoctorService.Object);
        serviceCollection.AddTransient(_ => mockIChatService.Object);
        serviceCollection.AddTransient(_ => mockCalendarService.Object);
        serviceCollection.AddTransient(_ => mockIProgramationService.Object);
        serviceCollection.AddTransient(_ => mockIMailService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task GetPatientsPendingByApproval_Should_Return_Patients()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetPatientsPendingByApproval(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPatientsPendingByApproval_Should_Throw_Exception()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetPatientsPendingByApproval(new Kernel(), "", "", "", "");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPatientsPendingByPayment_Should_Return_Patients()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetPatientsPendingByPayment(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPatientsPendingByPayment_Should_Throw_Exception()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.GetPatientsPendingByPayment(new Kernel(), "", "", "", "");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ApprovePatient_Should_Approve_Patient()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.ApprovePatient(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ApprovePatient_Should_Throw_Exception()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.ApprovePatient(new Kernel(), "", "");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RefusedPatient_Should_Refused_Patient()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.RefusedPatient(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RefusedPatient_Should_Throw_Exception()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.RefusedPatient(new Kernel(), "", "");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PayPatient_Should_Pay_Patient()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.PayPatient(new Kernel(), Guid.Empty.ToString(), Guid.Empty.ToString());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PayPatient_Should_Throw_Exception()
    {
        // Arrange
        var plugin = new DoctorsManagerPlugin(serviceProvider.GetService<IPatientService>()!, serviceProvider.GetService<IDoctorService>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<ICalendarService>()!, serviceProvider.GetService<IProgramationService>()!, serviceProvider.GetService<IMailService>()!);

        // Act
        var result = await plugin.PayPatient(new Kernel(), "", "");

        // Assert
        Assert.False(result);
    }
}
