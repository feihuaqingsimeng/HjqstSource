using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
public static class ListExpand
{
    public static T First<T>(this List<T> list)
    {
        if (list.Count > 0)
            return list[0];
        return default(T);
    }

    public static T Last<T>(this List<T> list)
    {
        if (list.Count > 0)
            return list[list.Count - 1];
        return default(T);
    }

    public static string ToCustomString<T>(this List<T> list, char split = ',')
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0, count = list.Count; i < count; i++)
        {
            result.Append(list[i].ToString());
            if (i != count - 1)
                result.Append(split);
        }
        return result.ToString();
    }

    public static string ToCustomString<T>(this List<T> list, char[] splits)
    {
        if (list == null)
            return string.Empty;
        StringBuilder result = new StringBuilder();
        for (int i = 0, count = list.Count; i < count; i++)
        {
            result.Append(list[i].ToString());
            if (i != count - 1)
                result.Append(new string(splits));
        }
        return result.ToString();
    }
    //public static string ToCustomString(this List<string> list, char split = ',')
    //{
    //    StringBuilder result = new StringBuilder();
    //    for (int i = 0, count = list.Count; i < count; i++)
    //    {
    //        result.Append(list[i].ToString());
    //        result.Append(split);
    //    }
    //    return result.ToString();
    //}

    #region sum
    public static int Sum(this List<int> list)
    {
        int result = default(int);
        for (int i = 0, count = list.Count; i < count; i++)
        {
            result += list[i];
        }
        return result;
    }

    public static uint Sum(this List<uint> list)
    {
        uint result = default(uint);
        for (int i = 0, count = list.Count; i < count; i++)
        {
            result += list[i];
        }
        return result;
    }
    public static float Sum(this List<float> list)
    {
        float result = default(float);
        for (int i = 0, count = list.Count; i < count; i++)
        {
            result += list[i];
        }
        return result;
    }
    #endregion

    #region Max && Min
    public static byte MaxValue(this List<byte> list)
    {
        if (list.Count == 0)
            return default(byte);
        byte temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp < list[i])
                temp = list[i];
        }
        return temp;
    }

    public static int MaxValue(this List<int> list)
    {
        if (list.Count == 0)
            return default(int);
        int temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp < list[i])
                temp = list[i];
        }
        return temp;
    }

    public static uint MaxValue(this List<uint> list)
    {
        if (list.Count == 0)
            return default(uint);
        uint temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp < list[i])
                temp = list[i];
        }
        return temp;
    }

    public static float MaxValue(this List<float> list)
    {
        if (list.Count == 0)
            return default(float);
        float temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp < list[i])
                temp = list[i];
        }
        return temp;
    }

    public static byte MinValue(this List<byte> list)
    {
        if (list.Count == 0)
            return default(byte);
        byte temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp > list[i])
                temp = list[i];
        }
        return temp;
    }

    public static int MinValue(this List<int> list)
    {
        if (list.Count == 0)
            return default(int);
        int temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp > list[i])
                temp = list[i];
        }
        return temp;
    }

    public static uint MinValue(this List<uint> list)
    {
        if (list.Count == 0)
            return default(uint);
        uint temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp > list[i])
                temp = list[i];
        }
        return temp;
    }

    public static float MinValue(this List<float> list)
    {
        if (list.Count == 0)
            return default(float);
        float temp = list[0];
        for (int i = 0, count = list.Count; i < count; i++)
        {
            if (temp > list[i])
                temp = list[i];
        }
        return temp;
    }
    #endregion

    #region sort
    public static void Sort(this List<byte> list, SortType st)
    {
        if (list.Count == 0 || st == SortType.None) return;
        byte temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = list.Count; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (list[j] > list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (list[j] < list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this List<int> list, SortType st)
    {
        if (list.Count == 0 || st == SortType.None) return;
        int temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = list.Count; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (list[j] > list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (list[j] < list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this List<uint> list, SortType st)
    {
        if (list.Count == 0 || st == SortType.None) return;
        uint temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = list.Count; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (list[j] > list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (list[j] < list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this List<float> list, SortType st)
    {
        if (list.Count == 0 || st == SortType.None) return;
        float temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = list.Count; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (list[j] > list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (list[j] < list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }

    public static void Sort(this List<long> list, SortType st)
    {
        if (list.Count == 0 || st == SortType.None) return;
        long temp = 0;
        bool flag = true;//优化冒泡排序
        for (int i = list.Count; i > 0 && flag; i--)
        {
            flag = false;
            for (int j = 0; j < i - 1; j++)
            {
                switch (st)
                {
                    case SortType.Asc:
                        if (list[j] > list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                    case SortType.Dec:
                        if (list[j] < list[j + 1])
                        {
                            temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = true;
                        }
                        break;
                }
            }
        }
    }
    #endregion

    public static void AddRange<T>(this List<T> list, List<T> targets, bool replace = false) where T : struct
    {
        if (replace)
        {
            for (int i = 0, count = targets.Count; i < count; i++)
            {
                if (!list.Contains(targets[i]))
                    list.Add(targets[i]);
            }
        }
        else
            list.AddRange(targets);
    }

    public static void AddRange<T>(this List<T> list, T[] targets, bool replace = false) where T : struct
    {
        if (replace)
        {
            for (int i = 0, count = targets.Length; i < count; i++)
            {
                if (!list.Contains(targets[i]))
                    list.Add(targets[i]);
            }
        }
        else
            list.AddRange(targets);
    }

    #region combine same item
    public static List<T> CombineSameItem<T>(this List<T> list) where T : struct
    {
        List<T> result = new List<T>();
        for (int i = 0, count = list.Count; i < count; i++)
        {
            T t = list[i];
            if (result.Contains(t))
                continue;
            result.Add(t);
        }
        return result;
    }

    public static List<string> CombineSameItem(this List<string> list)
    {
        List<string> result = new List<string>();
        for (int i = 0, count = list.Count; i < count; i++)
        {
            string str = list[i];
            if (result.Contains(str))
                continue;
            result.Add(str);
        }
        list = result;
        result.Clear();
        result = null;
        return list;
    }
    #endregion

    #region remove
    public static bool Remove<T>(this List<T> list, T t, bool sameItem) where T : struct
    {
        if (sameItem)
        {
            T[] ts = list.ToArray();
            for (int i = 0, count = ts.Length; i < count; i++)
            {
                if (ts[i].Equals(t))
                    list.Remove(t);
            }
            return true;
        }
        else
        {
            return list.Remove(t);
        }
    }

    public static bool Remove(this List<string> list, string str, bool sameItem)
    {
        if (sameItem)
        {
            for (int i = 0, count = list.Count; i < count; i++)
            {
                if (list[i].Equals(str))
                    list.Remove(str);
            }
            return true;
        }
        else
        {
            return list.Remove(str);
        }
    }
    #endregion
}

public enum SortType
{
    None = 0,
    Asc = 1,
    Dec = 2
}