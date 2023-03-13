using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 文件操作工具类
/// </summary>
public static class IOUtils
{
    /// <summary>
    /// 是否为文件
    /// </summary>
    public static bool IsFile(string path)
    {
        return !string.IsNullOrEmpty(Path.GetExtension(path));
    }

    /// <summary>
    /// 是否为文件夹
    /// </summary>
    public static bool IsFolder(string path)
    {
        return string.IsNullOrEmpty(Path.GetExtension(path));
    }

    /// <summary>
    /// 重新整理路径
    /// </summary>
    public static string ReconstructPath(string path)
    {
        path = path.Replace('\\', '/').Replace("//", "/");
        return path;
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    public static void OpenFolder(string path)
    {
        Process process = new Process();
        process.StartInfo.FileName = path;
        process.Start();
    }

    /// <summary>
    /// 拷贝文件
    /// </summary>
    /// destPath：目标文件路径或目标文件夹路径
    public static bool CopyFile(string srcFilePath, string destPath, bool destroySrcFile = false, bool overweite = true)
    {
        if (!IsFile(srcFilePath))
        {
            Debug.LogError($"原始文件路径有误，srcFilePath：{srcFilePath}");
            return false;
        }
        if (!File.Exists(srcFilePath))
        {
            Debug.LogError($"原始文件不存在，srcFilePath：{srcFilePath}");
            return false;
        }
        string destFilePath = "";
        string destDirPath = "";
        if (IsFile(destPath))
        {
            destFilePath = destPath;
            destDirPath = Path.GetDirectoryName(destPath);
        }
        else
        {
            destFilePath = Path.Combine(destPath, Path.GetFileName(srcFilePath));
            destDirPath = destPath;
        }
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        File.Copy(srcFilePath, destFilePath, overweite);
        if (destroySrcFile)
        {
            File.Delete(srcFilePath);
        }
        return true;
    }

    /// <summary>
    /// 拷贝文件夹
    /// </summary>
    public static bool CopyFolder(string srcDirPath, string destDirPath, bool containRootDir = true, bool overweite = true)
    {
        if (!IsFolder(srcDirPath))
        {
            Debug.LogError($"原始路径有误，不是文件夹路径，srcDirPath：{srcDirPath}");
            return false;
        }
        if (!IsFolder(destDirPath))
        {
            Debug.LogError($"目标路径有误，不是文件夹路径，destDirPath：{destDirPath}");
            return false;
        }
        if (!Directory.Exists(srcDirPath))
        {
            Debug.LogError($"原始路径不存在，srcDirPath：{srcDirPath}");
            return false;
        }
        destDirPath = containRootDir ? Path.Combine(destDirPath, Path.GetFileName(srcDirPath)) : destDirPath;
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        string[] filePaths = Directory.GetFileSystemEntries(srcDirPath);
        foreach (var temp in filePaths)
        {
            if (IsFile(temp))
            {
                string destFilePath = Path.Combine(destDirPath, Path.GetFileName(temp));
                File.Copy(temp, destFilePath, overweite);
            }
            else
            {
                CopyFolder(temp, destDirPath);
            }
        }
        return true;
    }
}