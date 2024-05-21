using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olivia.Api.Controllers;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Api.Controllers;

public class ChatAssistanceControllerTest
{
    private ServiceProvider serviceProvider;

    public ChatAssistanceControllerTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockMailSettings = new Mock<IMailSettings>();
        var mockIGoogleCalendarSettings = new Mock<IGoogleCalendarSettings>();
        var mockIChatService = new Mock<IChatService>();
        var mockIDoctorService = new Mock<IDoctorService>();
        var mockIPatientService = new Mock<IPatientService>();
        var mockIAgen = new Mock<IAgent>();
        var mockContext = new Mock<OliviaDbContext>(new DbContextOptions<OliviaDbContext>());

        mockIChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).ReturnsAsync(new List<Message>() { new Message() { } });
        mockIDoctorService.Setup(x => x.Find(It.IsAny<Guid>())).ReturnsAsync(new Doctor() { Name = "Test" });

        serviceCollection.AddTransient(_ => mockMailSettings.Object);
        serviceCollection.AddTransient(_ => mockIGoogleCalendarSettings.Object);
        serviceCollection.AddTransient(_ => mockIChatService.Object);
        serviceCollection.AddTransient(_ => mockIDoctorService.Object);
        serviceCollection.AddTransient(_ => mockIPatientService.Object);
        serviceCollection.AddTransient(_ => mockIAgen.Object);
        serviceCollection.AddTransient(_ => mockContext.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task NewChat_Should_Ok()
    {
        // Arrange
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), serviceProvider.GetRequiredService<IChatService>(), serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.NewChat();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task NewChat_Should_StatusCode_500()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.Create()).Throws(new Exception());
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.NewChat();

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task PatientNewMessage_Should_Ok()
    {
        // Arrange
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), serviceProvider.GetRequiredService<IChatService>(), serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.PatientNewMessage(new PatientNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test" });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task PatientNewMessage_Should_Message_Null()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.PatientNewMessage(new PatientNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task PatientNewMessage_Should_Message_Empty()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).ReturnsAsync(new List<Message>());
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.PatientNewMessage(new PatientNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test" });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task PatientNewMessage_Should_Throw_Exception()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).Throws(new Exception());
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.PatientNewMessage(new PatientNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test" });

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task DoctorNewMessage_Should_Ok()
    {
        // Arrange
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), serviceProvider.GetRequiredService<IChatService>(), serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.DoctorNewMessage(new DoctorNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test", DoctorId = Guid.NewGuid() });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DoctorNewMessage_Should_Message_Null()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.DoctorNewMessage(new DoctorNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test", DoctorId = Guid.NewGuid() });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DoctorNewMessage_Should_Message_Empty()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).ReturnsAsync(new List<Message>());
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.DoctorNewMessage(new DoctorNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test", DoctorId = Guid.NewGuid() });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DoctorNewMessage_Should_Throw_Exception()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).Throws(new Exception());
        var chatAssistanceController = new ChatAssistanceController(serviceProvider.GetRequiredService<IMailSettings>(), serviceProvider.GetRequiredService<IGoogleCalendarSettings>(), mockChatService.Object, serviceProvider.GetRequiredService<IDoctorService>(), serviceProvider.GetRequiredService<IPatientService>(), serviceProvider.GetRequiredService<IAgent>(), serviceProvider.GetRequiredService<OliviaDbContext>());

        // Act
        var result = await chatAssistanceController.DoctorNewMessage(new DoctorNewMessageDto() { ChatId = Guid.NewGuid(), Content = "Test", DoctorId = Guid.NewGuid() });

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
}
