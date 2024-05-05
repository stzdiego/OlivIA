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

namespace Olivia.Tests.Olivia.AI.Agents
{
    public class OpenAIAgentTest
    {
        private ServiceProvider serviceProvider;

        public OpenAIAgentTest()
        {
            var serviceCollection = new ServiceCollection();

            var mockDatabase = new Mock<IDatabase>();
            var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
            var mockService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
            var mockContext = new Mock<OliviaDbContext>(new DbContextOptions<OliviaDbContext>());

            // Registrar tus dependencias y sus mocks
            serviceCollection.AddTransient(_ => mockDatabase.Object);
            serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);
            serviceCollection.AddTransient(_ => mockService.Object);
            serviceCollection.AddTransient(_ => mockContext.Object);

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void AddPlugin_Should_Add_Plugin_Of_Type_T()
        {
            // Arrange
            var agent = new OpenAIAgent();

            // Act
            agent.AddPlugin<DoctorsManagerPlugin>();

            // Assert
            Assert.Single(agent.Plugins);
        }

        [Fact]
        public void AddPlugin_Should_Catch_Exception_When_Not_Plugin()
        {
            // Arrange
            var agent = new OpenAIAgent();

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
            var agent = new OpenAIAgent();

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
            var agent = new OpenAIAgent();

            // Act
            agent.AddSingleton<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddSingleton_Should_Not_Add_Singleton_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent();

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
            var agent = new OpenAIAgent();

            // Act
            agent.AddScoped<DoctorService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Not_Add_Scoped_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent();

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
            var agent = new OpenAIAgent();

            // Act
            agent.AddScoped<IDatabase, DatabaseService>();

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddScoped_Should_Add_Scoped_Of_Type_T_And_Interface_If_Not_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent();

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
            var agent = new OpenAIAgent();

            // Act
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void AddContext_Should_Not_Add_Context_If_Already_Exists()
        {
            // Arrange
            var agent = new OpenAIAgent();

            // Act
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);
            agent.AddDbContext<DbContext, OliviaDbContext>(serviceProvider.GetService<OliviaDbContext>()!);

            // Assert
            Assert.Single(agent.Services);
        }

        [Fact]
        public void Initialize_Should_Initialize_OpenAI_Agent()
        {
            // Arrange
            var agent = new OpenAIAgent();

            // Act
            agent.Initialize("gpt-3.5-turbo", "xxxxxxxxxxx", 500);

            // Assert

        }

        [Fact]
        public void Send_Should_Send_Message_To_OpenAI()
        {
            // Arrange
            var agent = new OpenAIAgent();
            StringBuilder message = new StringBuilder();

            // Act
            message.Append("Hello, how are you?");
            var response = agent.Send(message);

            // Assert
            Assert.NotNull(response);
        }
    }
}