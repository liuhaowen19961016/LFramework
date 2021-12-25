using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 多边形Image组件
/// </summary>
[AddComponentMenu("LFramework/UI/PolygonImage", 12)]
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonImage : Image
{
    /// <summary>
    /// 2D多边形碰撞器
    /// </summary>
    PolygonCollider2D m_polygonCollider2D;

    protected override void Awake()
    {
        m_polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out worldPos);
        return m_polygonCollider2D.OverlapPoint(worldPos);
    }
}
