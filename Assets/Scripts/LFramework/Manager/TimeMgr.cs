using UnityEngine;
using System;

public class TimeMgr : Singleton<TimeMgr>
{
    private DateTime m_StartDT = new DateTime(1970, 1, 1, 0, 0, 0);//UTC起始时间1970/1/1

    private int m_TimeZone;//时区

    private long m_LoseTimestampMS;//与正确时间戳的差（毫秒）

    /// <summary>
    /// 更新服务器时间（服务器每隔几秒传来正确的时间戳做比对）
    /// </summary>
    public void UpdateServerTime(long serverTimestampMS)
    {
        long localTimestampMS = GetLocalTimestampMS();
        m_LoseTimestampMS = serverTimestampMS - localTimestampMS;
    }

    /// <summary>
    /// 设置时区
    /// </summary>
    public void SetTimeZone(int timeZone)
    {
        m_TimeZone = timeZone;
    }

    /// <summary>
    /// 获取本地时间ticks
    /// </summary>
    private long GetTicks()
    {
        return DateTime.Now.ToUniversalTime().Ticks - 621355968000000000;
    }

    #region 常用接口

    /// <summary>
    /// 获取服务器的时间
    /// </summary>
    public DateTime GetServerDateTime()
    {
        long serverTimestamp = GetServerTimestamp();
        return ConvertToServerTime(serverTimestamp);
    }

    /// <summary>
    /// 获取本地时间戳（毫秒）
    /// </summary>
    public long GetLocalTimestampMS()
    {
        long ticks = GetTicks();
        long localTimestamp = ticks / 10000;
        return localTimestamp;
    }

    /// <summary>
    /// 获取本地时间戳（秒）
    /// </summary>
    public long GetLocalTimestamp()
    {
        long localTimestamp = GetLocalTimestampMS() / 1000;
        return localTimestamp;
    }

    /// <summary>
    /// 获取服务器时间戳（毫秒）
    /// </summary>
    public long GetServerTimestampMS()
    {
        long serverTimestamp = GetLocalTimestampMS() + m_LoseTimestampMS;
        return serverTimestamp;
    }

    /// <summary>
    /// 获取服务器时间戳（秒）
    /// </summary>
    public long GetServerTimestamp()
    {
        long serverTimestamp = GetServerTimestampMS() / 1000;
        return serverTimestamp;
    }

    #endregion 常用接口

    /// <summary>
    /// 转换为服务器时间（根据服务器时区计算）
    /// </summary>
    private DateTime ConvertToServerTime(long timestamp)
    {
        DateTime utcDT = m_StartDT.AddSeconds(timestamp);//UTC时间
        DateTime serverDT = utcDT.AddSeconds(m_TimeZone * TimeUtils.SecHour);
        return serverDT;
    }
}