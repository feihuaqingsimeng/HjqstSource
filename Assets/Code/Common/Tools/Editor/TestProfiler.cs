using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Common.Tools.Editor
{
    public struct MemoryStuct
    {
        public int count;
        public int size;
    }

    public class TestProfiler
    {
        static Dictionary<string, MemoryStuct> previousDetailMemoryDict = new Dictionary<string, MemoryStuct>();
        static Dictionary<string, MemoryStuct> detailMemoryDict = new Dictionary<string, MemoryStuct>();

        [MenuItem("Test Profiler/Clear Dictionary")]
        public static void ClearDictionary()
        {
            detailMemoryDict.Clear();
        }

        [MenuItem("Test Profiler/Memory Compare")]
        public static void MemoryCompare()
        {
            Type t = null;
            previousDetailMemoryDict = detailMemoryDict;
            detailMemoryDict = new Dictionary<string, MemoryStuct>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                if (asm.FullName == "UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                    t = asm.GetType("UnityEditor.ProfilerWindow");
            System.Object profilerWindow = Resources.FindObjectsOfTypeAll(t)[0];

            MethodInfo refreshMemoryData_MethodInfo = t.GetMethod("RefreshMemoryData", BindingFlags.Instance | BindingFlags.NonPublic);
            refreshMemoryData_MethodInfo.Invoke(profilerWindow, null);

            FieldInfo m_MemoryListViewFi = t.GetField("m_MemoryListView", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Object m_MemoryListView = m_MemoryListViewFi.GetValue(profilerWindow);
            Type m_MemoryListViewType = m_MemoryListView.GetType();
            FieldInfo m_RootFi = m_MemoryListViewType.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Object m_Root = m_RootFi.GetValue(m_MemoryListView);
            if (m_Root == null)
            {
                Debugger.Log("please take sample !");
                return;
            }
            Compare(m_Root);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, MemoryStuct> kvp in detailMemoryDict)
            {
                if (previousDetailMemoryDict.ContainsKey(kvp.Key))
                {
                    if (previousDetailMemoryDict[kvp.Key].count < detailMemoryDict[kvp.Key].count)
                    {
                        //Debug.LogError("name:" + kvp.Key + ",count:" + (detailMemoryDict[kvp.Key] - previousDetailMemoryDict[kvp.Key]) + " added, total:" + detailMemoryDict[kvp.Key]);
                        sb.AppendLine("name:" + kvp.Key + ",size:" + detailMemoryDict[kvp.Key].size + " mb,count:" + (detailMemoryDict[kvp.Key].count - previousDetailMemoryDict[kvp.Key].count) + " added, total:" + detailMemoryDict[kvp.Key].count);
                    }
                }
                else
                {
                    //Debug.LogError("name:" + kvp.Key + ",count:" + detailMemoryDict[kvp.Key] + " added, total:" + detailMemoryDict[kvp.Key]);
                    sb.AppendLine("name:" + kvp.Key + ",size:" + detailMemoryDict[kvp.Key].size + " mb,count:" + detailMemoryDict[kvp.Key].count + " added, total:" + detailMemoryDict[kvp.Key].count);
                }
            }
            Debugger.LogError(sb.ToString());
            string dir = Application.dataPath + "/../memory";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            DateTime date = System.DateTime.Now;
            string path = dir + "/" + date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" + date.Minute + "-" + date.Second + ".txt";
            System.IO.File.WriteAllText(path, sb.ToString());
            Debugger.LogError("content saved to {0}", path);
        }

        public static void Compare(System.Object obj)
        {
            Type t = obj.GetType();
            //FieldInfo totalChildCountFi = t.GetField("totalChildCount");
            //int totalChildCount = (int)totalChildCountFi.GetValue(obj);
            FieldInfo childrenFi = t.GetField("children");
            ICollection children = childrenFi.GetValue(obj) as ICollection;
            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    Compare(child);
                }
            }
            else
            {
                FieldInfo nameFi = t.GetField("name");
                FieldInfo totalMemoryFi = t.GetField("totalMemory");
                string name = nameFi.GetValue(obj) as string;
                int totalMemory = (int)totalMemoryFi.GetValue(obj);
                if (!detailMemoryDict.ContainsKey(name))
                {
                    //Debug.LogError(name + "," + totalMemory);
                    MemoryStuct ms = new MemoryStuct();
                    ms.count = 1;
                    ms.size = totalMemory / 1024 / 1024;
                    detailMemoryDict.Add(name, ms);
                }
                else
                {
                    MemoryStuct ms = detailMemoryDict[name];
                    ms.count++;
                    ms.size = totalMemory;
                }
            }
        }
    }
}