using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Text顶点索引顺序
    5-0 ---- 1
    | \    |
    |  \   |
    |   \  |
    |    \ |
    4-----3-2
*/
/// <summary>
/// 渐变
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("LFramework/UI/Effects/Gradient", 1)]
public class Gradient : BaseMeshEffect
{
    protected Gradient()
    {

    }

    /// <summary>
    /// 渐变方向
    /// </summary>
    public enum EGradientDir
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft,
    }

    /// <summary>
    /// 渐变方向
    /// </summary>
    [SerializeField]
    EGradientDir m_GradientDir = EGradientDir.TopToBottom;
    public EGradientDir GradientDir
    {
        get
        {
            return m_GradientDir;
        }
        set
        {
            m_GradientDir = value;
            graphic.SetVerticesDirty();
        }
    }

    //颜色数组
    [SerializeField]
    Color32[] m_ColorArray = new Color32[2] { Color.black, Color.white };
    public Color32[] ColorArray
    {
        get
        {
            return m_ColorArray;
        }
        set
        {
            m_ColorArray = value;
            graphic.SetVerticesDirty();
        }
    }

    //顶点缓存
    List<UIVertex> m_VertexCache = new List<UIVertex>();
    //绘制使用的顶点列表
    List<UIVertex> m_VertexList = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        vh.GetUIVertexStream(m_VertexCache);

        switch (m_GradientDir)
        {
            case EGradientDir.TopToBottom:
                ApplyGradient_TopToBottom(m_VertexCache);
                break;
            case EGradientDir.BottomToTop:
                ApplyGradient_BottomToTop(m_VertexCache);
                break;
            case EGradientDir.LeftToRight:
                ApplyGradient_LeftToRight(m_VertexCache);
                break;
            case EGradientDir.RightToLeft:
                ApplyGradient_RightToLeft(m_VertexCache);
                break;
            default:
                break;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_VertexList);
        m_VertexCache.Clear();
        m_VertexList.Clear();
    }

    void ApplyGradient_TopToBottom(List<UIVertex> vertexCache)
    {
        if (vertexCache.Count == 0)
        {
            return;
        }
        if (m_ColorArray.Length < 2)
        {
            return;
        }

        int vertexCountPer = 6;//每一个文字的顶点数
        int vertexCount = vertexCache.Count;
        int colorCount = m_ColorArray.Length;
        for (int n = 0; n < vertexCount / 6; n++)
        {
            UIVertex lastVertexLB = new UIVertex();
            UIVertex lastVertexRB = new UIVertex();
            for (int i = 0; i < colorCount - 1; i++)
            {
                UIVertex vertexRT;
                UIVertex vertexLT;
                UIVertex vertexRB;
                UIVertex vertexLB;

                //左上角和右上角
                if (i == 0)
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], m_ColorArray[i]);
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], m_ColorArray[i]);
                }
                else
                {
                    vertexLT = lastVertexLB;
                    vertexRT = lastVertexRB;
                }

                //左下角和右下角
                if (i == colorCount - 2)
                {
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], m_ColorArray[i + 1]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], m_ColorArray[i + 1]);
                }
                else
                {
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], vertexCache[n * vertexCountPer + 0],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], vertexCache[n * vertexCountPer + 1],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                }

                lastVertexLB = vertexLB;
                lastVertexRB = vertexRB;

                m_VertexList.Add(vertexLT);
                m_VertexList.Add(vertexRT);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexLB);
                m_VertexList.Add(vertexLT);
            }
        }
    }

    void ApplyGradient_BottomToTop(List<UIVertex> vertexCache)
    {
        if (vertexCache.Count == 0)
        {
            return;
        }
        if (m_ColorArray.Length < 2)
        {
            return;
        }

        int vertexCountPer = 6;//每一个文字的顶点数
        int vertexCount = vertexCache.Count;
        int colorCount = m_ColorArray.Length;
        for (int n = 0; n < vertexCount / 6; n++)
        {
            UIVertex lastVertexLT = new UIVertex();
            UIVertex lastVertexRT = new UIVertex();
            for (int i = 0; i < colorCount - 1; i++)
            {
                UIVertex vertexRT;
                UIVertex vertexLT;
                UIVertex vertexRB;
                UIVertex vertexLB;

                //左下角和右下角
                if (i == 0)
                {
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], m_ColorArray[i]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], m_ColorArray[i]);
                }
                else
                {
                    vertexLB = lastVertexLT;
                    vertexRB = lastVertexRT;
                }

                //左上角和右上角
                if (i == colorCount - 2)
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], m_ColorArray[i + 1]);
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], m_ColorArray[i + 1]);
                }
                else
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], vertexCache[n * vertexCountPer + 4],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], vertexCache[n * vertexCountPer + 2],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                }

                lastVertexLT = vertexLT;
                lastVertexRT = vertexRT;

                m_VertexList.Add(vertexLT);
                m_VertexList.Add(vertexRT);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexLB);
                m_VertexList.Add(vertexLT);
            }
        }
    }

    void ApplyGradient_LeftToRight(List<UIVertex> vertexCache)
    {
        if (vertexCache.Count == 0)
        {
            return;
        }
        if (m_ColorArray.Length < 2)
        {
            return;
        }

        int vertexCountPer = 6;//每一个文字的顶点数
        int vertexCount = vertexCache.Count;
        int colorCount = m_ColorArray.Length;
        for (int n = 0; n < vertexCount / 6; n++)
        {
            UIVertex lastVertexRT = new UIVertex();
            UIVertex lastVertexRB = new UIVertex();
            for (int i = 0; i < colorCount - 1; i++)
            {
                UIVertex vertexRT;
                UIVertex vertexLT;
                UIVertex vertexRB;
                UIVertex vertexLB;

                //左上角和左下角
                if (i == 0)
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], m_ColorArray[i]);
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], m_ColorArray[i]);
                }
                else
                {
                    vertexLT = lastVertexRT;
                    vertexLB = lastVertexRB;
                }

                //右上角和右下角
                if (i == colorCount - 2)
                {
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], m_ColorArray[i + 1]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], m_ColorArray[i + 1]);
                }
                else
                {
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], vertexCache[n * vertexCountPer + 0],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], vertexCache[n * vertexCountPer + 4],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                }

                lastVertexRT = vertexRT;
                lastVertexRB = vertexRB;

                m_VertexList.Add(vertexLT);
                m_VertexList.Add(vertexRT);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexLB);
                m_VertexList.Add(vertexLT);
            }
        }
    }

    void ApplyGradient_RightToLeft(List<UIVertex> vertexCache)
    {
        if (vertexCache.Count == 0)
        {
            return;
        }
        if (m_ColorArray.Length < 2)
        {
            return;
        }

        int vertexCountPer = 6;//每一个文字的顶点数
        int vertexCount = vertexCache.Count;
        int colorCount = m_ColorArray.Length;
        for (int n = 0; n < vertexCount / 6; n++)
        {
            UIVertex lastVertexLT = new UIVertex();
            UIVertex lastVertexLB = new UIVertex();
            for (int i = 0; i < colorCount - 1; i++)
            {
                UIVertex vertexRT;
                UIVertex vertexLT;
                UIVertex vertexRB;
                UIVertex vertexLB;

                //右上角和右下角
                if (i == 0)
                {
                    vertexRT = CalcVertex(vertexCache[n * vertexCountPer + 1], m_ColorArray[i]);
                    vertexRB = CalcVertex(vertexCache[n * vertexCountPer + 2], m_ColorArray[i]);
                }
                else
                {
                    vertexRT = lastVertexLT;
                    vertexRB = lastVertexLB;
                }

                //左上角和左下角
                if (i == colorCount - 2)
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], m_ColorArray[i + 1]);
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], m_ColorArray[i + 1]);
                }
                else
                {
                    vertexLT = CalcVertex(vertexCache[n * vertexCountPer + 0], vertexCache[n * vertexCountPer + 1],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                    vertexLB = CalcVertex(vertexCache[n * vertexCountPer + 4], vertexCache[n * vertexCountPer + 2],
                        (colorCount - i - 2) * 1f / (colorCount - 1), m_ColorArray[i + 1]);
                }

                lastVertexLT = vertexLT;
                lastVertexLB = vertexLB;

                m_VertexList.Add(vertexLT);
                m_VertexList.Add(vertexRT);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexRB);
                m_VertexList.Add(vertexLB);
                m_VertexList.Add(vertexLT);
            }
        }
    }

    /// <summary>
    /// 计算顶点数据(只计算颜色)
    /// </summary>
    UIVertex CalcVertex(UIVertex vertex, Color32 color)
    {
        vertex.color = color;
        return vertex;
    }

    /// <summary>
    /// 计算顶点数据
    /// </summary>
    UIVertex CalcVertex(UIVertex vertexA, UIVertex vertexB, float ratio, Color32 color)
    {
        UIVertex vertexTemp = new UIVertex();
        vertexTemp.position = (vertexB.position - vertexA.position) * ratio + vertexA.position;
        vertexTemp.color = color;
        vertexTemp.normal = (vertexB.normal - vertexA.normal) * ratio + vertexA.normal;
        vertexTemp.tangent = (vertexB.tangent - vertexA.tangent) * ratio + vertexA.tangent;
        vertexTemp.uv0 = (vertexB.uv0 - vertexA.uv0) * ratio + vertexA.uv0;
        vertexTemp.uv1 = (vertexB.uv1 - vertexA.uv1) * ratio + vertexA.uv1;
        return vertexTemp;
    }
}