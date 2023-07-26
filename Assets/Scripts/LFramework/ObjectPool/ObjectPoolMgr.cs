using System.Collections.Generic;
using System;

/// <summary>
/// 对象池管理器
/// </summary>
public partial class ObjectPoolMgr : MonoSingleton<ObjectPoolMgr>
{
    private Dictionary<string, object> m_PoolDict = new Dictionary<string, object>();//所有的对象池

    /// <summary>
    /// 获取对象池
    /// </summary>
    public ObjectPool<T> GetOrCreatePool<T>(Func<T> onCreate, string poolKey = "", Action<T> onGet = null, Action<T> onPut = null, Action<T> onDestroy = null,
        int capacity = -1, bool needPreLoad = false)
        where T : class
    {
        poolKey = string.IsNullOrEmpty(poolKey) ? typeof(T).Name : poolKey;
        if (m_PoolDict.TryGetValue(poolKey, out object pool))
        {
            if (pool == null)
            {
                m_PoolDict.Remove(poolKey);
                ObjectPool<T> objectPool = CreatePool<T>(onCreate, onGet, onPut, onDestroy, capacity, needPreLoad, poolKey);
                return objectPool;
            }
            else
            {
                return pool as ObjectPool<T>;
            }
        }
        else
        {
            ObjectPool<T> objectPool = CreatePool<T>(onCreate, onGet, onPut, onDestroy, capacity, needPreLoad, poolKey);
            return objectPool;
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    private ObjectPool<T> CreatePool<T>(Func<T> onCreate, Action<T> onGet = null, Action<T> onPut = null, Action<T> onDestroy = null,
        int capacity = -1, bool needPreLoad = false, string poolKey = "")
        where T : class
    {
        ObjectPool<T> objectPool = new ObjectPool<T>(onCreate, onGet, onPut, onDestroy, capacity, needPreLoad, poolKey);
        m_PoolDict.Add(poolKey, objectPool);
        return objectPool;
    }

    /// <summary>
    /// 清空所有对象池
    /// </summary>
    public void ClearAll()
    {
        m_PoolDict.Clear();
    }
}