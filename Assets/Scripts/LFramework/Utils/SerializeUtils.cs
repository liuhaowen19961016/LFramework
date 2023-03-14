using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;
using System.Text;
using System.IO.Pipes;

/// <summary>
/// 序列化工具类
/// </summary>
public static class SerializeUtils
{
    #region 二进制

    /// <summary>
    /// 序列化二进制
    /// </summary>
    public static bool BinarySerialize(string outputPath, object obj)
    {
        if (!IOUtils.IsFile(outputPath))
        {
            Debug.LogError($"序列化二进制失败，输出路径有误，filePath：{outputPath}");
            return false;
        }
        try
        {
            string dirPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, obj);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"序列化二进制失败，{e}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 反序列化二进制
    /// </summary>
    public static T BinaryDeserialize<T>(string binaryFilePath)
    {
        if (!File.Exists(binaryFilePath))
        {
            Debug.LogError($"反序列化二进制失败，找不到二进制文件：{binaryFilePath}");
            return default;
        }
        T obj = default;
        try
        {
            using (FileStream fs = File.OpenRead(binaryFilePath))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                obj = (T)binaryFormatter.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"反序列化二进制失败，{e}");
            return obj;
        }
        return obj;
    }

    #endregion 二进制

    #region XML

    /// <summary>
    /// 序列化XML
    /// </summary>
    public static bool XmlSerialize(string outputPath, object obj)
    {
        if (!IOUtils.IsFile(outputPath))
        {
            Debug.LogError($"序列化XML失败，输出路径有误，filePath：{outputPath}");
            return false;
        }
        try
        {
            string dirPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(sw, obj);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"序列化XML失败，{e}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 反序列化XML
    /// </summary>
    public static T XmlDeserialize<T>(string xmlFilePath)
    {
        if (!File.Exists(xmlFilePath))
        {
            Debug.LogError($"反序列化XML失败，找不到XML文件：{xmlFilePath}");
            return default;
        }
        T t = default;
        try
        {
            using (FileStream fs = File.OpenRead(xmlFilePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                t = (T)xmlSerializer.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"反序列化XML失败，{e}");
            return t;
        }
        return t;
    }

    #endregion XML
}
