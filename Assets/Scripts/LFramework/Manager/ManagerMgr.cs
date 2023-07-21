using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理器的总管理器
/// </summary>
/// 需要控制FixedUpdate、Update、LateUpdate顺序的需求时使用ManagerMgr统一管理，每个Manager继承自ManagerBase
/// 其他Mangaer可以直接继承MonoSingleton
public class ManagerMgr : MonoSingleton<ManagerMgr>
{
    private List<IManager> m_ManagerRegisterList = new List<IManager>();//管理器注册列表
    private List<IManager> m_ManagerInitedList = new List<IManager>();//初始化过的管理器列表

    private List<IFixedUpdate> m_FixedUpdateList = new List<IFixedUpdate>();
    private List<IUpdate> m_UpdateList = new List<IUpdate>();
    private List<ILateUpdate> m_LateUpdateList = new List<ILateUpdate>();
    private List<ISlowUpdate> m_SlowUpdateList = new List<ISlowUpdate>();

    private const float SLOW_DELTATIME = 1f;//SlowUpdate的间隔
    private float m_LastSlowUpdateTime;//上一次SlowUpdate的时间

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        //手动注册所有管理器
        //RegisterManager(Manager1.Ins);
        //RegisterManager(Manager2.Ins);
        //.....

        InitManagerList();
        m_LastSlowUpdateTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// 注册管理器
    /// </summary>
    private void RegisterManager<T>(ManagerBase<T> manager, EManagerWeight managerWeight = EManagerWeight.High, EManagerType managerType = EManagerType.Global)
        where T : ManagerBase<T>
    {
        manager.Register(managerWeight, managerType);
        m_ManagerRegisterList.AddUnique(manager);
    }

    /// <summary>
    /// 初始化管理器列表
    /// </summary>
    private void InitManagerList()
    {
        m_ManagerRegisterList.Sort((x, y) => y.ManagerWeight.CompareTo(x.ManagerWeight));
        for (int i = 0; i < m_ManagerRegisterList.Count; i++)
        {
            IManager manager = m_ManagerRegisterList[i];
            if (manager is ISlowUpdate)
            {
                m_SlowUpdateList.Add((ISlowUpdate)manager);
            }
            if (manager is IFixedUpdate)
            {
                m_FixedUpdateList.Add((IFixedUpdate)manager);
            }
            if (manager is IUpdate)
            {
                m_UpdateList.Add((IUpdate)manager);
            }
            if (manager is ILateUpdate)
            {
                m_LateUpdateList.Add((ILateUpdate)manager);
            }
            m_ManagerRegisterList[i]?.OnInit();
            m_ManagerInitedList.Add(manager);
        }
    }

    private void FxiedUpdate()
    {
        if (m_FixedUpdateList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < m_FixedUpdateList.Count; i++)
        {
            m_FixedUpdateList[i].OnFixedUpdate(Time.deltaTime);
        }
    }

    private void Update()
    {
        if (m_UpdateList.Count > 0)
        {
            for (int i = 0; i < m_UpdateList.Count; i++)
            {
                m_UpdateList[i].OnUpdate(Time.deltaTime);
            }
        }
        if (m_SlowUpdateList.Count > 0)
        {
            if (Time.realtimeSinceStartup - m_LastSlowUpdateTime >= SLOW_DELTATIME)
            {
                for (int i = 0; i < m_SlowUpdateList.Count; i++)
                {
                    m_SlowUpdateList[i].OnSlowUpdate(SLOW_DELTATIME);
                }
                m_LastSlowUpdateTime += SLOW_DELTATIME;
            }
        }
    }

    private void LateUpdate()
    {
        if (m_LateUpdateList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < m_LateUpdateList.Count; i++)
        {
            m_LateUpdateList[i].OnLateUpdate(Time.deltaTime);
        }
    }

    public void OnPause(bool isPause)
    {
        if (m_ManagerInitedList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < m_ManagerInitedList.Count; i++)
        {
            m_ManagerInitedList[i].OnPause(isPause);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_ManagerRegisterList.Count; i++)
        {
            m_ManagerRegisterList[i].OnDestroy();
        }
        m_ManagerRegisterList.Clear();
        m_ManagerInitedList.Clear();
        m_FixedUpdateList.Clear();
        m_UpdateList.Clear();
        m_LateUpdateList.Clear();
        m_SlowUpdateList.Clear();
    }
}

/// <summary>
/// 管理器基类
/// </summary>
public abstract class ManagerBase<T> : Singleton<T>, IManager
    where T : class
{
    private EManagerWeight m_ManagerWeight;//管理器权重
    public EManagerWeight ManagerWeight
    {
        get
        {
            return m_ManagerWeight;
        }
    }

    private EManagerType m_ManagerType;//管理器类型
    public EManagerType ManagerType
    {
        get
        {
            return m_ManagerType;
        }
    }

    protected bool m_IsResister;//是否注册
    protected bool m_IsInit;//是否初始化

    /// <summary>
    /// 注册
    /// </summary>
    public void Register(EManagerWeight managerWeight = EManagerWeight.High, EManagerType managerType = EManagerType.Global)
    {
        m_ManagerWeight = managerWeight;
        m_ManagerType = managerType;
        m_IsResister = true;
    }

    public virtual void OnInit()
    {
        m_IsInit = true;
    }

    public virtual void OnDestroy()
    {

    }

    public virtual void OnPause(bool isPause)
    {

    }
}

/// <summary>
/// 管理器接口
/// </summary>
public interface IManager
{
    EManagerWeight ManagerWeight { get; }
    EManagerType ManagerType { get; }

    void OnInit();
    void OnDestroy();
    void OnPause(bool isPause);
}

public interface IFixedUpdate
{
    void OnFixedUpdate(float deltaTime);
}

public interface IUpdate
{
    void OnUpdate(float deltaTime);
}

public interface ILateUpdate
{
    void OnLateUpdate(float deltaTime);
}

public interface ISlowUpdate
{
    void OnSlowUpdate(float deltaTime);
}

/// <summary>
/// 管理器权重
/// </summary>
public enum EManagerWeight
{
    Low = 1,
    Middle = 2,
    High = 3,
}

/// <summary>
/// 管理器类型
/// </summary>
public enum EManagerType
{
    Global = 1,
}
