using UnityEngine;
using System;

/// <summary>
/// 时间工具类
/// </summary>
public static class TimeUtils
{
    static readonly int SEC_PERMINUTE = 60;//每分钟的秒数
    static readonly int SEC_PERHOUR = 3600;//每小时的秒数
    static readonly int SEC_PERDAY = 86400;//每天的秒数

    static int m_TimeZone;//服务器时区
    static DateTime m_StartDT = new DateTime(1970, 1, 1, 0, 0, 0);

    /// <summary>
    /// 设置服务器时区
    /// </summary>
    public static void SetSeverTimeZone(int timeZone)
    {
        m_TimeZone = timeZone;
    }

    /// <summary>
    /// 设置服务器时区
    /// </summary>
    public static void SetSeverTimeZone(string timeZoneStr)
    {
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneStr);
        if (timeZoneInfo == null)
        {
            Debug.LogError("时区字符串错误：" + timeZoneStr);
            return;
        }
        m_TimeZone = timeZoneInfo.BaseUtcOffset.Hours;
    }

    /// <summary>
    /// 得到本地10位时间戳
    /// </summary>
    public static long GetLocalTimestamp()
    {
        long timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return timestamp;
    }

    /// <summary>
    /// 得到服务器时间
    /// </summary>
    public static DateTime GetSeverDateTime()
    {
        DateTime now = ConvertToSeverDateTime(GetLocalTimestamp());
        return now;
    }

    /// <summary>
    /// 转换为服务器时间
    /// </summary>
    public static DateTime ConvertToSeverDateTime(long timestamp)
    {
        int timeZoneOffset = m_TimeZone * SEC_PERHOUR;
        DateTime dt = m_StartDT.AddSeconds(timestamp + timeZoneOffset);
        return dt;
    }

    /// <summary>
    /// 得到距离下一天的秒数(距离零点)
    /// </summary>
    public static int GetDistNextDayTime()
    {
        DateTime now = GetSeverDateTime();
        int sec = now.Hour * SEC_PERHOUR + now.Minute * SEC_PERMINUTE + now.Second;
        int remainSec = SEC_PERDAY - sec;
        return remainSec;
    }

    /// <summary>
    /// 得到距离下一周第一天的秒数(距离周一零点)
    /// </summary>
    public static int GetDistNextWeekTime()
    {
        DateTime now = GetSeverDateTime();
        int sec = Convert.ToInt32(now.DayOfWeek) * SEC_PERDAY + now.Hour * SEC_PERHOUR + now.Minute * SEC_PERMINUTE + now.Second;
        int remainSec = SEC_PERDAY * 7 - sec + SEC_PERDAY;
        return remainSec;
    }

    /// <summary>
    /// 得到距离下一月第一天的秒数(距离下月1号零点)
    /// </summary>
    public static int GetDistNextMonthTime()
    {
        DateTime now = GetSeverDateTime();
        int sec = now.Day * SEC_PERDAY + now.Hour * SEC_PERHOUR + now.Minute * SEC_PERMINUTE + now.Second;
        int remainSec = SEC_PERDAY * DateTime.DaysInMonth(now.Year, now.Month) - sec + SEC_PERDAY;
        return remainSec;
    }

    /// <summary>
    /// 得到距离某一天的秒数
    /// </summary>
    public static int GetDistDayTime(int targetDay)
    {
        DateTime now = GetSeverDateTime();
        int dayOfWeek = Convert.ToInt32(now.DayOfWeek);
        int dayOffset = dayOfWeek >= targetDay
            ? 7 - dayOfWeek + targetDay
            : targetDay - dayOfWeek;
        int sec = now.Hour * SEC_PERHOUR + now.Minute * SEC_PERMINUTE + now.Second;
        int remainSec = SEC_PERDAY * dayOffset - sec;
        return remainSec;
    }

    /// <summary>
    /// 得到间隔时间 
    /// </summary>
    /// 单位:秒
    public static int GetDuration(DateTime dt1, DateTime dt2)
    {
        TimeSpan ts = dt2 - dt1;
        return (int)ts.TotalSeconds;
    }

    /// <summary>
    /// 秒转换为分钟
    /// </summary>
    public static int Sec2Minute(int sec)
    {
        int minute = sec / SEC_PERMINUTE;
        return minute;
    }

    /// <summary>
    /// 分钟转换为秒
    /// </summary>
    public static int Minute2Sec(int minute)
    {
        int sec = minute * SEC_PERMINUTE;
        return sec;
    }

    /// <summary>
    /// 秒转换为小时
    /// </summary>
    public static int Sec2Hour(int sec)
    {
        int hour = sec / SEC_PERHOUR;
        return hour;
    }

    /// <summary>
    /// 小时转换为秒
    /// </summary>
    public static int Hour2Sec(int hour)
    {
        int sec = hour * SEC_PERHOUR;
        return sec;
    }

    /// <summary>
    /// 秒转换为天
    /// </summary>
    public static int Sec2Day(int sec)
    {
        int day = sec / SEC_PERDAY;
        return day;
    }

    /// <summary>
    /// 天转换为秒
    /// </summary>
    public static int Day2Sec(int day)
    {
        int sec = day * SEC_PERDAY;
        return sec;
    }
}
