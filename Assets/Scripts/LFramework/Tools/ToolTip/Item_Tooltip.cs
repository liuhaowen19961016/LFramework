using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 提示框item
/// </summary>
public class Item_Tooltip : MonoBehaviour
{
    public Text txt_content;
    public Image img_bg;
    public Animator animator;
    public RectTransform rect_pos;
    public RectTransform rect_bg;
    public RectTransform rect_arrow;
    public RectTransform rect_normal;
    public RectTransform rect_custom;
    public RectTransform rect_sv_normal;
    public RectTransform rect_sv_custom;

    TooltipData m_Data;//当前提示框的数据
    public TooltipData Data
    {
        get { return m_Data; }
    }

    Canvas m_UICanvas;//UI画布
    float[] m_ShowRectBorder = new float[4];//显示区域边界（上下左右）

    TimerTask m_AutoCloseTimer;//自动关闭的计时器
    TimerTask m_SetScrollTimer;//设置滑动条计时器

    /// <summary>
    /// 设置数据
    /// </summary>
    public void Set(TooltipData data)
    {
        m_Data = data;
        m_UICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        CalcShowRectBorder();
        CalcDirType();
        RefreshContent();
        RefreshPos();
        RefreshPivot();

        animator.enabled = true;

        if (m_Data.isAutoClose)
        {
            m_AutoCloseTimer = TimerMgr.Ins.Register(m_Data.autoCloseSec,
                onComplete: () => TooltipMgr.Ins.Close(this)
                );
        }
    }

    /// <summary>
    /// 刷新内容
    /// </summary>
    void RefreshContent()
    {
        if (m_Data.contentType == TooltipData.EContentType.Normal)
        {
            rect_sv_normal.GetComponent<ScrollRect>().elasticity = 0;
            txt_content.text = m_Data.content;
            if (txt_content.preferredWidth > m_Data.maxWidth)
            {
                txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Data.maxWidth);
                if (m_Data.canScroll)
                {
                    if (txt_content.preferredHeight > m_Data.maxHeight)
                    {
                        txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Data.maxHeight);
                    }
                    else
                    {
                        txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt_content.preferredHeight);
                    }
                }
                else
                {
                    txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt_content.preferredHeight);
                }
            }
            else
            {
                txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, txt_content.preferredWidth);
                txt_content.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt_content.preferredHeight);
            }
            ClientUtils.SetRectTransformSize(rect_sv_normal, txt_content.rectTransform.rect.size);
            ClientUtils.SetRectTransformSize(rect_normal, txt_content.rectTransform.rect.size);
            ClientUtils.SetRectTransformSize(rect_bg, txt_content.rectTransform.rect.size + m_Data.margin);
            ClientUtils.SetRectTransformSize(rect_pos, txt_content.rectTransform.rect.size + m_Data.margin);
            if (m_Data.canScroll)
            {
                m_SetScrollTimer = TimerMgr.Ins.Register(0.1f, onComplete:
                    () =>
                    {
                        rect_sv_normal.GetComponent<ScrollRect>().elasticity = 0.1f;
                        if (m_SetScrollTimer != null)
                        {
                            m_SetScrollTimer.Dispose();
                            m_SetScrollTimer = null;
                        }
                    });
            }
            else
            {
                rect_sv_normal.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                rect_sv_normal.GetComponent<ScrollRect>().enabled = false;
            }
        }
        else if (m_Data.contentType == TooltipData.EContentType.Custom)
        {
            rect_sv_custom.GetComponent<ScrollRect>().elasticity = 0;
            if (m_Data.canScroll)
            {
                if (m_Data.customGoSize.y > m_Data.maxHeight)
                {
                    m_Data.customGoSize.y = m_Data.maxHeight;
                }
            }
            rect_sv_custom.GetComponent<ScrollRect>().content = m_Data.customGo.GetComponent<RectTransform>();
            m_Data.customGo.transform.SetParent(rect_sv_custom.GetComponent<ScrollRect>().viewport.transform, false);
            m_Data.customGo.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_Data.customGo.transform.localScale = Vector3.one;
            ClientUtils.SetRectTransformSize(rect_sv_custom, m_Data.customGoSize);
            ClientUtils.SetRectTransformSize(rect_custom, m_Data.customGoSize);
            ClientUtils.SetRectTransformSize(rect_bg, m_Data.customGoSize + m_Data.margin);
            ClientUtils.SetRectTransformSize(rect_pos, m_Data.customGoSize + m_Data.margin);
            if (m_Data.canScroll)
            {
                m_SetScrollTimer = TimerMgr.Ins.Register(0.1f, onComplete:
                    () =>
                    {
                        rect_sv_custom.GetComponent<ScrollRect>().elasticity = 0.1f;
                        if (m_SetScrollTimer != null)
                        {
                            m_SetScrollTimer.Dispose();
                            m_SetScrollTimer = null;
                        }
                    });
            }
            else
            {
                rect_sv_custom.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                rect_sv_custom.GetComponent<ScrollRect>().enabled = false;
            }
        }
    }

    /// <summary>
    /// 刷新位置（背景和箭头）
    /// </summary>
    void RefreshPos()
    {
        GetComponent<RectTransform>().anchoredPosition = ClientUtils.World2UI(m_Data.pos, TooltipMgr.Ins.ParentRect, m_UICanvas.worldCamera);

        float bgOffset = m_Data.offset;
        float arrowOffset = 0;
        switch (m_Data.dirType)
        {
            case TooltipData.EDirType.Up:
                bgOffset += rect_bg.rect.height / 2;
                if (m_Data.isShowArrow)
                {
                    bgOffset += rect_arrow.rect.height / 2;
                    arrowOffset += rect_bg.rect.height / 2;
                    arrowOffset += rect_arrow.rect.height / 2;
                    rect_arrow.anchoredPosition += Vector2.down * arrowOffset;
                    RefreshArrow();
                }
                rect_pos.anchoredPosition += Vector2.up * bgOffset;
                break;
            case TooltipData.EDirType.Down:
                bgOffset += rect_bg.rect.height / 2;
                if (m_Data.isShowArrow)
                {
                    bgOffset += rect_arrow.rect.height / 2;
                    arrowOffset += rect_bg.rect.height / 2;
                    arrowOffset += rect_arrow.rect.height / 2;
                    rect_arrow.anchoredPosition += Vector2.up * arrowOffset;
                    RefreshArrow();
                }
                rect_pos.anchoredPosition += Vector2.down * bgOffset;
                break;
            case TooltipData.EDirType.Left:
                bgOffset += rect_bg.rect.width / 2;
                if (m_Data.isShowArrow)
                {
                    bgOffset += rect_arrow.rect.width / 2;
                    arrowOffset += rect_bg.rect.width / 2;
                    arrowOffset += rect_arrow.rect.width / 2;
                    rect_arrow.anchoredPosition += Vector2.right * arrowOffset;
                    RefreshArrow();
                }
                rect_pos.anchoredPosition += Vector2.left * bgOffset;
                break;
            case TooltipData.EDirType.Right:
                bgOffset += rect_bg.rect.width / 2;
                if (m_Data.isShowArrow)
                {
                    bgOffset += rect_arrow.rect.width / 2;
                    arrowOffset += rect_bg.rect.width / 2;
                    arrowOffset += rect_arrow.rect.width / 2;
                    rect_arrow.anchoredPosition += Vector2.left * arrowOffset;
                    RefreshArrow();
                }
                rect_pos.anchoredPosition += Vector2.right * bgOffset;
                break;
            default:
                break;
        }

        ClampInCanvas();
    }

    /// <summary>
    /// 刷新箭头
    /// </summary>
    /// 美术素材的箭头是朝左的情况下可以用下面的逻辑设置方向
    void RefreshArrow()
    {
        switch (m_Data.dirType)
        {
            case TooltipData.EDirType.Up:
                rect_arrow.eulerAngles = new Vector3(0, 0, 90);
                break;
            case TooltipData.EDirType.Down:
                rect_arrow.eulerAngles = new Vector3(0, 0, -90);
                break;
            case TooltipData.EDirType.Left:
                rect_arrow.eulerAngles = new Vector3(0, 0, 180);
                break;
            case TooltipData.EDirType.Right:
                rect_arrow.eulerAngles = new Vector3(0, 0, 0);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 刷新pivot（针对有弹出动画的情况）
    /// </summary>
    public void RefreshPivot()
    {
        Vector2 pivot = Vector2.zero;
        switch (m_Data.dirType)
        {
            case TooltipData.EDirType.Up:
                pivot = new Vector2((rect_arrow.anchoredPosition.x + rect_bg.rect.width / 2) / rect_bg.rect.width, 0f);
                break;
            case TooltipData.EDirType.Down:
                pivot = new Vector2((rect_arrow.anchoredPosition.x + rect_bg.rect.width / 2) / rect_bg.rect.width, 1f);
                break;
            case TooltipData.EDirType.Left:
                pivot = new Vector2(1f, (rect_arrow.anchoredPosition.y + rect_bg.rect.height / 2) / rect_bg.rect.height);
                break;
            case TooltipData.EDirType.Right:
                pivot = new Vector2(0f, (rect_arrow.anchoredPosition.y + rect_bg.rect.height / 2) / rect_bg.rect.height);
                break;
            default:
                break;
        }
        rect_pos.pivot = pivot;
        rect_pos.anchoredPosition += new Vector2(rect_bg.rect.width * (pivot.x - 0.5f), rect_bg.rect.height * (pivot.y - 0.5f));
    }

    /// <summary>
    /// 限制在画布中
    /// </summary>
    /// 以UICanvas为画布基准计算相对位置
    void ClampInCanvas()
    {
        Vector3[] bgCorners = new Vector3[4];
        rect_bg.GetWorldCorners(bgCorners);

        if (m_Data.dirType == TooltipData.EDirType.Up
            || m_Data.dirType == TooltipData.EDirType.Down)
        {
            float bgCornerRight = ClientUtils.World2UI(bgCorners[3], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).x;
            float bgCornerLeft = ClientUtils.World2UI(bgCorners[0], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).x;
            if (bgCornerRight > m_ShowRectBorder[3])
            {
                float offset = bgCornerRight - m_ShowRectBorder[3];
                rect_pos.anchoredPosition += Vector2.left * offset;
                rect_arrow.anchoredPosition += Vector2.right * offset;
            }
            else if (bgCornerLeft < m_ShowRectBorder[2])
            {
                float offset = m_ShowRectBorder[2] - bgCornerLeft;
                rect_pos.anchoredPosition += Vector2.right * offset;
                rect_arrow.anchoredPosition += Vector2.left * offset;
            }
        }
        else if (m_Data.dirType == TooltipData.EDirType.Left
            || m_Data.dirType == TooltipData.EDirType.Right)
        {
            float bgCornerUp = ClientUtils.World2UI(bgCorners[1], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).y;
            float bgCornerDown = ClientUtils.World2UI(bgCorners[0], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).y;
            if (bgCornerUp > m_ShowRectBorder[0])
            {
                float offset = bgCornerUp - m_ShowRectBorder[0];
                rect_pos.anchoredPosition += Vector2.down * offset;
                rect_arrow.anchoredPosition += Vector2.up * offset;
            }
            else if (bgCornerDown < m_ShowRectBorder[1])
            {
                float offset = m_ShowRectBorder[1] - bgCornerDown;
                rect_pos.anchoredPosition += Vector2.up * offset;
                rect_arrow.anchoredPosition += Vector2.down * offset;
            }
        }
    }

    /// <summary>
    /// 计算显示区域边界
    /// </summary>
    /// 以UICanvas为画布基准计算相对位置
    void CalcShowRectBorder()
    {
        if (m_Data.showRect == null)
        {
            m_Data.showRect = m_UICanvas.GetComponent<RectTransform>();
        }

        Vector3[] showRectCorners = new Vector3[4];
        m_Data.showRect.GetWorldCorners(showRectCorners);
        m_ShowRectBorder[0] = ClientUtils.World2UI(showRectCorners[1], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).y;
        m_ShowRectBorder[1] = ClientUtils.World2UI(showRectCorners[0], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).y;
        m_ShowRectBorder[2] = ClientUtils.World2UI(showRectCorners[0], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).x;
        m_ShowRectBorder[3] = ClientUtils.World2UI(showRectCorners[3], m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera).x;
    }

    /// <summary>
    /// 计算方向类型
    /// </summary>
    /// 以UICanvas为画布基准计算相对位置
    void CalcDirType()
    {
        if (m_Data.dirType != TooltipData.EDirType.Auto)
        {
            return;
        }

        float showRectWidth = m_ShowRectBorder[3] - m_ShowRectBorder[2];
        float showRectHeight = m_ShowRectBorder[0] - m_ShowRectBorder[1];
        Vector2 tooltipUIPos = ClientUtils.World2UI(m_Data.pos, m_UICanvas.GetComponent<RectTransform>(), m_UICanvas.worldCamera);
        float borderToUpRatio = (m_ShowRectBorder[0] - tooltipUIPos.y) / showRectHeight;
        float borderToDownRatio = (tooltipUIPos.y - m_ShowRectBorder[1]) / showRectHeight;
        float borderToLeftRatio = (tooltipUIPos.x - m_ShowRectBorder[2]) / showRectWidth;
        float borderToRightRatio = (m_ShowRectBorder[3] - tooltipUIPos.x) / showRectWidth;
        List<float> tempList = new List<float>()
        {
            borderToUpRatio,borderToDownRatio,borderToLeftRatio,borderToRightRatio,
        };
        m_Data.dirType = (TooltipData.EDirType)ClientUtils.GetMax(tempList);
    }

    /// <summary>
    /// 关闭自身
    /// </summary>
    public void CloseSelf()
    {
        m_Data.onClose?.Invoke();

        if (m_AutoCloseTimer != null)
        {
            m_AutoCloseTimer.Dispose();
            m_AutoCloseTimer = null;
        }
        if (m_SetScrollTimer != null)
        {
            m_SetScrollTimer.Dispose();
            m_SetScrollTimer = null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 强制关闭自身
    /// </summary>
    public void ForceCloseSelf()
    {
        m_Data.onClose?.Invoke();

        if (m_AutoCloseTimer != null)
        {
            m_AutoCloseTimer.Dispose();
            m_AutoCloseTimer = null;
        }
        if (m_SetScrollTimer != null)
        {
            m_SetScrollTimer.Dispose();
            m_SetScrollTimer = null;
        }

        Destroy(gameObject);
    }
}