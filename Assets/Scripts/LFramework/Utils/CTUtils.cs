using UnityEngine;

/// <summary>
/// 坐标转换工具类
/// </summary>
public static class CTUtils
{
    /// <summary>
    /// 世界坐标转屏幕坐标
    /// </summary>
    public static Vector2 World2Screen(Vector3 worldPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToScreenPoint(worldPos);
    }

    /// <summary>
    /// 屏幕坐标转世界坐标
    /// </summary>
    public static Vector3 Screen2World(Vector3 screenPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// 世界坐标转视口坐标
    /// </summary>
    public static Vector2 World2Viewport(Vector3 worldPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToViewportPoint(worldPos);
    }

    /// <summary>
    /// 视口坐标转世界坐标
    /// </summary>
    public static Vector3 Viewport2World(Vector3 viewPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToWorldPoint(viewPos);
    }

    /// <summary>
    /// 屏幕坐标转视口坐标
    /// </summary>
    public static Vector2 Screen2Viewport(Vector2 screenPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToViewportPoint(screenPos);
    }

    /// <summary>
    /// 视口坐标转屏幕坐标
    /// </summary>
    public static Vector2 Viewport2Screen(Vector2 viewPos, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToScreenPoint(viewPos);
    }

    #region UI相关

    /// <summary>
    /// 屏幕坐标转UI局部坐标
    /// </summary>
    public static Vector2 Screen2UI(Vector2 screenPos, RectTransform rect)
    {
        Camera uiCamera = GameObject.Find("UICanvas").GetComponent<Canvas>().worldCamera;//TODO
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCamera, out Vector2 uiPos);
        return uiPos;
    }

    /// <summary>
    /// 世界坐标转UI坐标
    /// </summary>
    public static Vector2 World2UI(Vector3 worldPos, RectTransform rect, Camera worldCamera = null)
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
        Vector2 screenPos = World2Screen(worldPos, worldCamera);
        return Screen2UI(screenPos, rect);
    }

    /// <summary>
    /// UI世界坐标转屏幕坐标
    /// </summary>
    public static Vector2 UIWorld2Screen(Vector3 v)
    {
        Canvas uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();//TODO
        Camera uiCamera = uiCanvas.worldCamera;
        if (uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay)//transform.position = 屏幕坐标
        {
            return v;
        }
        else if (uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            return World2Screen(v, uiCamera);
        }
        else if (uiCanvas.renderMode == RenderMode.WorldSpace)
        {
            return World2Screen(v, uiCamera);
        }
        return Vector2.zero;
    }

    /// <summary>
    /// UI世界坐标转世界坐标
    /// </summary>
    public static Vector3 UIWorld2World(Vector2 v)
    {
        Vector2 screenPos = UIWorld2Screen(v);
        return Screen2World(screenPos);
    }

    #endregion UI相关
}