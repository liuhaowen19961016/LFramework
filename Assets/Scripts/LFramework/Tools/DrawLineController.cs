using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 画线控制器
/// </summary>
public class DrawLineController : MonoSingleton<DrawLineController>
{
    private bool m_EnableDraw = true;//是否开启绘制
    public bool EnableDraw
    {
        get { return m_EnableDraw; }
        set { m_EnableDraw = value; }
    }

    public bool openOptimize;//是否开启优化（顶点数增加会带来性能消耗）
    public int optimizeAddVertexCount;//优化后两个点之间添加的顶点数

    private bool m_IsMouseDown;//鼠标是否按下

    private Vector2 m_LastMousePos;//上一次的鼠标位置
    private List<LineRenderer> m_LineRendererList = new List<LineRenderer>();//存储所有的LineRenderer
    private LineRenderer m_CurLineRenderer;//当前的LineRenderer

    public float drawLineMinDist = 0.2f;//绘制线段的最小距离
    public LayerMask ignoreLayer;//忽略的层

    //线段参数
    public float lineWidth = 0.2f;//线的宽度
    public Material lineMat;//线的材质
    public int lineCapVertices;//线顶点平滑度
    public int lineCornerVertices;//线拐点顶点数
    public Color startColor;//线的颜色
    public Color endColor;//线的颜色

    private Action<LineRenderer> m_OnBeginDrawLine;//开始绘制的回调
    private Action<LineRenderer, Vector2, bool> m_OnMouseDrag;//鼠标拖拽时的回调
    private Action<LineRenderer, Vector2, Vector2> m_OnDrawLine;//绘制的回调（只有绘制的时候才会调用）
    private Action<LineRenderer> m_OnEndDrawLine;//结束绘制的回调

    public void RegisterOnBeginDrawLineCallback(Action<LineRenderer> onBeginDrawLine)
    {
        m_OnBeginDrawLine = onBeginDrawLine;
    }

    public void RegisterOnMouseDragCallback(Action<LineRenderer, Vector2, bool> onMouseDrag)
    {
        m_OnMouseDrag = onMouseDrag;
    }

    public void RegisterOnDrawLineCallback(Action<LineRenderer, Vector2, Vector2> onDrawLine)
    {
        m_OnDrawLine = onDrawLine;
    }

    public void RegisterOnEndDrawLineCallback(Action<LineRenderer> onEndDrawLine)
    {
        m_OnEndDrawLine = onEndDrawLine;
    }

    /// <summary>
    /// 创建LineRenderer
    /// </summary>
    private void CreateLineRenderer()
    {
        GameObject go = new GameObject("LineRenderer");
        go.transform.SetParent(transform);
        m_CurLineRenderer = go.AddComponent<LineRenderer>();
        m_CurLineRenderer.positionCount = 0;
        m_CurLineRenderer.startWidth = lineWidth;
        m_CurLineRenderer.endWidth = lineWidth;
        m_CurLineRenderer.material = lineMat;
        m_CurLineRenderer.numCapVertices = lineCapVertices;
        m_CurLineRenderer.numCornerVertices = lineCornerVertices;
        m_CurLineRenderer.useWorldSpace = false;
        m_CurLineRenderer.startColor = startColor;
        m_CurLineRenderer.endColor = endColor;
        m_LineRendererList.Add(m_CurLineRenderer);
    }

    private void Update()
    {
        if (!m_EnableDraw)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            BeginDrawLine();
            m_IsMouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndDrawLine();
            m_IsMouseDown = false;
        }
        if (m_IsMouseDown)
        {
            InDrawLine();
        }
    }

    /// <summary>
    /// 开始画线
    /// </summary>
    private void BeginDrawLine()
    {
        CreateLineRenderer();
        AddPosition(CTUtils.Screen2World(Input.mousePosition));

        m_OnBeginDrawLine?.Invoke(m_CurLineRenderer);
    }

    /// <summary>
    /// 画线中
    /// </summary>
    private void InDrawLine()
    {
        Vector2 curLineWorldPos = CTUtils.Screen2World(Input.mousePosition);
        Vector2 lastLineWorldPos = m_CurLineRenderer.GetPosition(m_CurLineRenderer.positionCount - 1);
        bool isMoving = curLineWorldPos != m_LastMousePos;
        m_OnMouseDrag?.Invoke(m_CurLineRenderer, curLineWorldPos, isMoving);
        if (Vector2.Distance(lastLineWorldPos, curLineWorldPos) >= drawLineMinDist && CanDraw(lastLineWorldPos, curLineWorldPos))
        {
            if (openOptimize)
            {
                //为两个点之间再添加多个点，让线段看起来更平滑
                optimizeAddVertexCount = optimizeAddVertexCount == 0 ? 1 : optimizeAddVertexCount;
                Vector2 deltaPos = (curLineWorldPos - lastLineWorldPos) / optimizeAddVertexCount;
                for (int i = 1; i <= optimizeAddVertexCount; i++)
                {
                    AddPosition(lastLineWorldPos + deltaPos * i);
                }
            }
            else
            {
                AddPosition(curLineWorldPos);
            }
            m_OnDrawLine?.Invoke(m_CurLineRenderer, lastLineWorldPos, curLineWorldPos);
        }
        m_LastMousePos = curLineWorldPos;
    }

    /// <summary>
    /// 结束画线
    /// </summary>
    private void EndDrawLine()
    {
        m_OnEndDrawLine?.Invoke(m_CurLineRenderer);

        if (m_CurLineRenderer.positionCount <= 1)
        {
            ClearLine(m_CurLineRenderer);
        }
        m_CurLineRenderer = null;
    }

    /// <summary>
    /// 向LineRenderer添加点
    /// </summary>
    private void AddPosition(Vector2 curLineWorldPos)
    {
        m_CurLineRenderer.SetPosition(m_CurLineRenderer.positionCount++, curLineWorldPos);
    }

    /// <summary>
    /// 能否画线
    /// </summary>
    private bool CanDraw(Vector2 lastPos, Vector2 nextPos)
    {
        //是否画在忽略的层上
        Vector2 dir = (nextPos - lastPos).normalized;
        float dist = (nextPos - lastPos).magnitude;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(lastPos, dir, dist, ignoreLayer.value);
        if (raycastHit2D.collider != null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 清除全部线
    /// </summary>
    public void ClearAllLine()
    {
        for (int i = m_LineRendererList.Count - 1; i >= 0; i--)
        {
            ClearLine(m_LineRendererList[i]);
        }
    }

    /// <summary>
    /// 清除线
    /// </summary>
    private void ClearLine(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 0;
        m_LineRendererList.Remove(lineRenderer);
        Destroy(lineRenderer.gameObject);
    }

    /// <summary>
    /// 设置是否开启绘制
    /// </summary>
    public void SetEnableDraw(bool isEnable)
    {
        m_EnableDraw = isEnable;
    }
}