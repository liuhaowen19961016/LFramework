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
    public static Vector2 Screen2UILocal(Vector2 screenPos, RectTransform rect, Camera uiCamera = null)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCamera, out Vector2 uiLocalPos);
        return uiLocalPos;
    }

    /// <summary>
    /// 世界坐标转UI局部坐标
    /// </summary>
    public static Vector2 World2UILocal(Vector3 worldPos, RectTransform rect, Camera worldCamera = null, Camera uiCamera = null)
    {
        Vector2 screenPos = World2Screen(worldPos, worldCamera);
        Vector2 uiLocalPos = Screen2UILocal(screenPos, rect, uiCamera);
        return uiLocalPos;
    }

    /// <summary>
    /// UI世界坐标转屏幕坐标
    /// </summary>
    public static Vector2 UIWorld2Screen(Vector3 worldPos, Camera uiCamera = null)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);
        return screenPos;
    }

    /// <summary>
    /// UI局部坐标转UI锚点坐标
    /// </summary>
    public static Vector2 UILocal2UIAnchor(Vector2 localPos, RectTransform parentRect, RectTransform rect)
    {
        Vector2 anchorMinPos = parentRect.rect.min + Vector2.Scale(rect.anchorMin, parentRect.rect.size);
        Vector2 rectMinPos = rect.rect.min + localPos;
        Vector2 offsetMin = rectMinPos - anchorMinPos;
        Vector2 anchorMaxPos = parentRect.rect.max - Vector2.Scale(Vector2.one - rect.anchorMax, parentRect.rect.size);
        Vector2 rectMaxPos = rect.rect.max + localPos;
        Vector2 offsetMax = rectMaxPos - anchorMaxPos;
        Vector2 sizeDelta = offsetMax - offsetMin;
        Vector2 anchoredPosition = offsetMin + Vector2.Scale(sizeDelta, rect.pivot);
        return anchoredPosition;
    }

    #endregion UI相关
}