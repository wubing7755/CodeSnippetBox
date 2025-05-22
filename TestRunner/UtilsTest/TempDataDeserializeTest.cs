using CodeRunner.Models;
using CodeRunner.Utils;

namespace TestRunner.UtilsTest;

public class TempDataDeserializeTest
{
    [Fact]
    public void TestTryDeserializeTempData()
    {
        // Arrange
        var filePath = "../../../Resources/test_data.xml";
        var stream = File.Open(filePath, FileMode.Open);

        // Act
        var tempData = TempDataDeserialize.TryDeserialize<TempData>(stream);

        // Assert
        Assert.NotNull(tempData);
    }
}
