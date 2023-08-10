using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 配置管理
/// </summary>
public class ConfigService : Singleton<ConfigService>
{
    public static Dictionary<string, object> m_ConfigDict = new Dictionary<string, object>();//配置字典

    /// <summary>
    /// 创建配置类
    /// </summary>
    private IConfig<T> CreateConfig<T>()
        where T : ConfigClassBase
    {
        IConfig<T> config = new CsvConfig<T>();
        return config;
    }

    /// <summary>
    /// 获取表格中某个id的配置数据
    /// </summary>
    public T GetConfig<T>(int id)
        where T : ConfigClassBase
    {
        return GetConfig<T>().GetConfig(id);
    }

    /// <summary>
    /// 获取配置列表（每行的配置数据）
    /// </summary>
    public List<T> GetConfigs<T>()
        where T : ConfigClassBase
    {
        return GetConfig<T>().GetConfigs();
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        m_ConfigDict.Clear();
    }

    /// <summary>
    /// 获取配置类
    /// </summary>
    private IConfig<T> GetConfig<T>()
        where T : ConfigClassBase
    {
        string configName = typeof(T).Name;
        if (m_ConfigDict.TryGetValue(configName, out object config))
        {
            return config as IConfig<T>;
        }
        else
        {
            IConfig<T> newConfig = CreateConfig<T>();
            m_ConfigDict.Add(configName, newConfig);
            return newConfig;
        }
    }
}
