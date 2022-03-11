#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// 通过AssetDatabase加载资源
/// </summary>
public class AssetDatabaseService : IAssetService
{
    #region 通用

    /// <summary>
    /// 同步加载资源到内存
    /// </summary>
    public T LoadAssetSync<T>(string assetName)
        where T : UnityEngine.Object
    {
        string assetPath = "";
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            Debug.LogError("加载资源失败，assetPath：" + assetPath);
        }
        return asset;
    }

    /// <summary>
    /// 异步加载资源到内存
    /// </summary>
    public void LoadAssetAsync<T>(string assetName, Action<T> onCompleted)
         where T : UnityEngine.Object
    {
        string assetPath = "";
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            Debug.LogError("加载资源失败，assetPath：" + assetPath);
        }
        onCompleted?.Invoke(asset);
    }

    #endregion

    #region Scene

    #endregion

    #region GameObject

    /// <summary>
    /// 预加载多个预制体(GameObject)
    /// </summary>
    public void PreLoadPrefab(List<string> prefabNameList, Action<Dictionary<string, GameObject>> onCompleted)
    {
        Dictionary<string, GameObject> assetDict = new Dictionary<string, GameObject>();
        int totalCount = prefabNameList.Count;
        int count = 0;
        for (int i = 0; i < totalCount; i++)
        {
            string assetPath = prefabNameList[i];
            if (string.IsNullOrEmpty(assetPath))
            {
                count++;
                continue;
            }

            GameObject asset = LoadAssetSync<GameObject>(assetPath);
            if (asset != null)
            {
                assetDict.Add(assetPath, asset);
            }
            count++;
            if (count >= totalCount)
            {
                onCompleted?.Invoke(assetDict);
            }
        }
    }

    /// <summary>
    /// 实例化已加载到内存中的GameObject
    /// </summary>
    public GameObject Instantiate(GameObject asset, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        if (asset == null)
        {
            return null;
        }
        GameObject go;
        if (parent == null)
        {
            go = GameObject.Instantiate(asset, pos, rotation);
        }
        else
        {
            go = GameObject.Instantiate(asset, pos, rotation, parent);
        }
        return go;
    }

    /// <summary>
    /// 同步加载实例化GameObject
    /// </summary>
    public GameObject InstantiateSync(string assetName, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject asset = LoadAssetSync<GameObject>(assetName);
        if (asset == null)
        {
            return null;
        }

        GameObject go = Instantiate(asset, pos, rotation, parent);
        return go;
    }

    /// <summary>
    /// 异步加载实例化GameObject
    /// </summary>
    public void InstantiateAsync(string assetName, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject asset = LoadAssetSync<GameObject>(assetName);
        if (asset == null)
        {
            return;
        }
        Thread thread = new Thread(() =>
        {
            GameObject go = Instantiate(asset, pos, rotation, parent);
            onCompleted?.Invoke(go);
        });
    }

    /// <summary>
    /// 立即释放GameObject
    /// </summary>
    public void Release(GameObject go)
    {
        GameObject.Destroy(go);
    }

    /// <summary>
    /// 延迟释放游GameObject
    /// </summary>
    public void Release(GameObject go, float delayTime)
    {
        GameObject.Destroy(go, delayTime);
    }

    #endregion

    #region Sprite

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    public Sprite LoadSpriteSync(string assetName)
    {
        return LoadAssetSync<Sprite>(assetName);
    }

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    public void LoadSpriteAsync(string assetName, Action<Sprite> onCompleted)
    {
        LoadAssetAsync<Sprite>(assetName, onCompleted);
    }

    #endregion

    #region Texture

    /// <summary>
    /// 同步加载Texture
    /// </summary>
    public Texture LoadTextureSync(string assetName)
    {
        return LoadAssetSync<Texture>(assetName);
    }

    /// <summary>
    /// 异步加载Texture
    /// </summary>
    public void LoadTextureAsync(string assetName, Action<Texture> onCompleted)
    {
        LoadAssetAsync<Texture>(assetName, onCompleted);
    }

    #endregion

    #region AudioClip

    /// <summary>
    /// 同步加载AudioClip
    /// </summary>
    public AudioClip LoadAudioClipSync(string assetName)
    {
        return LoadAssetSync<AudioClip>(assetName);
    }

    /// <summary>
    /// 异步加载AudioClip
    /// </summary>
    public void LoadAudioClipAsync(string assetName, Action<AudioClip> onCompleted)
    {
        LoadAssetAsync<AudioClip>(assetName, onCompleted);
    }

    #endregion

    #region Animation

    /// <summary>
    /// 同步加载Animation
    /// </summary>
    public Animation LoadAnimationSync(string assetName)
    {
        return LoadAssetSync<Animation>(assetName);
    }

    /// <summary>
    /// 异步加载Animation
    /// </summary>
    public void LoadAnimationAsync(string assetName, Action<Animation> onCompleted)
    {
        LoadAssetAsync<Animation>(assetName, onCompleted);
    }

    #endregion

    #region Material

    /// <summary>
    /// 同步加载Material
    /// </summary>
    public Material LoadMaterialSync(string assetName)
    {
        return LoadAssetSync<Material>(assetName);
    }

    /// <summary>
    /// 异步加载Material
    /// </summary>
    public void LoadMaterialAsync(string assetName, Action<Material> onCompleted)
    {
        LoadAssetAsync<Material>(assetName, onCompleted);
    }

    #endregion

    #region TextAsset

    /// <summary>
    /// 同步加载TextAsset
    /// </summary>
    public TextAsset LoadTextAssetSync(string assetName)
    {
        return LoadAssetSync<TextAsset>(assetName);
    }

    /// <summary>
    /// 异步加载TextAsset
    /// </summary>
    public void LoadTextAssetAsync(string assetName, Action<TextAsset> onCompleted)
    {
        LoadAssetAsync<TextAsset>(assetName, onCompleted);
    }

    #endregion

    #region Object

    /// <summary>
    /// 同步加载Object
    /// </summary>
    public UnityEngine.Object LoadObjectSync(string assetName)
    {
        return LoadAssetSync<UnityEngine.Object>(assetName);
    }

    /// <summary>
    /// 异步加载Object
    /// </summary>
    public void LoadObjectAsync(string assetName, Action<UnityEngine.Object> onCompleted)
    {
        LoadAssetAsync<UnityEngine.Object>(assetName, onCompleted);
    }

    #endregion
}

#endif
