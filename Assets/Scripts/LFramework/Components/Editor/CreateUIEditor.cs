using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIEditor
{
    const int UI_LAYER = 5;

    [MenuItem("GameObject/LFramework/UI/CircleImage", priority = 10)]
    public static void CreateCircleImage()
    {
        GameObject obj = InitUIComponent("CircleImage");
        CircleImage component = obj.AddComponent<CircleImage>();
        component.raycastTarget = false;
    }

    [MenuItem("GameObject/LFramework/UI/PolygonImage", priority = 11)]
    public static void CreatePolygonImage()
    {
        GameObject obj = InitUIComponent("PolygonImage");
        PolygonImage component = obj.AddComponent<PolygonImage>();
    }

    /// <summary>
    /// UI组件初始化
    /// </summary>
    static GameObject InitUIComponent(string componentName)
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
