using CodeRunner.Models;
using CodeRunner.Utils;

namespace TestRunner.UtilsTest;

public class TempDataBinarySerializerTest
{
    [Fact]
    public void TestToBigEndianBinary()
    {
        // Arrange
        var filePath = "../../../Resources/test_data.xml";
        var stream = File.Open(filePath, FileMode.Open);
        var tempData = TempDataDeserialize.TryDeserialize<TempData>(stream);
        Assert.NotNull(tempData);

        // Act
        var res = TempDataBinarySerializer.ToBigEndianBinary(tempData);

        var binFilePath = "../../../Resources/test_data.bin";
        File.WriteAllBytes(binFilePath, res);

        // Assert
        Assert.NotNull(res);
    }
}
