using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 计时器单位类型
/// </summary>
public enum ETimerUnitType
{
    Millisecond,        //毫秒
    Second,             //秒
    Minute,             //分钟
    Hour,               //小时
    Day,                //天
}

/// <summary>
/// 计时器任务
/// </summary>
public class TimerTask
{
    private ETimerUnitType m_UnitType;//单位类型
    public ETimerUnitType UnityType
    {
        get { return m_UnitType; }
    }
    private float m_Duration;//持续时间
    public float Duration
    {
        get { return m_Duration; }
    }
    private int m_LoopCount;//循环次数（-1代表无限循环）
    public int LoopCount
    {
        get { return m_LoopCount; }
    }
    private bool m_IgnoreTimeScale;//是否忽略时间缩放
    public bool IgnoreTimeScale
    {
        get { return m_IgnoreTimeScale; }
    }
    private Action m_OnRegister;//注册时的回调
    public Action OnRegister
    {
        get { return m_OnRegister; }
    }
    private Action<float> m_OnUpdate;//更新时的回调(每帧)
    public Action<float> OnUpdate
    {
        get { return m_OnUpdate; }
    }
    private Action m_OnComplete;//完成时的回调
    public Action OnComplete
    {
        get { return m_OnComplete; }
    }

    private float m_TargetTime;//目标时间             
    private float m_LastUpdateTime;//上一次更新的时间

    private bool m_IsDone;//是否完成(到达目标时间)
    private bool m_IsPaused;//是否暂停
    private bool m_IsDisposed;//是否被销毁
    public bool IsCompleted//是否完成
    {
        get
        {
            return m_IsDone || m_IsDisposed;
        }
    }

    /// <summary>
    /// 注册计时器
    /// </summary>
    public void Register(float duration, ETimerUnitType unitType, int loopCount, bool ignoreTimeScale,
        Action onRegister, Action onComplete, Action<float> onUpdate)
    {
        m_Duration = ConvertUnitToSecond(duration, unitType);
        m_UnitType = unitType;
        m_LoopCount = loopCount;
        m_IgnoreTimeScale = ignoreTimeScale;
        m_OnRegister = onRegister;
        m_OnComplete = onComplete;
        m_OnUpdate = onUpdate;

        m_OnRegister?.Invoke();

        float curTime = GetWorldTime();
        m_LastUpdateTime = curTime;
        m_TargetTime = curTime + m_Duration;
    }

    /// <summary>
    /// 重新设置持续时间
    /// </summary>
    public void ResetDuration(float duration)
    {
        m_Duration = duration;
        float curTime = GetWorldTime();
        m_LastUpdateTime = curTime;
        m_TargetTime = curTime + m_Duration;
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void Pause()
    {
        if (m_IsPaused
            || IsCompleted)
        {
            return;
        }

        m_IsPaused = true;
    }

    /// <summary>
    /// 恢复计时器
    /// </summary>
    public void Resume()
    {
        if (!m_IsPaused
            || IsCompleted)
        {
            return;
        }

        m_IsPaused = false;
    }

    /// <summary>
    /// 销毁计时器
    /// </summary>
    public void Dispose()
    {
        if (IsCompleted)
        {
            return;
        }

        m_IsDisposed = true;
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    public void Update()
    {
        if (IsCompleted)
        {
            return;
        }

        if (m_IsPaused)
        {
            m_TargetTime += GetDeltaTime();
            m_LastUpdateTime = GetWorldTime();
            return;
        }

        m_OnUpdate?.Invoke(GetLeftTime());
        m_LastUpdateTime = GetWorldTime();
        if (GetWorldTime() >= m_TargetTime)
        {
            try
            {
                m_OnComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            if (m_LoopCount == -1)
            {
                m_TargetTime = GetWorldTime() + m_Duration;
            }
            else
            {
                m_LoopCount--;
                if (m_LoopCount <= 0)
                {
                    m_IsDone = true;
                }
                else
                {
                    m_TargetTime = GetWorldTime() + m_Duration;
                }
            }
        }
    }

    /// <summary>
    /// 得到剩余时间（秒）
    /// </summary>
    public float GetLeftTime()
    {
        if (IsCompleted
            || GetWorldTime() >= m_TargetTime)
        {
            return 0;
        }

        float leftTime = m_TargetTime - GetWorldTime();
        return leftTime;
    }

    /// <summary>
    /// 得到剩余时间比例
    /// </summary>
    public float GetLeftTimeRatio()
    {
        if (IsCompleted
            || GetWorldTime() >= m_TargetTime)
        {
            return 1;
        }

        float timeLeftRatio = GetLeftTime() / m_Duration;
        return timeLeftRatio;
    }

    /// <summary>
    /// 得到增量时间
    /// </summary>
    private float GetDeltaTime()
    {
        float deltaTime = GetWorldTime() - m_LastUpdateTime;
        return deltaTime;
    }

    /// <summary>
    /// 得到时间(距游戏开始运行的时间)
    /// </summary>
    private float GetWorldTime()
    {
        float worldTime = m_IgnoreTimeScale
            ? Time.realtimeSinceStartup
            : Time.time;
        return worldTime;
    }

    /// <summary>
    /// 转换时间单位为秒
    /// </summary>
    private float ConvertUnitToSecond(float duration, ETimerUnitType unitType)
    {
        switch (unitType)
        {
            case ETimerUnitType.Millisecond:
                return duration / 1000;

            case ETimerUnitType.Second:
                return duration;

            case ETimerUnitType.Minute:
                return duration * 60;

            case ETimerUnitType.Hour:
                return duration * 60 * 60;

            case ETimerUnitType.Day:
                return duration * 60 * 60 * 24;

            default:
                return duration;
        }
    }
}

/// <summary>
/// 计时器管理器
/// </summary>
public class TimerMgr : MonoSingleton<TimerMgr>
{
    private List<TimerTask> m_TaskList = new List<TimerTask>();//计时器任务列表 
    private List<TimerTask> m_TaskListToAdd = new List<TimerTask>();//计时器任务列表(先缓存所有计时器)

    /// <summary>
    /// 注册计时器
    /// </summary>
    public TimerTask Register(float duration, ETimerUnitType unitType = ETimerUnitType.Second, int loopCount = 1, bool ignoreTimeScale = false
        , Action onRegister = null, Action onComplete = null, Action<float> onUpdate = null)
    {
        TimerTask task = new TimerTask();
        task.Register(duration, unitType, loopCount, ignoreTimeScale, onRegister, onComplete, onUpdate);
        m_TaskListToAdd.Add(task);
        return task;
    }

    /// <summary>
    /// 重新设置计时器
    /// </summary>
    public void Resetting(float duration, TimerTask task)
    {
        if (m_TaskList.Contains(task)
            || m_TaskListToAdd.Contains(task))
        {
            task.ResetDuration(duration);
        }
        else
        {
            Register(task.Duration, task.UnityType, task.LoopCount, task.IgnoreTimeScale, task.OnRegister, task.OnComplete, task.OnUpdate);
        }
    }

    /// <summary>
    /// 销毁所有计时器
    /// </summary>
    public void DisposeAll()
    {
        foreach (var task in m_TaskList)
        {
            task.Dispose();
        }
        m_TaskListToAdd.Clear();
        m_TaskList.Clear();
    }

    /// <summary>
    /// 暂停所有计时器
    /// </summary>
    public void PauseAll()
    {
        foreach (var task in m_TaskList)
        {
            task.Pause();
        }
    }

    /// <summary>
    /// 恢复所有计时器
    /// </summary>
    public void ResumeAll()
    {
        foreach (var task in m_TaskList)
        {
            task.Resume();
        }
    }

    /// <summary>
    /// 更新所有计时器
    /// </summary>
    public void UpdateAll()
    {
        //用m_TaskListToAdd缓存一下是防止foreach时添加task报错
        foreach (var timer in m_TaskListToAdd)
        {
            m_TaskList.Add(timer);
        }
        m_TaskListToAdd.Clear();
        foreach (var timer in m_TaskList)
        {
            timer.Update();
        }

        //移除计时器任务
        for (int i = m_TaskList.Count - 1; i >= 0; i--)
        {
            if (m_TaskList[i].IsCompleted)
            {
                m_TaskList.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        UpdateAll();
    }
}