using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过AssetBundle加载资源
/// </summary>
public class AssetBundleService : IAssetService
{
    public GameObject Instantiate(GameObject obj, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public void InstantiateAsync(string assetName, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public GameObject InstantiateSync(string assetName, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public void LoadAnimationAsync(string assetName, Action<Animation> onCompleted)
    {
        throw new NotImplementedException();
    }

    public Animation LoadAnimationSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadAssetAsync<T>(string assetName, Action<T> onCompleted) where T : UnityEngine.Object
    {
        throw new NotImplementedException();
    }

    public T LoadAssetSync<T>(string assetName) where T : UnityEngine.Object
    {
        throw new NotImplementedException();
    }

    public void LoadAudioClipAsync(string assetName, Action<AudioClip> onCompleted)
    {
        throw new NotImplementedException();
    }

    public AudioClip LoadAudioClipSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadMaterialAsync(string assetName, Action<Material> onCompleted)
    {
        throw new NotImplementedException();
    }

    public Material LoadMaterialSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadObjectAsync(string assetName, Action<UnityEngine.Object> onCompleted)
    {
        throw new NotImplementedException();
    }

    public UnityEngine.Object LoadObjectSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadSpriteAsync(string assetName, Action<Sprite> onCompleted)
    {
        throw new NotImplementedException();
    }

    public Sprite LoadSpriteSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadTextAssetAsync(string assetName, Action<TextAsset> onCompleted)
    {
        throw new NotImplementedException();
    }

    public TextAsset LoadTextAssetSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void LoadTextureAsync(string assetName, Action<Texture> onCompleted)
    {
        throw new NotImplementedException();
    }

    public Texture LoadTextureSync(string assetName)
    {
        throw new NotImplementedException();
    }

    public void PreLoadPrefab(List<string> prefabNameList, Action<Dictionary<string, GameObject>> onCompleted)
    {
        throw new NotImplementedException();
    }

    public void Release(GameObject go)
    {
        throw new NotImplementedException();
    }

    public void Release(GameObject go, float delayTime)
    {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
