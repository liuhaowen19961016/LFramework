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

    /// <summary>
    /// 屏幕坐标转UI坐标
    /// </summary>
    public static Vector2 Screen2UI(Vector2 screenPos, RectTransform rect, Camera uiCamera = null)
    {
        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCamera, out uiPos);
        return uiPos;
    }

    /// <summary>
    /// 世界坐标转UI坐标
    /// </summary>
    public static Vector2 World2UI(Vector3 worldPos, RectTransform rect, Camera worldCamera, Camera uiCamera)
    {
        Vector2 screenPos = World2Screen(worldPos, worldCamera);
        return Screen2UI(screenPos, rect, uiCamera);
    }
}