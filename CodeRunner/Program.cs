using CodeRunner.Utils;

namespace CodeRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string folderPath, outputFilePath;

                // 如果有命令行参数，使用参数
                if (args.Length >= 2)
                {
                    folderPath = args[0];
                    outputFilePath = args[1];
                }
                else
                {
                    // 否则使用交互式输入
                    Console.WriteLine("文件夹文件列表导出工具");
                    Console.WriteLine("=====================\n");

                    Console.Write("请输入文件夹路径: ");
                    folderPath = Console.ReadLine();

                    Console.Write("请输入输出文件路径: ");
                    outputFilePath = Console.ReadLine();
                }
                
                ReadFolderFile.ProcessFolder(folderPath, outputFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("\n用法: FolderFileList.exe <文件夹路径> <输出文件路径>");
                Console.WriteLine("示例: FolderFileList.exe C:\\MyFolder D:\\output\\list.txt");
            }
        }
    }
}
