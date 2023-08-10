using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏工具类
/// </summary>
public static class GameUtils
{
    private static IAssetService m_AssetService;//资源管理
    public static IAssetService AssetService
    {
        get
        {
            if (m_AssetService == null)
            {
                m_AssetService = ABUtils.GetAssetService();
            }
            return m_AssetService;
        }
    }

    private static ConfigService m_ConfigService;//配置管理
    public static ConfigService ConfigService
    {
        get
        {
            if (m_ConfigService == null)
            {
                m_ConfigService = ConfigService.Ins;
            }
            return m_ConfigService;
        }
    }

    /// <summary>
    /// 二维列表转二维数组
    /// </summary>
    public static T[,] List2Array<T>(List<List<T>> list)
    {
        if (list == null || list.Count <= 0)
        {
            return null;
        }
        int row = list.Count;
        int col = list[0].Count;
        T[,] array = new T[row, col];
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                array[i, j] = list[i][j];
            }
        }
        return array;
    }
}
