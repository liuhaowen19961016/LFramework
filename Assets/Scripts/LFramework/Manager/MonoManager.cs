using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Mono管理器
/// </summary>
public class MonoManager : Singleton<MonoManager>
{
    private MonoManager()
    {

    }

    private MonoController m_MonoController;

    public override void Init()
    {
        base.Init();

        if (m_MonoController == null)
        {
            GameObject go = new GameObject(typeof(MonoController).Name);
            m_MonoController = go.AddComponent<MonoController>();
            GameObject.DontDestroyOnLoad(go);
        }
    }

    /// <summary>
    /// 添加FixedUpdate事件
    /// </summary>
    public void AddFixedUpdateListener(Action callback)
    {
        m_MonoController.AddFixedUpdateListener(callback);
    }

    /// <summary>
    /// 移除FixedUpdate事件
    /// </summary>
    public void RemoveFixedUpdateListener(Action callback)
    {
        m_MonoController.RemoveFixedUpdateListener(callback);
    }

    /// <summary>
    /// 移除所有FixedUpdate事件
    /// </summary>
    public void RemoveAllFixedUpdateListeners()
    {
        m_MonoController.RemoveAllFixedUpdateListeners();
    }

    /// <summary>
    /// 添加Update事件
    /// </summary>
    public void AddUpdateListener(Action callback)
    {
        m_MonoController.AddUpdateListener(callback);
    }

    /// <summary>
    /// 移除Update事件
    /// </summary>
    public void RemoveUpdateListener(Action callback)
    {
        m_MonoController.RemoveUpdateListener(callback);
    }

    /// <summary>
    /// 移除所有Update事件
    /// </summary>
    public void RemoveAllUpdateListeners()
    {
        m_MonoController.RemoveAllUpdateListeners();
    }

    /// <summary>
    /// 添加LateUpdate事件
    /// </summary>
    public void AddLateUpdateListener(Action callback)
    {
        m_MonoController.AddLateUpdateListener(callback);
    }

    /// <summary>
    /// 移除LateUpdate事件
    /// </summary>
    public void RemoveLateUpdateListener(Action callback)
    {
        m_MonoController.RemoveLateUpdateListener(callback);
    }

    /// <summary>
    /// 移除所有LateUpdate事件
    /// </summary>
    public void RemoveAllLateUpdateListeners()
    {
        m_MonoController.RemoveAllLateUpdateListeners();
    }


    /// <summary>
    /// 移除所有Update、FixedUpdate、LateUpdate事件
    /// </summary>
    public void RemoveAllListeners()
    {
        m_MonoController.RemoveAllListeners();
    }

    /// <summary>
    /// 开启协程
    /// </summary>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return m_MonoController.StartCoroutine(routine);
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    public void StopCoroutine(IEnumerator routine)
    {
        if (routine != null)
        {
            m_MonoController.StopCoroutine(routine);
        }
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    public void StopCoroutine(Coroutine routine)
    {
        if (routine != null)
        {
            m_MonoController.StopCoroutine(routine);
        }
    }

    /// <summary>
    /// 停止所有协程
    /// </summary>
    public void StopAllCoroutines()
    {
        m_MonoController.StopAllCoroutines();
    }

    /// <summary>
    /// 执行Mono的脚本
    /// </summary>
    private class MonoController : MonoBehaviour
    {
        private event Action UpdateEvent;
        private event Action FixedUpdaetEvent;
        private event Action LateUpdateEvent;

        public void FixedUpdate()
        {
            FixedUpdaetEvent?.Invoke();
        }

        public void Update()
        {
            UpdateEvent?.Invoke();
        }

        public void LateUpdate()
        {
            LateUpdateEvent?.Invoke();
        }

        public void AddFixedUpdateListener(Action callback)
        {
            FixedUpdaetEvent += callback;
        }

        public void RemoveFixedUpdateListener(Action callback)
        {
            FixedUpdaetEvent -= callback;
        }

        public void RemoveAllFixedUpdateListeners()
        {
            FixedUpdaetEvent = null;
        }

        public void AddUpdateListener(Action callback)
        {
            UpdateEvent += callback;
        }

        public void RemoveUpdateListener(Action callback)
        {
            UpdateEvent -= callback;
        }

        public void RemoveAllUpdateListeners()
        {
            UpdateEvent = null;
        }

        public void AddLateUpdateListener(Action callback)
        {
            LateUpdateEvent += callback;
        }

        public void RemoveLateUpdateListener(Action callback)
        {
            LateUpdateEvent -= callback;
        }

        public void RemoveAllLateUpdateListeners()
        {
            LateUpdateEvent = null;
        }

        public void RemoveAllListeners()
        {
            RemoveAllFixedUpdateListeners();
            RemoveAllUpdateListeners();
            RemoveAllLateUpdateListeners();
        }
    }
}