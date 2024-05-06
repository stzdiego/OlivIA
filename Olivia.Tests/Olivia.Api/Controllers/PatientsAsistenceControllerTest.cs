using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Olivia.Data;
using Olivia.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Olivia.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Olivia.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Olivia.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Olivia.Tests.Olivia.Api.Controllers;

public class PatientsAsistenceControllerTest
{
    private ServiceProvider _serviceProvider;
    private Mock<IChatService> _mockChatService;
    private Mock<IAgent> _mockAgent1;

    public PatientsAsistenceControllerTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockConfiguration = new Mock<IConfiguration>();
        var mockLoggerPatientsAsistenceController = new Mock<ILogger<PatientsAsistenceController>>();
        _mockChatService = new Mock<IChatService>();
        var mockContext = new Mock<OliviaDbContext>(new DbContextOptions<OliviaDbContext>());
        _mockAgent1 = new Mock<IAgent>();
        var mockAgent2 = new Mock<IAgent>();
        var mockModelSection = new Mock<IConfigurationSection>();
        var mockKeySection = new Mock<IConfigurationSection>();
        var mockMaxTokensSection = new Mock<IConfigurationSection>();
        var mockTemperatureSection = new Mock<IConfigurationSection>();
        var mockPenaltySection = new Mock<IConfigurationSection>();

        mockModelSection.SetupGet(m => m.Value).Returns("model");
        mockKeySection.SetupGet(m => m.Value).Returns("key");
        mockMaxTokensSection.SetupGet(m => m.Value).Returns("100");
        mockTemperatureSection.SetupGet(m => m.Value).Returns("0.5");
        mockPenaltySection.SetupGet(m => m.Value).Returns("0.1");
        mockConfiguration.Setup(x => x.GetSection("Agents:Reception:Model")).Returns(mockModelSection.Object);
        mockConfiguration.Setup(x => x.GetSection("Agents:Reception:Key")).Returns(mockKeySection.Object);
        mockConfiguration.Setup(x => x.GetSection("Agents:Reception:MaxTokens")).Returns(mockMaxTokensSection.Object);
        mockConfiguration.Setup(x => x.GetSection("Agents:Reception:Temperature")).Returns(mockTemperatureSection.Object);
        mockConfiguration.Setup(x => x.GetSection("Agents:Reception:Penalty")).Returns(mockPenaltySection.Object);

        _mockChatService.Setup(x => x.Create()).ReturnsAsync(Guid.NewGuid());
        _mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).ReturnsAsync(new StringBuilder("Summary"));
        _mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(new Chat());
        _mockAgent1.Setup(x => x.Send(It.IsAny<StringBuilder>())).ReturnsAsync("Response");
        mockAgent2.Setup(x => x.Send(It.IsAny<StringBuilder>())).ReturnsAsync("Response");

        serviceCollection.AddTransient(_ => mockConfiguration.Object);
        serviceCollection.AddTransient(_ => mockLoggerPatientsAsistenceController.Object);
        serviceCollection.AddTransient(_ => mockContext.Object);
        serviceCollection.AddTransient(_ => _mockChatService.Object);
        serviceCollection.AddTransient(_ => _mockAgent1.Object);
        serviceCollection.AddTransient(_ => mockAgent2.Object);

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Post_Initialize_Should_Return_Ok()
    {
        // Arrange
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);

        // Act
        var result = await PatientsAsistenceController.Post();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Post_Initialize_Should_Return_InternalServerError()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.Create()).Throws(new Exception("Error creating chat"));
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, mockChatService.Object, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);

        // Act
        var result = await PatientsAsistenceController.Post();

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Post_NewMessage_Should_Return_Ok()
    {
        // Arrange
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);

        // Act
        var result = await PatientsAsistenceController.Post();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /*
    [Fact]
    public async Task Post_NewMessage_Should_Chat_Not_Found()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync((Chat?)null);
        var PatientsAsistenceController = new PatientsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, mockChatService.Object, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<IAgent>()!);

        // Act
        var result = await PatientsAsistenceController.Post();

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    */

    [Fact]
    public async Task Post_NewMessage_Should_Patient_Not_Null_Return_Ok()
    {
        // Arrange
        _mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(new Chat() { PatientId = new Guid(), Patient = new Patient() { Identification = 123456, Name = "John", LastName = "Doe", Email = "email@email.com", Phone = 123456, Reason = "Reason" } });
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);
        var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

        // Act
        var result = await PatientsAsistenceController.Post(newMessageDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Post_NewMessage_Should_Patient_Not_Null_And_PatientId_Not_Null_Return_Ok()
    {
        // Arrange
        _mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(new Chat() { PatientId = new Guid(), Patient = new Patient() { Identification = 123456, Name = "John", LastName = "Doe", Email = "email@email.com", Phone = 123456, Reason = "Reason" } });
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);
        var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

        // Act
        var result = await PatientsAsistenceController.Post(newMessageDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Post_NewMessage_Should_Exception()
    {
        // Arrange
        _mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).Throws(new Exception("Exception"));
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);
        var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

        // Act
        var result = await PatientsAsistenceController.Post(newMessageDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Post_Resume_Should_Return_Ok()
    {
        // Arrange
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, _serviceProvider.GetService<IChatService>()!, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);
        var idDto = new IdDto() { Id = Guid.NewGuid() };

        // Act
        var result = await PatientsAsistenceController.Post(idDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Post_Resume_Should_Return_InternalServerError()
    {
        // Arrange
        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).Throws(new Exception("Error getting chat"));
        var PatientsAsistenceController = new PatientsAsistenceController(_serviceProvider.GetService<IConfiguration>()!, _serviceProvider.GetService<ILogger<PatientsAsistenceController>>()!, mockChatService.Object, _serviceProvider.GetService<OliviaDbContext>()!, _serviceProvider.GetService<IAgent>()!, _serviceProvider.GetService<IAgent>()!);
        var idDto = new IdDto() { Id = Guid.NewGuid() };

        // Act
        var result = await PatientsAsistenceController.Post(idDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
