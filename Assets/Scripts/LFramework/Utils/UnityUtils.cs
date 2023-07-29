using UnityEngine;

/// <summary>
/// Unity工具类
/// </summary>
public static class UnityUtils
{
    /// <summary>
    /// 获取或添加一个节点
    /// </summary>
    public static GameObject GetOrAddGameObjectRoot(string rootName, bool dontDestroyOnLoad = true, Transform parent = null)
    {
        GameObject root = GameObject.Find(rootName);
        if (root == null)
        {
            root = new GameObject(rootName);
        }
        root.Reset(parent: parent);
        if (dontDestroyOnLoad)
        {
            GameObject.DontDestroyOnLoad(root);
        }
        return root;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public static void Reset(this GameObject go, bool isLocal = true, bool isActive = true, Transform parent = null)
    {
        if (go == null)
        {
            return;
        }
        Reset(go.transform, isLocal, isActive, parent);
    }

    /// <summary>
    /// 重置
    /// </summary>
    public static void Reset(this Transform trans, bool isLocal = true, bool isActive = true, Transform parent = null)
    {
        if (trans == null)
        {
            return;
        }
        if (parent != null)
        {
            trans.SetParent(parent);
        }
        if (isLocal)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        else
        {
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        trans.gameObject.SetActive(isActive);
    }
}
