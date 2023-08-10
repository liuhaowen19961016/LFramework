using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 配置工具类
/// </summary>
public static class ConfigUtils
{
    public const int FieldNameColIndex = 0;//字段名的行下标
    public const int FieldTypeColIndex = 1;//字段类型的行下标
    public const int FieldCommentColIndex = 2;//字段注释的行下标
    public const int IgnoreRowCount = 3;//忽略的行数量（前三行分别为字段名、字段类型、字段注释）

    public const string CsvFileRootPath = "Assets/BundleAssets/CSV";//CSV文件根目录
    public const string ConfigClassNameFormat = "{0}Config";//配置类名字的格式化字符串
}
