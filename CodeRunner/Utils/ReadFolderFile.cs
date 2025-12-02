using System.Text;

namespace CodeRunner.Utils;

/// <summary>
/// 读取文件夹内所有文件的文件名，并输出为 .txt 文件
/// </summary>
public static class ReadFolderFile
{
    public static void ProcessFolder(string folderPath, string outputFilePath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("文件夹路径不能为空");

        if (string.IsNullOrWhiteSpace(outputFilePath))
            throw new ArgumentException("输出文件路径不能为空");

        // 检查文件夹是否存在
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"文件夹不存在: {folderPath}");

        // 确保输出目录存在
        string? outputDir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // 确保文件扩展名为 .txt
        if (!outputFilePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            outputFilePath += ".txt";
        }

        // 获取所有文件（包括子目录）
        string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        
        // 写入文件
        using (StreamWriter writer = new StreamWriter(outputFilePath, false, Encoding.UTF8))
        {
            // 只写入文件名和扩展名，按读取顺序
            foreach (string filePath in allFiles)
            {
                string fileName = Path.GetFileName(filePath); // 获取文件名.扩展名
                writer.WriteLine(fileName);
            }
        }

        Console.WriteLine($"操作完成！共找到 {allFiles.Length} 个文件。");
        Console.WriteLine($"文件名列表已保存到: {outputFilePath}");
    }
}