using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public interface IAssetService
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

    #region Scene

    /// <summary>
    /// 异步加载场景
    /// </summary>
    void LoadSceneAsync(string assetPath, bool additive, bool activateOnLoad, Action<AsyncLoadSceneAssetCallBackParam> onCompleted);

    /// <summary>
    /// 卸载场景
    /// </summary>
    bool UnLoadScene(Scene scene, Action callback);

    #endregion Scene

    #region GameObject

    /// <summary>
    /// 同步实例化GameObject
    /// </summary>
    GameObject InstantiateSync(string assetPath, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 异步实例化GameObject
    /// </summary>
    void InstantiateAsync(string assetPath, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 实例化已加载的GameObject对象
    /// </summary>
    GameObject Instantiate(GameObject go, Vector3 pos = default, Quaternion rotation = default, Transform parent = null);

    /// <summary>
    /// 同步加载GameObject
    /// </summary>
    GameObject LoadGameObjectSync(string assetPath);

    /// <summary>
    /// 异步加载GameObject
    /// </summary>
    void LoadGameObjectAsync(string assetPath, Action<GameObject> onCompleted);

    /// <summary>
    /// 销毁GameObject
    /// </summary>
    void ReleaseGameObject(Object go, float delayTime = 0);

    #endregion GameObject

    #region Object

    /// <summary>
    /// 同步加载Object
    /// </summary>
    Object LoadObjectSync(string assetPath);

    /// <summary>
    /// 异步加载Object
    /// </summary>
    void LoadObjectAsync(string assetPath, Action<Object> onCompleted);

    #endregion Object

    #region Sprite

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    Sprite LoadSpriteSync(string assetPath);

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    void LoadSpriteAsync(string assetPath, Action<Sprite> onCompleted);

    #endregion Sprite

    #region Texture

    /// <summary>
    /// 同步加载Texture
    /// </summary>
    Texture LoadTextureSync(string assetPath);

    /// <summary>
    /// 异步加载Texture
    /// </summary>
    void LoadTextureAsync(string assetPath, Action<Texture> onCompleted);

    #endregion Texture

    #region AudioClip

    /// <summary>
    /// 同步加载AudioClip
    /// </summary>
    AudioClip LoadAudioClipSync(string assetPath);

    /// <summary>
    /// 异步加载AudioClip
    /// </summary>
    void LoadAudioClipAsync(string assetPath, Action<AudioClip> onCompleted);

    #endregion AudioClip

    #region Animation

    /// <summary>
    /// 同步加载Animation
    /// </summary>
    Animation LoadAnimationSync(string assetPath);

    /// <summary>
    /// 异步加载Animation
    /// </summary>
    void LoadAnimationAsync(string assetPath, Action<Animation> onCompleted);

    #endregion Animation

    #region Material

    /// <summary>
    /// 同步加载Material
    /// </summary>
    Material LoadMaterialSync(string assetPath);

    /// <summary>
    /// 异步加载Material
    /// </summary>
    void LoadMaterialAsync(string assetPath, Action<Material> onCompleted);

    #endregion Material

    #region TextAsset

    /// <summary>
    /// 同步加载TextAsset
    /// </summary>
    TextAsset LoadTextAssetSync(string assetPath);

    /// <summary>
    /// 异步加载TextAsset
    /// </summary>
    void LoadTextAssetAsync(string assetPath, Action<TextAsset> onCompleted);

    #endregion TextAsset

    /// <summary>
    /// 卸载资源
    /// </summary>
    bool UnloadAsset(Object obj);
}
