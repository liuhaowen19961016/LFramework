using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 计时器数据
/// </summary>
public class TimerData
{
    //持续时间
    float duration;
    //是否循环
    bool isLoop;
    //是否忽略时间缩放
    bool ignoreTimeScale;
    //注册时的回调
    Action onRegister;
    //更新时的回调(每帧)
    Action<float> onUpdate;
    //完成时的回调
    Action onComplete;

    //是否完成(到达目标时间)
    bool isDone;
    //开始计时的时间(距游戏开始运行)
    float beginTime;
    //上一次更新的时间(距游戏开始运行)
    float lastUpdateTime;
    //暂停的总时长
    float pauseTotalTime;
    //是否暂停
    bool isPaused;
    //是否被销毁
    bool isDisposed;
    //是否完成
    public bool IsCompleted
    {
        get
        {
            return isDone || isDisposed;
        }
    }

    /// <summary>
    /// 注册计时器
    /// </summary>
    public void Register(float duration, bool isLoop, bool ignoreTimeScale,
        Action onRegister, Action onComplete, Action<float> onUpdate)
    {
        this.duration = duration;
        this.isLoop = isLoop;
        this.ignoreTimeScale = ignoreTimeScale;
        this.onRegister = onRegister;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        onRegister?.Invoke();

        pauseTotalTime = 0;
        beginTime = GetWorldTime();
        lastUpdateTime = beginTime;
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void Pause()
    {
        if (isPaused
            || IsCompleted)
        {
            return;
        }

        isPaused = true;
    }

    /// <summary>
    /// 恢复计时器
    /// </summary>
    public void Resume()
    {
        if (!isPaused
            || IsCompleted)
        {
            return;
        }

        isPaused = false;
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

        isDisposed = true;
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

        if (isPaused)
        {
            pauseTotalTime += GetDeltaTime();
            lastUpdateTime = GetWorldTime();
            return;
        }

        onUpdate?.Invoke(GetTimeLeft());
        lastUpdateTime = GetWorldTime();
        if (GetWorldTime() >= GetTargetTime())
        {
            onComplete?.Invoke();

            if (isLoop)
            {
                beginTime = GetWorldTime();
                pauseTotalTime = 0;
            }
            else
            {
                isDone = true;
            }
        }
    }

    /// <summary>
    /// 得到剩余时间
    /// </summary>
    public float GetTimeLeft()
    {
        if (IsCompleted
            || GetWorldTime() >= GetTargetTime())
        {
            return 0;
        }

        float timeLeft = GetTargetTime() - GetWorldTime();
        return timeLeft;
    }

    /// <summary>
    /// 得到剩余时间比例
    /// </summary>
    public float GetTimeLeftRatio()
    {
        if (IsCompleted
            || GetWorldTime() >= GetTargetTime())
        {
            return 1;
        }

        float timeLeftRatio = GetTimeLeft() / duration;
        return timeLeftRatio;
    }

    /// <summary>
    /// 得到时间(距游戏开始运行)
    /// </summary>
    float GetWorldTime()
    {
        float worldTime = ignoreTimeScale
            ? Time.realtimeSinceStartup
            : Time.time;
        return worldTime;
    }

    /// <summary>
    /// 得到目标时间(距游戏开始运行)
    /// </summary>
    float GetTargetTime()
    {
        float targetTime = beginTime + duration + pauseTotalTime;
        return targetTime;
    }

    /// <summary>
    /// 得到增量时间
    /// </summary>
    float GetDeltaTime()
    {
        float deltaTime = GetWorldTime() - lastUpdateTime;
        return deltaTime;
    }
}

/// <summary>
/// 计时器管理器
/// </summary>
public class TimerMgr : MonoSingleton<TimerMgr>
{
    //计时器列表
    List<TimerData> m_TimerList = new List<TimerData>();
    //计时器列表(先缓存所有计时器)
    List<TimerData> m_TimerListToAdd = new List<TimerData>();

    /// <summary>
    /// 注册计时器
    /// </summary>
    public TimerData Register(float duration, bool isLoop = false, bool ignoreTimeScale = false
        , Action onRegister = null, Action onComplete = null, Action onUpdate = null)
    {
        TimerData data = new TimerData();
        data.Register(duration, isLoop, ignoreTimeScale, onRegister, onComplete, onUpdate);
        m_TimerListToAdd.Add(data);
        return data;
    }

    /// <summary>
    /// 注册计时器
    /// </summary>
    public TimerData Register(float duration, Action onComplete = null)
    {
        TimerData data = new TimerData();
        data.Register(duration, false, false, null, onComplete, null);
        m_TimerListToAdd.Add(data);
        return data;
    }

    /// <summary>
    /// 销毁所有计时器
    /// </summary>
    public void DisposeAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Dispose();
        }
        m_TimerListToAdd.Clear();
        m_TimerList.Clear();
    }

    /// <summary>
    /// 暂停所有计时器
    /// </summary>
    public void PauseAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Pause();
        }
    }

    /// <summary>
    /// 恢复所有计时器
    /// </summary>
    public void ResumeAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Resume();
        }
    }

    /// <summary>
    /// 更新所有计时器
    /// </summary>
    public void UpdateAll()
    {
        foreach (var timer in m_TimerListToAdd)
        {
            m_TimerList.Add(timer);
        }
        m_TimerListToAdd.Clear();

        foreach (var timer in m_TimerList)
        {
            timer.Update();
        }
        m_TimerList.RemoveAll(t => t.IsCompleted);
    }

    private void Update()
    {
        UpdateAll();
    }
}