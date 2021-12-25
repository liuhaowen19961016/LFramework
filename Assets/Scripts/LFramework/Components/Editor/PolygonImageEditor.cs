using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(PolygonImage))]
[CanEditMultipleObjects]
public class PolygonImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
