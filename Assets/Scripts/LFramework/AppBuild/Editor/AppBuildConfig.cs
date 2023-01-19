using UnityEditor;
using UnityEngine;

/// <summary>
/// App打包配置
/// </summary>
public class AppBuildConfig : ScriptableObject
{
    private static AppBuildConfig _ins;//单例
    public static AppBuildConfig Ins
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

    public string m_ProductName;//产品名称
    public string m_CompanyName;//公司名称
    public string m_Version = "0.0.0";//版本号
    public int m_BuildNum = 0;//构建号
    public string m_DefineSymbols;//编译宏
    public bool m_IsDevelopment;//是否为Debug版本
    public bool m_BuildAAB;//是否构建AAB

    /// <summary>
    /// 初始化
    /// </summary>
    private static void Init()
    {
        _ins = AssetDatabase.LoadAssetAtPath<AppBuildConfig>(BuildUtils.AppBuildConfigPath);
        if (_ins == null)
        {
            _ins = CreateInstance<AppBuildConfig>();
            AssetDatabase.CreateAsset(_ins, BuildUtils.AppBuildConfigPath);
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
