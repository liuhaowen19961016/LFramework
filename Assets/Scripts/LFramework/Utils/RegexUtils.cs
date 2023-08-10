using System.Text.RegularExpressions;

/// <summary>
/// 正则工具类
/// </summary>
public static class RegexUtils
{
    /// <summary>
    /// 是否为合法的C#命名
    /// </summary>
    public static bool IsValidName(string str)
    {
        Regex regex = new Regex(@"^[a-zA-Z_][A-Za-z0-9_]*$");
        return regex.IsMatch(str);
    }
}
