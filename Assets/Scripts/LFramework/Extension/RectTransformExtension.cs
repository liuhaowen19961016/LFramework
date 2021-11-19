using UnityEngine;

/// <summary>
/// RectTransform扩展类
/// </summary>
public static class RectTransformExtension 
{
    /// <summary>
    /// 设置Rect大小
    /// </summary>
    public static void SetRectTransformSize(this RectTransform rt, Vector2 size)
    {
        Vector2 oldSize = rt.rect.size;
        Vector2 deltaSize = size - oldSize;
        rt.offsetMin = rt.offsetMin - new Vector2(deltaSize.x * rt.pivot.x, deltaSize.y * rt.pivot.y);
        rt.offsetMax = rt.offsetMax + new Vector2(deltaSize.x * (1f - rt.pivot.x), deltaSize.y * (1f - rt.pivot.y));
    }
}
