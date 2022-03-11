using System.Text;
using UnityEngine;

/// <summary>
/// FPS工具(OnGUI绘制)
/// </summary>
public class FPS_Tool : MonoBehaviour
{
    //fps更新间隔
    const float UpdateInterval = 0.5f;

    //上一次更新的时间
    float m_LastUpdateTime;
    //帧数
    float m_FrameCount;
    //帧率
    float m_FPS;

    Rect m_ShowRect;
    GUIStyle m_GuiStyle = new GUIStyle();
    StringBuilder sb = new StringBuilder();

    private void Start()
    {
        m_LastUpdateTime = Time.realtimeSinceStartup;
        m_FPS = 0;

        InitStyle();
    }

    void InitStyle()
    {
        m_ShowRect = new Rect(5, 10, 200, 40);
        m_GuiStyle.normal.background = null;
        m_GuiStyle.normal.textColor = Color.red;
        m_GuiStyle.fontSize = 30;
        m_GuiStyle.fontStyle = FontStyle.Bold;
    }

    private void OnGUI()
    {
        sb.Length = 0;
        sb.Append("FPS: ");
        sb.Append(m_FPS.ToString("f1"));
        GUI.Label(m_ShowRect, sb.ToString(), m_GuiStyle);
    }

    private void Update()
    {
        m_FrameCount++;
        float curTime = Time.realtimeSinceStartup;
        if (curTime - m_LastUpdateTime >= UpdateInterval)
        {
            m_FPS = m_FrameCount / (curTime - m_LastUpdateTime);

            m_LastUpdateTime = Time.realtimeSinceStartup;
            m_FrameCount = 0;
        }
    }
}
