using UnityEngine;

/// <summary>
/// 颜色工具类
/// </summary>
public static class ColorUtils
{
    /// <summary>
    /// Color转Hex
    /// </summary>
    /// alpha:是否有透明度
    public static string Color2Hex(Color color, bool alpha = true)
    {
        string hex;
        if (alpha)
        {
            hex = ColorUtility.ToHtmlStringRGBA(color);
        }
        else
        {
            hex = ColorUtility.ToHtmlStringRGB(color);
        }
        return hex;
    }

    /// <summary>
    /// Hex转Color
    /// </summary>
    /// Hex：#000000
    public static Color HexRGB2Color(string hexRGB)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexRGB, out color);
        return color;
    }

    /// <summary>
    /// Color转HSV
    /// </summary>
    public static void Color2HSV(Color color, out float h, out float s, out float v)
    {
        Color.RGBToHSV(color, out h, out s, out v);
    }

    /// <summary>
    /// HSV转Color
    /// </summary>
    public static Color HSV2Color(float h, float s, float v)
    {
        Color color = Color.HSVToRGB(h, s, v);
        return color;
    }
}