using UnityEngine;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 文本记录器（记录log信息并保存成外部文件）
/// </summary>
public class TextLogger
{
    private string m_OutputPath;//文件输出的路径
    private StringBuilder m_StringBuilder;//字符串

    /// <summary>
    /// 初始化
    /// </summary>
    public TextLogger(string outputPath)
    {
        m_StringBuilder = new StringBuilder();
        m_OutputPath = outputPath;
    }

    /// <summary>
    /// 写入一行文本
    /// </summary>
    public void WriteLine(string foramt, params object[] param)
    {
        StreamWriter sw = null;
        try
        {
            if (string.IsNullOrEmpty(m_OutputPath))
            {
                Debug.LogError("输出路径不能为空");
                return;
            }
            if (!IOUtils.IsFile(m_OutputPath))
            {
                Debug.LogError("输出路径不是文件路径，必须有后缀");
                return;
            }
            if (!File.Exists(m_OutputPath))
            {
                sw = File.CreateText(m_OutputPath);
            }
            else
            {
                sw = File.AppendText(m_OutputPath);
            }
            if (sw != null)
            {
                m_StringBuilder.Clear();
                if (param.Length == 0)
                {
                    m_StringBuilder.Append(foramt);
                }
                else
                {
                    m_StringBuilder.AppendFormat(foramt, param);
                }
                sw.WriteLine(m_StringBuilder);
            }
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.WriteLine(e);
            }
            Debug.LogError($"写入中断，{e}");
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
                sw.Dispose();
            }
        }
    }

    /// <summary>
    /// 写入文本
    /// </summary>
    public void Write(string foramt, params object[] param)
    {
        StreamWriter sw = null;
        try
        {
            if (string.IsNullOrEmpty(m_OutputPath))
            {
                Debug.LogError("输出路径不能为空");
                return;
            }
            if (!IOUtils.IsFile(m_OutputPath))
            {
                Debug.LogError("输出路径不是文件路径，必须有后缀");
                return;
            }
            if (!File.Exists(m_OutputPath))
            {
                sw = File.CreateText(m_OutputPath);
            }
            else
            {
                sw = File.AppendText(m_OutputPath);
            }
            if (sw != null)
            {
                m_StringBuilder.Clear();
                if (param.Length == 0)
                {
                    m_StringBuilder.Append(foramt);
                }
                else
                {
                    m_StringBuilder.AppendFormat(foramt, param);
                }
                sw.Write(m_StringBuilder);
            }
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.Write(e);
            }
            Debug.LogError($"写入中断，{e}");
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
                sw.Dispose();
            }
        }
    }
}