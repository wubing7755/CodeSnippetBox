namespace DocReader;

public static class Program
{
    static void Main(string[] args)
    {
        LogHelper.Write(DocCs.OutputTitle);
        Console.Write("请输入内容文件夹路径：");
        string? folderPath = Console.ReadLine()?.Trim('"');

        Console.Write("请输入要处理的 docx 文件路径：");
        string? docxPath = Console.ReadLine()?.Trim('"');

        if (!Directory.Exists(folderPath))
        {
            LogHelper.Write("❌ 文件夹不存在。");
            return;
        }

        if (!File.Exists(docxPath))
        {
            LogHelper.Write("❌ Docx 文件不存在。");
            return;
        }

        string outputPath = Path.Combine(
            Path.GetDirectoryName(docxPath)!,
            Path.GetFileNameWithoutExtension(docxPath) + DocCs.FileSuffix
        );

        File.Copy(docxPath, outputPath, overwrite: true);

        DocHelper.BuildStructureAndInsert(folderPath, outputPath);
        DocHelper.ProcessDocument(folderPath, outputPath);
    }
}
    