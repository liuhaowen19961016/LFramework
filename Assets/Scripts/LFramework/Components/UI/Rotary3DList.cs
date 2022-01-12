using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// 仿3D轮转图组件
/// </summary>
[AddComponentMenu("LFramework/UI/Rotary3DList", 50)]
[RequireComponent(typeof(CommonImage))]
public class Rotary3DList : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Rotary3DList()
    {

    }

    /// <summary>
    /// 列表item数据
    /// </summary>
    public class ListItemData
    {
        public int index;
        public GameObject go;
        public float targetValue;//目标位置长度值
        public float tempValue;//临时位置长度值(每次拖拽结束后才更新数值)
    }

    /// <summary>
    /// 轮转类型
    /// </summary>
    public enum RotaryType
    {
        Horizontal,
        Vertical,
    }

    //轮转类型
    public RotaryType rotaryType;
    //间隔
    public float spacing;
    //最大缩放值
    public float maxScale = 1;
    //最小缩放值
    public float minScale = 0.5f;
    //缓动插值
    public float t = 0.1f;
    //设置item
    public Action<ListItemData> OnSetItem;
    //拖拽开始
    public Action<PointerEventData> OnDragBegin;
    //拖拽中
    public Action<PointerEventData> OnDragging;
    //拖拽结束
    public Action<PointerEventData> OnDragEnd;

    //中心item下标
    public int CenterIndex
    {
        get { return GetCenterItemIndex(); }
    }
    //列表item总数量
    int m_ItemCount;
    //总长度值
    float m_TotalValue;
    //长度值增量
    float m_DeltaValue;
    GameObject m_Prefab;
    RectTransform m_ItemContainer;
    List<ListItemData> m_ListItemDataList = new List<ListItemData>();

    //是否在拖拽中
    bool m_InDrag;
    //每次拖拽的起始位置
    float m_BeginPos;
    //初始每个item的位置长度值
    List<float> m_InitValueList = new List<float>();

    /// <summary>
    /// 设置数据
    /// </summary>
    public void SetData(GameObject prefab, int itemCount)
    {
        m_ItemContainer = GetComponent<RectTransform>();
        m_ItemCount = itemCount;
        m_Prefab = prefab;
        m_DeltaValue = rotaryType == RotaryType.Horizontal
            ? (spacing + m_Prefab.GetComponent<RectTransform>().rect.width)
            : (spacing + m_Prefab.GetComponent<RectTransform>().rect.height);
        m_TotalValue = m_DeltaValue * m_ItemCount;

        InitData();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    void InitData()
    {
        float tempValue = 0;
        for (int i = 0; i < m_ItemCount; i++)
        {
            ListItemData data = new ListItemData();
            data.index = i;
            data.go = Instantiate(m_Prefab, transform, m_ItemContainer);
            data.targetValue = tempValue;
            data.tempValue = tempValue;
            m_ListItemDataList.Add(data);
            m_InitValueList.Add(tempValue);

            tempValue += m_DeltaValue;
        }
        m_InitValueList.Add(m_TotalValue);
    }

    /// <summary>
    /// 设置列表
    /// </summary>
    public void SetList()
    {
        foreach (var data in m_ListItemDataList)
        {
            OnSetItem?.Invoke(data);
        }

        UpdateItem(true);
    }

    /// <summary>
    /// 移动到某个下标位置
    /// </summary>
    public void MoveToIndex(int index, bool isScroll = true)
    {
        if (index < 0
            || index >= m_ItemCount)
        {
            Debug.LogError("下标超出范围,index : " + index);
            return;
        }

        int indexOffset = CenterIndex - index;
        foreach (var data in m_ListItemDataList)
        {
            float targetValue = data.tempValue + m_DeltaValue * indexOffset < 0
                ? m_TotalValue - Mathf.Abs(data.tempValue + m_DeltaValue * indexOffset) % m_TotalValue
                : (data.tempValue + m_DeltaValue * indexOffset) % m_TotalValue;
            data.targetValue = targetValue;
            data.tempValue = targetValue;
        }

        UpdateItem(!isScroll);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_InDrag = true;
        m_BeginPos = rotaryType == RotaryType.Horizontal
            ? eventData.position.x
            : eventData.position.y;
        OnDragBegin?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragging?.Invoke(eventData);

        float endPos = rotaryType == RotaryType.Horizontal
            ? eventData.position.x
            : eventData.position.y;
        float offset = endPos - m_BeginPos;

        //计算item数据
        foreach (var data in m_ListItemDataList)
        {
            float tempValue = data.tempValue + offset < 0
                 ? m_TotalValue - Mathf.Abs(data.tempValue + offset) % m_TotalValue
                 : (data.tempValue + offset) % m_TotalValue;
            data.targetValue = tempValue;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_InDrag = false;
        OnDragEnd?.Invoke(eventData);

        foreach (var data in m_ListItemDataList)
        {
            float nearlyValue = GetNearlyValue(data.targetValue);
            data.targetValue = nearlyValue;
            data.tempValue = nearlyValue;
        }
    }

    private void Update()
    {
        UpdateItem(m_InDrag);
    }

    /// <summary>
    /// 更新item
    /// </summary>
    /// isForce : 是否强制复位
    void UpdateItem(bool isForce)
    {
        foreach (var data in m_ListItemDataList)
        {
            float ratio = data.targetValue / m_TotalValue;

            //更新位置
            float pos = GetPos(ratio);
            Vector2 targetPos = rotaryType == RotaryType.Horizontal
                ? new Vector2(pos, 0)
                : new Vector2(0, pos);
            float targetPosOffset = rotaryType == RotaryType.Horizontal
                ? data.go.transform.localPosition.x - targetPos.x
                : data.go.transform.localPosition.y - targetPos.y;
            data.go.transform.localPosition = Vector2.Lerp(data.go.transform.localPosition, targetPos, isForce ? 1 : t);
            if (Mathf.Abs(targetPosOffset) <= 0.01f)
            {
                data.go.transform.localPosition = targetPos;
            }

            //更新缩放值
            float scale = GetScale(ratio);
            Vector3 targetScale = Vector3.one * scale;
            float targetScaleOffset = data.go.transform.localScale.x - targetScale.x;
            data.go.transform.localScale = Vector3.Lerp(data.go.transform.localScale, targetScale, isForce ? 1 : t);
            if (Mathf.Abs(targetScaleOffset) <= 0.01f)
            {
                data.go.transform.localScale = targetScale;
            }
        }

        //更新层级
        var listItemDataList = m_ListItemDataList.OrderBy(data => GetScale(data.targetValue / m_TotalValue)).ToList();
        for (int i = 0; i < m_ItemCount; i++)
        {
            listItemDataList[i].go.transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 得到位置
    /// </summary>
    float GetPos(float ratio)
    {
        if (ratio < 0
            && ratio > 1)
        {
            Debug.LogError("比例值错误，比例值必须为[0-1]，ratio : " + ratio);
            return 0;
        }

        if (ratio >= 0 && ratio <= 0.25f)
        {
            return m_TotalValue * ratio;
        }
        else if (ratio > 0.25f && ratio <= 0.75f)
        {
            return m_TotalValue * (0.5f - ratio);
        }
        else
        {
            return m_TotalValue * (ratio - 1);
        }
    }

    /// <summary>
    /// 得到缩放值
    /// </summary>
    float GetScale(float ratio)
    {
        if (ratio < 0
               && ratio > 1)
        {
            Debug.LogError("比例值错误，比例值必须为[0-1]，ratio : " + ratio);
            return 0;
        }

        float v = (maxScale - minScale) * 2;
        if (ratio >= 0 && ratio <= 0.5f)
        {
            return maxScale - ratio * v;
        }
        else
        {
            return maxScale - (1 - ratio) * v;
        }
    }

    /// <summary>
    /// 得到距离最近的位置长度值
    /// </summary>
    float GetNearlyValue(float curValue)
    {
        float minDis = Mathf.Abs(curValue - m_InitValueList.First());
        float nearlyValue = m_InitValueList.First();
        foreach (var value in m_InitValueList)
        {
            float tempDis = Mathf.Abs(curValue - value);
            if (tempDis < minDis)
            {
                minDis = tempDis;
                nearlyValue = value == m_TotalValue
                    ? m_InitValueList.First()
                    : value;
            }
        }
        return nearlyValue;
    }

    /// <summary>
    /// 得到中心item的下标
    /// </summary>
    int GetCenterItemIndex()
    {
        int index = 0;
        float minDis = Mathf.Min(Mathf.Abs(m_ListItemDataList[0].targetValue - m_InitValueList.First()), Mathf.Abs(m_ListItemDataList[0].targetValue - m_InitValueList.Last()));
        foreach (var data in m_ListItemDataList)
        {
            float tempDis = Mathf.Min(Mathf.Abs(data.targetValue - m_InitValueList.First()), Mathf.Abs(data.targetValue - m_InitValueList.Last()));
            if (tempDis < minDis)
            {
                minDis = tempDis;
                index = data.index;
            }
        }
        return index;
    }

    /// <summary>
    /// 得到列表item数据
    /// </summary>
    public ListItemData GetListItemData(int index)
    {
        if (index < 0
           || index >= m_ItemCount)
        {
            Debug.LogError("下标超出范围,index : " + index);
            return null;
        }

        return m_ListItemDataList[index];
    }
}