using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 日志管理
/// </summary>
public class LogService : Singleton<LogService>
{
    public static ELogLevel CurLogLevel = ELogLevel.None;//当前日志等级

    private LogService()
    {

    }

    #region Log

    [Conditional("UNITY_EDITOR")]
    public void Log(object message)
    {
        if (CurLogLevel < ELogLevel.Log)
        {
            return;
        }
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public void LogFormat(string format, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Log)
        {
            return;
        }
        if (args.Length == 0)
        {
            UnityEngine.Debug.Log(format);
        }
        else
        {
            UnityEngine.Debug.LogFormat(format, args);
        }
    }

    [Conditional("UNITY_EDITOR")]
    public void Log(object message, Color color, bool highlighting = true)
    {
        if (CurLogLevel < ELogLevel.Log)
        {
            return;
        }
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public void LogFormat(string format, Color color, bool highlighting = true, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Log)
        {
            return;
        }
        string message = string.Format(format, args);
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.Log(message);
    }

    #endregion Log

    #region Warning

    public void LogWarning(object message)
    {
        if (CurLogLevel < ELogLevel.Warning)
        {
            return;
        }
        UnityEngine.Debug.LogWarning(message);
    }

    public void LogWarningFormat(string format, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Warning)
        {
            return;
        }
        if (args.Length == 0)
        {
            UnityEngine.Debug.LogWarning(format);
        }
        else
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }
    }

    public void LogWarning(object message, Color color, bool highlighting = true)
    {
        if (CurLogLevel < ELogLevel.Warning)
        {
            return;
        }
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.LogWarning(message);
    }

    public void LogWarningFormat(string format, Color color, bool highlighting = true, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Warning)
        {
            return;
        }
        string message = string.Format(format, args);
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.LogWarning(message);
    }

    #endregion Warning

    #region Error

    public void LogError(object message)
    {
        if (CurLogLevel < ELogLevel.Error)
        {
            return;
        }
        UnityEngine.Debug.LogError(message);
    }

    public void LogErrorFormat(string format, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Error)
        {
            return;
        }
        if (args.Length == 0)
        {
            UnityEngine.Debug.LogError(format);
        }
        else
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }
    }

    public void LogError(object message, Color color, bool highlighting = true)
    {
        if (CurLogLevel < ELogLevel.Error)
        {
            return;
        }
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.LogError(message);
    }

    public void LogErrorFormat(string format, Color color, bool highlighting = true, params object[] args)
    {
        if (CurLogLevel < ELogLevel.Error)
        {
            return;
        }
        string message = string.Format(format, args);
        if (highlighting)
        {
            message = string.Format("<b><color=#{0:X2}{1:X2}{2:X2}>{3}</color></b>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        else
        {
            message = string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), message);
        }
        UnityEngine.Debug.LogError(message);
    }

    #endregion Error

    #region Exception

    public void LogException(Exception exception)
    {
        if (CurLogLevel < ELogLevel.Exception)
        {
            return;
        }
        UnityEngine.Debug.LogException(exception);
    }

    #endregion Exception
}

/// <summary>
/// 日志等级（值越大，日志输出越详细）
/// </summary>
public enum ELogLevel
{
    None = 0,
    Exception,
    Error,
    Warning,
    Log,
    All,
}
