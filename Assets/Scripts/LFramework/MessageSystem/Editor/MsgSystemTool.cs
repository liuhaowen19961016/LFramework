using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

/// <summary>
/// 事件系统工具
/// </summary>
public class MsgSystemTool
{
    public static List<Type> EventClassTypes = new List<Type>()//所有事件类
        {
            typeof(MsgConst),
        };

    [MenuItem("工具/事件系统/检查事件绑定关系", priority = 1001)]
    private static void CheckMsgBind()
    {
        string addListenerMethod = "AddListener";
        string removeListenerMethod = "RemoveListener";

        Dictionary<string, Dictionary<string, int>> checkDict = new Dictionary<string, Dictionary<string, int>>();//<脚本名，<事件名称-回调函数参数字符串-回调函数名，绑定数量>>

        List<string> msgTypeStrs = new List<string>();
        foreach (var temp in EventClassTypes)
        {
            foreach (var fi in temp.GetFields())
            {
                if (!msgTypeStrs.Contains(fi.Name))
                {
                    msgTypeStrs.Add(fi.Name);
                }
            }
        }
        //遍历所有脚本检查事件绑定关系
        Dictionary<string, AssetInfo> assetPath2AssetInfo = AssetTool.AnalysisAssets(AssetTool.AssetsRootPath, AssetTool.ScriptPatterns);
        try
        {
            List<AssetInfo> assetInfos = assetPath2AssetInfo.Values.ToList();
            for (int i = 0; i < assetInfos.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("正在检查绑定关系", $"正在检查{assetInfos[i].assetName}", (i + 1) * 1f / assetInfos.Count))
                {
                    throw new Exception("用户取消检查绑定关系");
                }
                string assetName = assetInfos[i].assetName;//脚本名
                string[] lines = File.ReadAllLines(assetInfos[i].relPath);
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];//每行内容
                    line = line.Replace(" ", "");
                    int leftBracketIndex = line.IndexOf("(");
                    int rightBracketIndex = line.IndexOf(")");
                    for (int k = 0; k < msgTypeStrs.Count; k++)
                    {
                        string msgTypeStr = msgTypeStrs[k];//事件名称
                        bool containMsgTypeStr = EventClassTypes.Any(t => line.Contains($"{t.Name}.{msgTypeStr}"));
                        if (containMsgTypeStr && line.Contains(addListenerMethod))
                        {
                            if (line.Contains("//") && line.IndexOf("//") < line.IndexOf(addListenerMethod))
                            {
                                continue;
                            }
                            string callbackParamStr = line.Substring(line.IndexOf(addListenerMethod) + addListenerMethod.Length, leftBracketIndex - (line.IndexOf(addListenerMethod) + addListenerMethod.Length));//回调函数参数字符串
                            string callbackName = line.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Split(',')[1];//回调函数名
                            string infoStr = $"{msgTypeStr}-{callbackParamStr}-{callbackName}";
                            if (!checkDict.TryGetValue(assetName, out Dictionary<string, int> infoStr2BindCount))
                            {
                                infoStr2BindCount = new Dictionary<string, int>();
                                checkDict.Add(assetName, infoStr2BindCount);
                            }
                            if (!infoStr2BindCount.ContainsKey(infoStr))
                            {
                                infoStr2BindCount.Add(infoStr, 0);
                            }
                            infoStr2BindCount[infoStr]++;
                        }
                        else if (containMsgTypeStr && line.Contains(removeListenerMethod))
                        {
                            if (line.Contains("//") && line.IndexOf("//") < line.IndexOf(removeListenerMethod))
                            {
                                continue;
                            }
                            string callbackParamStr = line.Substring(line.IndexOf(removeListenerMethod) + removeListenerMethod.Length, leftBracketIndex - (line.IndexOf(removeListenerMethod) + removeListenerMethod.Length));//回调函数参数字符串
                            string callbackName = line.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Split(',')[1];//回调函数名
                            string infoStr = $"{msgTypeStr}-{callbackParamStr}-{callbackName}";
                            if (!checkDict.TryGetValue(assetName, out Dictionary<string, int> infoStr2BindCount))
                            {
                                infoStr2BindCount = new Dictionary<string, int>();
                                checkDict.Add(assetName, infoStr2BindCount);
                            }
                            if (!infoStr2BindCount.ContainsKey(infoStr))
                            {
                                infoStr2BindCount.Add(infoStr, 0);
                            }
                            infoStr2BindCount[infoStr]--;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"检查绑定关系中断，{e}");
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        //遍历checkDict获取到绑定数量不为0的
        foreach (var pairs1 in checkDict)
        {
            string assetName = pairs1.Key;
            foreach (var pairs2 in pairs1.Value)
            {
                string infoStr = pairs2.Key;
                int bindCount = pairs2.Value;
                if (bindCount == 0)
                {
                    continue;
                }
                string msgTypeStr = infoStr.Split('-')[0];
                string callbackParamStr = infoStr.Split('-')[1];
                string callbackName = infoStr.Split('-')[2];
                if (bindCount > 0)
                {
                    Debug.LogError($"{assetName}中的{msgTypeStr}事件有{bindCount}个未移除监听\n回调参数：{callbackParamStr}\n回调函数名：{callbackName}");
                }
                if (bindCount < 0)
                {
                    Debug.LogError($"{assetName}中的{msgTypeStr}事件有{Mathf.Abs(bindCount)}个未添加监听\n回调参数：{callbackParamStr}\n回调函数名：{callbackName}");
                }
            }
        }
    }
}