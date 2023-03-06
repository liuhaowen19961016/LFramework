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
        assetPath = ReconstructPath(assetPath, ".prefab");
        return LoadAssetSync<GameObject>(assetPath);
    }

    /// <summary>
    /// 异步加载GameObject
    /// </summary>
    public void LoadGameObjectAsync(string assetPath, Action<GameObject> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".prefab");
        LoadAssetAsync<GameObject>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as GameObject);
        });
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
        LoadAssetAsync<Object>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as Object);
        });
    }

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    public Sprite LoadSpriteSync(string assetPath)
    {
        assetPath = ReconstructPath(assetPath, ".png");
        return LoadAssetSync<Sprite>(assetPath);
    }

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    public void LoadSpriteAsync(string assetPath, Action<Sprite> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".png");
        LoadAssetAsync<Sprite>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as Sprite);
        });
    }

    /// <summary>
    /// 同步加载Texture
    /// </summary>
    public Texture LoadTextureSync(string assetPath)
    {
        assetPath = ReconstructPath(assetPath, ".png");
        return LoadAssetSync<Texture>(assetPath);
    }

    /// <summary>
    /// 异步加载Texture
    /// </summary>
    public void LoadTextureAsync(string assetPath, Action<Texture> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".png");
        LoadAssetAsync<Texture>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as Texture);
        });
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
        LoadAssetAsync<AudioClip>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as AudioClip);
        });
    }

    /// <summary>
    /// 同步加载Animation
    /// </summary>
    public Animation LoadAnimationSync(string assetPath)
    {
        assetPath = ReconstructPath(assetPath, ".anim");
        return LoadAssetSync<Animation>(assetPath);
    }

    /// <summary>
    /// 异步加载Animation
    /// </summary>
    public void LoadAnimationAsync(string assetPath, Action<Animation> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".anim");
        LoadAssetAsync<Animation>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as Animation);
        });
    }

    /// <summary>
    /// 同步加载Material
    /// </summary>
    public Material LoadMaterialSync(string assetPath)
    {
        assetPath = ReconstructPath(assetPath, ".mat");
        return LoadAssetSync<Material>(assetPath);
    }

    /// <summary>
    /// 异步加载Material
    /// </summary>
    public void LoadMaterialAsync(string assetPath, Action<Material> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".mat");
        LoadAssetAsync<Material>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as Material);
        });
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
        LoadAssetAsync<TextAsset>(assetPath, (param) =>
        {
            onCompleted?.Invoke(param == null ? null : param.obj as TextAsset);
        });
    }
}

#endif
