using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// 配置系统的相关工具
/// </summary>
public class ConfigSystemTool
{
    public const string ConfigClassRootPath = "Assets/Scripts/ConfigClass";//生成的配置结构类根目录

    [MenuItem("工具/表格配置/生成配置结构类", priority = 1)]
    public static void Csv2ConfigClass()
    {
        bool genComplete = true;
        if (!Directory.Exists(ConfigUtils.CsvFileRootPath))
        {
            Debug.LogError($"CSV文件根目录不存在：{ConfigUtils.CsvFileRootPath}");
            return;
        }
        if (!Directory.Exists(ConfigClassRootPath))
        {
            Directory.CreateDirectory(ConfigClassRootPath);
        }
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(ConfigUtils.CsvFileRootPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles($"*{CsvUtils.Suffix}", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo fileInfo = fileInfos[i];
                string fileName = string.Format(ConfigUtils.ConfigClassNameFormat, Path.GetFileNameWithoutExtension(fileInfo.Name));
                if (EditorUtility.DisplayCancelableProgressBar("生成配置结构类", $"正在生成 {fileName}", i * 1f / fileInfos.Length))
                {
                    genComplete = false;
                    throw new Exception("用户取消");
                };
                if (!RegexUtils.IsValidName(fileName))
                {
                    Debug.LogError($"生成配置结构类失败，表格名称不符合C#命名规范，表格名称：{fileName}");
                    continue;
                }
                string filePath = Path.Combine(ConfigClassRootPath, fileName) + ".cs";
                List<string> fieldNameStrs = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldNameColIndex);//CSV文件的第1行是字段名
                List<string> fieldTypeStrs = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldTypeColIndex);//CSV文件的第2行是字段类型
                List<string> fieldCommentStrs = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldCommentColIndex);//CSV文件的第3行是字段注释
                if (fieldNameStrs == null || fieldTypeStrs == null || fieldCommentStrs == null)
                {
                    continue;
                }
                if (fieldNameStrs.Count != fieldTypeStrs.Count || fieldNameStrs.Count != fieldCommentStrs.Count || fieldTypeStrs.Count != fieldCommentStrs.Count)
                {
                    Debug.LogError($"表格中字段名列数、字段类型列数、字段注释列数应该相同，表格名称：{fileName}");
                    continue;
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 工具自动生成，禁止手动修改");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("using System.Collections.Generic;");
                        sb.AppendLine("using System;");
                        sb.AppendLine();
                        sb.AppendLine($"public class {fileName} : {typeof(ConfigClassBase).Name}");
                        sb.AppendLine("{");
                        for (int j = 0; j < fieldTypeStrs.Count; j++)
                        {
                            string typeStr = fieldTypeStrs[j];
                            if (string.IsNullOrEmpty(typeStr))//类型不填视为此列为注释
                            {
                                continue;
                            }
                            if (typeStr.Contains("[]"))//如果是数组则转换为List<T>形式
                            {
                                typeStr = typeStr.Replace("[]", ">");
                                typeStr = "List<" + typeStr;
                            }
                            sb.AppendLine("\t///<summary>");
                            sb.AppendLine($"\t///{fieldCommentStrs[j]}");
                            sb.AppendLine("\t///<summary>");
                            sb.AppendLine($"\tpublic {typeStr} {fieldNameStrs[j]};");
                            sb.AppendLine();
                        }
                        sb.AppendLine("}");

                        sw.Write(sb);
                        sw.Dispose();
                    }
                    fs.Dispose();
                }
            }
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (genComplete)
            {
                EditorUtility.DisplayDialog("生成配置结构类完成", $"生成配置结构类完成\n生成目录：{ConfigClassRootPath}", "确定");
            }
        }
    }

    [MenuItem("工具/表格配置/检查（检查无误后再生成配置结构类）", priority = 100)]
    public static void Check()
    {
        bool noError = true;
        bool checkComplete = true;
        if (!Directory.Exists(ConfigUtils.CsvFileRootPath))
        {
            Debug.LogError($"CSV文件根目录不存在：{ConfigUtils.CsvFileRootPath}");
            noError = false;
            return;
        }
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(ConfigUtils.CsvFileRootPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles($"*{CsvUtils.Suffix}", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo fileInfo = fileInfos[i];
                string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                if (EditorUtility.DisplayCancelableProgressBar("检查表格填写是否正确", $"正在检查 {fileName}", i * 1f / fileInfos.Length))
                {
                    checkComplete = false;
                    throw new Exception("用户取消");
                };
                //获取注释的列
                List<int> ignoreColIndexList = new List<int>();
                List<string> fieldTypeStrList = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldTypeColIndex);
                for (int col = 0; col < fieldTypeStrList.Count; col++)
                {
                    if (string.IsNullOrEmpty(fieldTypeStrList[col]))
                    {
                        ignoreColIndexList.Add(col);
                    }
                }
                //获取前三行和id为空的行
                List<int> ignoreRowIndexList = new List<int>() { ConfigUtils.FieldNameColIndex, ConfigUtils.FieldTypeColIndex, ConfigUtils.FieldCommentColIndex };
                List<string> col1StrList = CsvUtils.ParseCol(fileInfo.FullName, 0);
                List<string> col1FieldValueStrList = col1StrList.GetRange(ConfigUtils.IgnoreRowCount, col1StrList.Count - ConfigUtils.IgnoreRowCount);
                for (int row = 0; row < col1FieldValueStrList.Count; row++)
                {
                    if (string.IsNullOrEmpty(col1FieldValueStrList[row]))
                    {
                        ignoreRowIndexList.Add(ConfigUtils.IgnoreRowCount + row);
                    }
                }
                //检查表格名称是否合法
                if (!RegexUtils.IsValidName(fileName))
                {
                    Debug.LogError($"{fileName}表格有误，表格名称不符合C#命名规范");
                    noError = false;
                }
                //检查表格第一列定义的字段类型是否为int类型
                if (fieldTypeStrList[0] != "int")
                {
                    Debug.LogError($"{fileName}表格有误，表格第一列必须为int类型，当前类型：{fieldTypeStrList[0]}");
                    noError = false;
                }
                //检查表格中第一列id是否唯一
                List<string> col1FieldValueStrGroupList = col1FieldValueStrList.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
                foreach (var repeatFieldValueStr in col1FieldValueStrGroupList)
                {
                    if (string.IsNullOrEmpty(repeatFieldValueStr))
                    {
                        continue;
                    }
                    Debug.LogError($"{fileName}表格有误，表格中第一列id有重复，id：{repeatFieldValueStr}");
                    noError = false;
                }
                //检查表格中字段名是否合法
                List<string> fieldNameStrList = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldNameColIndex, ignoreColIndexList);
                foreach (var fieldNameStr in fieldNameStrList)
                {
                    if (!RegexUtils.IsValidName(fieldNameStr))
                    {
                        Debug.LogError($"{fileName}表格有误，字段名不符合C#命名规范，字段名：{fieldNameStr}");
                        noError = false;
                    }
                }
                //检查表格中字段名是否唯一
                fieldNameStrList = fieldNameStrList.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
                foreach (var repeatFieldNameStr in fieldNameStrList)
                {
                    if (string.IsNullOrEmpty(repeatFieldNameStr))
                    {
                        continue;
                    }
                    Debug.LogError($"{fileName}表格有误，表格中字段名有重复，字段名：{repeatFieldNameStr}");
                    noError = false;
                }
                //检查表格中每列字段的类型是否与定义的字段类型相同
                fieldTypeStrList = CsvUtils.ParseRow(fileInfo.FullName, ConfigUtils.FieldTypeColIndex, ignoreColIndexList);
                List<List<string>> cellStrList = CsvUtils.ParseRowAll(fileInfo.FullName, ignoreRowIndexList, ignoreColIndexList);
                for (int row = 0; row < cellStrList.Count; row++)
                {
                    for (int col = 0; col < cellStrList[row].Count; col++)
                    {
                        string fieldTypeStr = fieldTypeStrList[col];
                        string fieldValueStr = cellStrList[row][col];
                        if (!ReadValue(fieldTypeStr, fieldValueStr, fileInfo.Name))
                        {
                            noError = false;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"检查中断，{e}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (checkComplete)
            {
                EditorUtility.DisplayDialog("检查完成", noError ? "检查完成，无错误" : "检查完成，有错误", "确定");
            }
        }
    }

    /// <summary>
    /// 读取表格中的值
    /// </summary>
    private static bool ReadValue(string fieldTypeStr, string fieldValueStr, string fileName)
    {
        bool noError = true;
        if (fieldTypeStr == "string")
        {

        }
        else if (fieldTypeStr == "int")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!int.TryParse(fieldValueStr, out int intValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "bool")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!bool.TryParse(fieldValueStr, out bool boolValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "double")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!double.TryParse(fieldValueStr, out double doubleValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "float")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!float.TryParse(fieldValueStr, out float floatValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "char")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!char.TryParse(fieldValueStr, out char charValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "TimeSpan")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!TimeSpan.TryParse(fieldValueStr, out TimeSpan timeSpanValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr == "DateTime")
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                if (!DateTime.TryParse(fieldValueStr, out DateTime dateTimeValue))
                {
                    Debug.LogError($"{fileName}表格中的{fieldValueStr}不是{fieldTypeStr}类型字段");
                    noError = false;
                }
            }
        }
        else if (fieldTypeStr.Contains("[]"))
        {
            if (!string.IsNullOrEmpty(fieldValueStr))
            {
                string elementTypeStr = fieldTypeStr.Split('[')[0];
                string[] cellStrArray = fieldValueStr.Split('|');
                for (int i = 0; i < cellStrArray.Length; i++)
                {
                    noError = ReadValue(elementTypeStr, cellStrArray[i], fileName);
                }
            }
        }
        else
        {
            Debug.LogError($"无法转换{fieldTypeStr}类型的字段，表格：{fileName}，值：{fieldValueStr}");
            noError = false;
        }
        return noError;
    }
}
