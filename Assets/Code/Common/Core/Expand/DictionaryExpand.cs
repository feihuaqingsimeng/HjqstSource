using System;
using System.Collections;
using System.Collections.Generic;

public static class DictionaryExpand
{
    /// <summary>
    /// 获取字典中被添加顺序最前的键值对
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static KeyValuePair<TKey, TValue> First<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        int i = -1;
        KeyValuePair<TKey, TValue> result = default(KeyValuePair<TKey, TValue>);
        foreach (var kvp in dic)
        {
            i++;
            if (i == 0)
            {
                result = kvp;
                break;
            }
        }
        if (i == -1)
            Debugger.Log("字典为空");
        return result;
    }

    /// <summary>
    /// 移除顺序第一的键值对
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static bool RemoveFirst<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        if (dic.Count == 0) return false;
        int i = -1;
        bool flag = true;
        KeyValuePair<TKey, TValue> result = default(KeyValuePair<TKey, TValue>);
        foreach (var kvp in dic)
        {
            i++;
            if (i == 0)
            {
                result = kvp;
                break;
            }
        }
        dic.Remove(result.Key);
        return flag;
    }

    /// <summary>
    /// 获取字典中被添加顺序最后的键值对
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static KeyValuePair<TKey, TValue> Last<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        int i = 0;
        int count = dic.Count;
        KeyValuePair<TKey, TValue> result = default(KeyValuePair<TKey, TValue>);
        foreach (var kvp in dic)
        {
            i++;
            if (i == count)
            {
                result = kvp;
                break;
            }
        }
        if (i == 0)
            Debugger.Log("字典为空");
        return result;
    }

    /// <summary>
    /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
    /// </summary>
    public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (!dict.ContainsKey(key)) dict.Add(key, value);
        else Debugger.Log("已存在相同的key:" + key.ToString());
        return dict;
    }

    /// <summary>
    /// 尝试从字典中移除Tkey的键值对：如果不存在，忽略；存在，删除
    /// </summary>
    public static Dictionary<TKey, TValue> TryDelete<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict.ContainsKey(key)) dict.Remove(key);
        return dict;
    }

    /// <summary>
    /// 尝试从字典中获取Tkey的值：如果不存在，返回null；存在，返回TValue
    /// </summary>
    public static TValue TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict.ContainsKey(key)) return dict[key];
        return default(TValue);
    }

    /// <summary>
    /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
    /// </summary>
    public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict[key] = value;
        return dict;
    }

    /// <summary>
    /// 向字典中批量添加键值对
    /// </summary>
    public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        foreach (var item in values)
        {
            dict.Add(item.Key, item.Value);
        }
        return dict;
    }

    /// <summary>
    /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
    /// </summary>
    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
    {
        return dict.ContainsKey(key) ? dict[key] : defaultValue;
    }

    /// <summary>
    /// 获取字典中的最大值(byte)
    /// </summary>
    public static KeyValuePair<TKey, byte> MaxValue<TKey, TValue>(this Dictionary<TKey, byte> dict)
    {
        //if(t==typeof(int))
        KeyValuePair<TKey, byte> result = default(KeyValuePair<TKey, byte>);
        if (dict.Values.Count > 0)
        {
            result = dict.First();
            foreach (var v in dict)
            {
                if (result.Value < v.Value)
                    result = v;
            }
        }
        else
            Debugger.Log("dic is empty!");
        return result;
    }

    /// <summary>
    /// 获取字典中的最大值(int)
    /// </summary>
    public static KeyValuePair<TKey, int> MaxValue<TKey, TValue>(this Dictionary<TKey, int> dict)
    {
        //if(t==typeof(int))
        KeyValuePair<TKey, int> result = default(KeyValuePair<TKey, int>);
        if (dict.Values.Count > 0)
        {
            result = dict.First();
            foreach (var v in dict)
            {
                if (result.Value < v.Value)
                    result = v;
            }
        }
        else
            Debugger.Log("dic is empty!");
        return result;
    }

    /// <summary>
    /// 获取字典中的最大值的键值对
    /// </summary>
    public static KeyValuePair<TKey, uint> MaxValue<TKey, TValue>(this Dictionary<TKey, uint> dict)
    {
        //if(t==typeof(int))
        KeyValuePair<TKey, uint> result = default(KeyValuePair<TKey, uint>);
        if (dict.Values.Count > 0)
        {
            result = dict.First();
            foreach (var v in dict)
            {
                if (result.Value < v.Value)
                    result = v;
            }
        }
        else
            Debugger.Log("dic is empty!");
        return result;
    }

    /// <summary>
    /// 获取字典中的最大值(float)
    /// </summary>
    public static KeyValuePair<TKey, float> MaxValue<TKey, TValue>(this Dictionary<TKey, float> dict)
    {
        //if(t==typeof(int))
        KeyValuePair<TKey, float> result = default(KeyValuePair<TKey, float>);
        if (dict.Values.Count > 0)
        {
            result = dict.First();
            foreach (var v in dict)
            {
                if (result.Value < v.Value)
                    result = v;
            }
        }
        else
            Debugger.Log("dic is empty!");
        return result;
    }

    /// <summary>
    /// 获取字典中keys列表的list
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TKey> GetKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        List<TKey> result = new List<TKey>(dict.Keys);
        return result;
    }

    /// <summary>
    /// 获取字典中keys列表的array
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TKey[] GetKeyArray<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        TKey[] result = new TKey[dict.Count];
        dict.Keys.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    /// 获取字典中values列表的array
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TValue[] GetValueArray<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        TValue[] result = new TValue[dict.Count];
        dict.Values.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    /// 获取字典中values列表的list
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TValue> GetValues<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        List<TValue> result = new List<TValue>(dict.Values);
        return result;
    }


    public static KeyValuePair<TKey, TValue> Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, int index)
    {
        KeyValuePair<TKey, TValue> result = default(KeyValuePair<TKey, TValue>);
        if (dict.Count - 1 < index)
            return result;
        int i = 0;
        foreach (var kvp in dict)
        {
            if (i == index)
            {
                result = kvp;
                break;
            }
            i++;
        }
        return result;
    }
}

