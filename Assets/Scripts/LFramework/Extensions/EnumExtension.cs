using System;
using System.Reflection;
using System.ComponentModel;

/// <summary>
/// 枚举扩展类
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 转换为Description
    /// </summary>
    public static string ToDescription(this Enum e)
    {
        Type type = e.GetType();
        FieldInfo fi = type.GetField(e.ToString());
        DescriptionAttribute attribute = fi.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
        if (attribute == null)
        {
            return e.ToString();
        }
        return attribute.Description;
    }
}
