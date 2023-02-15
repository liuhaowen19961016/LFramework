using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 对象池管理器
/// </summary>
public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    private Dictionary<Type, object> m_ClassObjectPoolDict = new Dictionary<Type, object>();//类对象池缓存
    private Dictionary<string, GameObjectPool> m_GameObjectPoolDict = new Dictionary<string, GameObjectPool>();//游戏物体对象池缓存

    #region 类对象池

    /// <summary>
    /// 获取或创建一个类对象池
    /// </summary>
    public ClassObjectPool<T> GetOrCreateClassObjectPool<T>(int capacity)
        where T : class, new()
    {
        Type type = typeof(T);
        if (m_ClassObjectPoolDict.TryGetValue(type, out object outPool))
        {
            if (outPool == null)
            {
                m_ClassObjectPoolDict.Remove(type);
                ClassObjectPool<T> pool = new ClassObjectPool<T>(capacity);
                m_ClassObjectPoolDict.Add(type, pool);
                return pool;
            }
            else
            {
                return outPool as ClassObjectPool<T>;
            }

        }
        else
        {
            ClassObjectPool<T> pool = new ClassObjectPool<T>(capacity);
            m_ClassObjectPoolDict.Add(type, pool);
            return pool;
        }
    }

    #endregion 类对象池

    #region 游戏物体对象池

    /// <summary>
    /// 获取或创建一个游戏物体对象池
    /// </summary>
    public GameObjectPool GetOrCreateGameObjectPool(GameObject prefab, int capacity, Transform parent)
    {
        GameObjectPool pool = null;
        if (!m_GameObjectPoolDict.TryGetValue(prefab.name, out pool))
        {
            pool = new GameObjectPool();
            pool.Init(prefab, capacity, parent);
            m_GameObjectPoolDict.Add(prefab.name, pool);
        }
        return pool;
    }

    #endregion 游戏物体对象池

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        m_ClassObjectPoolDict.Clear();
        m_GameObjectPoolDict.Clear();
    }
}
