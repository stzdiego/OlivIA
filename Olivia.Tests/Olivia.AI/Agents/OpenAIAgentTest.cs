using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Olivia.AI.Agents;
using Olivia.AI.Plugins;
using Olivia.Services;
using Microsoft.Extensions.Logging;
using Olivia.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Olivia.Data;
using System.Text;
using Olivia.Shared.Entities;
using Castle.Core.Configuration;
using Olivia.Shared.Settings;
using System.IO.Compression;

namespace Olivia.Tests.Olivia.AI.Agents
{
    public class OpenAIAgentTest
    {
        private ServiceProvider serviceProvider;
        private Mock<IAgentSettings> mockIAgentSettings;

        public OpenAIAgentTest()
        {
            var serviceCollection = new ServiceCollection();

            var mockDatabase = new Mock<IDatabase>();
            var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
            var mockService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
            var mockContext = new Mock<OliviaDbContext>(new DbContextOptions<OliviaDbContext>());
            mockIAgentSettings = new Mock<IAgentSettings>();

            // Registrar tus dependencias y sus mocks
            serviceCollection.AddTransient(_ => mockDatabase.Object);
            serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);
            serviceCollection.AddTransient(_ => mockService.Object);
            serviceCollection.AddTransient(_ => mockContext.Object);
            serviceCollection.AddTransient(_ => mockIAgentSettings.Object);

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void AddPlugin_Should_Add_Plugin_Of_Type_T()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddPlugin<DoctorsManagerPlugin>();

            // Assert
            Assert.Single(agent.Plugins);
        }

        [Fact]
        public void AddPlugin_Should_Catch_Exception_When_Not_Plugin()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            var ex = Assert.Throws<Exception>(() => agent.AddPlugin<DoctorService>());

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<Exception>(ex);
            Assert.Equal("T is not a plugin", ex.Message);
        }

        [Fact]
        public void AddPlugin_Should_Not_Add_Plugin_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddPlugin<DoctorsManagerPlugin>();
            agent.AddPlugin<DoctorsManagerPlugin>();

            // Assert
            Assert.Single(agent.Plugins);
        }

        [Fact]
        public void AddSingleton_Should_Add_Singleton_Of_Type_T()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddSingleton<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddSingleton_Should_Not_Add_Singleton_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddSingleton<DoctorService>();
            agent.AddSingleton<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Add_Scoped_Of_Type_T()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddScoped<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Not_Add_Scoped_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddScoped<DoctorService>();
            agent.AddScoped<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Add_Scoped_Of_Type_T_And_Interface()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddScoped<IDatabase, DatabaseService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Add_Scoped_Of_Type_T_And_Interface_If_Not_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddScoped<IDatabase, DatabaseService>();
            agent.AddScoped<IDatabase, DatabaseService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddContext_Should_Add_Context()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddContext_Should_Not_Add_Context_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void Send_Should_Send_Message_To_OpenAI()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Hello, how are you?", Type = Shared.Enums.MessageTypeEnum.Prompt},
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Hello, how are you?", Type = Shared.Enums.MessageTypeEnum.Agent},
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Hello, how are you?", Type = Shared.Enums.MessageTypeEnum.User},
            };

            // Act
            var response = agent.Send(messages);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void AddPlugin_Should_Add_Plugin_With_Interface()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddPlugin<IPlugin, DoctorsManagerPlugin>();

            // Assert
            Assert.Single(agent.Plugins);
        }

        [Fact]
        public void AddPlugin_Should_Not_Add_Plugin_With_Interface_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddPlugin<IPlugin, DoctorsManagerPlugin>();
            agent.AddPlugin<IPlugin, DoctorsManagerPlugin>();

            // Assert
            Assert.Single(agent.Plugins);
        }

        [Fact]
        public void AddSingleton_Should_Add_Singleton_Of_Type_T_With_Interface()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddSingleton<IMailSettings>(new MailSettings());
            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddSingleton_Should_Not_Add_Singleton_With_Interface_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.AddSingleton<IMailSettings>(new MailSettings());
            agent.AddSingleton<IMailSettings>(new MailSettings());

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void Initialize_Should_Initialize_OpenAI()
        {
            // Arrange
            mockIAgentSettings.Setup(x => x.Model).Returns("gpt-3.5-turbo");
            mockIAgentSettings.Setup(x => x.Key).Returns("key");
            var agent = new OpenAIAgent(this.mockIAgentSettings.Object);

            // Act
            agent.Initialize();

            // Assert
            Assert.True(true);
        }
    }
}