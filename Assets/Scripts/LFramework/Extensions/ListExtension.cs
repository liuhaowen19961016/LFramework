using System.Collections.Generic;

/// <summary>
/// List扩展
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// 向列表中添加唯一的元素
    /// </summary>
    public static void AddUnique<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }

    /// <summary>
    /// 向列表中添加唯一的元素
    /// </summary>
    public static void AddRangeUnique<T>(this List<T> list, List<T> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            list.AddUnique(itemList[i]);
        }
    }
}
