using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.EntityFrameworkCore;
using Olivia.Services;
using Olivia.Shared.Entities;
namespace Olivia.Tests.Olivia.Services;

public class DatabaseServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<DbContext> _mockDbContext;

    public DatabaseServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        _mockDbContext = new Mock<DbContext>();

        serviceCollection.AddTransient(_ => _mockDbContext.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Exist_Should_Return_False_When_Entity_Does_Not_Exist()
    {
        // Arrange
        var chats = new List<Chat>();
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Exist<Chat>(x => x.Id == Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Exist_Should_Return_True_When_Entity_Exists()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Exist<Chat>(x => x.Id == chat.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Add_Should_Add_Entity()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat };

        var mockSet = new Mock<DbSet<Chat>>();
        mockSet.Setup(x => x.AddAsync(It.IsAny<Chat>(), default)).Callback<Chat, CancellationToken>((entity, _) => chats.Add(entity));
        _mockDbContext.Setup(x => x.Set<Chat>()).Returns(mockSet.Object);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        await databaseService.Add(chat);

        // Assert
        Assert.Contains(chat, chats);
    }

    [Fact]
    public async Task Find_Should_Return_Entity_When_Entity_Exists()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Find<Chat>(x => x.Id == chat.Id);

        // Assert
        Assert.Equal(chat, result);
    }

    [Fact]
    public async Task Find_Should_Return_Null_When_Entity_Does_Not_Exist()
    {
        // Arrange
        var chats = new List<Chat>();
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Find<Chat>(x => x.Id == Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_Should_Return_All_Entities()
    {
        // Arrange
        var chat1 = new Chat { Id = Guid.NewGuid() };
        var chat2 = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat1, chat2 };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Get<Chat>();

        // Assert
        Assert.Equal(chats, result);
    }

    [Fact]
    public async Task Get_Should_Return_Empty_List_When_No_Entities()
    {
        // Arrange
        var chats = new List<Chat>();
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Get<Chat>();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Get_Should_Return_Entities_Based_On_Condition()
    {
        // Arrange
        var chat1 = new Chat { Id = Guid.NewGuid() };
        var chat2 = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat1, chat2 };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Get<Chat>(x => x.Id == chat1.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(chat1, result.First());
    }

    [Fact]
    public async Task Get_Should_Return_Empty_List_When_No_Entities_Based_On_Condition()
    {
        // Arrange
        var chat1 = new Chat { Id = Guid.NewGuid() };
        var chat2 = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat1, chat2 };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Get<Chat>(x => x.Id == Guid.NewGuid());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Update_Should_Update_Entity()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat };
        _mockDbContext.Setup(x => x.Set<Chat>()).ReturnsDbSet(chats);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        var result = await databaseService.Update(chat);

        // Assert
        Assert.Equal(chat, result);
    }

    [Fact]
    public async Task Delete_Should_Delete_Entity()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var chats = new List<Chat> { chat };

        var mockSet = new Mock<DbSet<Chat>>();
        mockSet.Setup(x => x.Remove(It.IsAny<Chat>())).Callback<Chat>((entity) => chats.Remove(entity));
        _mockDbContext.Setup(x => x.Set<Chat>()).Returns(mockSet.Object);

        var databaseService = new DatabaseService(_mockDbContext.Object);

        // Act
        await databaseService.Delete(chat);

        // Assert
        Assert.DoesNotContain(chat, chats);
    }
}
