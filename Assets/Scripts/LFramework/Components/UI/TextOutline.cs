using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LFramework/UI/Effect/TextOutline", 0)]
public class TextOutline : BaseMeshEffect
{
    protected TextOutline()
    {

    }

    //描边颜色
    public Color m_OutlineColor = Color.white;
    //描边宽度
    public float m_OutlineWidth = 1;

    List<UIVertex> m_VetexList = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(m_VetexList);

        int startIndex = 0;
        int endIndex = m_VetexList.Count;
        ProcessVertex(m_VetexList, startIndex, endIndex, m_OutlineWidth, 0);
        startIndex = endIndex;
        endIndex = m_VetexList.Count;
        ProcessVertex(m_VetexList, startIndex, endIndex, -m_OutlineWidth, 0);
        startIndex = endIndex;
        endIndex = m_VetexList.Count;
        ProcessVertex(m_VetexList, startIndex, endIndex, 0, m_OutlineWidth);
        startIndex = endIndex;
        endIndex = m_VetexList.Count;
        ProcessVertex(m_VetexList, startIndex, endIndex, 0, -m_OutlineWidth);

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_VetexList);
        m_VetexList.Clear();
    }

    void ProcessVertex(List<UIVertex> vertexList, int startIndex, int endIndex, float x, float y)
    {
        UIVertex uiVertex;
        for (int i = startIndex; i < endIndex; i++)
        {
            uiVertex = vertexList[i];

            vertexList.Add(uiVertex);
            Vector3 offset = new Vector3(x, y);
            uiVertex.position += offset;
            uiVertex.color = m_OutlineColor;
            vertexList[i] = uiVertex;
        }
    }
}