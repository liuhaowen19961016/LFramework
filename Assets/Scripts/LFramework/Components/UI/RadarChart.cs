using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// 雷达图组件
/// </summary>
public class RadarChart : MaskableGraphic
{
    protected RadarChart()
    {

    }

    /// <summary>
    /// 描边数据
    /// </summary>
    [Serializable]
    public class OutlineData
    {
        [SerializeField]
        public float width = 5;
        [SerializeField]
        public Color color = Color.red;
    }

    //Sprite图片
    [SerializeField]
    Sprite m_Sprite;
    public Sprite Sprite
    {
        get { return m_Sprite; }
    }

    //贴图
    public override Texture mainTexture
    {
        get
        {
            if (m_Sprite == null)
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }

            return m_Sprite.texture;
        }
    }

    //半径
    [SerializeField]
    float m_Radius = 100;

    //边数(几边形)
    [SerializeField]
    int m_SideCount;
    public int SideCount
    {
        get
        {
            m_SideCount = Mathf.Clamp(m_SideCount, 3, 65000);
            return m_SideCount;
        }
    }

    //是否显示雷达图内部
    [SerializeField]
    bool m_ShowInner = true;

    //是否显示雷达图描边
    [SerializeField]
    bool m_ShowOutline;

    //雷达图描边数据
    [SerializeField]
    OutlineData m_OutlineData;

    //比例值列表
    List<float> m_RatioList = new List<float>();

    //顶点位置列表
    List<Vector3> m_TempVertexList = new List<Vector3>();
    List<Vector3> m_VertexList = new List<Vector3>();

    //比例值变化后
    public Action OnRatioValueChanged;

    /// <summary>
    /// 初始化比例值列表
    /// </summary>
    void InitRatioList()
    {
        int ratioCount = m_RatioList.Count;
        if (ratioCount < SideCount)
        {
            for (int i = 0; i < SideCount - ratioCount; i++)
            {
                m_RatioList.Add(1);
            }
        }
    }

    /// <summary>
    /// 设置比例值列表
    /// </summary>
    public void SetRatioList(List<float> ratioList)
    {
        for (int i = 0; i < m_RatioList.Count; i++)
        {
            if (ratioList.Count - 1 >= i)
            {
                m_RatioList[i] = ratioList[i];
            }
        }
        SetVerticesDirty();
        CalcVertexPos();

        OnRatioValueChanged?.Invoke();
    }

    /// <summary>
    /// 设置雷达图
    /// </summary>
    public void SetRadarChart()
    {
        rectTransform.sizeDelta = new Vector2(m_Radius * 2, m_Radius * 2);
        InitRatioList();
    }

    /// <summary>
    /// 得到比例值列表
    /// </summary>
    public List<float> GetRatioList()
    {
        return m_RatioList;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        m_TempVertexList.Clear();

        GenerateInner(vh);
        if (m_ShowOutline)
        {
            GenerateOutline(vh);
        }
    }

    /// <summary>
    /// 生成雷达图内部
    /// </summary>
    void GenerateInner(VertexHelper vh)
    {
        Vector4 uv = m_Sprite == null
             ? Vector4.zero
             : DataUtility.GetOuterUV(m_Sprite);
        float uvWidth = uv.z - uv.x;
        float uvHeight = uv.w - uv.y;
        float diameter = m_Radius * 2;
        Vector2 uvCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        Vector3 posCenter = new Vector2((0.5f - rectTransform.pivot.x) * diameter, (0.5f - rectTransform.pivot.y) * diameter);
        float uvScaleX = uvWidth / diameter;
        float uvScaleY = uvHeight / diameter;
        float deltaRad = 2 * Mathf.PI / SideCount;

        float curRad = 0;
        int vertexCount = SideCount + 1;
        int triangleCount = SideCount;

        UIVertex vertex = new UIVertex();
        vh.AddVert(posCenter, color, uvCenter);
        for (int i = 0; i < vertexCount - 1; i++)
        {
            float r = m_RatioList.Count <= i
                     ? m_Radius
                     : m_RatioList[i] == 0 ? m_Radius : m_Radius * m_RatioList[i];
            Vector3 posOffset = new Vector3(r * Mathf.Cos(curRad), r * Mathf.Sin(curRad));
            vertex.position = posCenter + posOffset;
            vertex.color = color;
            vertex.uv0 = new Vector2(uvCenter.x + posOffset.x * uvScaleX, uvCenter.y + posOffset.y * uvScaleY);
            vh.AddVert(vertex);
            m_TempVertexList.Add(vertex.position);

            curRad += deltaRad;
        }

        if (m_ShowInner)
        {
            for (int i = 0; i < triangleCount; i++)
            {
                vh.AddTriangle(0, i + 1, i + 2 >= vertexCount ? 1 : i + 2);
            }
        }
    }

    /// <summary>
    /// 生成雷达图描边
    /// </summary>
    void GenerateOutline(VertexHelper vh)
    {
        int vertexCount = m_TempVertexList.Count + 1;
        int triangleCount = m_TempVertexList.Count * 2;
        for (int i = 0; i < m_TempVertexList.Count; i++)
        {
            Vector2 curPos = m_TempVertexList[i];
            Vector2 prePos = i - 1 < 0 ? m_TempVertexList[m_TempVertexList.Count - 1] : m_TempVertexList[i - 1];
            Vector2 nextPos = m_TempVertexList[(i + 1) % m_TempVertexList.Count];
            Vector2 dir1 = (curPos - prePos).normalized;
            Vector2 dir2 = (curPos - nextPos).normalized;
            Vector2 normal1 = GetNormal(dir1);
            Vector2 normal2 = GetNormal(-dir2);
            Vector2 pos1 = prePos + normal1 * m_OutlineData.width;
            Vector2 pos2 = nextPos + normal2 * m_OutlineData.width;
            Vector2 crossPoint = GetCrossPoint(pos1, dir1, pos2, dir2);

            vh.AddVert(curPos, m_OutlineData.color, Vector2.zero);
            vh.AddVert(crossPoint, m_OutlineData.color, Vector2.zero);
        }

        for (int i = vertexCount; i < m_TempVertexList.Count * 3 + 1; i += 2)
        {
            vh.AddTriangle(i, i + 1, i + 3 >= m_TempVertexList.Count * 3 + 1 ? m_TempVertexList.Count + 2 : i + 3);
            vh.AddTriangle(i, i + 2 >= m_TempVertexList.Count * 3 + 1 ? m_TempVertexList.Count + 1 : i + 2, i + 3 >= m_TempVertexList.Count * 3 + 1 ? m_TempVertexList.Count + 2 : i + 3);
        }
    }

    /// <summary>
    /// 得到法线
    /// </summary>
    Vector2 GetNormal(Vector2 dir)
    {
        return new Vector2(dir.y, -dir.x);
    }

    //误差范围
    const float ERROR_RANGE = 0.001f;
    /// <summary>
    /// 得到交点
    /// </summary>
    Vector2 GetCrossPoint(Vector2 pos1, Vector2 dir1, Vector2 pos2, Vector2 dir2)
    {
        bool parallelToY1 = false;
        bool parallelToY2 = false;

        float k1;
        float k2;
        if (Mathf.Abs(dir1.x) <= ERROR_RANGE
            || Mathf.Abs(dir1.y) <= ERROR_RANGE)
        {
            k1 = 0;
            if (Mathf.Abs(dir1.x) <= ERROR_RANGE)
            {
                parallelToY1 = true;
            }
        }
        else
        {
            k1 = dir1.y / dir1.x;
        }
        if (Mathf.Abs(dir2.x) <= ERROR_RANGE
            || Mathf.Abs(dir2.y) <= ERROR_RANGE)
        {
            k2 = 0;
            if (Mathf.Abs(dir2.x) <= ERROR_RANGE)
            {
                parallelToY2 = true;
            }
        }
        else
        {
            k2 = dir2.y / dir2.x;
        }
        float b1 = pos1.y - k1 * pos1.x;
        float b2 = pos2.y - k2 * pos2.x;
        if (parallelToY1)
        {
            float x = pos1.x;
            float y = k2 * x + b2;
            return new Vector2(x, y);
        }
        else if (parallelToY2)
        {
            float x = pos2.x;
            float y = k1 * x + b1;
            return new Vector2(x, y);
        }
        else
        {
            float x = (b2 - b1) / (k1 - k2);
            float y = k1 * x + b1;
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// 计算顶点位置
    /// </summary>
    void CalcVertexPos()
    {
        m_VertexList.Clear();
        float diameter = m_Radius * 2;
        Vector3 posCenter = new Vector2((0.5f - rectTransform.pivot.x) * diameter, (0.5f - rectTransform.pivot.y) * diameter);
        float deltaRad = 2 * Mathf.PI / SideCount;

        float curRad = 0;
        for (int i = 0; i < SideCount; i++)
        {
            float r = m_RatioList.Count <= i
                     ? m_Radius
                     : m_RatioList[i] == 0 ? m_Radius : m_Radius * m_RatioList[i];
            Vector3 pos = posCenter + new Vector3(r * Mathf.Cos(curRad), r * Mathf.Sin(curRad));
            m_VertexList.Add(pos);

            curRad += deltaRad;
        }
    }
}