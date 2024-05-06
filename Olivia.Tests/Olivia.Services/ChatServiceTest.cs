using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.Services;

public class ChatServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDatabase> _mockDatabase;

    public ChatServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        _mockDatabase = new Mock<IDatabase>();
        var mockLoggerChatService = new Mock<ILogger<ChatService>>();

        serviceCollection.AddTransient(_ => _mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerChatService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_Should_Create_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);

        // Act
        var chatId = await chatService.Create();

        // Assert
        Assert.NotEqual(Guid.Empty, chatId);
    }

    [Fact]
    public async Task Get_Should_Return_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat() { Id = Guid.NewGuid() } as Chat);

        var chatId = await chatService.Create();

        // Act
        var chat = await chatService.Get(chatId);

        // Assert
        Assert.NotNull(chat);
    }

    [Fact]
    public async Task Get_Should_Throw_Exception_When_Chat_Not_Found()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => chatService.Get(Guid.NewGuid()));
    }


    [Fact]
    public async Task NewMessage_Should_Add_Message_To_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        var chatId = await chatService.Create();

        // Act
        await chatService.NewMessage(chatId, MessageTypeEnum.User, "Hello");

        // Assert
        _mockDatabase.Verify(x => x.Add(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task GetSummary_Should_Return_Chat_Summary()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat() { Id = Guid.NewGuid() } as Chat);
        _mockDatabase.Setup(x => x.Get<Message>(It.IsAny<Expression<Func<Message, bool>>>())).ReturnsAsync(new List<Message>() { new Message() { Content = "Hello" } });

        var chatId = await chatService.Create();

        // Act
        var chat = await chatService.GetSummary(chatId);

        // Assert
        Assert.NotNull(chat);
    }

    [Fact]
    public async Task Update_Should_Update_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat() { Id = Guid.NewGuid() } as Chat);
        var chatId = await chatService.Create();
        var chat = await chatService.Get(chatId);
        chat.Patient = new Patient() { Name = "John", LastName = "Doe", Email = "patient@email.com", Phone = 123456789, Reason = "Headache" };

        // Act
        await chatService.Update(chat);

        // Assert
        _mockDatabase.Verify(x => x.Update(It.IsAny<Chat>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Delete_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat() { Id = Guid.NewGuid() } as Chat);
        var chatId = await chatService.Create();

        // Act
        await chatService.Delete(chatId);

        // Assert
        _mockDatabase.Verify(x => x.Delete(It.IsAny<Chat>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Throw_Exception_When_Chat_Not_Found()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => chatService.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetMessages_Should_Return_Chat_Messages()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Get<Message>(It.IsAny<Expression<Func<Message, bool>>>())).ReturnsAsync(new List<Message>() { new Message() { Content = "Hello", Type = MessageTypeEnum.User }, new Message() { Content = "Hi", Type = MessageTypeEnum.Agent } });
        var chatId = await chatService.Create();
        // Act
        var messages = await chatService.GetMessages(chatId);

        // Assert
        Assert.NotNull(messages);
    }

    [Fact]
    public async Task AsociatePatient_Should_Asociate_Patient_To_Chat()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        _mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat() { Id = Guid.NewGuid() } as Chat);
        var chatId = await chatService.Create();
        var patientId = Guid.NewGuid();

        // Act
        await chatService.AsociatePatient(chatId, patientId);

        // Assert
        _mockDatabase.Verify(x => x.Update(It.IsAny<Chat>()), Times.Once);
    }

    [Fact]
    public async Task AsociatePatient_Should_Throw_Exception_When_Chat_Not_Found()
    {
        // Arrange
        var chatService = new ChatService(serviceProvider.GetService<IDatabase>()!, serviceProvider.GetService<ILogger<ChatService>>()!);
        var patientId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => chatService.AsociatePatient(Guid.NewGuid(), patientId));
    }
}
