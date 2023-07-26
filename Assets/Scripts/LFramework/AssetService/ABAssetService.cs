using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// 通用资源
/// </summary>
public class CommonAsset
{
    public string assetPath;//资源路径
    public int guid;//唯一标识
    public Object obj;//已加载的对象
    public float lastUseTime;//最后一次使用的时间
    public LoadedAB loadedAB;//已加载的ab包

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        assetPath = string.Empty;
        guid = 0;
        obj = null;
        lastUseTime = 0;
        loadedAB = null;
    }
}

/// <summary>
/// 场景资源
/// </summary>
public class SceneAsset
{
    public string assetPath;//资源路径
    public Scene scene;//已加载的场景
    public LoadedAB loadedAB;//已加载的ab包

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        assetPath = string.Empty;
        scene = default;
        loadedAB = null;
    }
}

/// <summary>
/// 异步加载通用资源
/// </summary>
public class AsyncLoadCommonAssetOperation
{
    public string assetPath;//资源路径
    public Type assetType;//资源类型
    public AssetBundleRequest assetRequest;//资源异步加载请求
    public List<Action<AsyncLoadCommonAssetCallBackParam>> onCompletedList = new List<Action<AsyncLoadCommonAssetCallBackParam>>();//加载完成的回调列表

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        assetPath = string.Empty;
        assetType = null;
        assetRequest = null;
        onCompletedList.Clear();
    }
}

/// <summary>
/// 异步加载通用资源回调参数
/// </summary>
public class AsyncLoadCommonAssetCallBackParam
{
    public Object obj;//加载后的对象

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        obj = null;
    }
}

/// <summary>
/// 异步加载场景资源
/// </summary>
public class AsyncLoadSceneAssetOperation
{
    public string assetPath;//资源路径
    public AsyncOperation sceneRequest;//场景异步加载请求
    public bool additive;//场景是否叠加
    public bool activateOnLoad;//加载完成后是否直接激活
    public Action<AsyncLoadSceneAssetCallBackParam> onCompleted;//加载完成的回调

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        assetPath = string.Empty;
        sceneRequest = null;
        additive = false;
        activateOnLoad = false;
        onCompleted = null;
    }
}

/// <summary>
/// 异步加载场景资源回调参数
/// </summary>
public class AsyncLoadSceneAssetCallBackParam
{
    public AsyncOperation sceneRequest;//异步加载请求
    public Scene scene;//已加载的场景

    /// <summary>
    /// 激活场景
    /// </summary>
    public void Activate()
    {
        sceneRequest.allowSceneActivation = true;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        sceneRequest = null;
        scene = default;
    }
}

/// <summary>
/// AssetBundle资源管理
/// </summary>
public partial class ABAssetService : MonoSingleton<ABAssetService>, IAssetService
{
    private const float CLEAN_TICK = 200;//200
    private const float GC_TICK = 60;//60
    private float m_LastCleanTime;//上一次clean时间
    private float m_LastGCTime;//上一次GC时间

    private ObjectPool<CommonAsset> m_CommonAssetPool;//通用资源类的类对象池
    private ObjectPool<SceneAsset> m_SceneAssetPool;//场景资源类的类对象池
    private ObjectPool<AsyncLoadCommonAssetOperation> m_AsyncLoadCommonAssetOperationPool;//异步加载通用资源类的类对象池
    private ObjectPool<AsyncLoadCommonAssetCallBackParam> m_AsyncLoadCommonAssetCallBackParamPool;//异步加载通用资源回调参数类的类对象池
    private ObjectPool<AsyncLoadSceneAssetOperation> m_AsyncLoadSceneAssetOperationPool;//异步加载场景资源类的类对象池
    private ObjectPool<AsyncLoadSceneAssetCallBackParam> m_AsyncLoadSceneAssetCallBackParamPool;//异步加载场景资源回调参数类的类对象池

    //通用资源
    private Dictionary<string, AsyncLoadCommonAssetOperation> m_AsyncLoading_CommonAsset = new Dictionary<string, AsyncLoadCommonAssetOperation>();//正在异步加载的通用资源字典
    private List<string> m_KeyToRemoveList_CommonAsset = new List<string>();//将要从正在异步加载的通用资源字典中移除的key列表
    private Dictionary<string, CommonAsset> m_CacheDict_CommonAsset = new Dictionary<string, CommonAsset>();//通用资源缓存字典
    private List<AsyncLoadCommonAssetOperation> m_AsyncLoading_CommonAssetList = new List<AsyncLoadCommonAssetOperation>();//正在异步加载的通用资源列表

    //场景资源
    private Dictionary<string, AsyncLoadSceneAssetOperation> m_AsyncLoading_SceneAsset = new Dictionary<string, AsyncLoadSceneAssetOperation>();//正在异步加载的场景资源字典
    private List<string> m_KeyToRemoveList_SceneAsset = new List<string>();//将要从正在异步加载的场景资源字典中移除的key列表
    private Dictionary<string, SceneAsset> m_CacheDict_SceneAsset = new Dictionary<string, SceneAsset>();//场景资源缓存字典

    private bool m_IsInited;//是否初始化

    private ABMgr abMgr;//ab管理器

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        abMgr = new ABMgr();
        abMgr.Init();
        InitPool();
        m_IsInited = true;
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitPool()
    {
        m_CommonAssetPool = new ObjectPool<CommonAsset>
        (
            onCreate: () => { return new CommonAsset(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 300
        );
        m_SceneAssetPool = new ObjectPool<SceneAsset>
        (
            onCreate: () => { return new SceneAsset(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 2
        );
        m_AsyncLoadCommonAssetOperationPool = new ObjectPool<AsyncLoadCommonAssetOperation>
        (
            onCreate: () => { return new AsyncLoadCommonAssetOperation(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 5
        );
        m_AsyncLoadCommonAssetCallBackParamPool = new ObjectPool<AsyncLoadCommonAssetCallBackParam>
        (
            onCreate: () => { return new AsyncLoadCommonAssetCallBackParam(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 5
        );
        m_AsyncLoadSceneAssetOperationPool = new ObjectPool<AsyncLoadSceneAssetOperation>
       (
           onCreate: () => { return new AsyncLoadSceneAssetOperation(); },
           onPut: (obj) => { obj.Reset(); },
           onDestroy: (obj) => { obj.Reset(); obj = null; },
           capacity: 5
       );
        m_AsyncLoadSceneAssetCallBackParamPool = new ObjectPool<AsyncLoadSceneAssetCallBackParam>
        (
            onCreate: () => { return new AsyncLoadSceneAssetCallBackParam(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 5
        );
    }

    #region 实例化

    /// <summary>
    /// 同步实例化GameObject
    /// </summary>
    public GameObject InstantiateSync(string assetPath, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject obj = LoadGameObjectSync(assetPath);
        return Instantiate(obj, pos, rotation, parent);
    }

    /// <summary>
    /// 异步实例化GameObject
    /// </summary>
    public void InstantiateAsync(string assetPath, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        LoadGameObjectAsync(assetPath, (obj) =>
        {
            GameObject go = Instantiate(obj, pos, rotation, parent);
            onCompleted?.Invoke(go);
        });
    }

    /// <summary>
    /// 实例化已加载的GameObject对象
    /// </summary>
    public GameObject Instantiate(GameObject go, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        if (go == null)
        {
            return null;
        }
        return GameObject.Instantiate(go, pos, rotation, parent);
    }

    /// <summary>
    /// 销毁GameObject
    /// </summary>
    public void ReleaseGameObject(Object obj, float delayTime = 0)
    {
        if (obj is GameObject)
        {
            if (delayTime == 0)
            {
                GameObject.Destroy(obj);
            }
            else
            {
                GameObject.Destroy(obj, delayTime);
            }
        }
    }

    /// <summary>
    /// 立即销毁GameObject
    /// </summary>
    public void ReleaseGameObjectImmediate(Object obj)
    {
        if (obj is GameObject)
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    #endregion

    #region 资源（预制体、贴图、声音等）

    /// <summary>
    /// 同步加载资源
    /// </summary>
    private T LoadAssetSync<T>(string assetPath)
        where T : Object
    {
        CommonAsset asset = GetCacheCommonAsset(assetPath);
        if (asset != null)
        {
            return asset.obj as T;
        }
        else
        {
            string abName = abMgr.GetABName(assetPath);
            LoadedAB loadedAB = abMgr.LoadABSync(abName);
            if (loadedAB == null)
            {
                return null;
            }
            T obj = loadedAB.bundle.LoadAsset<T>(assetPath);
            if (obj == null)
            {
                Debug.LogError($"从ab包中加载资源失败，abName：{abName}，type：{typeof(T)}，assetPath：{assetPath}");
                return null;
            }
            CacheCommonAsset(assetPath, obj, loadedAB);//缓存资源
            return obj;
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    private void LoadAssetAsync<T>(string assetPath, Action<AsyncLoadCommonAssetCallBackParam> onCompleted)
        where T : Object
    {
        CommonAsset asset = GetCacheCommonAsset(assetPath);
        if (asset != null)
        {
            AsyncLoadCommonAssetCallBackParam param = m_AsyncLoadCommonAssetCallBackParamPool.Get();
            param.obj = asset.obj;
            onCompleted?.Invoke(param);
            param.Reset();
            m_AsyncLoadCommonAssetCallBackParamPool.Put(param);
        }
        else
        {
            if (!m_AsyncLoading_CommonAsset.TryGetValue(assetPath, out AsyncLoadCommonAssetOperation operation))
            {
                string abName = abMgr.GetABName(assetPath);
                if (abMgr.LoadABAsync(abName))
                {
                    //添加到正在异步加载字典中
                    operation = m_AsyncLoadCommonAssetOperationPool.Get();
                    operation.assetPath = assetPath;
                    operation.assetType = typeof(T);
                    m_AsyncLoading_CommonAsset.Add(assetPath, operation);
                }
                else
                {
                    onCompleted?.Invoke(null);
                }
            }
            //添加回调
            if (operation != null)
            {
                operation.onCompletedList.Add(onCompleted);
            }
        }
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
        CommonAsset tempAsset = null;
        foreach (var v in m_CacheDict_CommonAsset.Values)
        {
            if (v.guid == obj.GetInstanceID())
            {
                tempAsset = v;
                break;
            }
        }
        if (tempAsset == null)
        {
            return false;
        }
        m_CacheDict_CommonAsset.Remove(tempAsset.assetPath);
        //abMgr.UnloadAB(tempAsset.loadedAB);
        tempAsset.Reset();
        m_CommonAssetPool.Put(tempAsset);
        return true;
    }

    /// <summary>
    /// 缓存通用资源
    /// </summary>
    private void CacheCommonAsset(string assetPath, Object obj, LoadedAB loadedAB)
    {
        if (m_CacheDict_CommonAsset.ContainsKey(assetPath))
        {
            return;
        }
        if (string.IsNullOrEmpty(assetPath) || obj == null || loadedAB == null)
        {
            return;
        }
        {
            CommonAsset asset = m_CommonAssetPool.Get();
            asset.assetPath = assetPath;
            asset.loadedAB = loadedAB;
            asset.lastUseTime = Time.realtimeSinceStartup;
            asset.guid = obj.GetInstanceID();
            asset.obj = obj;
            m_CacheDict_CommonAsset.Add(asset.assetPath, asset);
        }
    }

    /// <summary>
    /// 获取缓存的通用资源
    /// </summary>
    private CommonAsset GetCacheCommonAsset(string assetPath)
    {
        if (m_CacheDict_CommonAsset.TryGetValue(assetPath, out CommonAsset asset))
        {
            asset.lastUseTime = Time.realtimeSinceStartup;
            return asset;
        }
        return asset;
    }

    #endregion 资源（预制体、贴图、声音等）

    #region 场景

    /// <summary>
    /// 异步加载场景
    /// </summary>
    public void LoadSceneAsync(string assetPath, bool additive, bool activateOnLoad, Action<AsyncLoadSceneAssetCallBackParam> onCompleted)
    {
        assetPath = ReconstructPath(assetPath, ".unity");
        if (!assetPath.EndsWith(".unity"))
        {
            Debug.LogError($"只能加载后缀为.unity的场景文件，{assetPath}");
            return;
        }
        if (m_AsyncLoading_SceneAsset.TryGetValue(assetPath, out AsyncLoadSceneAssetOperation operation))
        {
            Debug.LogError($"正在加载此场景，{assetPath}");
            return;
        }
        if (m_CacheDict_SceneAsset.TryGetValue(assetPath, out SceneAsset asset))
        {
            Debug.LogError($"已经加载此场景，{assetPath}");
            return;
        }

        string abName = abMgr.GetABName(assetPath);
        if (abMgr.LoadABAsync(abName))
        {
            //添加到正在异步加载字典中
            operation = m_AsyncLoadSceneAssetOperationPool.Get();
            operation.assetPath = assetPath;
            operation.additive = additive;
            operation.activateOnLoad = activateOnLoad;
            operation.onCompleted = onCompleted;
            m_AsyncLoading_SceneAsset.Add(assetPath, operation);
        }
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    public bool UnLoadScene(Scene scene, Action callback)
    {
        if (scene == default)
        {
            return false;
        }

        SceneAsset tempAsset = null;
        foreach (var v in m_CacheDict_SceneAsset.Values)
        {
            if (scene.Equals(v.scene))
            {
                tempAsset = v;
                break;
            }
        }
        if (tempAsset == null)
        {
            return false;
        }
        m_CacheDict_SceneAsset.Remove(tempAsset.assetPath);
        AsyncOperation operation = SceneManager.UnloadSceneAsync(tempAsset.scene);
        operation.completed += (op) =>
        {
            callback?.Invoke();
        };
        return true;
    }

    /// <summary>
    /// 缓存场景资源
    /// </summary>
    private void CacheSceneAsset(string assetPath, Scene scene, LoadedAB loadedAB)
    {
        if (m_CacheDict_SceneAsset.ContainsKey(assetPath))
        {
            return;
        }
        if (string.IsNullOrEmpty(assetPath) || scene == default || loadedAB == null)
        {
            return;
        }
        {
            SceneAsset asset = m_SceneAssetPool.Get();
            asset.assetPath = assetPath;
            asset.loadedAB = loadedAB;
            asset.scene = scene;
            m_CacheDict_SceneAsset.Add(asset.assetPath, asset);
        }
    }

    #endregion 场景

    /// <summary>
    /// 重新组织路径
    /// </summary>
    /// suffix：后缀（.xxx）
    private string ReconstructPath(string assetPath, string suffix)
    {
        assetPath = assetPath.Replace('\\', '/');
        if (!Path.HasExtension(assetPath))
        {
            assetPath = assetPath + suffix;
        }
        return assetPath;
    }

    private void Update()
    {
        if (!m_IsInited)
        {
            return;
        }

        abMgr.Update();

        //通用资源
        if (m_AsyncLoading_CommonAsset.Count > 0)
        {
            m_AsyncLoading_CommonAssetList.Clear();
            m_KeyToRemoveList_CommonAsset.Clear();
            foreach (var pairs in m_AsyncLoading_CommonAsset)//防止回调中异步加载另一个资源导致m_AsyncLoading_CommonAsset在迭代遍历时被修改
            {
                m_AsyncLoading_CommonAssetList.Add(pairs.Value);
            }
            for (int i = 0; i < m_AsyncLoading_CommonAssetList.Count; i++)
            {
                AsyncLoadCommonAssetOperation operation = m_AsyncLoading_CommonAssetList[i];
                string abName = abMgr.GetABName(operation.assetPath);
                LoadedAB loadedAB = abMgr.GetLoadedAB(abName, out bool isError);
                if (isError)
                {
                    m_KeyToRemoveList_CommonAsset.Add(operation.assetPath);
                }
                else
                {
                    if (loadedAB == null)
                    {
                        continue;
                    }
                    if (operation.assetRequest == null)
                    {
                        operation.assetRequest = loadedAB.bundle.LoadAssetAsync(operation.assetPath, operation.assetType);
                    }
                    if (operation.assetRequest.isDone)
                    {
                        for (int j = 0; j < operation.onCompletedList.Count; j++)
                        {
                            AsyncLoadCommonAssetCallBackParam param = m_AsyncLoadCommonAssetCallBackParamPool.Get();
                            param.obj = operation.assetRequest.asset;
                            if (param.obj == null)
                            {
                                Debug.LogError($"加载资源失败，type：{operation.assetType}，assetPath：{operation.assetPath}");
                            }
                            try
                            {
                                operation.onCompletedList[j]?.Invoke(param);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                            param.Reset();
                            m_AsyncLoadCommonAssetCallBackParamPool.Put(param);
                        }
                        CacheCommonAsset(operation.assetPath, operation.assetRequest.asset, loadedAB);//缓存资源
                        m_KeyToRemoveList_CommonAsset.Add(operation.assetPath);
                    }
                }
            }
            for (int i = 0; i < m_KeyToRemoveList_CommonAsset.Count; i++)
            {
                AsyncLoadCommonAssetOperation tempOperation = m_AsyncLoading_CommonAsset[m_KeyToRemoveList_CommonAsset[i]];
                tempOperation.Reset();
                m_AsyncLoadCommonAssetOperationPool.Put(tempOperation);
                m_AsyncLoading_CommonAsset.Remove(m_KeyToRemoveList_CommonAsset[i]);
            }
        }

        //场景资源
        if (m_AsyncLoading_SceneAsset.Count > 0)
        {
            m_KeyToRemoveList_SceneAsset.Clear();
            foreach (var pairs in m_AsyncLoading_SceneAsset)
            {
                AsyncLoadSceneAssetOperation operation = pairs.Value;
                string abName = abMgr.GetABName(operation.assetPath);
                LoadedAB loadedAB = abMgr.GetLoadedAB(abName, out bool isError);
                if (isError)
                {
                    m_KeyToRemoveList_SceneAsset.Add(pairs.Key);
                }
                else
                {
                    if (loadedAB == null)
                    {
                        continue;
                    }
                    if (operation.sceneRequest == null)
                    {
                        operation.sceneRequest = SceneManager.LoadSceneAsync(operation.assetPath, operation.additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                        operation.sceneRequest.allowSceneActivation = operation.activateOnLoad;
                    }
                    if (operation.sceneRequest.isDone || (!operation.sceneRequest.allowSceneActivation && operation.sceneRequest.progress >= 0.9f))
                    {
                        AsyncLoadSceneAssetCallBackParam param = m_AsyncLoadSceneAssetCallBackParamPool.Get();
                        param.sceneRequest = operation.sceneRequest;
                        param.scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        try
                        {
                            operation.onCompleted?.Invoke(param);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                        CacheSceneAsset(operation.assetPath, param.scene, loadedAB);//缓存资源
                        param.Reset();
                        m_AsyncLoadSceneAssetCallBackParamPool.Put(param);
                        m_KeyToRemoveList_SceneAsset.Add(pairs.Key);
                    }
                }
            }
            for (int i = 0; i < m_KeyToRemoveList_SceneAsset.Count; i++)
            {
                AsyncLoadSceneAssetOperation tempOperation = m_AsyncLoading_SceneAsset[m_KeyToRemoveList_SceneAsset[i]];
                tempOperation.Reset();
                m_AsyncLoadSceneAssetOperationPool.Put(tempOperation);
                m_AsyncLoading_SceneAsset.Remove(m_KeyToRemoveList_SceneAsset[i]);
            }
        }

        //定时GC
        float curTime = Time.realtimeSinceStartup;
        if (curTime >= m_LastCleanTime + CLEAN_TICK)
        {
            foreach (var v in m_CacheDict_CommonAsset.Values)
            {
                v.Reset();
                m_CommonAssetPool.Put(v);
            }
            m_CacheDict_CommonAsset.Clear();
            Resources.UnloadUnusedAssets();
            m_LastCleanTime = curTime;
        }
        else if (curTime >= m_LastGCTime + GC_TICK)
        {
            GC.Collect();
            m_LastGCTime = curTime;
        }
    }
}