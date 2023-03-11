using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderData;

/// <summary>
/// 事件系统编辑器扩展
/// </summary>
public class MsgSystemEditor : EditorWindow
{
    private static Vector2 windowSize = new Vector2(1000, 500);//窗口尺寸
    private Vector2 m_ScrollPosition1 = Vector2.zero;//存储滑动条位置-事件总览
    private Vector2 m_ScrollPosition2 = Vector2.zero;//存储滑动条位置-实时绑定关系

    private int curToolbarIndex;
    private string[] toolbarStr = { "事件总览", "实时绑定关系" };

    private List<string> m_MsgTypeStrs = new List<string>();//所有消息类型

    [MenuItem("工具/事件系统/事件系统编辑器", priority = 0)]
    private static void Open()
    {
        EditorWindow window = GetWindow<MsgSystemEditor>("事件系统编辑器");
        window.minSize = windowSize;
        window.maxSize = windowSize;
        window.Focus();
        window.Show();
    }

    private void OnEnable()
    {
        Type type = typeof(MsgConst);
        foreach (var temp in type.GetFields())
        {
            if (!m_MsgTypeStrs.Contains(temp.Name))
            {
                m_MsgTypeStrs.Add(temp.Name);
            }
        }
    }

    private void OnGUI()
    {
        curToolbarIndex = GUILayout.Toolbar(curToolbarIndex, toolbarStr);
        //事件总览
        if (curToolbarIndex == 0)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginScrollView(m_ScrollPosition1);
            foreach (var temp in m_MsgTypeStrs)
            {
                GUILayout.Label(temp);
            }
            GUILayout.EndScrollView();
        }
        //实时绑定关系
        else if (curToolbarIndex == 1)
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("请先运行游戏", MessageType.Warning);
                return;
            }
            GUILayout.BeginScrollView(m_ScrollPosition2);
            foreach (var pairs in MsgSystem.EventDict)
            {
                GUILayout.Label($"事件名称：{pairs.Key}");
                foreach (var temp in pairs.Value)
                {
                    GUILayout.Label($"    对应的回调函数：{temp.GetMethodInfo().Name}（{temp.GetMethodInfo().DeclaringType}.cs）");
                }
                EditorGUILayout.Separator();
            }
            GUILayout.EndScrollView();
        }
    }
}