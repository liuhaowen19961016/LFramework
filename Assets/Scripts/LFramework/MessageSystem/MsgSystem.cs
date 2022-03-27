using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

/// <summary>
/// 事件系统
/// </summary>
public static class MsgSystem
{
    //存所有事件的字典
    static Dictionary<string, List<Delegate>> m_EventDict = new Dictionary<string, List<Delegate>>();

    /// <summary>
    /// 添加监听
    /// </summary>
    static void AddListener(string key, Delegate callBack)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            eventList.Add(callBack);
        }
        else
        {
            eventList = new List<Delegate>();
            eventList.Add(callBack);
            m_EventDict.Add(key, eventList);
        }
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    static void RemoveListener(string key, Delegate callBack)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            eventList.Remove(callBack);
        }

        if (eventList.Count == 0)
        {
            m_EventDict.Remove(key);
        }
    }

    /// <summary>
    /// 移除所有监听
    /// </summary>
    public static void RemoveAllListener()
    {
        m_EventDict.Clear();
    }

    #region 添加监听

    public static void AddListener(string key, Action callBack)
    {
        AddListener(key, (Delegate)callBack);
    }
    public static void AddListener<T1>(string key, Action<T1> callBack)
    {
        AddListener(key, (Delegate)callBack);
    }
    public static void AddListener<T1, T2>(string key, Action<T1, T2> callBack)
    {
        AddListener(key, (Delegate)callBack);
    }
    public static void AddListener<T1, T2, T3>(string key, Action<T1, T2, T3> callBack)
    {
        AddListener(key, (Delegate)callBack);
    }
    public static void AddListener<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> callBack)
    {
        AddListener(key, (Delegate)callBack);
    }
    public static void AddListener<T1, T2, T3, T4, T5>(string key, Action<T1, T2, T3, T4, T5> callBack)
    {
        AddListener(key, (Delegate)callBack);
    }

    #endregion

    #region 移除监听

    public static void RemoveListener(string key, Action callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }
    public static void RemoveListener<T1>(string key, Action<T1> callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }
    public static void RemoveListener<T1, T2>(string key, Action<T1, T2> callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }
    public static void RemoveListener<T1, T2, T3>(string key, Action<T1, T2, T3> callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }
    public static void RemoveListener<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }
    public static void RemoveListener<T1, T2, T3, T4, T5>(string key, Action<T1, T2, T3, T4, T5> callBack)
    {
        RemoveListener(key, (Delegate)callBack);
    }

    #endregion

    #region 分发消息

    public static void Dispatch(string key)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action)
                {
                    Action callBack = eventList[i] as Action;
                    callBack?.Invoke();
                }
            }
        }
    }

    public static void Dispatch<T1>(string key, T1 arg1)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action<T1>)
                {
                    Action<T1> callBack = eventList[i] as Action<T1>;
                    callBack?.Invoke(arg1);
                }
            }
        }
    }

    public static void Dispatch<T1, T2>(string key, T1 arg1, T2 arg2)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action<T1, T2>)
                {
                    Action<T1, T2> callBack = eventList[i] as Action<T1, T2>;
                    callBack?.Invoke(arg1, arg2);
                }
            }
        }
    }

    public static void Dispatch<T1, T2, T3>(string key, T1 arg1, T2 arg2, T3 arg3)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action<T1, T2, T3>)
                {
                    Action<T1, T2, T3> callBack = eventList[i] as Action<T1, T2, T3>;
                    callBack?.Invoke(arg1, arg2, arg3);
                }
            }
        }
    }

    public static void Dispatch<T1, T2, T3, T4>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action<T1, T2, T3, T4>)
                {
                    Action<T1, T2, T3, T4> callBack = eventList[i] as Action<T1, T2, T3, T4>;
                    callBack?.Invoke(arg1, arg2, arg3, arg4);
                }
            }
        }
    }

    public static void Dispatch<T1, T2, T3, T4, T5>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        List<Delegate> eventList;
        if (m_EventDict.TryGetValue(key, out eventList))
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i] is Action<T1, T2, T3, T4, T5>)
                {
                    Action<T1, T2, T3, T4, T5> callBack = eventList[i] as Action<T1, T2, T3, T4, T5>;
                    callBack?.Invoke(arg1, arg2, arg3, arg4, arg5);
                }
            }
        }
    }

    #endregion

    public static void Log()
    {
        var sb = new StringBuilder();
        sb.Append("----------输出所有注册事件------------");
        sb.Append("\n");
        foreach (var pair in m_EventDict)
        {
            sb.AppendFormat($"key = {pair.Key}");
            sb.Append("\n");
            sb.AppendFormat($"event = {pair.Value.Count}");
            sb.Append("\n");
            foreach (var callback in pair.Value)
            {
                sb.AppendFormat($"cs : {callback.Method.DeclaringType} , event name : {callback.Method.Name}");
                sb.Append("\n");
            }
            sb.Append("\n");
        }
        sb.Append("\n");
        sb.Append("---------------------------------------");
        Debug.Log(sb);
    }
}
