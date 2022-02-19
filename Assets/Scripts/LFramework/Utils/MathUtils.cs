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
        float multiple = Mathf.Pow(10, digits);
        float tempValue = value * multiple + 0.5f;
        tempValue = Mathf.FloorToInt(tempValue);
        return tempValue / multiple;
    }
}