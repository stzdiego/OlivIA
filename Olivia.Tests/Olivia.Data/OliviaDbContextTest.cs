using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Olivia.Data;

namespace Olivia.Tests.Olivia.Data;

public class OliviaDbContextTest
{
    private ServiceProvider serviceProvider;

    public OliviaDbContextTest()
    {
        var serviceCollection = new ServiceCollection();

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Constructor_Should_Create_Instance()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OliviaDbContext>()
            .UseInMemoryDatabase("OliviaDbContextTest")
            .Options;

        // Act
        var oliviaDbContext = new OliviaDbContext(options);

        // Assert
        Assert.NotNull(oliviaDbContext);
    }
}
