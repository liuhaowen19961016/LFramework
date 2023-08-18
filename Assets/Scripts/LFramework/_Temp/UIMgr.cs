using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI基类
/// </summary>
public class BaseUI : MonoBehaviour
{
    protected object m_Data;

    public void Init(object data)
    {
        this.m_Data = data;
    }

    /// <summary>
    /// 打开UI时
    /// </summary>
    public virtual void OnView()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭UI时
    /// </summary>
    public virtual void OnDisView()
    {
        gameObject.SetActive(false);
    }
}

/// <summary>
/// UI在Canvas下的层级
/// </summary>
public enum EUILayer
{
    Hud = 0,
    Window,
    Popups,
    Guide,
    Top,
}

/// <summary>
/// UI管理器
/// </summary>
public class UIMgr : MonoSingleton<UIMgr>
{
    /// <summary>
    /// UI界面配置
    /// </summary>
    public class UIConfig
    {
        public string uiPath;
        public EUILayer uiLayer;
    }

    /// <summary>
    /// UI界面数据
    /// </summary>
    public class UIInfo
    {
        public GameObject go;
    }

    private Canvas m_UICanvas;//UI画布
    public Canvas UICanvas
    {
        get { return m_UICanvas; }
    }
    private Camera m_UICamera;//UI相机
    public Camera UICamera
    {
        get { return m_UICamera; }
    }
    private CanvasScaler m_UICanvasScaler;//CanvasScaler组件
    public CanvasScaler UICanvasScaler
    {
        get { return m_UICanvasScaler; }
    }
    private RectTransform m_UIContainer;//UI父节点
    public RectTransform UIContainer
    {
        get { return UIContainer; }
    }

    private Dictionary<string, UIConfig> m_UIPath2UIConfig = new Dictionary<string, UIConfig>();//根据配表缓存所有的界面配置 <UI路径 , UI界面配置>
    private Dictionary<EUILayer, Transform> m_Layer2Trans = new Dictionary<EUILayer, Transform>();//<UI层级 , 对应的节点>

    private Dictionary<string, UIInfo> m_UICache = new Dictionary<string, UIInfo>();//缓存当前已实例化的UI界面数据 <UI路径 , UI界面数据>

    #region 外部调用接口

    /// <summary>
    /// 初始化（游戏初始化时调用一次）
    /// </summary>
    /// <param name="referResolution">参照的分辨率</param>
    /// <param name="isLandscape">是否为横屏</param>
    public void Init(Vector2 referResolution, bool isLandscape)
    {
        //初始化数据
        InitData();
        //初始化UI相关的游戏物体和组件
        if (m_UICamera == null)
        {
            CreateUICamera();
        }
        if (m_UICanvas == null)
        {
            CreateUICanvas();
        }
        m_UICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        m_UICanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        m_UICanvasScaler.matchWidthOrHeight = isLandscape ? 1 : 0;
        m_UICanvasScaler.referenceResolution = referResolution;
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    public void Show(string uiPath, object data = null)
    {
        if (m_UICache.TryGetValue(uiPath, out UIInfo outUIInfo))
        {
            outUIInfo.go.transform.SetAsLastSibling();
            outUIInfo.go.GetComponent<BaseUI>().OnView();
        }
        else
        {
            GameObject uiGo = CreateUI(uiPath);
            if (uiGo != null)
            {
                outUIInfo = new UIInfo { go = uiGo };
                m_UICache.Add(uiPath, outUIInfo);
                outUIInfo.go.GetComponent<BaseUI>().Init(data);
                outUIInfo.go.GetComponent<BaseUI>().OnView();
            }
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public bool Close(string uiPath, bool isDestroy = true)
    {
        if (m_UICache.TryGetValue(uiPath, out UIInfo outUIInfo))
        {
            outUIInfo.go.GetComponent<BaseUI>().OnDisView();
            if (isDestroy)
            {
                Destroy(outUIInfo.go);
                outUIInfo.go = null;
                m_UICache.Remove(uiPath);
            }
            return true;
        }
        return false;
    }

    #endregion 外部调用接口

    /// <summary>
    /// 创建UI
    /// </summary>
    private GameObject CreateUI(string uiPath)
    {
        UIConfig uiConfig = GetUIConfig(uiPath);
        if (uiConfig == null)
        {
            Debug.LogError($"表里没有此ui，uiPath：{uiPath}");
            return null;
        }
        GameObject uiGo = Instantiate(Resources.Load<GameObject>(uiPath));
        uiGo.transform.SetParent(GetUILayerTrans(uiConfig.uiLayer));
        uiGo.transform.localScale = Vector3.one;
        uiGo.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        uiGo.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        uiGo.transform.localPosition = Vector3.zero;
        return uiGo;
    }

    /// <summary>
    /// 获取UI配置
    /// </summary>
    private UIConfig GetUIConfig(string uiPath)
    {
        if (m_UIPath2UIConfig.TryGetValue(uiPath, out UIConfig outUIConfig))
        {
            return outUIConfig;
        }
        return null;
    }

    /// <summary>
    /// 获取UI层级节点
    /// </summary>
    public Transform GetUILayerTrans(EUILayer uILayer)
    {
        if (m_Layer2Trans.TryGetValue(uILayer, out Transform outTrans))
        {
            return outTrans;
        }
        return null;
    }

    #region 私有方法

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void InitData()
    {
        string u1 = "Prefabs/UI/UI_Win_Game";
        m_UIPath2UIConfig.Add(u1, new UIConfig { uiPath = u1, uiLayer = (EUILayer)1 });
    }

    /// <summary>
    /// 创建UI画布
    /// </summary>
    private void CreateUICanvas()
    {
        //创建Canvas
        GameObject canvasGo = new GameObject("UICanvas");
        canvasGo.layer = LayerMask.NameToLayer("UI");
        m_UICanvas = canvasGo.AddComponent<Canvas>();
        m_UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        if (m_UICamera == null)
        {
            CreateUICamera();
        }
        m_UICanvas.worldCamera = m_UICamera;
        m_UICanvasScaler = canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(m_UICanvas);
        //创建EventSystem
        GameObject eventSystemGo = new GameObject("EventSystem");
        eventSystemGo.AddComponent<EventSystem>();
        eventSystemGo.AddComponent<StandaloneInputModule>();
        eventSystemGo.transform.SetParent(canvasGo.transform);
        //创建Canvas下的结构
        GameObject containerGo = new GameObject("UIContainer");
        containerGo.transform.SetParent(canvasGo.transform, false);
        m_UIContainer = containerGo.AddComponent<RectTransform>();
        m_UIContainer.anchorMax = Vector2.one;
        m_UIContainer.anchorMin = Vector2.zero;
        m_UIContainer.offsetMin = Vector2.zero;
        m_UIContainer.offsetMax = Vector2.zero;
        string[] layerNames = Enum.GetNames(typeof(EUILayer));
        for (int i = 0; i < layerNames.Length; i++)
        {
            GameObject layerGo = new GameObject(layerNames[i]);
            layerGo.transform.SetParent(m_UIContainer, false);
            layerGo.AddComponent<RectTransform>();
            layerGo.GetComponent<RectTransform>().anchorMax = Vector2.one;
            layerGo.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            layerGo.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            layerGo.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            m_Layer2Trans.Add((EUILayer)i, layerGo.transform);
        }
    }

    /// <summary>
    /// 创建UI相机
    /// </summary>
    private void CreateUICamera()
    {
        GameObject cameraGo = new GameObject("UICamera");
        m_UICamera = cameraGo.AddComponent<Camera>();
        m_UICamera.clearFlags = CameraClearFlags.Depth;
        m_UICamera.cullingMask = 1 << 5;//TODO：根据不同项目设置
        m_UICamera.orthographic = true;
        m_UICamera.depth = 0;//TODO：根据不同项目设置
                             //m_UICamera.orthographicSize = Camera.main.orthographicSize;
        DontDestroyOnLoad(cameraGo);
    }

    #endregion 私有方法
}