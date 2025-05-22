using System.Xml.Linq;

namespace CodeRunner.Utils;

public class XmlProcesser
{
    public static bool TravelAllNodesRecursive(XNode node, int depth = 0)
    {
        if (node == null)
            return true;

        // 根据节点类型处理
        switch (node)
        {
            case XElement element:
                // 打印元素名称（带缩进表示层级）
                Console.WriteLine($"{new string(' ', depth * 2)}Element: {element.Name}");

                // 打印属性
                foreach (var attr in element.Attributes())
                {
                    Console.WriteLine($"{new string(' ', (depth + 1) * 2)}Attribute: {attr.Name} = {attr.Value}");
                }

                // 递归处理子节点
                foreach (var child in element.Nodes())
                {
                    TravelAllNodesRecursive(child, depth + 1);
                }
                break;

            case XText text:
                Console.WriteLine($"{new string(' ', depth * 2)}Text: {text.Value.Trim()}");
                break;

            case XComment comment:
                Console.WriteLine($"{new string(' ', depth * 2)}Comment: {comment.Value}");
                break;

            // 可扩展其他节点类型（如处理指令、CDATA等）
            default:
                Console.WriteLine($"{new string(' ', depth * 2)}Unhandled Node: {node.NodeType}");
                break;
        }

        return true;
    }

}
