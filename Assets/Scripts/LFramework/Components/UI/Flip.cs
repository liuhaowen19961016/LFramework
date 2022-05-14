using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 翻转
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("LFramework/UI/Effects/Flip", 3)]
public class Flip : BaseMeshEffect
{
    protected Flip()
    {

    }

    /// <summary>
    /// 翻转类型
    /// </summary>
    public enum EFlipType
    {
        Horizontal,
        Vertical,
        HorizontalAndVertical,
    }

    //翻转类型
    [SerializeField]
    EFlipType m_FlipType;
    public EFlipType FlipType
    {
        get
        {
            return m_FlipType;
        }
        set
        {
            m_FlipType = value;
            graphic.SetVerticesDirty();
        }
    }

    //顶点缓存
    List<UIVertex> vertexCache = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        vh.GetUIVertexStream(vertexCache);

        ApplyFlip(vertexCache, graphic.rectTransform.rect.center);

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexCache);
        vertexCache.Clear();
    }

    void ApplyFlip(List<UIVertex> vertexCache, Vector2 pivot)
    {
        int vertexCount = vertexCache.Count;
        for (int i = 0; i < vertexCount; i++)
        {
            UIVertex veretx = vertexCache[i];
            if (m_FlipType == EFlipType.HorizontalAndVertical)
            {
                veretx.position.x = 2 * pivot.x - veretx.position.x;
                veretx.position.y = 2 * pivot.y - veretx.position.y;
            }
            else if (m_FlipType == EFlipType.Horizontal)
            {
                veretx.position.x = 2 * pivot.x - veretx.position.x;
            }
            else if (m_FlipType == EFlipType.Vertical)
            {
                veretx.position.y = 2 * pivot.y - veretx.position.y;
            }
            vertexCache[i] = veretx;
        }
    }
}