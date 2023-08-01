using UnityEngine;

/// <summary>
/// 粒子组件
/// </summary>
public class ParticleComponent : MonoBehaviour
{
    public ParticleSystem[] particleSystems;//所有的粒子特效

    private float m_Duration;//持续时间

    /// <summary>
    /// 获取持续时间
    /// </summary>
    private float GetDuration()
    {
        return m_Duration;
    }

    private void Awake()
    {
        UpdateComponentInfo();
    }

    [ContextMenu("更新组件数据")]
    private void UpdateComponentInfo()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
}
