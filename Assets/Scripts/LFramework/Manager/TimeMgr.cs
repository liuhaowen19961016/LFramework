using UnityEngine;
using System;

public class TimeMgr : Singleton<TimeMgr>
{
    private DateTime m_StartDT = new DateTime(1970, 1, 1, 0, 0, 0);//UTC起始时间1970/1/1

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
        long timestamp_server = GetTimestamp_Server();
        return ConvertToServerTime(timestamp_server);
    }

    /// <summary>
    /// 获取本地时间戳
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
    /// 获取服务器的时间戳
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