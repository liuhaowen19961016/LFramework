using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// AssetBundle工具
/// </summary>
public static class ABUtils
{
    public static bool IsPackMode//是否为打包模式（打包模式下通过AssetBundle加载，否则直接编辑器下通过AssetDatabase来加载,方便快速迭代）
    {
        get
        {
            bool isPackMode = true;
#if UNITY_EDITOR
            isPackMode = EditorPrefs.GetBool("IsPackMode", false);
#else
            isPackMode = true;
#endif
            return isPackMode;
        }
        set
        {
            EditorPrefs.SetBool("IsPackMode", value);
        }
    }

    /// <summary>
    /// 获取资源管理接口
    /// </summary>
    public static IAssetService GetAssetService()
    {
        IAssetService assetService = null;
        if (IsPackMode)
        {
            assetService = ABAssetService.Ins;
        }
        else
        {
            assetService = AssetDatabaseService.Ins;
        }
        return assetService;
    }

    #region 编辑器扩展
#if UNITY_EDITOR

    [UnityEditor.MenuItem("打包工具/开启打包模式", priority = 100)]
    public static void SetPackMode()
    {
        IsPackMode = !IsPackMode;
    }

    [UnityEditor.MenuItem("打包工具/开启打包模式", priority = 100, validate = true)]
    public static bool SetPackModeValidate()
    {
        UnityEditor.Menu.SetChecked("打包工具/开启打包模式", IsPackMode);
        return true;
    }

#endif
    #endregion 编辑器扩展
}