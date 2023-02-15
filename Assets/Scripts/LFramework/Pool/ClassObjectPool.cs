using System.Collections.Generic;

/// <summary>
/// 类对象池
/// </summary>
public class ClassObjectPool<T>
    where T : class, new()
{
    private Stack<T> m_ClassStack = new Stack<T>();//类栈
    private int m_Capacity;//容量（-1为无限容量）
    private int m_NoRecycleCount;//没有回收的数量

    /// <summary>
    /// 创建池子
    /// </summary>
    public ClassObjectPool(int capacity)
    {
        m_Capacity = capacity;
        for (int i = 0; i < m_Capacity; i++)
        {
            T t = new T();
            m_ClassStack.Push(t);
        }
    }

    /// <summary>
    /// 从池子中取
    /// </summary>
    /// <param name="createNew">如果缓存栈中没有了，是否new一个</param>
    public T Allocate(bool createNew = true)
    {
        T t = default;
        if (m_ClassStack.Count <= 0)
        {
            if (createNew)
            {
                t = new T();
                m_NoRecycleCount++;
            }
        }
        else
        {
            t = m_ClassStack.Pop();
            m_NoRecycleCount++;
        }
        return t;
    }

    /// <summary>
    /// 放回池子中
    /// </summary>
    public bool Recycle(T t)
    {
        bool ret = false;
        if (t != null)
        {
            m_NoRecycleCount--;
            if (m_Capacity > 0 && m_ClassStack.Count >= m_Capacity)
            {
                t = null;
            }
            else
            {
                m_ClassStack.Push(t);
                ret = true;
            }
        }
        return ret;
    }
}
