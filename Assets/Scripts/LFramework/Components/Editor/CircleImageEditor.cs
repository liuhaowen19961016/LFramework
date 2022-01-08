using UnityEditor;

/// <summary>
/// 圆形Image组件编辑器
/// </summary>
[CustomEditor(typeof(CircleImage))]
[CanEditMultipleObjects]
public class CircleImageEditor : Editor
{
    SerializedProperty m_Sprite;
    SerializedProperty m_Color;
    SerializedProperty m_RaycastTarget;
    SerializedProperty m_RenderType;
    SerializedProperty m_FilledType;
    SerializedProperty m_Origin360;
    SerializedProperty m_Clockwise;
    SerializedProperty m_FillAmount;

    private void OnEnable()
    {
        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Color = serializedObject.FindProperty("m_Color");
        m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        m_RenderType = serializedObject.FindProperty("m_RenderType");
        m_FilledType = serializedObject.FindProperty("m_FilledType");
        m_Origin360 = serializedObject.FindProperty("m_Origin360");
        m_Clockwise = serializedObject.FindProperty("m_Clockwise");
        m_FillAmount = serializedObject.FindProperty("m_FillAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Sprite);
        EditorGUILayout.PropertyField(m_Color);
        EditorGUILayout.PropertyField(m_RaycastTarget);
        EditorGUILayout.PropertyField(m_RenderType);
        if ((CircleImage.RenderType)m_RenderType.intValue == CircleImage.RenderType.Simple)
        {

        }
        else if ((CircleImage.RenderType)m_RenderType.intValue == CircleImage.RenderType.Filled)
        {
            EditorGUILayout.PropertyField(m_FilledType);
            if ((CircleImage.FilledType)m_FilledType.intValue == CircleImage.FilledType.Radial360)
            {
                EditorGUILayout.PropertyField(m_Origin360);
            }
            EditorGUILayout.PropertyField(m_Clockwise);
            EditorGUILayout.PropertyField(m_FillAmount);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
