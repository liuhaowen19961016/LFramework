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
    /// 注册事件
    /// </summary>
    static void Register(string key, Delegate callBack)
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
    /// 注销事件
    /// </summary>
    static void UnRegister(string key, Delegate callBack)
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
    /// 注销所有事件
    /// </summary>
    public static void UnRegisterAll()
    {
        m_EventDict.Clear();
    }

    #region 注册事件

    public static void Register(string key, Action callBack)
    {
        Register(key, (Delegate)callBack);
    }
    public static void Register<T1>(string key, Action<T1> callBack)
    {
        Register(key, (Delegate)callBack);
    }
    public static void Register<T1, T2>(string key, Action<T1, T2> callBack)
    {
        Register(key, (Delegate)callBack);
    }
    public static void Register<T1, T2, T3>(string key, Action<T1, T2, T3> callBack)
    {
        Register(key, (Delegate)callBack);
    }
    public static void Register<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> callBack)
    {
        Register(key, (Delegate)callBack);
    }
    public static void Register<T1, T2, T3, T4, T5>(string key, Action<T1, T2, T3, T4, T5> callBack)
    {
        Register(key, (Delegate)callBack);
    }

    #endregion

    #region 注销事件

    public static void UnRegister(string key, Action callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }
    public static void UnRegister<T1>(string key, Action<T1> callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }
    public static void UnRegister<T1, T2>(string key, Action<T1, T2> callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }
    public static void UnRegister<T1, T2, T3>(string key, Action<T1, T2, T3> callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }
    public static void UnRegister<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }
    public static void UnRegister<T1, T2, T3, T4, T5>(string key, Action<T1, T2, T3, T4, T5> callBack)
    {
        UnRegister(key, (Delegate)callBack);
    }

    #endregion

    #region 分发事件

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
