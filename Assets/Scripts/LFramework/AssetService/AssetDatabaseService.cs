#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// AssetDatabase管理
/// </summary>
public partial class AssetDatabaseService : MonoSingleton<AssetDatabaseService>, IAssetService
{
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    private T LoadAssetSync<T>(string assetPath)
        where T : Object
    {
        T obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
        return obj;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    private void LoadAssetAsync<T>(string assetPath, Action<T> onCompleted)
        where T : Object
    {
        T obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
        onCompleted?.Invoke(obj);
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    public bool UnloadAsset(Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        obj = null;
        return true;
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    public void LoadSceneAsync(string assetPath, bool additive, bool activateOnLoad, Action<AsyncLoadSceneAssetCallBackParam> onCompleted)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    public bool UnLoadScene(Scene scene, Action callback)
    {
        return false;
    }

    public GameObject LoadGoSync(string assetPath)
    {
        throw new NotImplementedException();
    }

    public void LoadGoAsync(string assetPath, Action<GameObject> onCompleted)
    {
        throw new NotImplementedException();
    }

    public GameObject InstantiateSync(string assetPath, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public void InstantiateAsync(string assetPath, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public new GameObject Instantiate(GameObject go, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public void ReleaseGameObject(GameObject go, float delayTime = 0)
    {
        throw new NotImplementedException();
    }

    public void ReleaseGameObject(Object go, float delayTime = 0)
    {
        throw new NotImplementedException();
    }
}

#endif
