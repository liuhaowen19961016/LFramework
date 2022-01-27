using UnityEditor;
using UnityEngine;

/// <summary>
/// 雷达图组件编辑器
/// </summary>
[CustomEditor(typeof(RadarChart))]
[CanEditMultipleObjects]
public class RadarChartEditor : Editor
{
    SerializedProperty m_Sprite;
    SerializedProperty m_Color;
    SerializedProperty m_RaycastTarget;
    SerializedProperty m_Radius;
    SerializedProperty m_SideCount;
    SerializedProperty m_ShowInner;
    SerializedProperty m_ShowOutline;
    SerializedProperty m_OutlineData;

    private void OnEnable()
    {
        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Color = serializedObject.FindProperty("m_Color");
        m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        m_Radius = serializedObject.FindProperty("m_Radius");
        m_SideCount = serializedObject.FindProperty("m_SideCount");
        m_ShowInner = serializedObject.FindProperty("m_ShowInner");
        m_ShowOutline = serializedObject.FindProperty("m_ShowOutline");
        m_OutlineData = serializedObject.FindProperty("m_OutlineData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Sprite);
        EditorGUILayout.PropertyField(m_Color);
        EditorGUILayout.PropertyField(m_RaycastTarget);
        EditorGUILayout.PropertyField(m_Radius);
        EditorGUILayout.PropertyField(m_SideCount);
        EditorGUILayout.PropertyField(m_ShowInner);
        EditorGUILayout.PropertyField(m_ShowOutline);
        if (m_ShowOutline.boolValue)
        {
            EditorGUILayout.PropertyField(m_OutlineData, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
