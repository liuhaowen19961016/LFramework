using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

/// <summary>
/// AssetBundle打包面板
/// </summary>
public class ABBuildPanel : EditorWindow
{
    ReorderableList m_list;//AB打包列表的可排序列表对象

    private static Vector2 windowSize = new Vector2(1000, 500);//窗口尺寸
    private Vector2 m_ScrollPosition = Vector2.zero;//存储打包信息列表滚动条的位置

    private ABBuildConfig m_BuildConfig;//AssetBundle打包配置

    [MenuItem("打包工具/AssetBundle打包面板", priority = 0)]
    private static void Open()
    {
        EditorWindow window = GetWindow<ABBuildPanel>("AssetBundle打包面板");
        window.minSize = windowSize;
        window.maxSize = windowSize;
        window.Focus();
        window.Show();
    }

    private void OnEnable()
    {
        m_BuildConfig = ABBuildConfig.Ins;

        InitBuildInfoReorderList();
    }

    #region 绘制打包面板

    private void OnGUI()
    {
        if (m_list == null)
        {
            return;
        }

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("保存配置", GUILayout.Width(100)))
            {
                ABBuildConfig.Save();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
        {
            m_list.DoLayoutList();
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("打包"))
        {
            ABBuildConfig.Save();
            BuildAssetBundle();
        }
    }

    /// <summary>
    /// 初始化打包信息的可排序列表
    /// </summary>
    private void InitBuildInfoReorderList()
    {
        m_list = new ReorderableList(m_BuildConfig.m_BuildList, typeof(ABBuildInfo));
        m_list.elementHeight = 22;
        m_list.drawHeaderCallback = (rect) =>
        {
            GUI.Label(rect, "打包信息列表");
        };
        m_list.onAddCallback = (list) =>
        {
            ABBuildInfo info = new ABBuildInfo();
            m_BuildConfig.m_BuildList.Add(info);
        };
        m_list.drawElementCallback = (rect, index, isActive, isForce) =>
        {
            ABBuildInfo tempInfo = m_BuildConfig.m_BuildList[index];

            const float GAP1 = 10;
            const float GAP2 = 3;
            Rect r = rect;
            r.height = 18;
            r.width = windowSize.x * 0.04f;
            tempInfo.isEnable = GUI.Toggle(r, tempInfo.isEnable, "启用");
            GUI.enabled = false;
            r.xMin += r.width + GAP1;
            r.width = windowSize.x * 0.5f;
            tempInfo.path = GUI.TextField(r, tempInfo.path);
            GUI.enabled = true;
            r.xMin += r.width + GAP2;
            r.width = windowSize.x * 0.06f;
            if (GUI.Button(r, "更换路径"))
            {
                tempInfo.path = SelectFolder();
            }
            r.xMin += r.width + GAP1;
            r.width = 75;
            GUI.Label(r, "匹配过滤符：");
            r.xMin += r.width;
            r.xMax = rect.xMax;
            tempInfo.filterStr = GUI.TextField(r, tempInfo.filterStr);
        };
    }

    #endregion 绘制打包面板

    /// <summary>
    /// 打包
    /// </summary>
    private void BuildAssetBundle()
    {
        //清理
        string outputPath = BuildUtils.GetABBuildOutputPath();
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        Directory.CreateDirectory(outputPath);
        AssetDatabase.RemoveUnusedAssetBundleNames();

        //解析
        Dictionary<string, ABAsset> abAssetDict = Analysis();

        //打包
        List<AssetBundleBuild> abBuilds = new List<AssetBundleBuild>();
        foreach (var v in abAssetDict.Values)
        {
            AssetBundleBuild bundle = new AssetBundleBuild
            {
                assetBundleName = v.abName,
                assetNames = new string[] { v.assetPath },
            };
            abBuilds.Add(bundle);
        }

        BuildPipeline.BuildAssetBundles(outputPath, abBuilds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        //生成ab资源配置文件
        GenABAssetConfigFile(abAssetDict);

        EditorUtility.DisplayDialog("打包完成", $"打包后ab文件目录：\n{BuildUtils.GetABBuildOutputPath()}", "确定");
    }

    /// <summary>
    /// 解析资源
    /// </summary>
    private Dictionary<string, ABAsset> Analysis()
    {
        Dictionary<string, ABAsset> abAssetDict = new Dictionary<string, ABAsset>();
        try
        {
            foreach (var abInfo in m_BuildConfig.m_BuildList)
            {
                if (!abInfo.isEnable)
                {
                    continue;
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(abInfo.path);
                string[] filters = abInfo.filterStr.Split(';');
                AnalysisRoot(abAssetDict, directoryInfo, filters);
            }

            AnalysisDependencies(abAssetDict);

            return abAssetDict;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            EditorUtility.ClearProgressBar();
            return abAssetDict;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// 解析根目录资源
    /// </summary>
    private void AnalysisRoot(Dictionary<string, ABAsset> abAssetDict, DirectoryInfo directoryInfo, string[] filters)
    {
        foreach (var filter in filters)
        {
            FileInfo[] fileInfos = directoryInfo.GetFiles(filter, SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                string assetPath = fileInfos[i].FullName.Replace("//", "/").Replace("\\", "/");
                int index = assetPath.IndexOf("Assets/");
                assetPath = assetPath.Substring(index);
                if (EditorUtility.DisplayCancelableProgressBar("解析根目录资源", assetPath, i * 1f / fileInfos.Length))
                {
                    throw new Exception("用户取消");
                };
                if (!abAssetDict.TryGetValue(assetPath, out ABAsset abAsset))
                {
                    abAsset = new ABAsset
                    {
                        assetPath = assetPath,
                        abName = directoryInfo.Name + BuildUtils.ABSuffix,
                        assetName = Path.GetFileName(Path.GetFileNameWithoutExtension(assetPath)),
                    };
                    abAssetDict.Add(assetPath, abAsset);
                }
            }
        }
    }

    /// <summary>
    /// 解析依赖资源
    /// </summary>
    private void AnalysisDependencies(Dictionary<string, ABAsset> abAssetDict)
    {
        ABAsset[] rootABAssets = abAssetDict.Values.ToArray();
        int count = 0;
        foreach (var temp in rootABAssets)
        {
            if (EditorUtility.DisplayCancelableProgressBar("解析依赖资源", temp.assetPath, (count++) * 1f / abAssetDict.Count))
            {
                throw new Exception("用户取消");
            };
            string[] depAssetPaths = AssetDatabase.GetDependencies(temp.assetPath);
            foreach (var depAssetPath in depAssetPaths)
            {
                if (depAssetPath == temp.assetPath || BuildUtils.IsIgnoreABFile(depAssetPath))
                {
                    continue;
                }
                if (!abAssetDict.TryGetValue(depAssetPath, out ABAsset abAsset))
                {
                    abAsset = new ABAsset
                    {
                        assetPath = depAssetPath,
                        abName = temp.abName,//将依赖的资源设置为根资源ab包名
                        assetName = Path.GetFileName(Path.GetFileNameWithoutExtension(depAssetPath)),
                    };
                    abAssetDict.Add(depAssetPath, abAsset);
                }
                //添加到资源的依赖AssetBundle列表中
                temp.AddDepABAsset(abAsset);
            }
        }
    }

    /// <summary>
    /// 生成ab资源配置文件
    /// </summary>
    private void GenABAssetConfigFile(Dictionary<string, ABAsset> abAssetDict)
    {
        List<ABAsset> abAssetList = abAssetDict.Values.ToList();
        string path = Path.Combine(BuildUtils.GetABBuildOutputPath(), BuildUtils.FileName_ABAssetsConfigXML);
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        XmlSerializer xmlSerializer = new XmlSerializer(abAssetList.GetType());
        xmlSerializer.Serialize(sw, abAssetList);
        sw.Close();
        fs.Close();
    }

    #region 工具

    /// <summary>
    /// 选择一个文件夹
    /// </summary>
    private string SelectFolder()
    {
        string dataPath = Application.dataPath;
        string path = EditorUtility.OpenFolderPanel("选择文件夹", dataPath, "");
        if (!path.Contains(dataPath))
        {
            ShowNotification(new GUIContent("不能在Assets目录之外!"));
            return string.Empty;
        }
        return "Assets/" + path.Substring(dataPath.Length + 1);
    }

    #endregion 工具
}