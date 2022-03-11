using UnityEngine;
using System;
using System.Collections.Generic;

public interface IAssetService
{
    #region 通用

    /// <summary>
    /// 同步加载资源到内存
    /// </summary>
    T LoadAssetSync<T>(string assetName) where T : UnityEngine.Object;

    /// <summary>
    /// 异步加载资源到内存
    /// </summary>
    void LoadAssetAsync<T>(string assetName, Action<T> onCompleted) where T : UnityEngine.Object;

    #endregion

    #region Scene

    /// <summary>
    /// 异步加载场景
    /// </summary>
    //void LoadSceneAssetAsync(string assetName, bool addictive, bool activateOnLoad, Action onCompleted);

    /// <summary>
    /// 释放场景
    /// </summary>
    //void UnLoadScene(string scene, bool autoReleaseHandle, Action onCompleted);

    #endregion

    #region GameObject

    /// <summary>
    /// 预加载多个预制体(GameObject)
    /// </summary>
    void PreLoadPrefab(List<string> prefabNameList, Action<Dictionary<string, GameObject>> onCompleted);

    /// <summary>
    /// 实例化已经加载到内存中的GameObject
    /// </summary>
    GameObject Instantiate(GameObject obj, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 同步加载+实例化GameObject
    /// </summary>
    GameObject InstantiateSync(string assetName, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 异步加载+实例化GameObject
    /// </summary>
    void InstantiateAsync(string assetName, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 释放GameObject
    /// </summary>
    void Release(GameObject go);

    /// <summary>
    /// 延时释放GameObject
    /// </summary>
    void Release(GameObject go, float delayTime);

    #endregion

    #region Sprite

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    Sprite LoadSpriteSync(string assetName);

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    void LoadSpriteAsync(string assetName, Action<Sprite> onCompleted);

    #endregion

    #region Texture

    /// <summary>
    /// 同步加载Texture
    /// </summary>
    Texture LoadTextureSync(string assetName);

    /// <summary>
    /// 异步加载Texture
    /// </summary>
    void LoadTextureAsync(string assetName, Action<Texture> onCompleted);

    #endregion

    #region AudioClip

    /// <summary>
    /// 同步加载AudioClip
    /// </summary>
    AudioClip LoadAudioClipSync(string assetName);

    /// <summary>
    /// 异步加载AudioClip
    /// </summary>
    void LoadAudioClipAsync(string assetName, Action<AudioClip> onCompleted);

    #endregion

    #region Animation

    /// <summary>
    /// 同步加载Animation
    /// </summary>
    Animation LoadAnimationSync(string assetName);

    /// <summary>
    /// 异步加载Animation
    /// </summary>
    void LoadAnimationAsync(string assetName, Action<Animation> onCompleted);

    #endregion

    #region Material

    /// <summary>
    /// 同步加载Material
    /// </summary>
    Material LoadMaterialSync(string assetName);

    /// <summary>
    /// 异步加载Material
    /// </summary>
    void LoadMaterialAsync(string assetName, Action<Material> onCompleted);

    #endregion

    #region TextAsset

    /// <summary>
    /// 同步加载TextAsset
    /// </summary>
    TextAsset LoadTextAssetSync(string assetName);

    /// <summary>
    /// 异步加载TextAsset
    /// </summary>
    void LoadTextAssetAsync(string assetName, Action<TextAsset> onCompleted);

    #endregion

    #region Object

    /// <summary>
    /// 同步加载Object
    /// </summary>
    UnityEngine.Object LoadObjectSync(string assetName);

    /// <summary>
    /// 异步加载Object
    /// </summary>
    void LoadObjectAsync(string assetName, Action<UnityEngine.Object> onCompleted);

    #endregion
}
