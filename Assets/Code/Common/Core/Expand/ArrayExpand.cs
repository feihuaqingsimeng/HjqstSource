using System;
using System.Collections.Generic;
using System.Text;
public static class ArrayExpand
{
    /// <summary>
    /// 获取数组中指定元素的索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tArray"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] tArray, T item) where T : class
    {
        if (item == null)
            throw new NullReferenceException();
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            if (item.Equals(tArray[i])) return i;
        }
        return -1;
    }

    public static int IndexOf(this byte[] tArray, byte item)
    {
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            if (item == tArray[i]) return i;
        }
        return -1;
    }

    public static int IndexOf(this int[] tArray, int item)
    {
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            if (item == tArray[i]) return i;
        }
        return -1;
    }

    public static int IndexOf(this uint[] tArray, uint item)
    {
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            if (item == tArray[i]) return i;
        }
        return -1;
    }

    public static int IndexOf(this float[] tArray, float item)
    {
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            if (item == tArray[i]) return i;
        }
        return -1;
    }

    public static List<T> ToList<T>(this T[] tArray, int offset = 0)
    {
        List<T> list = new List<T>();
        for (int i = offset, count = tArray.Length; i < count; i++)
        {
            list.Add(tArray[i]);
        }
        return list;
    }

    public static string ToCustomString<T>(this T[] tArray, char split = ',')
    {
        if (tArray == null)
            return string.Empty;
        StringBuilder result = new StringBuilder();
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            result.Append(tArray[i].ToString());
            if (i != count - 1)
                result.Append(split);
        }
        return result.ToString();
    }

    public static string ToCustomString<T>(this T[] tArray, char[] splits)
    {
        if (tArray == null)
            return string.Empty;
        StringBuilder result = new StringBuilder();
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            result.Append(tArray[i].ToString());
            if (i != count - 1)
                result.Append(new string(splits));
        }
        return result.ToString();
    }

    //public static string ToCustomString(this string[] tArray, char split = ',')
    //{
    //    StringBuilder result = new StringBuilder();
    //    for (int i = 0, count = tArray.Length; i < count; i++)
    //    {
    //        result.Append(tArray[i].ToString());
    //        result.Append(split);
    //    }
    //    return result.ToString();
    //}

    #region sum
    public static int Sum(this int[] tArray)
    {
        int result = default(int);
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            result += tArray[i];
        }
        return result;
    }

    public static uint Sum(this uint[] tArray)
    {
        uint result = default(uint);
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            result += tArray[i];
        }
        return result;
    }

    public static float Sum(this float[] tArray)
    {
        float result = default(float);
        for (int i = 0, count = tArray.Length; i < count; i++)
        {
            result += tArray[i];
        }
        return result;
    }
    #endregion

    #region Max && Min
    public static byte MaxValue(this byte[] array)
    {
        if (array.Length == 0)
            return default(byte);
        byte temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp < array[i])
                temp = array[i];
        }
        return temp;
    }

    public static int MaxValue(this int[] array)
    {
        if (array.Length == 0)
            return default(int);
        int temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp < array[i])
                temp = array[i];
        }
        return temp;
    }

    public static uint MaxValue(this uint[] array)
    {
        if (array.Length == 0)
            return default(uint);
        uint temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp < array[i])
                temp = array[i];
        }
        return temp;
    }

    public static float MaxValue(this float[] array)
    {
        if (array.Length == 0)
            return default(float);
        float temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp < array[i])
                temp = array[i];
        }
        return temp;
    }

    public static byte MinValue(this byte[] array)
    {
        if (array.Length == 0)
            return default(byte);
        byte temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp > array[i])
                temp = array[i];
        }
        return temp;
    }

    public static int MinValue(this int[] array)
    {
        if (array.Length == 0)
            return default(int);
        int temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp > array[i])
                temp = array[i];
        }
        return temp;
    }

    public static uint MinValue(this uint[] array)
    {
        if (array.Length == 0)
            return default(uint);
        uint temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp > array[i])
                temp = array[i];
        }
        return temp;
    }

    public static float MinValue(this float[] array)
    {
        if (array.Length == 0)
            return default(float);
        float temp = array[0];
        for (int i = 1, count = array.Length; i < count; i++)
        {
            if (temp > array[i])
                temp = array[i];
        }
        return temp;
    }
    #endregion

    #region sort
    public static void Sort(this byte[] array, SortType st)
    {
        if (array.Length == 0 || st == SortType.None) return;
        byte temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = array.Length; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (array[j] > array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (array[j] < array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this int[] array, SortType st)
    {
        if (array.Length == 0 || st == SortType.None) return;
        int temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = array.Length; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (array[j] > array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (array[j] < array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this uint[] array, SortType st)
    {
        if (array.Length == 0 || st == SortType.None) return;
        uint temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = array.Length; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (array[j] > array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (array[j] < array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this float[] array, SortType st)
    {
        if (array.Length == 0 || st == SortType.None) return;
        float temp = 0;
        bool flag = true;
        for (int i = array.Length; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (array[j] > array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (array[j] < array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }
    #endregion
}

