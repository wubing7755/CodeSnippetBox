using System.Text;

namespace DocReader;

public static class LogHelper
{
    private static readonly object _lock = new();

    public static void Write(string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}";

        // 输出到控制台
        Console.WriteLine(message);

        // 写入到 log.txt（自动追加）
        lock (_lock)
        {
            File.AppendAllText(DocCs.LogFile, line + Environment.NewLine, Encoding.UTF8);
        }
    }
}