using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// 多边形Image组件编辑器
/// </summary>
[CustomEditor(typeof(PolygonImage))]
[CanEditMultipleObjects]
public class PolygonImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
