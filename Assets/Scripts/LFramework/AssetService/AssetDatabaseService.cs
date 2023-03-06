#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// AssetDatabase管理
/// </summary>
public partial class AssetDatabaseService : MonoSingleton<AssetDatabaseService>, IAssetService
{
    private const float CLEAN_TICK = 200;//200
    private const float GC_TICK = 60;//60
    private float m_LastCleanTime;//上一次clean时间
    private float m_LastGCTime;//上一次GC时间

    private ClassObjectPool<AsyncLoadCommonAssetCallBackParam> m_AsyncLoadCommonAssetCallBackParamPool = ObjectPoolMgr.Ins.GetOrCreateClassObjectPool<AsyncLoadCommonAssetCallBackParam>(5);//异步加载通用资源回调参数类的类对象池
    private ClassObjectPool<AsyncLoadSceneAssetOperation> m_AsyncLoadSceneAssetOperationPool = ObjectPoolMgr.Ins.GetOrCreateClassObjectPool<AsyncLoadSceneAssetOperation>(5);//异步加载场景资源类的类对象池
    private ClassObjectPool<AsyncLoadSceneAssetCallBackParam> m_AsyncLoadSceneAssetCallBackParamPool = ObjectPoolMgr.Ins.GetOrCreateClassObjectPool<AsyncLoadSceneAssetCallBackParam>(5);//异步加载场景资源回调参数类的类对象池

    //场景资源
    private Dictionary<string, AsyncLoadSceneAssetOperation> m_AsyncLoading_SceneAsset = new Dictionary<string, AsyncLoadSceneAssetOperation>();//正在异步加载的场景资源字典
    private List<string> m_KeyToRemoveList_SceneAsset = new List<string>();//将要从正在异步加载的场景资源字典中移除的key列表

    private bool m_IsInited;//是否初始化

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        m_IsInited = true;
    }

    #region 实例化

    /// <summary>
    /// 同步实例化GameObject
    /// </summary>
    public GameObject InstantiateSync(string assetPath, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject obj = LoadGameObjectSync(assetPath);
        return Instantiate(obj);
    }

    /// <summary>
    /// 异步实例化GameObject
    /// </summary>
    public void InstantiateAsync(string assetPath, Action<GameObject> onCompleted, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        LoadGameObjectAsync(assetPath, (obj) =>
        {
            GameObject go = Instantiate(obj);
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

    #endregion  实例化

    #region 资源（预制体、贴图、声音等）

    /// <summary>
    /// 同步加载资源
    /// </summary>
    private T LoadAssetSync<T>(string assetPath)
        where T : Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        return asset;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    private void LoadAssetAsync<T>(string assetPath, Action<AsyncLoadCommonAssetCallBackParam> onCompleted)
        where T : Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        AsyncLoadCommonAssetCallBackParam param = m_AsyncLoadCommonAssetCallBackParamPool.Allocate();
        param.obj = asset;
        onCompleted?.Invoke(param);
        param.Reset();
    }

    #endregion 资源（预制体、贴图、声音等）

    #region 场景

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
        //添加到正在异步加载字典中
        operation = m_AsyncLoadSceneAssetOperationPool.Allocate();
        operation.assetPath = assetPath;
        operation.additive = additive;
        operation.activateOnLoad = activateOnLoad;
        operation.onCompleted = onCompleted;
        m_AsyncLoading_SceneAsset.Add(assetPath, operation);
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
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        operation.completed += (op) =>
        {
            callback?.Invoke();
        };
        return true;
    }

    #endregion 场景

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

        //场景资源
        if (m_AsyncLoading_SceneAsset.Count > 0)
        {
            m_KeyToRemoveList_SceneAsset.Clear();
            foreach (var pairs in m_AsyncLoading_SceneAsset)
            {
                AsyncLoadSceneAssetOperation operation = pairs.Value;
                if (operation.sceneRequest == null)
                {
                    operation.sceneRequest = SceneManager.LoadSceneAsync(operation.assetPath, operation.additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                    operation.sceneRequest.allowSceneActivation = operation.activateOnLoad;
                }
                if (operation.sceneRequest.isDone || (!operation.sceneRequest.allowSceneActivation && operation.sceneRequest.progress >= 0.9f))
                {
                    AsyncLoadSceneAssetCallBackParam param = m_AsyncLoadSceneAssetCallBackParamPool.Allocate();
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
                    param.Reset();
                    m_AsyncLoadSceneAssetCallBackParamPool.Recycle(param);
                    m_KeyToRemoveList_SceneAsset.Add(pairs.Key);
                }
            }
            for (int i = 0; i < m_KeyToRemoveList_SceneAsset.Count; i++)
            {
                AsyncLoadSceneAssetOperation tempOperation = m_AsyncLoading_SceneAsset[m_KeyToRemoveList_SceneAsset[i]];
                tempOperation.Reset();
                m_AsyncLoadSceneAssetOperationPool.Recycle(tempOperation);
                m_AsyncLoading_SceneAsset.Remove(m_KeyToRemoveList_SceneAsset[i]);
            }
        }

        //定时GC
        float curTime = Time.realtimeSinceStartup;
        if (curTime >= m_LastCleanTime + CLEAN_TICK)
        {
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

#endif
