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

namespace Olivia.Tests.Olivia.Api.Controllers
{
    public class DoctorAsistenceControllerTest
    {
        private ServiceProvider serviceProvider;

        public DoctorAsistenceControllerTest()
        {
            var serviceCollection = new ServiceCollection();

            var mockConfiguration = new Mock<IConfiguration>();
            var mockLoggerDoctorAsistenceController = new Mock<ILogger<DoctorsAsistenceController>>();
            var mockChatService = new Mock<IChatService>();
            var mockContext = new Mock<OliviaDbContext>(new DbContextOptions<OliviaDbContext>());
            var mockModelSection = new Mock<IConfigurationSection>();
            var mockKeySection = new Mock<IConfigurationSection>();
            var mockMaxTokensSection = new Mock<IConfigurationSection>();
            var mockTemperatureSection = new Mock<IConfigurationSection>();
            var mockPenaltySection = new Mock<IConfigurationSection>();
            var mockAgent = new Mock<IAgent>();
            var mockCalendarService = new Mock<ICalendarService>();

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

            mockChatService.Setup(x => x.Create()).ReturnsAsync(Guid.NewGuid());
            mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).ReturnsAsync(new StringBuilder("Summary"));
            mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(new Chat());
            mockAgent.Setup(x => x.Send(It.IsAny<StringBuilder>())).ReturnsAsync("Response");

            serviceCollection.AddTransient(_ => mockConfiguration.Object);
            serviceCollection.AddTransient(_ => mockLoggerDoctorAsistenceController.Object);
            serviceCollection.AddTransient(_ => mockContext.Object);
            serviceCollection.AddTransient(_ => mockChatService.Object);
            serviceCollection.AddTransient(_ => mockAgent.Object);
            serviceCollection.AddTransient(_ => mockCalendarService.Object);


            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Post_Initialize_Should_Return_Ok()
        {
            // Arrange
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);

            // Act
            var result = await doctorAsistenceController.Post();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Post_Initialize_Should_Throw_Exception()
        {
            // Arrange
            var mockAgent = new Mock<IAgent>();
            mockAgent.Setup(x => x.Send(It.IsAny<StringBuilder>())).Throws(new Exception("Exception"));
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<OliviaDbContext>()!, mockAgent.Object!, serviceProvider.GetService<ICalendarService>()!);

            // Act
            var result = await doctorAsistenceController.Post();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_NewMessage_Should_Return_Ok()
        {
            // Arrange
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);
            var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

            // Act
            var result = await doctorAsistenceController.Post(newMessageDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Post_NewMessage_Should_Throw_Exception_When_Chat_Not_Found()
        {
            // Arrange
            var mockChatService = new Mock<IChatService>();
            mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Chat>(null));
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, mockChatService.Object!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);
            var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

            // Act
            var result = await doctorAsistenceController.Post(newMessageDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_NewMessage_Should_Throw_Exception()
        {
            // Arrange
            var mockChatService = new Mock<IChatService>();
            mockChatService.Setup(x => x.Get(It.IsAny<Guid>())).Throws(new Exception("Exception"));
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, mockChatService.Object!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);
            var newMessageDto = new NewMessageDto() { ChatId = Guid.NewGuid(), Content = "Content" };

            // Act
            var result = await doctorAsistenceController.Post(newMessageDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_Resume_Should_Return_Ok()
        {
            // Arrange
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, serviceProvider.GetService<IChatService>()!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);
            var idDto = new IdDto() { Id = Guid.NewGuid() };

            // Act
            var result = await doctorAsistenceController.Post(idDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Post_Resume_Should_Throw_Exception()
        {
            // Arrange
            var mockChatService = new Mock<IChatService>();
            mockChatService.Setup(x => x.GetSummary(It.IsAny<Guid>())).Throws(new Exception("Exception"));
            var doctorAsistenceController = new DoctorsAsistenceController(serviceProvider.GetService<IConfiguration>()!, serviceProvider.GetService<ILogger<DoctorsAsistenceController>>()!, mockChatService.Object!, serviceProvider.GetService<OliviaDbContext>()!, serviceProvider.GetService<IAgent>()!, serviceProvider.GetService<ICalendarService>()!);
            var idDto = new IdDto() { Id = Guid.NewGuid() };

            // Act
            var result = await doctorAsistenceController.Post(idDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}