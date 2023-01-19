using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetBundle打包配置
/// </summary>
public class ABBuildConfig : ScriptableObject
{
    private static ABBuildConfig _ins;//单例
    public static ABBuildConfig Ins
    {
        get
        {
            if (_ins == null)
            {
                Init();
            }
            return _ins;
        }
    }

    public List<ABBuildInfo> m_BuildList = new List<ABBuildInfo>();//打包列表

    /// <summary>
    /// 初始化
    /// </summary>
    private static void Init()
    {
        _ins = AssetDatabase.LoadAssetAtPath<ABBuildConfig>(BuildUtils.ABBuildConfigPath);
        if (_ins == null)
        {
            _ins = CreateInstance<ABBuildConfig>();
            AssetDatabase.CreateAsset(_ins, BuildUtils.ABBuildConfigPath);
            Save();
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    public static void Save()
    {
        EditorUtility.SetDirty(_ins);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

/// <summary>
/// AssetBundle打包项信息
/// </summary>
[Serializable]
public class ABBuildInfo
{
    public bool isEnable = true;//是否开启
    public string path;//路径
    public string filterStr = "*.prefab";//过滤符字符串（用;分隔）
}


