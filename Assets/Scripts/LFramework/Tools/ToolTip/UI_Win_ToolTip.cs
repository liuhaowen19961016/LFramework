using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 提示框界面
/// </summary>
public class UI_Win_ToolTip : MonoBehaviour
{
    //组件
    public RectTransform c_toolTip;
    public RectTransform img_bg;
    public RectTransform img_arrow;
    public Text txt_tip;

    ToolTipData m_Data;//提示框数据

    float textMaxWidth = 500;//文本最大宽度

    TimerData m_AutoCloseTimer;//自动关闭的计时器

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(ToolTipData data)
    {
        m_Data = data;

        //设置显示
        if (m_Data.contentType == ToolTipData.ContentType.Common)
        {
            SetText();
            SetBg();
        }
        else if (m_Data.contentType == ToolTipData.ContentType.Custom)
        {
            m_Data.childGo.transform.SetParent(img_bg.transform, false);
            m_Data.childGo.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            SetBg();
        }

        //设置位置
        c_toolTip.anchoredPosition = m_Data.pos;
        if (m_Data.fixedDir)
        {
            SetPos();
        }
        else
        {
            CalcDirType();
            SetPos();
        }

        if (m_Data.isAutoClose)
        {
            m_AutoCloseTimer = TimerMgr.Ins.Register(m_Data.autoCloseSec, false, false, null, OnClose);
        }
    }

    /// <summary>
    /// 设置文本
    /// </summary>
    void SetText()
    {
        txt_tip.text = m_Data.text;
        if (txt_tip.preferredWidth < textMaxWidth)
        {
            txt_tip.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, txt_tip.preferredWidth);
            txt_tip.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt_tip.preferredHeight);
        }
        else
        {
            txt_tip.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textMaxWidth);
            txt_tip.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt_tip.preferredHeight);
        }
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    void SetPos()
    {
        Vector2 bgRectSize = img_bg.rect.size;
        Vector2 arrowRectSize = img_arrow.rect.size;

        float offset = m_Data.offset;
        float arrowOffset = 0;
        switch (m_Data.dirType)
        {
            case ToolTipData.DirType.Up:
                offset += bgRectSize.y / 2;
                arrowOffset += bgRectSize.y / 2;
                if (m_Data.isShowArrow)
                {
                    offset += arrowRectSize.y / 2;
                    arrowOffset += arrowRectSize.y / 2;
                    img_arrow.anchoredPosition -= Vector2.up * arrowOffset;
                }
                c_toolTip.anchoredPosition += Vector2.up * offset;
                AdjustOffset();
                SetArrow();
                break;
            case ToolTipData.DirType.Down:
                offset += bgRectSize.y / 2;
                arrowOffset += bgRectSize.y / 2;
                if (m_Data.isShowArrow)
                {
                    offset += arrowRectSize.y / 2;
                    arrowOffset += arrowRectSize.y / 2;
                    img_arrow.anchoredPosition -= Vector2.down * arrowOffset;
                }
                c_toolTip.anchoredPosition += Vector2.down * offset;
                AdjustOffset();
                SetArrow();
                break;
            case ToolTipData.DirType.Right:
                offset += bgRectSize.x / 2;
                arrowOffset += bgRectSize.x / 2;
                if (m_Data.isShowArrow)
                {
                    offset += arrowRectSize.x / 2;
                    arrowOffset += arrowRectSize.x / 2;
                    img_arrow.anchoredPosition -= Vector2.right * arrowOffset;
                }
                c_toolTip.anchoredPosition += Vector2.right * offset;
                AdjustOffset();
                SetArrow();
                break;
            case ToolTipData.DirType.Left:
                offset += bgRectSize.x / 2;
                arrowOffset += bgRectSize.x / 2;
                if (m_Data.isShowArrow)
                {
                    offset += arrowRectSize.x / 2;
                    arrowOffset += arrowRectSize.x / 2;
                    img_arrow.anchoredPosition -= Vector2.left * arrowOffset;
                }
                c_toolTip.anchoredPosition += Vector2.left * offset;
                AdjustOffset();
                SetArrow();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 设置箭头
    /// </summary>
    /// 美术素材的箭头是朝右的情况下可以用下面的逻辑设置方向，否则需要更改逻辑
    void SetArrow()
    {
        if (m_Data.isShowArrow)
        {
            switch (m_Data.dirType)
            {
                case ToolTipData.DirType.Up:
                    img_arrow.eulerAngles = new Vector3(0, 0, -90);
                    break;
                case ToolTipData.DirType.Down:
                    img_arrow.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case ToolTipData.DirType.Right:
                    img_arrow.eulerAngles = new Vector3(0, 0, 180);
                    break;
                case ToolTipData.DirType.Left:
                    img_arrow.eulerAngles = new Vector3(0, 0, 0);
                    break;
                default:
                    break;
            }
            img_arrow.gameObject.SetActive(true);
        }
        else
        {
            img_arrow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置背景
    /// </summary>
    void SetBg()
    {
        if (m_Data.isShowBg)
        {
            if (m_Data.contentType == ToolTipData.ContentType.Common)
            {
                img_bg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Data.isHaveMargin
                    ? txt_tip.rectTransform.rect.width + m_Data.margin.x
                    : txt_tip.rectTransform.rect.width);
                img_bg.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Data.isHaveMargin
                    ? txt_tip.rectTransform.rect.height + m_Data.margin.y
                    : txt_tip.rectTransform.rect.height);
            }
            else if (m_Data.contentType == ToolTipData.ContentType.Custom)
            {
                img_bg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Data.isHaveMargin
                    ? m_Data.bgRectSize.x + m_Data.margin.x
                    : m_Data.bgRectSize.x);
                img_bg.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Data.isHaveMargin
                    ? m_Data.bgRectSize.y + m_Data.margin.y
                    : m_Data.bgRectSize.y);
            }
            img_bg.gameObject.SetActive(true);
        }
        else
        {
            img_bg.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 调整偏移量(防止显示在屏幕外)
    /// </summary>
    void AdjustOffset()
    {
        Vector3[] corners = new Vector3[4];
        img_bg.GetWorldCorners(corners);

        float offset;
        if (m_Data.dirType == ToolTipData.DirType.Up
            || m_Data.dirType == ToolTipData.DirType.Down)
        {
            float xLeft = CTUtils.World2UI(true, corners[0], GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera).x;
            float xRight = CTUtils.World2UI(true, corners[3], GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera).x;
            if (xLeft < m_Data.canvasBorder[0])
            {
                offset = m_Data.canvasBorder[0] - xLeft;
                c_toolTip.anchoredPosition += new Vector2(offset, 0);
                img_arrow.anchoredPosition -= new Vector2(offset, 0);
            }
            else if (xRight > m_Data.canvasBorder[1])
            {
                offset = xRight - m_Data.canvasBorder[1];
                c_toolTip.anchoredPosition -= new Vector2(offset, 0);
                img_arrow.anchoredPosition += new Vector2(offset, 0);
            }
        }
        else if (m_Data.dirType == ToolTipData.DirType.Right
            || m_Data.dirType == ToolTipData.DirType.Left)
        {
            float yUp = CTUtils.World2UI(true, corners[1], GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera).y;
            float yDown = CTUtils.World2UI(true, corners[0], GameObject.Find("Canvas").transform as RectTransform, GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera).y;
            if (yUp > m_Data.canvasBorder[3])
            {
                offset = yUp - m_Data.canvasBorder[3];
                c_toolTip.anchoredPosition -= new Vector2(0, offset);
                img_arrow.anchoredPosition += new Vector2(0, offset);
            }
            else if (yDown < m_Data.canvasBorder[2])
            {
                offset = m_Data.canvasBorder[2] - yDown;
                c_toolTip.anchoredPosition += new Vector2(0, offset);
                img_arrow.anchoredPosition -= new Vector2(0, offset);
            }
        }
    }

    /// <summary>
    /// 计算方向类型
    /// </summary>
    void CalcDirType()
    {
        float screenHeight = m_Data.canvasBorder[3] - m_Data.canvasBorder[2];
        float screenWidth = m_Data.canvasBorder[1] - m_Data.canvasBorder[0];
        Vector2 pos = c_toolTip.anchoredPosition;
        bool horizontal;
        if (pos.x == 0
            && pos.y == 0)
        {
            horizontal = screenWidth >= screenHeight;
        }
        else
        {
            if (pos.x == 0)
            {
                horizontal = 0 >= screenHeight / pos.y;
            }
            else if (pos.y == 0)
            {
                horizontal = screenWidth / pos.x >= 0;
            }
            else
            {
                horizontal = screenWidth / pos.x >= screenHeight / pos.y;
            }
        }
        m_Data.dirType = horizontal
            ? (pos.x >= 0 ? ToolTipData.DirType.Left : ToolTipData.DirType.Right)
            : (pos.y >= 0 ? ToolTipData.DirType.Down : ToolTipData.DirType.Up);
    }

    private void Update()
    {
        if (m_Data.isTouchClose
            && Input.GetMouseButtonDown(0))
        {
            if (m_Data.isTouchSelfClose)
            {
                OnClose();
                return;
            }

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0)
            {
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    if (results[i].gameObject.transform.IsChildOf(transform))
                    {
                        return;
                    }
                }
                OnClose();
            }
            else
            {
                //OnClose();
            }
        }
    }

    /// <summary>
    /// 关闭的回调
    /// </summary>
    void OnClose()
    {
        m_Data.onClose?.Invoke();

        if (m_AutoCloseTimer != null)
        {
            m_AutoCloseTimer.Dispose();
            m_AutoCloseTimer = null;
        }

        //需要改，之后用UI管理器管理
        if (GameObject.Find("UI_Win_ToolTip(Clone)") != null)
        {
            GameObject.Destroy(GameObject.Find("UI_Win_ToolTip(Clone)"));
        }
    }
}
