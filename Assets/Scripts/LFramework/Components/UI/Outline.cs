using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 描边
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("LFramework/UI/Effects/Outline", 2)]
public class Outline : BaseMeshEffect
{
    protected Outline()
    {

    }

    //描边颜色
    [SerializeField]
    Color m_OutlineColor = Color.white;
    public Color OutlineColor
    {
        get
        {
            return m_OutlineColor;
        }
        set
        {
            m_OutlineColor = value;
            graphic.SetVerticesDirty();
        }
    }

    //描边宽度
    [SerializeField]
    [Range(0, 5)]
    int m_OutlineWidth = 1;
    public int OutlineWidth
    {
        get
        {
            return m_OutlineWidth;
        }
        set
        {
            m_OutlineWidth = value;
            graphic.SetVerticesDirty();
        }
    }

    //顶点缓存
    List<UIVertex> m_VertexCache = new List<UIVertex>();

    //材质路径
    const string MaterialPath = "Assets/Materials/UIOutline.mat";

    protected override void Awake()
    {
        base.Awake();

        if (graphic != null)
        {
            if (graphic.material == null
                || graphic.material.shader.name != "UI/Outline")
            {
                LoadOutlineMat();
            }
        }

        RefreshOutline();
    }

    void LoadOutlineMat()
    {
#if UNITY_EDITOR
        var mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (mat != null)
        {
            graphic.material = mat;
        }
        else
        {
            Debug.LogError("没有找到材质Outline.mat");
        }
#else
        var shader = Shader.Find("UI/Outline");
        base.graphic.material = new Material(shader);
#endif
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (graphic != null)
        {
            if (graphic.material == null
                || graphic.material.shader.name != "UI/Outline")
            {
                LoadOutlineMat();
            }
        }
    }
#endif

    public void RefreshOutline()
    {
        if (base.graphic.canvas)
        {
            var v1 = base.graphic.canvas.additionalShaderChannels;
            var v2 = AdditionalCanvasShaderChannels.TexCoord1;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
            v2 = AdditionalCanvasShaderChannels.TexCoord2;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
            v2 = AdditionalCanvasShaderChannels.TexCoord3;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
            v2 = AdditionalCanvasShaderChannels.Tangent;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
            v2 = AdditionalCanvasShaderChannels.Normal;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(m_VertexCache);

        ApplyOutline();

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_VertexCache);
        m_VertexCache.Clear();
    }

    void ApplyOutline()
    {
        for (int i = 0, count = m_VertexCache.Count - 3; i <= count; i += 3)
        {
            var v1 = m_VertexCache[i];
            var v2 = m_VertexCache[i + 1];
            var v3 = m_VertexCache[i + 2];
            //计算原顶点坐标中心点
            var minX = Min(v1.position.x, v2.position.x, v3.position.x);
            var minY = Min(v1.position.y, v2.position.y, v3.position.y);
            var maxX = Max(v1.position.x, v2.position.x, v3.position.x);
            var maxY = Max(v1.position.y, v2.position.y, v3.position.y);
            var posCenter = new Vector2(minX + maxX, minY + maxY) * 0.5f;
            //计算原始顶点坐标和UV的方向
            Vector2 triX, triY, uvX, uvY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;
            if (Mathf.Abs(Vector2.Dot((pos2 - pos1).normalized, Vector2.right))
                > Mathf.Abs(Vector2.Dot((pos3 - pos2).normalized, Vector2.right)))
            {
                triX = pos2 - pos1;
                triY = pos3 - pos2;
                uvX = v2.uv0 - v1.uv0;
                uvY = v3.uv0 - v2.uv0;
            }
            else
            {
                triX = pos3 - pos2;
                triY = pos2 - pos1;
                uvX = v3.uv0 - v2.uv0;
                uvY = v2.uv0 - v1.uv0;
            }
            //计算原始UV框
            var uvMin = Min(v1.uv0, v2.uv0, v3.uv0);
            var uvMax = Max(v1.uv0, v2.uv0, v3.uv0);
            //OutlineColor和OutlineWidth也传入，避免出现不同的材质球
            var col_rg = new Vector2(m_OutlineColor.r, m_OutlineColor.g);
            var col_ba = new Vector4(0, 0, m_OutlineColor.b, m_OutlineColor.a);
            var normal = new Vector3(0, 0, m_OutlineWidth);
            //为每个顶点设置新的Position和UV，并传入原始UV框
            v1 = SetNewPosAndUV(v1, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v1.uv3 = col_rg;
            v1.tangent = col_ba;
            v1.normal = normal;
            v2 = SetNewPosAndUV(v2, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v2.uv3 = col_rg;
            v2.tangent = col_ba;
            v2.normal = normal;
            v3 = SetNewPosAndUV(v3, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v3.uv3 = col_rg;
            v3.tangent = col_ba;
            v3.normal = normal;
            //应用设置后的UIVertex
            m_VertexCache[i] = v1;
            m_VertexCache[i + 1] = v2;
            m_VertexCache[i + 2] = v3;
        }
    }

    static UIVertex SetNewPosAndUV(UIVertex vertex, int width,
       Vector2 pPosCenter,
       Vector2 triangleX, Vector2 triangleY,
       Vector2 uvX, Vector2 uvY,
       Vector2 uvOriginMin, Vector2 uvOriginMax)
    {
        //Position
        var pos = vertex.position;
        var posXOffset = pos.x > pPosCenter.x ? width : -width;
        var posYOffset = pos.y > pPosCenter.y ? width : -width;
        pos.x += posXOffset;
        pos.y += posYOffset;
        vertex.position = pos;
        //UV
        var uv = vertex.uv0;
        uv += uvX / triangleX.magnitude * posXOffset * (Vector2.Dot(triangleX, Vector2.right) > 0 ? 1 : -1);
        uv += uvY / triangleY.magnitude * posYOffset * (Vector2.Dot(triangleY, Vector2.up) > 0 ? 1 : -1);
        vertex.uv0 = uv;

        vertex.uv1 = uvOriginMin;
        vertex.uv2 = uvOriginMax;

        return vertex;
    }

    static float Min(float pA, float pB, float pC)
    {
        return Mathf.Min(Mathf.Min(pA, pB), pC);
    }

    static float Max(float pA, float pB, float pC)
    {
        return Mathf.Max(Mathf.Max(pA, pB), pC);
    }

    static Vector2 Min(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Min(pA.x, pB.x, pC.x), Min(pA.y, pB.y, pC.y));
    }

    static Vector2 Max(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Max(pA.x, pB.x, pC.x), Max(pA.y, pB.y, pC.y));
    }
}