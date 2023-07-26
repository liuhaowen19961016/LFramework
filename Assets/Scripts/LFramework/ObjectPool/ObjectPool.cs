using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 对象池
/// </summary>
public class ObjectPool<T> : IObjectPool<T>
    where T : class
{
    private List<T> m_ActiveList = new List<T>();//所有激活的对象
    private List<T> m_DeactiveList = new List<T>();//所有未激活的对象

    private int m_Capacity;//容量（-1为无限容量）
    private Func<T> m_OnCreate;//创建的回调
    private Action<T> m_OnGet;//取出的回调
    private Action<T> m_OnPut;//放回的回调
    private Action<T> m_OnDestroy;//销毁的回调

    /// <summary>
    /// 初始化池子
    /// </summary>
    public ObjectPool(Func<T> onCreate, Action<T> onGet = null, Action<T> onPut = null, Action<T> onDestroy = null,
        int capacity = -1, bool needPreLoad = false)
    {
        m_OnCreate = onCreate;
        m_OnGet = onGet;
        m_OnPut = onPut;
        m_OnDestroy = onDestroy;
        m_Capacity = capacity;
        if (needPreLoad && capacity != -1)
        {
            for (int i = 0; i < capacity; i++)
            {
                m_OnCreate?.Invoke();
            }
        }
    }

    /// <summary>
    /// 从池子中取
    /// </summary>
    public T Get()
    {
        T t = null;
        if (m_DeactiveList.Count == 0)
        {
            t = m_OnCreate?.Invoke();
            if (t == null)
            {
                return t;
            }
            else
            {
                m_ActiveList.Add(t);
            }
        }
        else
        {
            t = m_DeactiveList[0];
            m_DeactiveList.Remove(t);
            if (t == null)
            {
                return t;
            }
            else
            {
                m_ActiveList.Add(t);
            }
        }
        m_OnGet?.Invoke(t);
        Log();
        return t;
    }

    /// <summary>
    /// 放回池子
    /// </summary>
    public bool Put(T t)
    {
        if (t == null)
        {
            return false;
        }
        if (m_DeactiveList.Contains(t))
        {
            return false;
        }
        if (!m_ActiveList.Contains(t))
        {
            Debug.LogError($"此对象不由对象池管理：{t}");
            m_OnDestroy?.Invoke(t);
            return false;
        }
        m_ActiveList.Remove(t);
        if (m_Capacity == -1 || m_DeactiveList.Count < m_Capacity)
        {
            m_DeactiveList.Add(t);
            m_OnPut?.Invoke(t);
            Log();
            return true;
        }
        else
        {
            m_OnDestroy?.Invoke(t);
            return false;
        }
    }

    /// <summary>
    /// 全部放回池子
    /// </summary>
    public void PutAll()
    {
        for (int i = 0; i < m_ActiveList.Count; i++)
        {
            Put(m_ActiveList[i]);
        }
        Log();
    }

    public void Log()
    {
        Debug.Log($"{typeof(T).Name}对象池中，激活对象数量：{m_ActiveList.Count},未激活对象数量：{m_DeactiveList.Count}");
    }
}
