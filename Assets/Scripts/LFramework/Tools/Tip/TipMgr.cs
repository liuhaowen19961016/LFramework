using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 提示类型
/// </summary>
public enum TipType
{
    Top,//顶部
}

/// <summary>
/// 提示
/// </summary>
public class Tip
{
    public string txt;//显示文字
    public TipType tipType;//提示类型
    public GameObject go;//游戏物体

    /// <summary>
    /// 创建提示
    /// </summary>
    public static Tip Create(string txt, TipType tipType = TipType.Top)
    {
        Tip tip = new Tip();
        tip.txt = txt;
        tip.tipType = tipType;
        return tip;
    }

    /// <summary>
    /// 创建提示
    /// </summary>
    public static Tip Create(int LID, TipType tipType = TipType.Top, params object[] arg)
    {
        Tip tip = new Tip();
        //TODO 走多语言
        //tip.txt = LID
        tip.tipType = tipType;
        return tip;
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    public Tip Show()
    {
        TipMgr.Ins.Enqueue(this);
        return this;
    }
}

/// <summary>
/// 提示管理器
/// </summary>
public class TipMgr : MonoSingleton<TipMgr>
{
    Dictionary<TipType, Queue<Tip>> m_TipDict = new Dictionary<TipType, Queue<Tip>>();//提示队列
    Dictionary<TipType, GameObject> m_TipAssetDict = new Dictionary<TipType, GameObject>();//提示预制体缓存

    Dictionary<TipType, bool> m_InShow = new Dictionary<TipType, bool>();//每个类型的提示是否正在显示

    const float QuickShowTime = 1;//显示时长（有下一个提示）
    const float ShowTime = 5;//显示时长(没有下一个提示)

    /// <summary>
    /// 创建提示
    /// </summary>
    public void Enqueue(Tip tip)
    {
        //存入对应tip类型的队列中
        Queue<Tip> tipQueue;
        if (!m_TipDict.TryGetValue(tip.tipType, out tipQueue))
        {
            tipQueue = new Queue<Tip>();
            m_TipDict.Add(tip.tipType, tipQueue);
        }
        m_TipDict[tip.tipType].Enqueue(tip);

        InitAsset(tip.tipType, OnTipAssetInit);
    }

    /// <summary>
    /// 初始化资源
    /// </summary>
    void InitAsset(TipType tipType, Action<TipType> callBack)
    {
        GameObject go;
        if (!m_TipAssetDict.TryGetValue(tipType, out go))
        {
            //TODO 之后需要改为统一的资源加载接口
            GameObject obj = Resources.Load<GameObject>(GetTipAssetPath(tipType));
            m_TipAssetDict.Add(tipType, obj);
            callBack?.Invoke(tipType);
        }
        else
        {
            callBack?.Invoke(tipType);
        }
    }

    /// <summary>
    /// 提示资源加载后的回调
    /// </summary>
    void OnTipAssetInit(TipType tipType)
    {
        if (InShow(tipType))
        {
            return;
        }
        Queue<Tip> tipQueue = GetTipQueue(tipType);
        if (tipQueue == null)
        {
            return;
        }

        Tip tip = tipQueue.Dequeue();
        //TODO：之后需要改为统一的资源加载接口
        GameObject go = GameObject.Instantiate(m_TipAssetDict[tipType]);
        //TODO：之后需要改为统一的初始化方法
        go.GetComponentInChildren<Text>().text = tip.txt;
        go.transform.SetParent(GameObject.Find("Canvas").transform, false);

        tip.go = go;
        SetInShow(tipType, true);
        TimerMgr.Ins.Register(QuickShowTime, onComplete: () =>
         {
             if (GetTipQueue(tipType) == null)
             {
                 TimerMgr.Ins.Register(ShowTime, onComplete: () =>
                 {
                     SetInShow(tipType, false);
                     GameObject.Destroy(tip.go);
                     OnTipAssetInit(tipType);
                 });
             }
             else
             {
                 SetInShow(tipType, false);
                 GameObject.Destroy(tip.go);
                 OnTipAssetInit(tipType);
             }
         });
    }

    /// <summary>
    /// 得到提示队列
    /// </summary>
    Queue<Tip> GetTipQueue(TipType tipType)
    {
        Queue<Tip> tipQueue;
        if (!m_TipDict.TryGetValue(tipType, out tipQueue))
        {
            return null;
        }
        if (tipQueue.Count <= 0)
        {
            return null;
        }
        return tipQueue;
    }

    /// <summary>
    /// 是否正在显示
    /// </summary>
    bool InShow(TipType tipType)
    {
        bool inShow;
        if (!m_InShow.TryGetValue(tipType, out inShow))
        {
            return false;
        }
        else
        {
            return m_InShow[tipType];
        }
    }

    /// <summary>
    /// 设置是否正在显示
    /// </summary>
    void SetInShow(TipType tipType, bool b)
    {
        bool inShow;
        if (!m_InShow.TryGetValue(tipType, out inShow))
        {
            m_InShow.Add(tipType, b);
        }
        else
        {
            m_InShow[tipType] = b;
        }
    }

    /// <summary>
    /// 得到提示与预制体路径
    /// </summary>
    string GetTipAssetPath(TipType tipType)
    {
        //TODO 添加tip类型在此扩展
        switch (tipType)
        {
            case TipType.Top:
                return "Prefabs/Item_TopTip";
            default:
                return "";
        }
    }
}
