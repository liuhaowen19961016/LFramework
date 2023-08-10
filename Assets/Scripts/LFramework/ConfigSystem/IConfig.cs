using System.Collections.Generic;

/// <summary>
/// 配置接口
/// </summary>
public interface IConfig<T>
{
    /// <summary>
    /// 获取表格中某个id的配置数据
    /// </summary>
    T GetConfig(int id);

    /// <summary>
    /// 获取配置列表（每行的配置数据）
    /// </summary>
    List<T> GetConfigs();
}
