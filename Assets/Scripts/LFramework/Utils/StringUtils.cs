using UnityEngine;
using System;
using System.Text;

/// <summary>
/// 字符串工具类
/// </summary>
public static class StringUtils
{
    //一般是单独配一个单位表 读表获取
    static string[] unitList = new string[] { "", "K", "M" };

    /// <summary>
    /// 格式化货币
    /// </summary>
    /// digit:保留几位小数
    public static string FormatCurrency(long num, int digit = 1)
    {
        float tempNum = num;
        long v = 1000;//几位一个单位
        int unitIndex = 0;
        while (tempNum >= v)
        {
            unitIndex++;
            tempNum /= v;
        }
        string str = "";
        if (unitIndex >= unitList.Length)
        {
            Debug.LogError("超出单位表中的最大单位");
            str = num.ToString();
        }
        else
        {
            tempNum = MathUtils.Round(tempNum, digit);
            str = $"{tempNum}{unitList[unitIndex]}";
        }
        return str;
    }

    /// <summary>
    /// 千分位添加逗号
    /// </summary>
    public static string FormatComma(long num)
    {
        return num.ToString("N0");
    }

    /// <summary>
    /// 获取字符串长度(中文占两个 英文占一个)
    /// </summary>
    public static int GetLength(string str)
    {
        int len = 0;
        ASCIIEncoding ascii = new ASCIIEncoding();
        byte[] bytes = ascii.GetBytes(str);
        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == 63)
            {
                len += 2;
            }
            else
            {
                len += 1;
            }
        }
        return len;
    }
}
