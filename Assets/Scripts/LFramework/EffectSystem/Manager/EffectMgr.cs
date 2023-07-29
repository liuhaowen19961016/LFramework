using UnityEngine;

/// <summary>
/// 特效管理器
/// </summary>
/// 所有特效都配置到一个总的特效表中
public class EffectMgr : MonoSingleton<EffectMgr>
{
    private EffectPool m_EffectPool;//特效对象池
    public EffectPool EffectPool
    {
        get
        {
            return m_EffectPool;
        }
    }

    private GameObject m_EffectRoot;//特效挂点
    public GameObject EffectRoot
    {
        get
        {
            return m_EffectRoot;
        }
    }

    private void Awake()
    {
        m_EffectPool = new EffectPool();
        m_EffectRoot = UnityUtils.GetOrAddGameObjectRoot("EffectRoot");
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    public IEffect PlayEffect(int effectId)
    {
        EffectDefine effectDefine = GetEffectDefine(effectId);
        if (effectDefine == null)
        {
            Debug.LogError($"特效表中找不到此特效配置：{effectId}");
            return null;
        }
        IEffect effect = m_EffectPool.GetEffect((EEffectType)effectDefine.effectType);
        if (effect == null)
        {
            Debug.LogError($"特效类获取失败：{effectId}");
            return null;
        }
        effect.Init(effectDefine);
        return effect;
    }

    /// <summary>
    /// 获取特效配置
    /// </summary>
    private EffectDefine GetEffectDefine(int effectId)
    {
        //TODO
        return new EffectDefine()
        {
            effectId = 1,
            effectPath = "effect1",
            effectType = (int)EEffectType.Common,
        };
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void Dispose()
    {
        m_EffectPool.DisposeAll();
    }
}

//TODO：test
public class EffectDefine
{
    public int effectId;
    public string effectPath;
    public int effectType;
}