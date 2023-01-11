using UnityEngine;

/// <summary>
/// 坐标转换工具类
/// </summary>
public static class CTUtils
{
    /// <summary>
    /// 世界坐标转屏幕坐标
    /// </summary>
    public static Vector2 World2Screen(Vector3 wp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToScreenPoint(wp);
    }

    /// <summary>
    /// 屏幕坐标转世界坐标
    /// </summary>
    public static Vector3 Screen2World(Vector3 sp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToWorldPoint(sp);
    }

    /// <summary>
    /// 世界坐标转视口坐标
    /// </summary>
    public static Vector2 World2Viewport(Vector3 wp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToViewportPoint(wp);
    }

    /// <summary>
    /// 视口坐标转世界坐标
    /// </summary>
    public static Vector3 Viewport2World(Vector3 vp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToWorldPoint(vp);
    }

    /// <summary>
    /// 屏幕坐标转视口坐标
    /// </summary>
    public static Vector2 Screen2Viewport(Vector2 sp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToViewportPoint(sp);
    }

    /// <summary>
    /// 视口坐标转屏幕坐标
    /// </summary>
    public static Vector2 Viewport2Screen(Vector2 vp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToScreenPoint(vp);
    }

    /// <summary>
    /// 屏幕坐标转UI坐标
    /// </summary>
    public static Vector2 Screen2UI(Vector2 sp, RectTransform rect, Camera camera = null)
    {
        Vector2 uiLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, sp, camera, out uiLocalPos);
        return uiLocalPos;
    }

    /// <summary>
    /// 世界坐标转UI坐标
    /// </summary>
    public static Vector2 World2UI(Vector3 wp, RectTransform rect, Camera uiCamera)
    {
        Vector2 screenPos = World2Screen(wp, uiCamera);
        return Screen2UI(screenPos, rect, uiCamera);
    }
}