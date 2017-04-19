/// <summary>
/// 字符串扩展
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;
public static class StringExpand
{
    /// <summary>
    /// 获取字节数扩展方法
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding">编码 默认utf-8</param>
    /// <returns></returns>
    public static int GetByteLen(this string str, System.Text.Encoding encoding = null)
    {
        if (encoding == null)
            encoding = System.Text.Encoding.UTF8;
        return encoding.GetBytes(str).Length;
    }

    public static long ToInt64(this string str)
    {
        long result = 0;
        try
        {
            long.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }
    public static uint ToUInt64(this string str)
    {
        uint result = 0;
        try
        {
            uint.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static int ToInt32(this string str)
    {
        int result = 0;
        try
        {
            int.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static uint ToUInt32(this string str)
    {
        uint result = 0;
        try
        {
            uint.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static short ToInt16(this string str)
    {
        short result = 0;
        try
        {
            short.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }
    public static ushort ToUInt16(this string str)
    {
        ushort result = 0;
        try
        {
            ushort.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static byte ToByte(this string str)
    {
        byte result = 0;
        try
        {
            byte.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static byte[] ToBytes(this string str, int len = 0, Encoding encoding = null)
    {
        byte[] buf = Encoding.UTF8.GetBytes(str);
        if (len == 0)
            len = buf.Length;
        if (buf.Length != len)
        {
            byte[] nbuf = new byte[len];
            for (int i = 0; i < nbuf.Length && i < buf.Length; i++)
            {
                nbuf[i] = buf[i];
            }
        }
        return buf;
    }

    public static float ToFloat(this string str)
    {
        float result = 0f;
        try
        {
            float.TryParse(str, out result);
        }
        catch (Exception ex)
        {
            Debugger.LogError(ex.StackTrace);
        }
        return result;
    }

    public static T[] ToArray<T>(this string str, char split = ',') where T : struct
    {
        if (string.IsNullOrEmpty(str))
            return new T[] { };
        string[] temp = str.Split(split);
        T[] t = new T[temp.Length];
        for (int i = 0, count = t.Length; i < count; i++)
        {
            try
            {
                t[i] = (T)Convert.ChangeType(temp[i], typeof(T));
            }
            catch (Exception e)
            {
                Debugger.Log("try to parse " + temp[i] + " to " + typeof(T).Name + " fail." + e.Message);
            }
        }
        return t;
    }

    public static T[] ToArray<T>(this string str, char[] splits) where T : struct
    {
        if (string.IsNullOrEmpty(str))
            return new T[] { };
        string[] temp = str.Split(splits);
        T[] t = new T[temp.Length];
        for (int i = 0, count = t.Length; i < count; i++)
        {
            try
            {
                t[i] = (T)Convert.ChangeType(temp[i], typeof(T));
            }
            catch (Exception e)
            {
                Debugger.Log("try to parse " + temp[i] + " to " + typeof(T).Name + " fail." + e.Message);
            }
        }
        return t;
    }

    public static string[] ToArray(this string str, char split = ',')
    {
        if (string.IsNullOrEmpty(str))
            return new string[] { };
        string[] result = str.Split(split);
        return result;
    }

    public static string[] ToArray(this string str, char[] splits)
    {
        if (string.IsNullOrEmpty(str))
            return new string[] { };
        string[] result = str.Split(splits);
        return result;
    }

    public static List<T> ToList<T>(this string str, char split = ',') where T : struct
    {
        if (string.IsNullOrEmpty(str))
            return new List<T>();
        string[] temp = str.Split(split);
        List<T> t = new List<T>();
        for (int i = 0, count = temp.Length; i < count; i++)
        {
            try
            {
                t.Add((T)Convert.ChangeType(temp[i], typeof(T)));
            }
            catch (Exception e)
            {
                Debugger.Log("try to parse " + temp[i] + " to " + typeof(T).Name + " fail." + e.Message);
            }

        }
        return t;
    }

    public static List<T> ToList<T>(this string str, char[] splits) where T : struct
    {
        if (string.IsNullOrEmpty(str))
            return new List<T>();
        string[] temp = str.Split(splits);
        List<T> t = new List<T>();
        for (int i = 0, count = temp.Length; i < count; i++)
        {
            try
            {
                t.Add((T)Convert.ChangeType(temp[i], typeof(T)));
            }
            catch (Exception e)
            {
                Debugger.Log("try to parse " + temp[i] + " to " + typeof(T).Name + " fail." + e.Message);
            }
        }
        return t;
    }

    public static List<string> ToList(this string str, char split = ',')
    {
        if (string.IsNullOrEmpty(str))
            return new List<string>();
        string[] result = str.Split(split);
        return result.ToList();
    }

    public static List<string> ToList(this string str, char[] splits)
    {
        if (string.IsNullOrEmpty(str))
            return new List<string>();
        string[] result = str.Split(splits);
        return result.ToList();
    }

    public static bool ToBoolean(this string str)
    {
        bool result = false;
        bool.TryParse(str, out result);
        return result;
    }

    public static UnityEngine.Vector2 ToVector2(this string str, char splitSymbol = ',')
    {
        UnityEngine.Vector2 vector2 = UnityEngine.Vector2.zero;
        string[] splitedStrings = str.Split(splitSymbol);
        if (splitedStrings.Length == 2)
        {
            float.TryParse(splitedStrings[0], out vector2.x);
            float.TryParse(splitedStrings[1], out vector2.y);
        }
        return vector2;
    }

    public static UnityEngine.Vector3 ToVector3(this string str, char split = ',')
    {
        UnityEngine.Vector3 vect = UnityEngine.Vector3.zero;
        string[] temp = str.Split(split);
        if (temp.Length == 3)
        {
            float.TryParse(temp[0], out  vect.x);
            float.TryParse(temp[1], out vect.y);
            float.TryParse(temp[2], out vect.z);
        }
        return vect;
    }

    public static UnityEngine.Vector3 ToVector3(this string str, char[] splits)
    {
        UnityEngine.Vector3 vect = UnityEngine.Vector3.zero;
        string[] temp = str.Split(splits);
        if (temp.Length == 3)
        {
            float.TryParse(temp[0], out vect.x);
            float.TryParse(temp[1], out vect.y);
            float.TryParse(temp[2], out vect.z);
        }
        return vect;
    }

    public static UnityEngine.Color ToColor(this string str, char split = ',')
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        string[] temp = str.Split(split);
        if (temp.Length == 3)
        {
            color.r = float.Parse(temp[0]) / 255;
            color.g = float.Parse(temp[1]) / 255;
            color.b = float.Parse(temp[2]) / 255;
            color.a = 1;
        }
        else if (temp.Length == 4)
        {
            color.r = float.Parse(temp[0]) / 255;
            color.g = float.Parse(temp[1]) / 255;
            color.b = float.Parse(temp[2]) / 255;
            color.a = float.Parse(temp[3]) / 255;
        }
        return color;
    }

    public static UnityEngine.Color ToColor(this string str, char[] splits)
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        string[] temp = str.Split(splits);
        if (temp.Length == 3)
        {
            color.r = float.Parse(temp[0]) / 255;
            color.g = float.Parse(temp[1]) / 255;
            color.b = float.Parse(temp[2]) / 255;
            color.a = 1;
        }
        else if (temp.Length == 4)
        {
            color.r = float.Parse(temp[0]) / 255;
            color.g = float.Parse(temp[1]) / 255;
            color.b = float.Parse(temp[2]) / 255;
            color.a = float.Parse(temp[3]) / 255;
        }
        return color;
    }

    public static KeyValuePair<T1, T2> SplitToKeyValuePair<T1, T2>(this string value, string splitStr = "_")
        where T1 : struct
        where T2 : struct
    {
        string str1 = string.Empty, str2 = string.Empty;
        string[] strs = value.Split(splitStr.ToCharArray());
        if (strs.Length != 2)
        {
            throw new System.FormatException(string.Format("字符串{0}不能被{1}分割成两个字符串", value, splitStr));
        }
        str1 = strs[0];
        str2 = strs[1];
        T1 t1 = (T1)System.Convert.ChangeType(str1, typeof(T1));
        T2 t2 = (T2)System.Convert.ChangeType(str2, typeof(T2));
        KeyValuePair<T1, T2> result = new KeyValuePair<T1, T2>(t1, t2);
        return result;
    }

    public static KeyValuePair<string, string> SplitToKeyValuePair(this string value, char[] splits)
    {
        string str1 = string.Empty, str2 = string.Empty;
        string[] strs = value.Split(splits);
        if (strs.Length != 2)
        {
            throw new System.FormatException(string.Format("字符串{0}不能被{1}分割成两个字符串", value, splits));
        }
        str1 = strs[0];
        str2 = strs[1];
        KeyValuePair<string, string> result = new KeyValuePair<string, string>(str1, str2);
        return result;
    }
}

