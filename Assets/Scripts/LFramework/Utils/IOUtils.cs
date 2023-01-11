using UnityEngine;
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
    /// 打开文件夹
    /// </summary>
    public static void OpenFolder(string path)
    {
        Process process = new Process();
        process.StartInfo.FileName = path;
        process.Start();
    }

    /// <summary>
    /// 拷贝单个文件
    /// </summary>
    public static void CopySingleFile(string srcFilePath, string destFilePath)
    {
        if (string.IsNullOrEmpty(Path.GetExtension(srcFilePath)))
        {
            Debug.LogError($"必须为文件路径，srcFilePath：{srcFilePath}");
            return;
        }
        if (string.IsNullOrEmpty(Path.GetExtension(destFilePath)))
        {
            Debug.LogError($"必须为文件路径，destFilePath：{destFilePath}");
            return;
        }
        string destDirPath = destFilePath.Substring(0, destFilePath.IndexOf(Path.GetFileName(destFilePath)) - 1);
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        File.Copy(srcFilePath, destFilePath, true);
    }

    /// <summary>
    /// 拷贝文件
    /// </summary>
    public static void CopyFile(string srcDirPath, string destDirPath)
    {
        if (!Directory.Exists(srcDirPath))
        {
            Debug.LogError($"不存在此路径，srcDirPath：{srcDirPath}");
            return;
        }
        destDirPath = Path.Combine(destDirPath, Path.GetFileName(srcDirPath));
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        string[] filePaths = Directory.GetFileSystemEntries(srcDirPath);
        foreach (var temp in filePaths)
        {
            if (File.Exists(temp))
            {
                string destFilePath = Path.Combine(destDirPath, Path.GetFileName(temp));
                File.Copy(temp, destFilePath, true); ;
            }
            else
            {
                CopyFile(temp, destDirPath);
            }
        }
    }
}
