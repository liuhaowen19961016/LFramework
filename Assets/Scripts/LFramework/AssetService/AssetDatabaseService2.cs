#if UNITY_EDITOR

using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// AssetDatabase管理
/// </summary>
public partial class AssetDatabaseService
{
    /// <summary>
    /// 同步加载GameObject
    /// </summary>
    public GameObject LoadGameObjectSync(string assetPath)
    {
        return LoadAssetSync<GameObject>(assetPath);
    }

    /// <summary>
    /// 异步加载GameObject
    /// </summary>
    public void LoadGameObjectAsync(string assetPath, Action<GameObject> onCompleted)
    {
        LoadAssetAsync<GameObject>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载Object
    /// </summary>
    public Object LoadObjectSync(string assetPath)
    {
        return LoadAssetSync<Object>(assetPath);
    }

    /// <summary>
    /// 异步加载Object
    /// </summary>
    public void LoadObjectAsync(string assetPath, Action<Object> onCompleted)
    {
        LoadAssetAsync<Object>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    public Sprite LoadSpriteSync(string assetPath)
    {
        return LoadAssetSync<Sprite>(assetPath);
    }

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    public void LoadSpriteAsync(string assetPath, Action<Sprite> onCompleted)
    {
        LoadAssetAsync<Sprite>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载Texture
    /// </summary>
    public Texture LoadTextureSync(string assetPath)
    {
        return LoadAssetSync<Texture>(assetPath);
    }

    /// <summary>
    /// 异步加载Texture
    /// </summary>
    public void LoadTextureAsync(string assetPath, Action<Texture> onCompleted)
    {
        LoadAssetAsync<Texture>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载AudioClip
    /// </summary>
    public AudioClip LoadAudioClipSync(string assetPath)
    {
        return LoadAssetSync<AudioClip>(assetPath);
    }

    /// <summary>
    /// 异步加载AudioClip
    /// </summary>
    public void LoadAudioClipAsync(string assetPath, Action<AudioClip> onCompleted)
    {
        LoadAssetAsync<AudioClip>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载Animation
    /// </summary>
    public Animation LoadAnimationSync(string assetPath)
    {
        return LoadAssetSync<Animation>(assetPath);
    }

    /// <summary>
    /// 异步加载Animation
    /// </summary>
    public void LoadAnimationAsync(string assetPath, Action<Animation> onCompleted)
    {
        LoadAssetAsync<Animation>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载Material
    /// </summary>
    public Material LoadMaterialSync(string assetPath)
    {
        return LoadAssetSync<Material>(assetPath);
    }

    /// <summary>
    /// 异步加载Material
    /// </summary>
    public void LoadMaterialAsync(string assetPath, Action<Material> onCompleted)
    {
        LoadAssetAsync<Material>(assetPath, onCompleted);
    }

    /// <summary>
    /// 同步加载TextAsset
    /// </summary>
    public TextAsset LoadTextAssetSync(string assetPath)
    {
        return LoadAssetSync<TextAsset>(assetPath);
    }

    /// <summary>
    /// 异步加载TextAsset
    /// </summary>
    public void LoadTextAssetAsync(string assetPath, Action<TextAsset> onCompleted)
    {
        LoadAssetAsync<TextAsset>(assetPath, onCompleted);
    }
}

#endif
