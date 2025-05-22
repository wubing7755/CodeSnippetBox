using Microsoft.Win32;

namespace CodeRunner.Utils;

public static class FindSoftwarePath
{
    /// <summary>
    /// 查询软件的安装路径：
    /// ① 注册表
    /// ② PATH环境变量
    /// </summary>
    /// <param name="softwareName"></param>
    /// <returns></returns>
    public static string FindPath(string softwareName)
    {
        var installPath = string.Empty;

        // 常见注册表路径
        string[] registryPaths =
        {
            $@"SOFTWARE\{softwareName}",
                $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{softwareName}",
                $@"SOFTWARE\WOW6432Node\{softwareName}", // 32 位程序在 64 位系统上的注册表
                $@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{softwareName}"
        };

        // 常见存储安装路径的键名
        string[] possibleValueNames = { "InstallLocation", "InstallDir", "UninstallString", "DisplayIcon" };

        using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
            foreach (var path in registryPaths)
            {
                using (RegistryKey key = baseKey.OpenSubKey(path))
                {
                    if(key != null)
                    {
                        foreach (var valueName in possibleValueNames)
                        {
                            installPath = key.GetValue(valueName)?.ToString();

                            if (!string.IsNullOrEmpty(installPath))
                            {
                                installPath = installPath.Trim('"');
                                return installPath;
                            }
                        }
                    }
                }
            }
        }

        // 如果没有找到安装路径，则在 PATH 环境变量中查找
        if (string.IsNullOrEmpty(installPath))
        {
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                string[] paths = pathEnv.Split(';');
                foreach (var path in paths)
                {
                    string exePath = Path.Combine(path, $"{softwareName}.exe");
                    if (File.Exists(exePath))
                    {
                        return exePath;
                    }
                }
            }
        }

        return string.Empty;
    }
}
