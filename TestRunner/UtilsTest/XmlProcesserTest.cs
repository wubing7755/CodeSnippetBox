using CodeRunner.Utils;
using System.Xml.Linq;

namespace TestRunner.UtilsTest;

public class XmlProcesserTest
{
    [Fact]
    public void TestTravelAllNodesRecursive()
    {
        string xmlContent = @"
        <root>
            <child1 attr1='value1' attr2='value2'>
                <subchild>Text content</subchild>
                <!-- This is a comment -->
            </child1>
            <child2>Another text</child2>
        </root>";
        var xDocument = XDocument.Parse(xmlContent);
        var result = XmlProcesser.TravelAllNodesRecursive(xDocument.Root);

        Assert.True(result);
    }
}
