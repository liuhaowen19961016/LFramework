using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏物体对象池
/// </summary>
public class GameObjectPool
{
    private string m_GoKey;//游戏物体唯一key
    private int m_Capacity;//容量（-1为无限容量）
    public GameObject m_Prefab;//预制体
    private Transform m_Parent;//父物体
    private List<GameObject> m_GoList_Active = new List<GameObject>();//游戏物体列表（激活的）
    private List<GameObject> m_GoList_Inactive = new List<GameObject>();//游戏物体列表（未激活的）

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(GameObject prefab, int capacity, Transform parent)
    {
        m_GoKey = prefab.name + "[pool]";
        m_Capacity = capacity;
        m_Prefab = prefab;
        m_Parent = parent;
        for (int i = 0; i < capacity; i++)
        {
            Instantiate();
        }
    }

    /// <summary>
    /// 实例化
    /// </summary>
    private GameObject Instantiate()
    {
        GameObject go = null;
        if (m_Prefab == null)
        {
            Debug.LogError($"无法实例化，m_Prefab为null");
            return go;
        }
        go = GameObject.Instantiate(m_Prefab);
        if (go == null)
        {
            Debug.LogError($"实例化游戏物体失败，m_Prefab：{m_Prefab}");
            return go;
        }
        go.transform.SetParent(m_Parent, false);
        go.name = m_GoKey;
        m_GoList_Inactive.Add(go);
        go.SetActive(false);
        return go;
    }

    /// <summary>
    /// 从池子中取
    /// </summary>
    public GameObject Get()
    {
        GameObject go = null;
        if (m_GoList_Inactive.Count <= 0)
        {
            go = Instantiate();
        }
        else
        {
            go = m_GoList_Inactive[0];
        }
        m_GoList_Inactive.Remove(go);
        m_GoList_Active.Add(go);
        go.SetActive(true);
        return go;
    }

    /// <summary>
    /// 放回池子
    /// </summary>
    public bool Put(GameObject go)
    {
        bool ret = false;
        if (m_GoList_Inactive.Contains(go))
        {
            return ret;
        }
        if (!m_GoList_Active.Contains(go))
        {
            Debug.LogError($"无法放入对象池，此游戏物体不由对象池管理或此游戏物体与池子中的游戏物体不是同一个GameObject，\n此游戏物体：{go.name}，池子中游戏物体：{m_Prefab.name}[pool]");
            GameObject.Destroy(go);
        }
        else
        {
            m_GoList_Active.Remove(go);
            if (m_Capacity > 0 && m_GoList_Inactive.Count >= m_Capacity)
            {
                GameObject.Destroy(go);
            }
            else
            {
                m_GoList_Inactive.Add(go);
                go.SetActive(false);
                go.GetComponent<PoolObject>()?.Reset();
                ret = true;
            }
        }
        return ret;
    }

    /// <summary>
    /// 将所有激活的放回池子
    /// </summary>
    public void PutAll()
    {
        for (int i = m_GoList_Active.Count - 1; i >= 0; i--)
        {
            Put(m_GoList_Active[i]);
        }
    }
}

/// <summary>
/// 池子物体
/// </summary>
public class PoolObject : MonoBehaviour
{
    public virtual void Reset() { }
}
