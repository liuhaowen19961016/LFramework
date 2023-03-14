using System;
using UnityEngine;

/// <summary>
/// 时间工具类
/// </summary>
public static class TimeUtils
{
    public const int SecMinute = 60;//一分钟的秒数
    public const int SecHour = 3600;//一小时的秒数
    public const int SecDay = 86400;//一天的秒数

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
        TimeSpan ts = new TimeSpan(0, 0, Mathf.Max(0, sec));
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
        TimeSpan ts = new TimeSpan(0, 0, Mathf.Max(0, sec));
        if (ts.Days > 0)
        {
            return $"{ts.Days}D:{ts.Hours}H:{ts.Minutes}Min";
        }
        else
        {
            return $"{ts.Hours}H:{ts.Minutes}Min:{ts.Seconds}S";
        }
    }

    /// <summary>
    /// 获取间隔时间
    /// </summary>
    public static int GetDuration(DateTime dt1, DateTime dt2)
    {
        TimeSpan ts = dt2 - dt1;
        return Mathf.Max(0, (int)ts.TotalSeconds);
    }

    #region 距离某一时间的秒数

    /// <summary>
    /// 获取距离某一时刻的秒数（例：周三下午2点—>周三下午4点）
    /// </summary>
    public static int GetDistTargetHourSec(int targetHour)
    {
        if (targetHour == 0)
        {
            targetHour = 24;
        }
        DateTime nowDT = TimeMgr.Ins.GetServerDateTime();
        int curSec = nowDT.Hour * SecHour + nowDT.Minute * SecMinute + nowDT.Second;
        int leftSec = targetHour * SecHour - curSec;
        return leftSec;
    }

    /// <summary>
    /// 获取距离周几的秒数（例：周三下午2点—>周几0点）
    /// </summary>
    public static int GetDistTargetDaySec(DayOfWeek targetDayOfWeekType)
    {
        DateTime nowDT = TimeMgr.Ins.GetServerDateTime();
        int curDayOfWeek = GetDayOfWeek(nowDT.DayOfWeek);
        int targetDayOfWeek = GetDayOfWeek(targetDayOfWeekType);
        int leftDay = curDayOfWeek >= targetDayOfWeek ? 7 - curDayOfWeek + targetDayOfWeek : targetDayOfWeek - curDayOfWeek;
        int curDayLeftSec = SecDay - (nowDT.Hour * SecHour + nowDT.Minute * SecMinute + nowDT.Second);
        int leftSec = SecDay * (leftDay - 1) + curDayLeftSec;
        return leftSec;
    }

    /// <summary>
    /// 获取距离下一天的秒数（例：3.30号下午2点—>3.31号0点）
    /// </summary>
    public static int GetDistNextDaySec()
    {
        DateTime nowDT = TimeMgr.Ins.GetServerDateTime();
        int curSec = nowDT.Hour * SecHour + nowDT.Minute * SecMinute + nowDT.Second;
        int leftSec = SecDay - curSec;
        return leftSec;
    }

    /// <summary>
    /// 获取距离下一周的秒数（例：周三下午2点—>下周一0点）
    /// </summary>
    public static int GetDistNextWeekSec()
    {
        DateTime nowDT = TimeMgr.Ins.GetServerDateTime();
        int dayOfWeek = GetDayOfWeek(nowDT.DayOfWeek);
        int leftDay = 7 - dayOfWeek;
        int curDayLeftSec = SecDay - (nowDT.Hour * SecHour + nowDT.Minute * SecMinute + nowDT.Second);
        int leftSec = SecDay * leftDay + curDayLeftSec;
        return leftSec;
    }

    /// <summary>
    /// 获取距离下一月的秒数（例：3.20号下午2点—>4月1日0点）
    /// </summary>
    public static int GetDistNextMonthTime()
    {
        DateTime nowDT = TimeMgr.Ins.GetServerDateTime();
        int leftDay = DateTime.DaysInMonth(nowDT.Year, nowDT.Month) - nowDT.Day;
        int curDayLeftSec = SecDay - (nowDT.Hour * SecHour + nowDT.Minute * SecMinute + nowDT.Second);
        int leftSec = SecDay * leftDay + curDayLeftSec;
        return leftSec;
    }

    /// <summary>
    /// 获取是当前周的第几天
    /// </summary>
    private static int GetDayOfWeek(DayOfWeek dayOfWeekType)
    {
        if (dayOfWeekType == DayOfWeek.Sunday)
        {
            return 7;
        }
        return (int)dayOfWeekType;
    }

    #endregion 距离某一时间的秒数
}