using CodeRunner.Utils;

namespace TestRunner.UtilsTest;

public class FindSoftwarePathTest
{
    [Theory]
    [InlineData("Notepad++")]
    public void TestFindPath(string softwareName)
    {
        var path = FindSoftwarePath.FindPath(softwareName);
        Assert.NotNull(path);
        Assert.NotEmpty(path);
    }
}
