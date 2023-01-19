using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.Build.Reporting;

/// <summary>
/// APP打包面板
/// </summary>
public class AppBuildPanel : EditorWindow
{
    private static Vector2 windowSize = new Vector2(1000, 500);//窗口尺寸

    private AppBuildConfig m_BuildConfig;//App打包配置

    [MenuItem("打包工具/App打包面板", priority = 1)]
    private static void Open()
    {
        EditorWindow window = GetWindow<AppBuildPanel>("App打包面板");
        window.minSize = windowSize;
        window.maxSize = windowSize;
        window.Focus();
        window.Show();
    }

    private void OnEnable()
    {
        m_BuildConfig = AppBuildConfig.Ins;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("保存配置", GUILayout.Width(100)))
            {
                AppBuildConfig.Save();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        m_BuildConfig.m_ProductName = EditorGUILayout.TextField("产品名称", m_BuildConfig.m_ProductName);
        m_BuildConfig.m_CompanyName = EditorGUILayout.TextField("公司名称", m_BuildConfig.m_CompanyName);
        m_BuildConfig.m_Version = EditorGUILayout.TextField("App版本号", m_BuildConfig.m_Version);
        m_BuildConfig.m_BuildNum = EditorGUILayout.IntField("Version Code", m_BuildConfig.m_BuildNum);
        m_BuildConfig.m_DefineSymbols = EditorGUILayout.TextField("编译宏", m_BuildConfig.m_DefineSymbols);
        m_BuildConfig.m_IsDevelopment = EditorGUILayout.Toggle(new GUIContent("是否构建Debug版本", "如果勾选表示打包带调试信息的版本，cs有行号信息"), m_BuildConfig.m_IsDevelopment);
        m_BuildConfig.m_BuildAAB = EditorGUILayout.Toggle(new GUIContent("是否构建AAB", "如果勾选表示构造.aab而不是.apk"), m_BuildConfig.m_BuildAAB);

        if (GUILayout.Button("打包"))
        {
            AppBuildConfig.Save();
            BuildApp();
        }
    }

    /// <summary>
    /// 打包
    /// </summary>
    private void BuildApp()
    {
        string outputPath = Path.Combine(BuildUtils.AppBuildRootPath, BuildUtils.GetPlatformName());

        //拷贝文件
        CopyFile(BuildUtils.ABBuildRootPath, Application.streamingAssetsPath);

        //清理
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        Directory.CreateDirectory(outputPath);

        //app打包信息
        AppBuildInfo buildInfo = new AppBuildInfo
        {
            buildTarget = EditorUserBuildSettings.activeBuildTarget,
            version = m_BuildConfig.m_Version,
            buildNum = m_BuildConfig.m_BuildNum,
            productName = m_BuildConfig.m_ProductName,
            companyName = m_BuildConfig.m_CompanyName,
            defineSymbols = m_BuildConfig.m_DefineSymbols,
            buildAAB = m_BuildConfig.m_BuildAAB,
            isDevelopment = m_BuildConfig.m_IsDevelopment,
            isIL2CPP = !m_BuildConfig.m_IsDevelopment || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS,
            outputPath = outputPath,
            options = m_BuildConfig.m_IsDevelopment ? (BuildOptions.Development | BuildOptions.ConnectWithProfiler) : BuildOptions.None,
        };
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildInfo.buildTarget);

        try
        {
            //切换到目标平台
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildInfo.buildTarget))
            {
                throw new Exception($"切换目标平台({buildInfo.buildTarget})失败");
            }
            //设置基础信息
            if (!string.IsNullOrEmpty(buildInfo.productName))
            {
                PlayerSettings.productName = buildInfo.productName;
            }
            if (!string.IsNullOrEmpty(buildInfo.companyName))
            {
                PlayerSettings.companyName = buildInfo.companyName;
            }
            if (!string.IsNullOrEmpty(buildInfo.version))
            {
                PlayerSettings.bundleVersion = buildInfo.version;
            }
            else
            {
                PlayerSettings.bundleVersion = $"0.0.0";
            }
            if (!string.IsNullOrEmpty(buildInfo.bundleIdentifier))
            {
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, buildInfo.bundleIdentifier);
            }
            //不同平台设置不同参数
            if (buildInfo.buildTarget == BuildTarget.Android)
            {
                if (buildInfo.isIL2CPP)
                {
                    EditorUserBuildSettings.buildAppBundle = buildInfo.buildAAB;
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                }
                else
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                }
                PlayerSettings.Android.bundleVersionCode = buildInfo.buildNum;
                //TODO：设置keystore 
            }
            else if (buildInfo.buildTarget == BuildTarget.iOS)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                PlayerSettings.iOS.buildNumber = buildInfo.buildNum.ToString();
            }
            else
            {
                EditorUserBuildSettings.buildAppBundle = false;
                if (buildInfo.isIL2CPP)
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                }
                else
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                    PlayerSettings.defaultScreenWidth = 1920;
                    PlayerSettings.defaultScreenHeight = 1080;
                    PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                }
            }
            //设置宏
            var oldDefineSymboles = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (buildInfo.defineSymbols != oldDefineSymboles)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildInfo.defineSymbols);
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                locationPathName = Path.Combine(buildInfo.outputPath, buildInfo.productName + "-" + DateTime.Now.Date.ToString("yyyy-MM-dd")) + BuildUtils.GetAppBuildFileExtension(buildInfo.buildTarget),
                target = buildInfo.buildTarget,
                targetGroup = buildTargetGroup,
                options = buildInfo.options,
            };

            //开始构建
            DateTime startDT = DateTime.Now;
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded)
            {
                long size = 0;
                if (File.Exists(report.summary.outputPath))     //TODO:Mac下始终为false，未找到原因
                {
                    FileInfo info = new FileInfo(report.summary.outputPath);
                    size = info.Length;
                }
                EditorUtility.DisplayDialog("打包完成", $"用时：{ (DateTime.Now - startDT).TotalSeconds}秒\n文件大小：{size / 1024f / 1024f:N2}M\n打包后文件目录：\n{outputPath}", "确定");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"构建app失败，{e}");
        }
    }

    /// <summary>
    /// 拷贝文件
    /// </summary>
    private void CopyFile(string srcDirPath, string destDirPath)
    {
        if (!Directory.Exists(srcDirPath))
        {
            Debug.LogError($"不存在此路径，srcDirPath：{srcDirPath}");
            return;
        }
        destDirPath = Path.Combine(destDirPath, Path.GetFileName(srcDirPath));
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        string[] filePaths = Directory.GetFileSystemEntries(srcDirPath);
        foreach (var temp in filePaths)
        {
            if (File.Exists(temp))
            {
                string destFilePath = Path.Combine(destDirPath, Path.GetFileName(temp));
                File.Copy(temp, destFilePath, true); ;
            }
            else
            {
                CopyFile(temp, destDirPath);
            }
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// app打包信息
    /// </summary>
    public struct AppBuildInfo
    {
        public string version;//版本号
        public int buildNum;//构建号
        public string productName;//项目名称
        public string companyName;//公司名称
        public string bundleIdentifier;//
        public BuildTarget buildTarget;//构建平台
        public string defineSymbols;//编译宏
        public bool isDevelopment;//是否为Debug版本
        public bool buildAAB;//是否构建AAB
        public bool isIL2CPP;//是否打包成IL2CPP
        public BuildOptions options;
        public string outputPath;//输出路径
    }
}
