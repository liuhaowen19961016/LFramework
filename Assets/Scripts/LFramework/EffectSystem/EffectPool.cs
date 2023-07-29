using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 特效对象池
/// </summary>
public class EffectPool
{
    private Dictionary<EEffectType, ObjectPool<IEffect>> m_EffectDict;//特效类对象池字典
    private Dictionary<string, ObjectPool<GameObject>> m_EffectGoDict;//特效实体对象池字典

    /// <summary>
    /// 初始化
    /// </summary>
    public EffectPool()
    {
        m_EffectDict = new Dictionary<EEffectType, ObjectPool<IEffect>>();
        m_EffectGoDict = new Dictionary<string, ObjectPool<GameObject>>();

        //手动注册所有EEffectType的特效类
        m_EffectDict.Add(EEffectType.Common,
            new ObjectPool<IEffect>(onCreate: () => { return new EffectCommon(); }, onGet: OnGetEffect, onPut: OnPutEffect));
    }

    #region 特效类

    /// <summary>
    /// 获取特效类
    /// </summary>
    public IEffect GetEffect(EEffectType effectType)
    {
        if (m_EffectDict.TryGetValue(effectType, out ObjectPool<IEffect> pool))
        {
            return pool.Get();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 将特效类放回池子
    /// </summary>
    public bool PutEffect(IEffect effect)
    {
        if (effect == null || m_EffectDict == null)
        {
            return false;
        }
        if (m_EffectDict.TryGetValue(effect.EffectType, out ObjectPool<IEffect> pool))
        {
            return pool.Put(effect);
        }
        else
        {
            return false;
        }
    }

    private void OnGetEffect(IEffect effect)
    {
        if (effect == null)
        {
            return;
        }
        effect.OnGet();
    }

    private void OnPutEffect(IEffect effect)
    {
        if (effect == null)
        {
            return;
        }
        effect.OnPut();
    }

    #endregion 特效类

    #region 特效实体

    /// <summary>
    /// 获取特效实体
    /// </summary>
    public GameObject GetEffectGo(string effectPath)
    {
        if (m_EffectGoDict.TryGetValue(effectPath, out ObjectPool<GameObject> pool))
        {
            return pool.Get();
        }
        else
        {
            pool = new ObjectPool<GameObject>(
                onCreate: () => OnCreateEffectGo(effectPath),
                onGet: OnGetEffectGo,
                onPut: OnPutEffectGo,
                onDestroy: OnDestroyEffectGo);
            m_EffectGoDict.Add(effectPath, pool);
            return pool.Get();
        }
    }

    /// <summary>
    /// 回收特效
    /// </summary>
    public bool PutEffectGo(GameObject effectGo)
    {
        if (effectGo == null)
        {
            return false;
        }
        if (m_EffectGoDict.TryGetValue(effectGo.name, out ObjectPool<GameObject> pool))
        {
            return pool.Put(effectGo);
        }
        else
        {
            return false;
        }
    }

    private GameObject OnCreateEffectGo(string path)
    {
        GameObject effectPrefab = Resources.Load<GameObject>(path);
        if (effectPrefab == null)
        {
            return null;
        }
        GameObject effectGo = GameObject.Instantiate(effectPrefab);
        effectGo.transform.SetParent(EffectMgr.Ins.EffectRoot.transform);
        if (effectGo == null)
        {
            Debug.LogError($"特效实例化失败：{path}");
        }
        else
        {
            effectGo.name = path;
        }
        return effectGo;
    }

    private void OnGetEffectGo(GameObject effectGo)
    {
        if (effectGo == null)
        {
            return;
        }
        effectGo.SetActive(true);
    }

    private void OnPutEffectGo(GameObject effectGo)
    {
        if (effectGo == null)
        {
            return;
        }
        effectGo.Reset(isActive: false, parent: EffectMgr.Ins.EffectRoot.transform);
    }

    private void OnDestroyEffectGo(GameObject effectGo)
    {
        if (effectGo == null)
        {
            return;
        }
        GameObject.Destroy(effectGo);
    }

    #endregion 特效实体

    /// <summary>
    /// 释放所有
    /// </summary>
    public void DisposeAll()
    {
        if (m_EffectDict != null)
        {
            foreach (var pairs in m_EffectDict)
            {
                pairs.Value.Dispose();
            }
            m_EffectDict.Clear();
            m_EffectDict = null;
        }
        if (m_EffectGoDict != null)
        {
            foreach (var pairs in m_EffectGoDict)
            {
                pairs.Value.Dispose();
            }
            m_EffectGoDict.Clear();
            m_EffectGoDict = null;
        }
    }
}
