using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 已加载的AssetBundle
/// </summary>
public class LoadedAB
{
    public string abName;//ab包名
    public AssetBundle bundle;//已加载的ab包

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        abName = string.Empty;
        bundle = null;
    }
}

/// <summary>
/// AssetBundle管理器
/// </summary>
public class ABMgr
{
    private ObjectPool<LoadedAB> m_LoadedABPool;//LoadedAB的类对象池

    public Dictionary<string, string> m_AssetPath2ABNameDict = new Dictionary<string, string>();//资源路径-ab包名
    private Dictionary<string, List<string>> m_ABName2DepABNamesDict = new Dictionary<string, List<string>>();//ab包名-依赖ab包名列表

    private Dictionary<string, LoadedAB> m_LoadedABDict = new Dictionary<string, LoadedAB>();//已加载的ab包字典（ab包名-LoadedAB）
    private Dictionary<string, AssetBundleCreateRequest> m_LoadingABDict = new Dictionary<string, AssetBundleCreateRequest>();//正在加载的ab包（ab包名-ab包异步加载请求）
    private List<string> m_KeyToRemove_LoadingAB = new List<string>();//将要从m_LoadingABDict移除的key列表
    private List<string> m_LoadErrorABList = new List<string>();//加载出错的ab包列表

    private bool m_IsInited;//是否初始化

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        InitPool();
        LoadConfig();
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
        string path = Path.Combine(BuildUtils.ABFilePath, BuildUtils.FileName_ABAssetsConfigXML);
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ABAsset>));
        List<ABAsset> abAssets = (List<ABAsset>)xmlSerializer.Deserialize(fs);
        fs.Close();

        foreach (var tempAsset in abAssets)
        {
            //资源路径-ab包名
            m_AssetPath2ABNameDict.Add(tempAsset.assetPath, tempAsset.abName);
            //ab包名-依赖ab包名列表
            if (!m_ABName2DepABNamesDict.TryGetValue(tempAsset.abName, out List<string> depABNames))
            {
                depABNames = new List<string>();
                m_ABName2DepABNamesDict.Add(tempAsset.abName, depABNames);
            }
            foreach (var tempDepAsset in tempAsset.depABAssets)
            {
                if (!depABNames.Contains(tempDepAsset.abName))
                {
                    depABNames.Add(tempDepAsset.abName);
                }
            }
        }

        m_IsInited = true;
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitPool()
    {
        m_LoadedABPool = new ObjectPool<LoadedAB>
        (
            onCreate: () => { return new LoadedAB(); },
            onPut: (obj) => { obj.Reset(); },
            onDestroy: (obj) => { obj.Reset(); obj = null; },
            capacity: 300
        );
    }

    /// <summary>
    /// 获取ab包名
    /// </summary>
    public string GetABName(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            return null;
        }
        if (m_AssetPath2ABNameDict.TryGetValue(assetPath, out string abName))
        {
            return abName;
        }
        Debug.LogError($"找不到此资源路径对应的ab包，assetPath：{assetPath}");
        return null;
    }

    /// <summary>
    /// 获取LoadedAB
    /// </summary>
    public LoadedAB GetLoadedAB(string abName, out bool isError)
    {
        if (string.IsNullOrEmpty(abName))
        {
            isError = true;
            return null;
        }
        if (m_LoadErrorABList.Contains(abName))
        {
            isError = true;
            return null;
        }
        isError = false;
        if (!m_LoadedABDict.TryGetValue(abName, out LoadedAB loadedAB))
        {
            //自身ab包都没有加载成功
            return null;
        }
        if (!m_ABName2DepABNamesDict.TryGetValue(abName, out List<string> depABNames))
        {
            //没有依赖
            return loadedAB;
        }
        for (int i = 0; i < depABNames.Count; i++)
        {
            //判断依赖的ab包是否加载成功
            if (!m_LoadedABDict.ContainsKey(depABNames[i]))
            {
                return null;
            }
        }
        return loadedAB;
    }

    #region 加载

    /// <summary>
    /// 同步加载ab包
    /// </summary>
    public LoadedAB LoadABSync(string abName)
    {
        if (string.IsNullOrEmpty(abName))
        {
            Debug.LogError($"加载ab包失败，ab包名有误");
            return null;
        }

        //加载自身
        LoadedAB loadedAB = LoadABSingleSync(abName);
        //加载依赖
        if (m_ABName2DepABNamesDict.TryGetValue(abName, out List<string> depABNames))
        {
            for (int i = 0; i < depABNames.Count; i++)
            {
                LoadABSingleSync(depABNames[i]);
            }
        }
        return loadedAB;
    }

    /// <summary>
    /// 同步加载某一个ab包
    /// </summary>
    private LoadedAB LoadABSingleSync(string abName)
    {
        if (m_LoadedABDict.TryGetValue(abName, out LoadedAB loadedAB))
        {
            //已经加载过
            return loadedAB;
        }

        string path = Path.Combine(BuildUtils.ABFilePath, abName);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
        if (assetBundle == null)
        {
            Debug.LogError($"加载ab包失败，{abName}");
            return null;
        }
        if (!m_LoadedABDict.TryGetValue(abName, out LoadedAB loadedAB2))
        {
            loadedAB2 = m_LoadedABPool.Get();
            loadedAB2.abName = abName;
            loadedAB2.bundle = assetBundle;
            m_LoadedABDict.Add(abName, loadedAB2);
            return loadedAB2;
        }
        else
        {
            return loadedAB2;
        }
    }

    /// <summary>
    /// 异步加载ab包
    /// </summary>
    public bool LoadABAsync(string abName)
    {
        if (string.IsNullOrEmpty(abName))
        {
            Debug.LogError($"加载ab包失败，ab包名有误");
            return false;
        }

        //加载自身
        bool isLoaded = LoadABSingleAsync(abName);
        if (!isLoaded)
        {
            //加载依赖
            if (m_ABName2DepABNamesDict.TryGetValue(abName, out List<string> depABNames))
            {
                for (int i = 0; i < depABNames.Count; i++)
                {
                    LoadABSingleAsync(depABNames[i]);
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 异步加载某一个ab包
    /// </summary>
    private bool LoadABSingleAsync(string abName)
    {
        if (m_LoadedABDict.TryGetValue(abName, out LoadedAB loadedAB))
        {
            //已经加载过
            return true;
        }
        if (m_LoadingABDict.TryGetValue(abName, out AssetBundleCreateRequest request))
        {
            //正在加载
            return true;
        }

        string path = Path.Combine(BuildUtils.ABFilePath, abName);
        AssetBundleCreateRequest tempRequest = AssetBundle.LoadFromFileAsync(path);
        m_LoadingABDict.Add(abName, tempRequest);
        return false;
    }

    #endregion 加载

    #region 卸载

    /// <summary>
    /// 卸载ab资源
    /// </summary>
    public void UnloadAB(LoadedAB loadedAB, bool unLoadAllLoadedObjects = true)
    {
        if (loadedAB == null)
        {
            return;
        }

        //卸载依赖
        if (m_ABName2DepABNamesDict.TryGetValue(loadedAB.abName, out List<string> depABNames))
        {
            for (int i = 0; i < depABNames.Count; i++)
            {
                UnloadABSingle(depABNames[i], unLoadAllLoadedObjects);
            }
        }
        //卸载自身
        UnloadABSingle(loadedAB.abName, unLoadAllLoadedObjects);
    }

    /// <summary>
    /// 卸载某一个ab包
    /// </summary>
    private void UnloadABSingle(string abName, bool unLoadAllLoadedObjects = true)
    {
        if (m_LoadedABDict.TryGetValue(abName, out LoadedAB loadedAB))
        {
            loadedAB.bundle.Unload(unLoadAllLoadedObjects);
            m_LoadedABDict.Remove(abName);
            loadedAB.Reset();
            m_LoadedABPool.Put(loadedAB);
        }
    }

    #endregion 卸载

    public void Update()
    {
        if (!m_IsInited)
        {
            return;
        }
        if (m_LoadingABDict.Count <= 0)
        {
            return;
        }

        m_KeyToRemove_LoadingAB.Clear();
        foreach (var pairs in m_LoadingABDict)
        {
            AssetBundleCreateRequest tempRequest = pairs.Value;
            if (tempRequest.isDone)
            {
                if (tempRequest.assetBundle == null)
                {
                    m_LoadErrorABList.Add(pairs.Key);
                    Debug.LogError($"加载ab包失败，{pairs.Key}");
                }
                else
                {
                    LoadedAB loadedAB = m_LoadedABPool.Get();
                    loadedAB.abName = pairs.Key;
                    loadedAB.bundle = tempRequest.assetBundle;
                    m_LoadedABDict.Add(pairs.Key, loadedAB);
                }
                m_KeyToRemove_LoadingAB.Add(pairs.Key);
            }
        }
        for (int i = 0; i < m_KeyToRemove_LoadingAB.Count; i++)
        {
            m_LoadingABDict.Remove(m_KeyToRemove_LoadingAB[i]);
        }
    }
}
