using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Common.Util
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CSVElementAttribute : Attribute
    {
        public string key { get; set; }
        public CSVElementAttribute(string key) { this.key = key; }
    }

    public static class CSVUtil
    {
        ///#字段分割符
        public const string SYMBOL_COMMENT = "#";
        ///行分割符
        public static string[] SYMBOL_LINE = new string[] { "\r\n" };
        ///,字段分割符
        public static char[] SYMBOL_FIELD = new char[] { ',' };
        ///;字段分割符
        public static char[] SYMBOL_SEMICOLON = new char[] { ';' };
        ///:字段分割符
        public static char[] SYMBOL_COLON = new char[] { ':' };
        ///|字段分割符
        public static char[] SYMBOL_PIPE = new char[] { '|' };
        ///&字段分割符
        public static char[] SYMBOL_AND = new char[] { '&' };

        private static Dictionary<string, int> ParseTitle(string csvTitleString)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            string[] arr = csvTitleString.Split(SYMBOL_FIELD);
            for (int i = 0, len = arr.Length; i < len; i++)
            {
				if (result.ContainsKey(arr[i]))
				{
					Debugger.LogError("csv title has save key :"+ arr[i]);
				}else
				{
					result.Add(arr[i], i);
				}
                
            }
            return result;
        }

        public static List<KeyValuePair<TKey, TValue>> GetArrayByString<TKey, TValue>(string str)
            where TValue : struct
            where TKey : struct
        {
            string[] strArr = str.Split(SYMBOL_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);
            int len = strArr.Length;
            List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
            string[] tempArr = null;
            Type keyType = typeof(TKey), valueType = typeof(TValue);
            System.Object keyObject, valueObject;
            for (int i = 0; i < len; i++)
            {
                tempArr = strArr[i].Split(SYMBOL_COLON);
                if (keyType == typeof(string))
                    keyObject = tempArr[0];
                else if (keyType == typeof(bool))
                    keyObject = tempArr[0] != "0";
                else
                    keyObject = Convert.ChangeType(tempArr[0], keyType);
                if (valueType == typeof(string))
                    valueObject = tempArr[1];
                else if (valueType == typeof(bool))
                    valueObject = tempArr[1] != "0";
                else
                    valueObject = Convert.ChangeType(tempArr[1], valueType);
                KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>((TKey)keyObject, (TValue)valueObject);
                list.Add(kvp);
            }
            return list;
        }

        /// <summary>
        /// 解析为dic
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvPath"></param>
        /// <param name="keyCSVMark"></param>
        /// <returns></returns>
        public static Dictionary<K, T> Parse<K, T>(string csvPath, string keyCSVMark) where T : new()
        {
            string text = string.Empty;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TextAsset ta = Resources.Load<TextAsset>(csvPath);
                if (ta == null)
                    return null;
                text = ta.text;
                Resources.UnloadAsset(ta);
            }
            else
            {
#endif
                string localPath = csvPath + ".csv";
#if UNITY_EDITOR
                if (Logic.Game.GameConfig.instance.loadCSVRemote)
#else
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath))
#endif
                    text = ResMgr.ResMgr.instance.LoadText(localPath);
                else
                {
                    TextAsset ta = Common.ResMgr.ResMgr.instance.Load<TextAsset>(csvPath);
                    if (ta == null)
                        return null;
                    text = ta.text;
                    Resources.UnloadAsset(ta);
                }
#if UNITY_EDITOR
            }
#endif
            if (string.IsNullOrEmpty(text))
            {
                Debugger.LogError("load {0} fail", csvPath);
                return null;
            }
            Dictionary<K, T> result = new Dictionary<K, T>();
            string[] lineArr = text.Split(SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> titleDic = ParseTitle(lineArr[0]);
            for (int i = 1, lineLen = lineArr.Length; i < lineLen; i++)//第一行是title所以i=1开始
            {
                if (lineArr[i].StartsWith(SYMBOL_COMMENT))
                    continue;
                K key = default(K);
                string[] strArr = lineArr[i].Split(SYMBOL_FIELD, StringSplitOptions.None);
                if (string.IsNullOrEmpty(strArr[0]))
                {
                    Debugger.LogError("数据有空行");
                    continue;
                }
                T instance = System.Activator.CreateInstance<T>();
                FieldInfo[] fieldArr = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, fieldLen = fieldArr.Length; k < fieldLen; k++)
                {
                    FieldInfo field = fieldArr[k];
                    System.Object[] attrArr = field.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        if (!titleDic.ContainsKey(csvele.key))
                        {
                            Debugger.LogError(csvele.key);
                            foreach (var item in titleDic)
                            {
                                Debugger.Log(item.Key);
                            }
                        }
						if(!titleDic.ContainsKey(csvele.key))
						{
							Debugger.LogError("---------------titleDic can not find key:"+csvele.key);
						}
						if (titleDic[csvele.key] >= strArr.Length)
						{
							Debugger.LogError("------------index out of range ,strAttr.length is:"+strArr.Length+",index:"+titleDic[csvele.key]);
						}
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (field.FieldType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else if (field.FieldType == typeof(bool))
                            {
                                valueObject = valueString != "0";
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, field.FieldType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + field.FieldType + "  fieldName:" + field.Name + " value:" + valueString);
                        }
                        field.SetValue(instance, valueObject);

                        if (csvele.key == keyCSVMark)
                        {
                            key = (K)valueObject;
                        }
                    }
                }



                PropertyInfo[] propertyInfoArr = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, propertyLen = propertyInfoArr.Length; k < propertyLen; k++)
                {
                    PropertyInfo pi = propertyInfoArr[k];
                    System.Object[] attrArr = pi.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        if (!titleDic.ContainsKey(csvele.key))
                        {
                            Debugger.LogError("can not find key:" + csvele.key);

                        }
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, pi.PropertyType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + pi.PropertyType + "  fieldName:" + pi.Name + " value:" + valueString);
                        }
                        pi.SetValue(instance, valueObject, null);

                        if (csvele.key == keyCSVMark)
                        {
                            key = (K)valueObject;
                        }
                    }
                }
                result.Add(key, instance);
            }

            return result;
        }
        /// <summary>
        /// 解析为单类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvPath"></param>
        /// <returns></returns>
        public static T ParseClass<T>(string csvPath) where T : new()
        {
            string text = string.Empty;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TextAsset ta = Resources.Load<TextAsset>(csvPath);
                if (ta == null)
                    return default(T);
                text = ta.text;
                Resources.UnloadAsset(ta);
            }
            else
            {
#endif
                string localPath = csvPath + ".csv";
#if UNITY_EDITOR
                if (Logic.Game.GameConfig.instance.loadCSVRemote)
#else
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath))
#endif
                    text = ResMgr.ResMgr.instance.LoadText(localPath);
                else
                {
                    TextAsset ta = Common.ResMgr.ResMgr.instance.Load<TextAsset>(csvPath);
                    if (ta == null)
                        return default(T);
                    text = ta.text;
                    Resources.UnloadAsset(ta);
                }
#if UNITY_EDITOR
            }
#endif
            if (string.IsNullOrEmpty(text))
            {
                Debugger.LogError("load {0} fail", csvPath);
                return default(T);
            }
            T instance = System.Activator.CreateInstance<T>();
            FieldInfo[] fieldArr = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            string[] lineArr = text.Split(CSVUtil.SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> lineDic = new Dictionary<string, string>();
            for (int i = 1, len = lineArr.Length; i < len; i++)
            {
                //忽略第一行
                string line = lineArr[i];
                if (line.StartsWith(SYMBOL_COMMENT))
                    continue;
                //                string[] strArr = line.Split(CSVUtil.SYMBOL_FIELD, 2);
                string[] strArr = line.Split(CSVUtil.SYMBOL_FIELD);
                lineDic.Add(strArr[0], strArr[1]);
            }

            for (int k = 0, fieldLen = fieldArr.Length; k < fieldLen; k++)
            {
                FieldInfo field = fieldArr[k];
                System.Object[] attrArr = field.GetCustomAttributes(typeof(CSVElementAttribute), false);
                if (attrArr.Length > 0)
                {
                    CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                    if (!lineDic.ContainsKey(csvele.key))
                    {
                        Debugger.Log(csvele.key);
                    }
                    string valueString = lineDic[csvele.key];
                    System.Object valueObject = null;
                    try
                    {
                        if (field.FieldType == typeof(string))
                        {
                            valueObject = valueString;
                        }
                        else if (field.FieldType == typeof(bool))
                        {
                            valueObject = valueString != "0";
                        }
                        else
                        {
                            valueObject = Convert.ChangeType(valueString, field.FieldType);
                        }
                    }
                    catch (Exception)
                    {
                        Debugger.LogError("change type error,can't find type:" + field.FieldType + "  fieldName:" + field.Name + " value:" + valueString);
                    }
                    field.SetValue(instance, valueObject);

                }
            }



            PropertyInfo[] propertyInfoArr = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int k = 0, propertyLen = propertyInfoArr.Length; k < propertyLen; k++)
            {
                PropertyInfo pi = propertyInfoArr[k];
                System.Object[] attrArr = pi.GetCustomAttributes(typeof(CSVElementAttribute), false);
                if (attrArr.Length > 0)
                {
                    CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                    string valueString = lineDic[csvele.key];
                    if (!lineDic.ContainsKey(csvele.key))
                    {
                        Debugger.LogError(csvele.key);
                    }
                    System.Object valueObject = null;
                    try
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            valueObject = valueString;
                        }
                        else
                        {
                            valueObject = Convert.ChangeType(valueString, pi.PropertyType);
                        }
                    }
                    catch (Exception)
                    {
                        Debugger.LogError("change type error,can't find type:" + pi.PropertyType + "  fieldName:" + pi.Name + " value:" + valueString);
                    }
                    pi.SetValue(instance, valueObject, null);

                }
            }
            return instance;
        }
        /// <summary>
        /// 解析为list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvPath"></param>
        /// <returns></returns>
        public static List<T> Parse<T>(string csvPath) where T : new()
        {
            string text = string.Empty;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TextAsset ta = Resources.Load<TextAsset>(csvPath);
                if (ta == null)
                    return null;
                text = ta.text;
                Resources.UnloadAsset(ta);
            }
            else
            {
#endif
                string localPath = csvPath + ".csv";
#if UNITY_EDITOR
                if (Logic.Game.GameConfig.instance.loadCSVRemote)
#else
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath))
#endif
                    text = ResMgr.ResMgr.instance.LoadText(localPath);
                else
                {
                    TextAsset ta = Common.ResMgr.ResMgr.instance.Load<TextAsset>(csvPath);
                    if (ta == null)
                        return null;
                    text = ta.text;
                    Resources.UnloadAsset(ta);
                }
#if UNITY_EDITOR
            }
#endif
            if (string.IsNullOrEmpty(text))
            {
                Debugger.LogError("load {0} fail", csvPath);
                return null;
            }
            List<T> result = new List<T>();
            string[] lineArr = text.Split(SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> titleDic = ParseTitle(lineArr[0]);
            for (int i = 1, lineLen = lineArr.Length; i < lineLen; i++)//第一行是title所以i=1开始
            {
                if (lineArr[i].StartsWith(SYMBOL_COMMENT))
                    continue;
                string[] strArr = lineArr[i].Split(SYMBOL_FIELD, StringSplitOptions.None);

                T instance = System.Activator.CreateInstance<T>();
                FieldInfo[] fieldArr = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, fieldLen = fieldArr.Length; k < fieldLen; k++)
                {
                    FieldInfo field = fieldArr[k];
                    System.Object[] attrArr = field.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        if (!titleDic.ContainsKey(csvele.key))
                        {
                            Debugger.Log(csvele.key);
                        }
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (field.FieldType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else if (field.FieldType == typeof(bool))
                            {
                                valueObject = valueString != "0";
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, field.FieldType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + field.FieldType + "  fieldName:" + field.Name + " value:" + valueString);
                        }
                        field.SetValue(instance, valueObject);
                    }
                }



                PropertyInfo[] propertyInfoArr = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, propertyLen = propertyInfoArr.Length; k < propertyLen; k++)
                {
                    PropertyInfo pi = propertyInfoArr[k];
                    System.Object[] attrArr = pi.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, pi.PropertyType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + pi.PropertyType + "  fieldName:" + pi.Name + " value:" + valueString);
                        }
                        pi.SetValue(instance, valueObject, null);
                    }
                }
                result.Add(instance);
            }
            return result;
        }

        public static Dictionary<K, T> ParseEx<K, T>(string csvPath, string keyCSVMark) where T : new()
        {
            string text = string.Empty;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TextAsset ta = Resources.Load<TextAsset>(csvPath);
                if (ta == null)
                    return null;
                text = ta.text;
                Resources.UnloadAsset(ta);
            }
            else
            {
#endif
                string localPath = csvPath + ".csv";
#if UNITY_EDITOR
                if (Logic.Game.GameConfig.instance.loadCSVRemote)
#else
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath))
#endif
                    text = ResMgr.ResMgr.instance.LoadText(localPath);
                else
                {
                    TextAsset ta = Common.ResMgr.ResMgr.instance.Load<TextAsset>(csvPath);
                    if (ta == null)
                        return null;
                    text = ta.text;
                    Resources.UnloadAsset(ta);
                }
#if UNITY_EDITOR
            }
#endif
            if (string.IsNullOrEmpty(text))
            {
                Debugger.LogError("load {0} fail", csvPath);
                return null;
            }
            Dictionary<K, T> result = new Dictionary<K, T>();
            string[] lineArr = text.Split(SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> titleDic = ParseTitle(lineArr[0]);
            for (int i = 1, lineLen = lineArr.Length; i < lineLen; i++)//第一行是title所以i=1开始
            {
                if (lineArr[i].StartsWith(SYMBOL_COMMENT))
                    continue;
                K key = default(K);
                string[] strArr = lineArr[i].Split(SYMBOL_FIELD, StringSplitOptions.None);

                T instance = System.Activator.CreateInstance<T>();
                FieldInfo[] fieldArr = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, fieldLen = fieldArr.Length; k < fieldLen; k++)
                {
                    FieldInfo field = fieldArr[k];
                    System.Object[] attrArr = field.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        if (!titleDic.ContainsKey(csvele.key))
                        {
                            Debugger.Log(csvele.key);
                        }
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (field.FieldType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else if (field.FieldType == typeof(bool))
                            {
                                valueObject = valueString != "0";
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, field.FieldType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + field.FieldType + "  fieldName:" + field.Name + " value:" + valueString);
                        }
                        field.SetValue(instance, valueObject);

                        if (csvele.key == keyCSVMark)
                        {
                            key = (K)valueObject;
                        }
                    }
                }



                PropertyInfo[] propertyInfoArr = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int k = 0, propertyLen = propertyInfoArr.Length; k < propertyLen; k++)
                {
                    PropertyInfo pi = propertyInfoArr[k];
                    System.Object[] attrArr = pi.GetCustomAttributes(typeof(CSVElementAttribute), false);
                    if (attrArr.Length > 0)
                    {
                        CSVElementAttribute csvele = attrArr[0] as CSVElementAttribute;
                        string valueString = strArr[titleDic[csvele.key]];
                        System.Object valueObject = null;
                        try
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                valueObject = valueString;
                            }
                            else
                            {
                                valueObject = Convert.ChangeType(valueString, pi.PropertyType);
                            }
                        }
                        catch (Exception)
                        {
                            Debugger.LogError("change type error,can't find type:" + pi.PropertyType + "  fieldName:" + pi.Name + " value:" + valueString);
                        }
                        pi.SetValue(instance, valueObject, null);

                        if (csvele.key == keyCSVMark)
                        {
                            key = (K)valueObject;
                        }
                    }
                }
                if (!result.ContainsKey(key))
                    result.Add(key, instance);
                else
                    result[key] = instance;
            }

            return result;
        }
    }
}
