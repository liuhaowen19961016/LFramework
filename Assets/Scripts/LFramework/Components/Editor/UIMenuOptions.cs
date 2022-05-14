using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hierarchy面板创建UI选项
/// </summary>
public static class UIMenuOptions
{
    const int UI_LAYER = 5;

    [MenuItem("GameObject/LFramework/UI/CommonImage", priority = 10)]
    public static void AddCommonImage()
    {
        GameObject obj = CreateUIComponent("CommonImage");
        CommonImage commonImage = obj.AddComponent<CommonImage>();
        commonImage.raycastTarget = false;
    }

    [MenuItem("GameObject/LFramework/UI/CircleImage", priority = 11)]
    public static void AddCircleImage()
    {
        GameObject obj = CreateUIComponent("CircleImage");
        CircleImage circleImage = obj.AddComponent<CircleImage>();
        circleImage.raycastTarget = false;
    }

    [MenuItem("GameObject/LFramework/UI/PolygonImage", priority = 12)]
    public static void AddPolygonImage()
    {
        GameObject obj = CreateUIComponent("PolygonImage");
        PolygonCollider2D polygonCollider2D = obj.AddComponent<PolygonCollider2D>();
        PolygonImage polygonImage = obj.AddComponent<PolygonImage>();
    }

    /// <summary>
    /// 创建UI组件
    /// </summary>
    static GameObject CreateUIComponent(string componentName)
    {
        Transform canvasTrans = GetCanvasRoot();

        GameObject obj = new GameObject(componentName);
        obj.layer = UI_LAYER;
        obj.AddComponent<RectTransform>();
        if (Selection.activeGameObject != null
            && Selection.activeGameObject.layer == UI_LAYER)
        {
            obj.transform.SetParent(Selection.activeGameObject.transform);
        }
        else
        {
            obj.transform.SetParent(canvasTrans);
        }
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        return obj;
    }

    /// <summary>
    /// 获取UI画布
    /// </summary>
    static Transform GetCanvasRoot()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.layer = UI_LAYER;
            canvasObj.AddComponent<RectTransform>();
            canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            return canvasObj.transform;
        }
        else
        {
            return canvas.transform;
        }
    }
}
