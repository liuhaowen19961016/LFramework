using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 打包工具类
/// </summary>
public static class BuildUtils
{
    public const string AppBuildConfigPath = "Assets/Scripts/AppBuild/Editor/AppBuildConfig.asset";//App打包配置的路径
    public static string AppBuildRootPath = Application.dataPath + "/../Build";//App打包后的文件根目录

    public const string ABBuildConfigPath = "Assets/Scripts/AssetBundle/Editor/ABBuildConfig.asset";//AssetBundle打包配置的路径
    private const string ABRootName = "AssetBundles";
    public static string ABBuildRootPath = Application.dataPath + "/../" + ABRootName;//AssetBundle打包后的文件根目录
    public const string FileName_ABAssetsConfigXML = "ABAssetsConfig.xml";//AssetBundle资源配置的文件名称
    public static string ABFilePath//AssetBundle文件目录
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(ABBuildRootPath, GetPlatformName());
#else
            return Path.Combine(Path.Combine(Application.streamingAssetsPath, ABRootName), GetPlatformName());
#endif
        }
    }
    public const string ABSuffix = ".ab";//ab包后缀
    private static string[] ABBuildIgnoreFileSuffix = //AssetBundle打包忽略的文件后缀
    {
        ".cs",
        ".meta",
        ".svn",
    };

    private static string PlatformName//平台名称
    {
        get
        {
#if UNITY_ANDROID
            return "ANDROID";
#elif UNITY_IOS
            return "IOS";
#elif UNITY_STANDALONE_WIN
            return "WINDOWS";
#elif UNITY_STANDALONE_OSX
            return "OSX";
#else
            return "UNKNOW"
#endif
        }
    }

    /// <summary>
    /// 是否为打ab包忽略的文件
    /// </summary>
    public static bool IsIgnoreABFile(string path)
    {
        string suffix = Path.GetExtension(path);
        foreach (var temp in ABBuildIgnoreFileSuffix)
        {
            if (temp == suffix)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取AssetBundle打包的输出路径
    /// </summary>
    public static string GetABBuildOutputPath()
    {
        return Path.Combine(ABBuildRootPath, GetPlatformName());
    }

    /// <summary>
    /// 获取平台名称
    /// </summary>
    public static string GetPlatformName()
    {
        return PlatformName;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 获取App打包后的文件扩展名
    /// </summary>
    public static string GetAppBuildFileExtension(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";

            case BuildTarget.StandaloneOSX:
                return ".app";

            case BuildTarget.Android:
                return ".apk"; //TODO ：".aab" or ".apk";

            case BuildTarget.iOS:
                return string.Empty;
        }
        Debug.LogError($"不认识的BuildTarget：{target}");
        return string.Empty;
    }
#endif
}

/// <summary>
/// AssetBundle资源
/// </summary>
public class ABAsset
{
    public string assetPath;//资源路径
    public string abName;//ab包名
    public string assetName;//资源名
    public List<ABAsset> depABAssets = new List<ABAsset>();//依赖的AssetBundle资源列表

    /// <summary>
    /// 添加依赖AssetBundle资源
    /// </summary>
    public void AddDepABAsset(ABAsset abAsset)
    {
        if (!depABAssets.Contains(abAsset))
        {
            depABAssets.Add(abAsset);
        }
    }
}
