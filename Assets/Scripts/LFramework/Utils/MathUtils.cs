using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数学相关工具类
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// 四舍五入
    /// </summary>
    /// digits:保留几位小数
    public static float Round(float value, int digits = 1)
    {
        if (value == 0)
        {
            return 0;
        }
        float multiple = Mathf.Pow(10, digits);
        float tempValue = value > 0 ? value * multiple + 0.5f : value * multiple - 0.5f;
        tempValue = Mathf.FloorToInt(tempValue);
        return tempValue / multiple;
    }

    /// <summary>
    /// 随机一个元素（不会随机到排除列表中的元素）
    /// </summary>
    public static T GetRandomElement<T>(List<T> list, List<T> excludeList = null)
    {
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        while (excludeList != null && excludeList.Contains(list[randomIndex]))
        {
            randomIndex = UnityEngine.Random.Range(0, list.Count);
        }
        return list[randomIndex];
    }

    /// <summary>
    /// 打乱列表中元素的顺序
    /// </summary>
    public static void OutOfOrder<T>(List<T> list)
    {
        int randomIndex;
        T temp;
        for (int i = 0; i < list.Count; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, list.Count);
            if (randomIndex != i)
            {
                temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
