using System;

/// <summary>
/// MonoBehaviour管理器
/// </summary>
/// 实现不继承MonoBehaviour的类去执行MonoBehaviour生命周期中的方法
public class MonoBehaviourMgr : MonoSingleton<MonoBehaviourMgr>
{
    private MonoBehaviourMgr()
    {

    }

    private event Action FixedUpdaetEvent;
    private event Action UpdateEvent;
    private event Action LateUpdateEvent;

    private bool m_IsEnableFixedUpdate = true;//是否开启FixedUpdate
    private bool m_IsEnableUpdate = true;//是否开启Update
    private bool m_IsEnableLateUpdate = true;//是否开启LateUpdate

    /// <summary>
    /// 设置是否开启FixedUpdate
    /// </summary>
    public void SetIsEnableFixedUpdate(bool enable)
    {
        m_IsEnableFixedUpdate = enable;
    }

    /// <summary>
    /// 设置是否开启Update
    /// </summary>
    public void SetIsEnableUpdate(bool enable)
    {
        m_IsEnableUpdate = enable;
    }

    /// <summary>
    /// 设置是否开启LateUpdate
    /// </summary>
    public void SetIsEnableLateUpdate(bool enable)
    {
        m_IsEnableLateUpdate = enable;
    }

    public void AddFixedUpdateEvent(Action fixedUpdateEvent)
    {
        FixedUpdaetEvent += fixedUpdateEvent;
    }

    public void RemoveFixedUpdateEvent(Action fixedUpdateEvent)
    {
        FixedUpdaetEvent -= fixedUpdateEvent;
    }

    public void RemoveAllFixedUpdateEvents()
    {
        FixedUpdaetEvent = null;
    }

    public void AddUpdateEvent(Action updateEvent)
    {
        UpdateEvent += updateEvent;
    }

    public void RemoveUpdateEvent(Action updateEvent)
    {
        UpdateEvent -= updateEvent;
    }

    public void RemoveAllUpdateEvents()
    {
        UpdateEvent = null;
    }

    public void AddLateUpdateEvent(Action lateUpdateEvent)
    {
        LateUpdateEvent += lateUpdateEvent;
    }

    public void RemoveLateUpdateEvent(Action lateUpdateEvent)
    {
        LateUpdateEvent -= lateUpdateEvent;
    }

    public void RemoveAllLateUpdateEvents()
    {
        LateUpdateEvent = null;
    }

    public void RemoveAllEvents()
    {
        RemoveAllFixedUpdateEvents();
        RemoveAllUpdateEvents();
        RemoveAllLateUpdateEvents();
    }

    private void FixedUpdate()
    {
        if (!m_IsEnableFixedUpdate)
        {
            return;
        }
        FixedUpdaetEvent?.Invoke();
    }

    private void Update()
    {
        if (!m_IsEnableUpdate)
        {
            return;
        }
        UpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        if (!m_IsEnableLateUpdate)
        {
            return;
        }
        LateUpdateEvent?.Invoke();
    }
}