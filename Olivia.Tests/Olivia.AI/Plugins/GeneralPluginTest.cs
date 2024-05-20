using Olivia.AI.Plugins;

namespace Olivia.Tests.Olivia.AI.Plugins;

public class GeneralPluginTest
{
    [Fact]
    public void GetTime_Should_Return_Current_Time()
    {
        // Act
        var result = GeneralPlugin.GetTime();

        // Assert
        Assert.NotNull(result);
    }
}
