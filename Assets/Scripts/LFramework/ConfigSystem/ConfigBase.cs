using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

/// <summary>
/// 配置基类
/// </summary>
public abstract class ConfigBase<T> : IConfig<T>
{
    protected Type m_ConfigType;//配置类的类型
    private FieldInfo[] m_FieldInfos;//所有字段数据

    protected Dictionary<int, int> m_Id2RowIndex = new Dictionary<int, int>();//id对应的行下标
    protected string[,] m_CellStrArray;//表格中每一格的数据字符串（只存数据）

    protected T[] m_ConfigArray;//存储每行的配置数据
    private bool m_LoadAllRow;//是否加载了所有行的数据

    public ConfigBase()
    {
        m_ConfigType = typeof(T);
        m_FieldInfos = m_ConfigType.GetFields(BindingFlags.Instance | BindingFlags.Public);

        InitConfig();
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    public abstract void InitConfig();

    public T GetConfig(int id)
    {
        T config;
        if (m_Id2RowIndex.TryGetValue(id, out int row))
        {
            config = m_ConfigArray[row];
            if (config == null)
            {
                config = m_ConfigArray[row] = ConvertToRowInstance(row);
            }
        }
        else
        {
            config = default(T);
        }
        return config;
    }

    public List<T> GetConfigs()
    {
        List<T> configs = new List<T>();
        if (!m_LoadAllRow)
        {
            for (int row = 0; row < m_CellStrArray.GetLength(0); row++)
            {
                if (m_ConfigArray[row] != null)
                {
                    continue;
                }
                m_ConfigArray[row] = ConvertToRowInstance(row);
            }
            m_LoadAllRow = true;
        }
        configs = new List<T>(m_ConfigArray);
        return configs;
    }

    /// <summary>
    /// 转换为行实例（赋值）
    /// </summary>
    private T ConvertToRowInstance(int row)
    {
        T instance = (T)Activator.CreateInstance(m_ConfigType);
        for (int j = 0; j < m_CellStrArray.GetLength(1); j++)
        {
            Type fieldType = m_FieldInfos[j].FieldType;
            m_FieldInfos[j].SetValue(instance, ReadValue(fieldType, m_CellStrArray[row, j]));
        }
        return instance;
    }

    /// <summary>
    /// 读取表格中的值
    /// </summary>
    private object ReadValue(Type type, string cellStr)
    {
        object value = null;
        if (typeof(string) == type)
        {
            value = "";
            if (!string.IsNullOrEmpty(cellStr))
            {
                value = cellStr;
            }
        }
        else if (typeof(int) == type)
        {
            value = 0;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (int.TryParse(cellStr, out int intValue))
                {
                    value = intValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(bool) == type)
        {
            value = false;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (bool.TryParse(cellStr, out bool boolValue))
                {
                    value = boolValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(double) == type)
        {
            value = 0D;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (double.TryParse(cellStr, out double doubleValue))
                {
                    value = doubleValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(float) == type)
        {
            value = 0F;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (float.TryParse(cellStr, out float floatValue))
                {
                    value = floatValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(char) == type)
        {
            value = (char)0;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (char.TryParse(cellStr, out char charValue))
                {
                    value = charValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(TimeSpan) == type)
        {
            value = TimeSpan.Zero;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (TimeSpan.TryParse(cellStr, out TimeSpan timeSpanValue))
                {
                    value = timeSpanValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(DateTime) == type)
        {
            value = DateTime.MinValue;
            if (!string.IsNullOrEmpty(cellStr))
            {
                if (DateTime.TryParse(cellStr, out DateTime dateTimeValue))
                {
                    value = dateTimeValue;
                }
                else
                {
                    Debug.LogError($"{m_ConfigType.Name}表格中的{cellStr}不是{type}类型字段");
                }
            }
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            if (!string.IsNullOrEmpty(cellStr))
            {
                IList list = (IList)Activator.CreateInstance(type);
                string[] cellStrArray = cellStr.Split('|');
                Type elementType = type.GetGenericArguments()[0];
                for (int i = 0; i < cellStrArray.Length; i++)
                {
                    list.Add(ReadValue(elementType, cellStrArray[i]));
                }
                value = list;
            }
        }
        else
        {
            Debug.LogError($"无法转换{type}类型的字段，表格：{m_ConfigType.Name}，值：{cellStr}");
        }
        return value;
    }
}
