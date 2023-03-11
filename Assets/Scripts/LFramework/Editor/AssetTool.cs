using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 资源工具
/// </summary>
public class AssetTool
{
    public const string AssetsRootPath = "Assets";//资源根目录

    public static string[] TexturePatterns = new string[]//贴图类型资源后缀
       {
            "*.png","*.jpg","*.jpeg","*.tga",
       };
    public static string[] ScriptPatterns = new string[]//脚本类型资源后缀
       {
            "*.cs",
       };
    public static string[] PrefabPatterns = new string[]//预制体类型资源后缀
       {
            "*.prefab",
       };

    /// <summary>
    /// 获取所有资源信息
    /// </summary>
    public static Dictionary<string, AssetInfo> AnalysisAssets(string dir, string[] searchPatterns = null)
    {
        Dictionary<string, AssetInfo> assetPath2AssetInfo = new Dictionary<string, AssetInfo>();
        try
        {
            if (string.IsNullOrEmpty(dir))
            {
                Debug.LogError("获取资源信息失败，目录为空!");
                return assetPath2AssetInfo;
            }
            if (searchPatterns == null)
            {
                searchPatterns = new[] { "*" };
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            for (int i = 0; i < searchPatterns.Length; i++)
            {
                FileInfo[] fis = dirInfo.GetFiles(searchPatterns[i], SearchOption.AllDirectories);
                for (int j = 0; j < fis.Length; j++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("正在分析资源", fis[j].Name, (j + 1) * 1f / fis.Length))
                    {
                        throw new Exception("用户取消分析资源");
                    }
                    string fileFullPath = IOUtils.ReconstructPath(fis[j].FullName);
                    AssetInfo assetInfo = new AssetInfo
                    {
                        assetName = Path.GetFileName(fileFullPath),
                        relPath = "Assets/" + fileFullPath.Substring(Application.dataPath.Length + 1),
                    };
                    assetPath2AssetInfo[assetInfo.relPath] = assetInfo;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"分析资源中断，{e}");
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        return assetPath2AssetInfo;
    }
}

/// <summary>
/// 资源信息
/// </summary>
public class AssetInfo
{
    public string assetName;//资源名（带后缀）
    public string relPath;//相对路径（Assets/xxx/xxx）
}