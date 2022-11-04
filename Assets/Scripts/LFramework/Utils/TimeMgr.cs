using UnityEngine;
using System;

public class TimeMgr : Singleton<TimeMgr>
{
    private DateTime m_StartDT = new DateTime(1970, 1, 1, 0, 0, 0);//UTC起始时间1970/1/1
    public const int SecMinute = 60;//一分钟的秒数
    public const int SecHour = 3600;//一小时的秒数
    public const int SecDay = 86400;//一天的秒数

    int m_TimeZone;//时区

    long m_LoseTimestamp;//与正确时间戳的差

    /// <summary>
    /// 更新服务器时间（服务器每隔几秒传来正确的时间戳做比对）
    /// </summary>
    public void UpdateServerTime(long serverTimestamp)
    {
        long localTimestamp = GetTimestamp_Local(false);
        m_LoseTimestamp = (serverTimestamp - localTimestamp) / 1000;
    }

    /// <summary>
    /// 设置时区
    /// </summary>
    public void SetTimeZone(int timeZone)
    {
        m_TimeZone = timeZone;
    }

    /// <summary>
    /// 转换为本地时间
    /// </summary>
    private void ConvertToLocalTime(long timestamp)
    {

    }

    /// <summary>
    /// 转换为服务器时间（根据服务器时区计算）
    /// </summary>
    private DateTime ConvertToServerTime(long timestamp)
    {
        DateTime utcDT = m_StartDT.AddSeconds(timestamp);//UTC时间
        DateTime serverDT = utcDT.AddSeconds(m_TimeZone * SecHour);
        return serverDT;
    }

    /// <summary>
    /// 得到本地时间戳
    /// </summary>
    public long GetTimestamp_Local(bool sec = true)
    {
        long ticks = GetTicks();
        long timestamp = sec
             ? ticks / 10000000
             : ticks / 10000;
        return timestamp;
    }

    /// <summary>
    /// 得到服务器的时间戳
    /// </summary>
    public long GetTimestamp_Server(bool sec = true)
    {
        long ticks = GetTicks();
        long timestamp = sec
             ? ticks / 10000000
             : ticks / 10000;
        timestamp += m_LoseTimestamp;
        return timestamp;
    }

    private long GetTicks()
    {
        return DateTime.Now.ToUniversalTime().Ticks - 621355968000000000;
    }

    #region 格式化时间

    /// <summary>
    /// 格式化时间显示
    /// </summary>
    public string FormatTime(int sec)
    {
        sec = Mathf.Max(0, sec);
        TimeSpan timeSpan = new TimeSpan(0, 0, sec);
        return timeSpan.ToString();
    }

    #endregion 格式化时间

    #region 距离某一时间的秒数

    /// <summary>
    /// 得到距离某一点的秒数
    /// </summary>
    public int GetDistTargetHourSec(int targetHour)
    {
        if (targetHour == 0)
        {
            targetHour = 24;
        }
        DateTime now = GetServerDateTime();
        int curSec = now.Hour * SecHour + now.Minute * SecMinute + now.Second;
        int leftSec = targetHour * SecHour - curSec;
        return leftSec;
    }

    /// <summary>
    /// 得到距离周几的秒数
    /// </summary>
    public int GetDistTargetDaySec(DayOfWeek targetDayOfWeekType)
    {
        DateTime now = GetServerDateTime();
        int dayOfWeek = GetDayOfWeek(now.DayOfWeek);
        int leftDay = dayOfWeek >= GetDayOfWeek(targetDayOfWeekType)
            ? 7 - dayOfWeek + dayOfWeek
            : dayOfWeek - dayOfWeek;
        int sec = now.Hour * SecHour + now.Minute * SecMinute + now.Second;
        int leftSec = SecDay * leftDay - sec;
        return leftSec;
    }

    /// <summary>
    /// 得到距离下一天的秒数(距离明天零点)
    /// </summary>
    public int GetDistNextDaySec()
    {
        DateTime now = GetServerDateTime();
        int curSec = now.Hour * SecHour + now.Minute * SecMinute + now.Second;
        int leftSec = SecDay - curSec;
        return leftSec;
    }

    /// <summary>
    /// 得到距离下一周的秒数(距离周一零点)
    /// </summary>
    public int GetDistNextWeekSec()
    {
        DateTime now = GetServerDateTime();
        int dayOfWeek = GetDayOfWeek(now.DayOfWeek);
        int curSec = dayOfWeek * SecDay + now.Hour * SecHour + now.Minute * SecMinute + now.Second;
        int leftSec = SecDay * 7 - curSec + SecDay;
        return leftSec;
    }

    /// <summary>
    /// 得到距离下一月的秒数(距离下月1号零点)
    /// </summary>
    public int GetDistNextMonthTime()
    {
        DateTime now = GetServerDateTime();
        int curSec = now.Day * SecDay + now.Hour * SecHour + now.Minute * SecMinute + now.Second;
        int leftSec = SecDay * DateTime.DaysInMonth(now.Year, now.Month) - curSec + SecDay;
        return leftSec;
    }

    #endregion 距离某一时间的秒数

    #region 工具

    /// <summary>
    /// 得到服务器的时间
    /// </summary>
    public DateTime GetServerDateTime()
    {
        long timestamp_server = GetTimestamp_Server();
        return ConvertToServerTime(timestamp_server);
    }

    /// <summary>
    /// 得到间隔时间 
    /// </summary>
    public int GetDuration(DateTime dt1, DateTime dt2)
    {
        TimeSpan ts = dt2 - dt1;
        return (int)ts.TotalSeconds;
    }

    /// <summary>
    /// 得到日期是一周的第几天
    /// </summary>
    private int GetDayOfWeek(DayOfWeek dayOfWeekType)
    {
        int dayOfWeek = -1;
        string str = dayOfWeekType.ToString();
        switch (str)
        {
            case "Monday": dayOfWeek = 1; break;
            case "Tuesday": dayOfWeek = 2; break;
            case "Wednesday": dayOfWeek = 3; break;
            case "Thursday": dayOfWeek = 4; break;
            case "Friday": dayOfWeek = 5; break;
            case "Saturday": dayOfWeek = 6; break;
            case "Sunday": dayOfWeek = 7; break;
        }
        return dayOfWeek;
    }

    #endregion 工具
}