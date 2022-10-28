using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 提示框管理器
/// </summary>
public class TooltipMgr : MonoSingleton<TooltipMgr>
{
    List<Item_Tooltip> m_TooltipList = new List<Item_Tooltip>();//提示框列表

    RectTransform m_ParentRect;//提示框的父物体
    public RectTransform ParentRect
    {
        get
        {
            if (m_ParentRect == null)
            {
                m_ParentRect = GameObject.Find("null").GetComponent<RectTransform>();
            }
            return m_ParentRect;
        }
    }

    /// <summary>
    /// 显示提示框
    /// </summary>
    public void Show(TooltipData data)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Item_Tooltip"));
        go.transform.SetParent(ParentRect, false);
        Item_Tooltip subView = go.GetComponent<Item_Tooltip>();

        subView.Set(data);
        m_TooltipList.Add(subView);
    }

    /// <summary>
    /// 强制关闭所提示框
    /// </summary>
    public void ForceCloseAll()
    {
        for (int i = m_TooltipList.Count - 1; i >= 0; i--)
        {
            m_TooltipList[i].ForceCloseSelf();
            m_TooltipList.RemoveAt(i);
        }
    }

    /// <summary>
    /// 关闭提示框
    /// </summary>
    public void Close(Item_Tooltip tooltip)
    {
        tooltip.CloseSelf();
        m_TooltipList.Remove(tooltip);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = m_TooltipList.Count - 1; i >= 0; i--)
            {
                Item_Tooltip tooltip = m_TooltipList[i];
                if (tooltip.Data.isTouchAnyClose)
                {
                    if (tooltip.Data.isTouchSelfClose)
                    {
                        Close(tooltip);
                    }
                    else
                    {
                        bool clickSelf = false;
                        PointerEventData eventData = new PointerEventData(EventSystem.current);
                        eventData.position = Input.mousePosition;
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(eventData, results);
                        for (int j = results.Count - 1; j >= 0; j--)
                        {
                            if (results[j].gameObject.transform.IsChildOf(tooltip.transform))
                            {
                                clickSelf = true;
                                break;
                            }
                        }
                        if (!clickSelf)
                        {
                            Close(tooltip);
                        }
                    }
                }
            }
        }
    }
}

/// <summary>
/// 提示框
/// </summary>
public class Tooltip
{
    public TooltipData data = new TooltipData();

    /// <summary>
    /// 创建
    /// </summary>
    /// pos：世界坐标
    public static Tooltip Create(string content, Vector2 pos)
    {
        Tooltip tooltip = new Tooltip();
        tooltip.data.content = content;
        tooltip.data.pos = pos;
        tooltip.data.contentType = TooltipData.EContentType.Normal;
        return tooltip;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// pos：世界坐标
    public static Tooltip Create(GameObject customGo, Vector2 customGoSize, Vector2 pos)
    {
        Tooltip tooltip = new Tooltip();
        tooltip.data.customGo = customGo;
        tooltip.data.customGoSize = customGoSize;
        tooltip.data.pos = pos;
        tooltip.data.contentType = TooltipData.EContentType.Custom;
        return tooltip;
    }

    /// <summary>
    /// 设置方向类型
    /// </summary>
    public Tooltip SetDirType(TooltipData.EDirType dirType)
    {
        data.dirType = dirType;
        return this;
    }

    /// <summary>
    /// 设置显示区域
    /// </summary>
    public Tooltip SetShowRect(RectTransform showRect)
    {
        data.showRect = showRect;
        return this;
    }

    /// <summary>
    /// 设置偏移量
    /// </summary>
    public Tooltip SetOffset(float offset)
    {
        data.offset = offset;
        return this;
    }

    /// <summary>
    /// 设置边距
    /// </summary>
    public Tooltip SetMargin(Vector2 margin)
    {
        data.margin = margin;
        return this;
    }

    /// <summary>
    /// 设置是否显示箭头
    /// </summary>
    public Tooltip SetIsShowArrow(bool isShowArrow)
    {
        data.isShowArrow = isShowArrow;
        return this;
    }

    /// <summary>
    /// 设置是否点击任意位置关闭
    /// </summary>
    public Tooltip SetIsTouchAnyClose(bool isTouchAnyClose)
    {
        data.isTouchAnyClose = isTouchAnyClose;
        return this;
    }

    /// <summary>
    /// 设置是否点击自身关闭
    /// </summary>
    public Tooltip SetIsTouchSelfClose(bool isTouchSelfClose)
    {
        data.isTouchSelfClose = isTouchSelfClose;
        return this;
    }

    /// <summary>
    /// 设置是否自动关闭
    /// </summary>
    public Tooltip SetIsAutoClose(bool isAutoClose)
    {
        data.isAutoClose = isAutoClose;
        return this;
    }

    /// <summary>
    /// 设置自动关闭的秒数
    /// </summary>
    public Tooltip SetAutoCloseSec(float autoCloseSec)
    {
        data.autoCloseSec = autoCloseSec;
        return this;
    }

    /// <summary>
    /// 设置关闭回调
    /// </summary>
    public Tooltip SetCloseAction(Action onClose)
    {
        data.onClose = onClose;
        return this;
    }

    /// <summary>
    /// 显示
    /// </summary>
    public Tooltip Show()
    {
        TooltipMgr.Ins.Show(data);
        return this;
    }
}

/// <summary>
/// 提示框数据
/// </summary>
public class TooltipData
{
    /// <summary>
    /// 内容类型
    /// </summary>
    public enum EContentType
    {
        Normal,         //标准的（只显示文字）
        Custom,         //自定义
    }

    /// <summary>
    /// 方向类型
    /// </summary>
    public enum EDirType
    {
        Up,
        Down,
        Left,
        Right,
        Auto,
    }

    public string content;//文字内容
    public GameObject customGo;//自定义的游戏物体
    public Vector2 customGoSize;//自定义的游戏物体长宽

    public Vector2 pos;//位置（世界坐标）
    public RectTransform showRect;//显示区域
    public Vector2 margin = new Vector2(60, 40);//页边空白
    public EContentType contentType;//内容类型
    public EDirType dirType = EDirType.Auto;//方向类型
    public float offset;//偏移量
    public bool isShowArrow = true;//是否显示箭头
    public bool isTouchAnyClose = true;//是否点击任意位置关闭
    public bool isTouchSelfClose = true;//是否点击自身关闭
    public bool isAutoClose = false;//是否自动关闭
    public float autoCloseSec = 3;//自动关闭的秒数
    public Action onClose;//关闭的回调
}
