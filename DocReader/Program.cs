namespace DocReader;

public static class Program
{
    public static void Main(string[] args)
    {
        string? inputFolder;
        string? outputFolder;

        if (args.Length >= 2)
        {
            // 从命令行参数获取
            inputFolder = args[0];
            outputFolder = args[1];
            LogHelper.Write($"从命令行参数获取:");
            LogHelper.Write($"输入文件夹: {inputFolder}");
            LogHelper.Write($"输出路径: {outputFolder}");
        }
        else
        {
            // 从控制台输入获取
            Console.Write("请输入\"输入文件夹\"路径：");
            inputFolder = Console.ReadLine()?.Trim('"');

            Console.Write("请输入\"输出文件夹\"路径：");
            outputFolder = Console.ReadLine()?.Trim('"');
        }

        if (!Directory.Exists(inputFolder) || !Directory.Exists(outputFolder))
        {
            LogHelper.Write("False: 文件夹不存在。");
            return;
        }

        try
        {
            LogHelper.Write($"输入文件夹: {inputFolder}");
            LogHelper.Write($"输出文件夹: {outputFolder}");
            
            var filePath = Path.Combine(outputFolder, "output.docx");
            DocHelper.BuildDocStructure(inputFolder, filePath);
            DocHelper.ProcessDocument(inputFolder, filePath);
            
            LogHelper.Write($"文档生成完成: {outputFolder}");
        }
        catch (Exception ex)
        {
            LogHelper.Write($"生成文档时发生错误: {ex.Message}");
            LogHelper.Write($"详细错误: {ex}");
        }
    }
}
    