
using System;
public static class EnumExpand
{

    #region Enum Tools
    /// <summary>
    /// 字符串转换枚举类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="defaultValue">转换失败的默认值</param>
    /// <param name="ignoreCase">区分大小写(true不区分)</param>
    /// <returns></returns>
    public static T Parse<T>(string name, int defaultValue = -1, bool ignoreCase = true) //where T : System.Enum
    {
        Type tp = typeof(T);
        object ret = defaultValue;
        try
        {
            ret = Enum.Parse(tp, name, ignoreCase);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return (T)ret;
    }


    #endregion
}

