using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 计时器类
/// </summary>
public class Timer
{
    //持续时间
    float duration;
    //是否循环
    bool isLoop;
    //是否忽略时间缩放
    bool ignoreTimeScale;
    //完成的回调
    Action onComplete;
    //更新的回调(每帧)
    Action<float> onUpdate;
    //注册的回调
    Action onRegister;

    //是否完成计时
    public bool isCompleted
    {
        get
        {
            return isDone || isDispose;
        }
    }
    //是否完成计时(时间完成)
    bool isDone;
    //是否销毁计时
    bool isDispose;
    //是否暂停计时
    bool isPause;

    //开始计时的时间
    float beginTime;
    //上一次更新的时间
    float lastUpdateTime;
    //暂停的总时间
    float pauseTime;

    public Timer(float duration, bool isLoop, bool ignoreTimeScale, Action onRegister, Action onComplete, Action<float> onUpdate)
    {
        this.duration = duration;
        this.isLoop = isLoop;
        this.ignoreTimeScale = ignoreTimeScale;
        this.onRegister = onRegister;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        onRegister?.Invoke();
        pauseTime = 0;
        beginTime = GetWorldTime();
        lastUpdateTime = beginTime;
    }

    /// <summary>
    /// 销毁计时器
    /// </summary>
    public void Dispose()
    {
        isDispose = true;
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// 恢复计时器
    /// </summary>
    public void Resume()
    {
        isPause = false;
    }

    /// <summary>
    /// 得到剩余时间
    /// </summary>
    public float GetTimeLeft()
    {
        if (GetWorldTime() >= GetTargetTime())
        {
            return 0;
        }
        return GetTargetTime() - GetWorldTime();
    }

    /// <summary>
    /// 得到剩余时间比例
    /// </summary>
    public float GetTimeLeftRatio()
    {
        return GetTimeLeft() / duration;
    }

    /// <summary>
    /// 得到当前时间
    /// </summary>
    float GetWorldTime()
    {
        return ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
    }

    /// <summary>
    /// 得到目标时间
    /// </summary>
    float GetTargetTime()
    {
        return beginTime + duration + pauseTime;
    }

    /// <summary>
    /// 得到时间增量
    /// </summary>
    float GetTimeDelta()
    {
        return GetWorldTime() - lastUpdateTime;
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    public void Update()
    {
        if (isCompleted)
        {
            return;
        }
        if (isPause)
        {
            pauseTime += GetTimeDelta();
            lastUpdateTime = GetWorldTime();
            return;
        }

        lastUpdateTime = GetWorldTime();
        onUpdate?.Invoke(GetTimeLeft());
        if (GetWorldTime() >= GetTargetTime())
        {
            onComplete?.Invoke();

            if (isLoop)
            {
                beginTime = GetWorldTime();
                pauseTime = 0;
            }
            else
            {
                isDone = true;
            }
        }
    }
}

/// <summary>
/// 计时器管理类
/// </summary>
public class TimerMgr : MonoSingleton<TimerMgr>
{
    /// <summary>
    /// 计时器列表
    /// </summary>
    List<Timer> m_TimerList = new List<Timer>();

    /// <summary>
    /// 注册倒计时
    /// </summary>
    public Timer Register(float duration, bool isLoop = false, bool ignoreTimeScale = false,
       Action onRegister = null, Action onComplete = null, Action<float> onUpdate = null)
    {
        Timer timer = new Timer(duration, isLoop, ignoreTimeScale, onRegister, onComplete, onUpdate);
        m_TimerList.Add(timer);
        return timer;
    }

    /// <summary>
    /// 销毁全部计时器
    /// </summary>
    public void DisposeAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Dispose();
        }
    }

    /// <summary>
    /// 暂停全部计时器
    /// </summary>
    public void PauseAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Pause();
        }
    }

    /// <summary>
    /// 恢复全部计时器
    /// </summary>
    public void ResumeAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Resume();
        }
    }

    /// <summary>
    /// 更新全部计时器
    /// </summary>
    void UpdateAll()
    {
        foreach (var timer in m_TimerList)
        {
            timer.Update();
        }

        m_TimerList.RemoveAll(t => t.isCompleted);
    }

    private void Update()
    {
        UpdateAll();
    }
}
