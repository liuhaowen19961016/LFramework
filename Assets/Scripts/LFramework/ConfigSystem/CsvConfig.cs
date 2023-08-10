using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// CSV配置
/// </summary>
public class CsvConfig<T> : ConfigBase<T>
{
    public override void InitConfig()
    {
        string csvPath = Path.Combine(ConfigUtils.CsvFileRootPath, $"{m_ConfigType.Name.Substring(0, m_ConfigType.Name.Length - 6)}{CsvUtils.Suffix}");

        //获取注释的列
        List<int> ignoreColIndexList = new List<int>();
        List<string> fieldTypeStrList = CsvUtils.ParseRow(csvPath, ConfigUtils.FieldTypeColIndex);
        for (int col = 0; col < fieldTypeStrList.Count; col++)
        {
            if (string.IsNullOrEmpty(fieldTypeStrList[col]))
            {
                ignoreColIndexList.Add(col);
            }
        }
        //获取前三行和id为空的行
        List<int> ignoreRowIndexList = new List<int>() { ConfigUtils.FieldNameColIndex, ConfigUtils.FieldTypeColIndex, ConfigUtils.FieldCommentColIndex };
        List<string> col1StrList = CsvUtils.ParseCol(csvPath, 0);
        List<string> col1FieldValueStrList = col1StrList.GetRange(ConfigUtils.IgnoreRowCount, col1StrList.Count - ConfigUtils.IgnoreRowCount);
        for (int row = 0; row < col1FieldValueStrList.Count; row++)
        {
            if (string.IsNullOrEmpty(col1FieldValueStrList[row]))
            {
                ignoreRowIndexList.Add(ConfigUtils.IgnoreRowCount + row);
            }
        }

        List<List<string>> cellStrList = CsvUtils.ParseRowAll(csvPath, ignoreRowIndexList, ignoreColIndexList);

        //for (int i = 0; i < cellStrList.Count; i++)
        //{
        //    for (int j = 0; j < cellStrList[i].Count; j++)
        //    {
        //        Debug.Log(cellStrList[i][j]);
        //    }
        //    Debug.Log("----------");
        //}

        //初始化数据
        m_CellStrArray = GameUtils.List2Array(cellStrList);
        m_ConfigArray = new T[cellStrList.Count];
        for (int row = 0; row < cellStrList.Count; row++)
        {
            m_Id2RowIndex.Add(Convert.ToInt32(cellStrList[row][0]), row);
        }
    }
}
