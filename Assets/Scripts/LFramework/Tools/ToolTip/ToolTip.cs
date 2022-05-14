using System;
using UnityEngine;

/// <summary>
/// 提示框
/// </summary>
public class ToolTip
{
    //提示框数据
    ToolTipData m_Data = new ToolTipData();

    /// <summary>
    /// 创建提示框
    /// </summary>
    public static ToolTip Create(string tipText, Transform trans)
    {
        ToolTip toolTip = new ToolTip();
        toolTip.m_Data.contentType = ToolTipData.ContentType.Common;
        toolTip.m_Data.text = tipText;
        //需要改
        toolTip.m_Data.pos = CTUtils.World2UI(true, trans.position, GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera);
        return toolTip;
    }

    /// <summary>
    /// 创建提示框
    /// </summary>
    public static ToolTip Create(int tipId, Transform trans)
    {
        ToolTip toolTip = new ToolTip();
        toolTip.m_Data.contentType = ToolTipData.ContentType.Common;
        //走多语言
        //toolTip.m_Data.text = tipId;
        //需要改
        toolTip.m_Data.pos = CTUtils.World2UI(true, trans.position, GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera);
        return toolTip;
    }

    /// <summary>
    /// 创建提示框
    /// </summary>
    public static ToolTip Create(GameObject childGo, Transform trans, Vector2 size)
    {
        ToolTip toolTip = new ToolTip();
        toolTip.m_Data.contentType = ToolTipData.ContentType.Custom;
        toolTip.m_Data.childGo = childGo;
        toolTip.m_Data.bgRectSize = size;
        //需要改
        toolTip.m_Data.pos = CTUtils.World2UI(true, trans.position, GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera);
        return toolTip;
    }

    /// <summary>
    /// 设置方向类型
    /// </summary>
    public ToolTip SetDirType(ToolTipData.DirType dirType)
    {
        m_Data.dirType = dirType;
        m_Data.fixedDir = true;
        return this;
    }

    /// <summary>
    /// 设置偏移量
    /// </summary>
    public ToolTip SetOffset(float offset)
    {
        m_Data.offset = offset;
        return this;
    }

    /// <summary>
    /// 设置左右画布边界
    /// </summary>
    public ToolTip SetCanvasBorderLR(float min, float max)
    {
        m_Data.canvasBorder[0] = min;
        m_Data.canvasBorder[1] = max;
        return this;
    }

    /// <summary>
    /// 设置上下画布边界
    /// </summary>
    public ToolTip SetCanvasBorderUD(float min, float max)
    {
        m_Data.canvasBorder[2] = min;
        m_Data.canvasBorder[3] = max;
        return this;
    }

    /// <summary>
    /// 设置是否有页边空白
    /// </summary>
    public ToolTip SetIsHaveMargin(bool isHaveMargin)
    {
        m_Data.isHaveMargin = isHaveMargin;
        return this;
    }

    /// <summary>
    /// 设置页边空白
    /// </summary>
    public ToolTip SetMargin(Vector2 margin)
    {
        m_Data.margin = margin;
        return this;
    }

    /// <summary>
    /// 设置是否显示背景
    /// </summary>
    public ToolTip SetIsShowBg(bool isShowBg)
    {
        m_Data.isShowBg = isShowBg;
        return this;
    }

    /// <summary>
    /// 设置是否显示箭头
    /// </summary>
    public ToolTip SetIsShowArrow(bool isShowArrow)
    {
        m_Data.isShowArrow = isShowArrow;
        return this;
    }

    /// <summary>
    /// 设置是否点击任意位置关闭
    /// </summary>
    public ToolTip SetIsTouchClose(bool isTouchClose)
    {
        m_Data.isTouchClose = isTouchClose;
        return this;
    }

    /// <summary>
    /// 设置是否点击任意位置关闭
    /// </summary>
    public ToolTip SetIsTouchSelfClose(bool isTouchSelfClose)
    {
        m_Data.isTouchSelfClose = isTouchSelfClose;
        return this;
    }

    /// <summary>
    /// 设置关闭回调
    /// </summary>
    public ToolTip SetCloseCallBack(Action onClose)
    {
        m_Data.onClose = onClose;
        return this;
    }

    /// <summary>
    /// 设置自动关闭的秒数
    /// </summary>
    public ToolTip SetAutoCloseSec(float autoCloseSec)
    {
        m_Data.autoCloseSec = autoCloseSec;
        m_Data.isAutoClose = true;
        return this;
    }

    /// <summary>
    /// 显示提示框
    /// </summary>
    public ToolTip Show()
    {
        Close();

        //需要改，之后用UI管理器管理
        UI_Win_ToolTip view = GameObject.Instantiate(Resources.Load<UI_Win_ToolTip>("UI_Win_ToolTip"));
        view.transform.SetParent(GameObject.Find("Canvas").transform, false);
        view.Init(m_Data);
        return this;
    }

    /// <summary>
    /// 关闭提示框
    /// </summary>
    public ToolTip Close()
    {
        //需要改，之后用UI管理器管理
        if (GameObject.Find("UI_Win_ToolTip(Clone)") != null)
        {
            GameObject.Destroy(GameObject.Find("UI_Win_ToolTip(Clone)"));
        }
        return this;
    }
}

/// <summary>
/// 提示框数据
/// </summary>
public class ToolTipData
{
    /// <summary>
    /// 提示框内容类型
    /// </summary>
    public enum ContentType
    {
        Common, //通用样式(背景框+文字)
        Custom, //自定义
    }

    /// <summary>
    /// 提示框方向类型
    /// </summary>
    public enum DirType
    {
        Up,
        Down,
        Right,
        Left,
    }

    //——————————通用样式
    //文本
    public string text;

    //——————————自定义样式
    //子物体
    public GameObject childGo;
    //背景框的大小
    public Vector2 bgRectSize;

    //提示框内容类型
    public ContentType contentType = ContentType.Common;
    //方向类型
    public DirType dirType;
    //是否固定方向
    public bool fixedDir = false;
    //位置
    public Vector2 pos;
    //是否显示背景
    public bool isShowBg = true;
    //是否显示箭头
    public bool isShowArrow = true;
    //偏移量
    public float offset;
    //是否有页边空白
    public bool isHaveMargin = true;
    //页边空白
    public Vector2 margin = new Vector2(50, 50);
    //画布边界
    public float[] canvasBorder = new float[4]
    {
        -GameObject.Find("Canvas").GetComponent<RectTransform>().rect.width / 2,
        GameObject.Find("Canvas").GetComponent<RectTransform>().rect.width/2,
        -GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height / 2,
        GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height / 2};
    //是否点击任意位置关闭
    public bool isTouchClose = true;
    //是否点击自身关闭
    public bool isTouchSelfClose = false;
    //关闭的回调
    public Action onClose;
    //是否自动关闭
    public bool isAutoClose = false;
    //自动关闭的秒数
    public float autoCloseSec;
}
