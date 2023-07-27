using UnityEngine;

/// <summary>
/// 特效基类
/// </summary>
public abstract class EffectBase : IEffect
{
    private EEffectType m_EffectType;//特效类型
    public EEffectType EffectType
    {
        get
        {
            return m_EffectType;
        }
    }

    private GameObject m_EffectGo;//特效实体
    public GameObject EffectGo
    {
        get
        {
            return m_EffectGo;
        }
    }

    private EffectDefine m_EffectDefine;//特效配置
    public EffectDefine EffectDefine
    {
        get
        {
            return m_EffectDefine;
        }
    }

    private float m_Duration;//持续时间
    public float Duration
    {
        get
        {
            return m_Duration;
        }
    }

    private TimerTask m_CompleteTimerTask;

    public bool Init(EffectDefine effectDefine)
    {
        m_EffectType = (EEffectType)effectDefine.effectType;
        m_EffectDefine = effectDefine;
        m_EffectGo = EffectMgr.Ins.EffectPool.GetEffectGo(effectDefine.effectPath);
        if (m_EffectGo == null)
        {
            Debug.LogError("特效实体为null");
            EffectMgr.Ins.EffectPool.PutEffect(this);
            return false;
        }
        else
        {
            m_Duration = GetDuration(m_EffectGo.transform);
            Debug.Log(m_Duration);
            m_CompleteTimerTask = TimerMgr.Ins.Register(m_Duration, onComplete: () =>
                {
                    EffectMgr.Ins.EffectPool.PutEffect(this);
                });
            return true;
        }
    }

    public void SetWorldPosition(Vector3 worldPos)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        m_EffectGo.transform.position = worldPos;
    }

    public void SetWorldRotation(Vector3 worldRotation)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        m_EffectGo.transform.rotation = Quaternion.Euler(worldRotation);
    }

    public void SetLocalPosition(Vector3 localPos)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        m_EffectGo.transform.localPosition = localPos;
    }

    public void SetLocalRotation(Vector3 localRotation)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        m_EffectGo.transform.localRotation = Quaternion.Euler(localRotation);
    }

    public void SetScale(Vector3 scale)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        m_EffectGo.transform.localScale = scale;
    }

    public void SetSpeed(float speed)
    {
        if (m_EffectGo == null)
        {
            return;
        }
    }

    public void SetLayer(int layer, bool inculdeAllChildren)
    {
        if (m_EffectGo == null)
        {
            return;
        }
    }

    public void SetOrder(int order)
    {
        if (m_EffectGo == null)
        {
            return;
        }
        Renderer[] renderers = m_EffectGo.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].sortingOrder = order;
        }
    }

    public void ChangeRoot(Transform parent)
    {
        if (m_EffectGo == null || parent == null)
        {
            return;
        }
        m_EffectGo.transform.SetParent(parent);
    }

    public void OnGet()
    {

    }

    public void OnPut()
    {
        EffectMgr.Ins.EffectPool.PutEffectGo(m_EffectGo);
        m_EffectType = EEffectType.Null;
        m_EffectGo = null;
        m_EffectDefine = null;
        m_CompleteTimerTask?.Dispose();
    }

    /// <summary>
    /// 获取持续时间
    /// </summary>
    public float GetDuration(Transform root)
    {
        //TODO
        float duration = 0;
        ParticleSystem rootParticleSystem = root.GetComponent<ParticleSystem>();
        if (rootParticleSystem != null)
        {
            duration = Mathf.Max(duration, rootParticleSystem.main.duration);
        }
        foreach (Transform trans in root)
        {
            ParticleSystem particleSystem = trans.GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                continue;
            }
            float tempDuration = particleSystem.main.duration;
            duration = Mathf.Max(duration, tempDuration);
            duration = Mathf.Max(duration, GetDuration(trans));
        }
        return duration;
    }
}