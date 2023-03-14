using System;
using UnityEngine;

/// <summary>
/// 时间工具类
/// </summary>
public static class TimeUtils
{
    /// <summary>
    /// 格式化日期
    /// </summary>
    public static string FormatDateTime(DateTime dt, string foramt = "yyyy/MM/dd")
    {
        return dt.ToString(foramt);
    }

    /// <summary>
    /// 格式化时间(只显示最大单位的)
    /// </summary>
    public static string FormatCountdown1(int sec)
    {
        TimeSpan ts = new TimeSpan(0, 0, sec);
        if (ts.Days > 0)
        {
            return $"{ts.Days}D";
        }
        else if (ts.Hours > 0)
        {
            return $"{ts.Hours}H";
        }
        else if (ts.Minutes > 0)
        {
            return $"{ts.Minutes}Min";
        }
        else
        {
            return $"{ts.Seconds}S";
        }
    }

    /// <summary>
    /// 格式化倒计时（5H:22Min:26S）
    /// </summary>
    public static string FormatCountdown2(int sec)
    {
        TimeSpan ts = new TimeSpan(0, 0, sec);
        if (ts.Days > 0)
        {
            return $"{ts.Days}D:{ts.Hours}H:{ts.Minutes}Min";
        }
        else
        {
            return $"{ts.Hours}H:{ts.Minutes}Min:{ts.Seconds}S";
        }
    }
}
