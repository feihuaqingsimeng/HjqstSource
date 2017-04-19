using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace Common.OutLogger
{
    public class OutLogger : MonoBehaviour
    {
#if !UNITY_EDITOR
        //static List<string> mLines = new List<string>();
#endif
        static List<string> mWriteTxt = new List<string>();
        private string outpath;
        public bool logScreen;
        void Start()
        {
            //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
            outpath = Application.persistentDataPath + "/outLog.txt";
            //每次启动客户端删除之前保存的Log
            if (System.IO.File.Exists(outpath))
            {
                File.Delete(outpath);
            }
            //在这里做一个Log的监听
            Application.logMessageReceived += HandleLog;
#if !UNITY_EDITOR
            Debugger.Log("path="+outpath);
            //Debugger.Log("------------------------------------------------------------------start log--------------------------------------------------------------------");           
#endif
        }

        void Update()
        {
            //因为写入文件的操作必须在主线程中完成，所以在Update中哦给你写入文件。
            try
            {
                if (mWriteTxt.Count > 0)
                {
                    string[] temp = mWriteTxt.ToArray();
                    foreach (string t in temp)
                    {
                        using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                        {
                            writer.WriteLine(t);
                        }
                        mWriteTxt.Remove(t);
                    }
                }
            }
            catch (System.UnauthorizedAccessException unauthorizedAccessException)
            {
                Debugger.LogError(unauthorizedAccessException.Message + "+++++" + unauthorizedAccessException.StackTrace);
                Object.Destroy(this);
            }
            catch (System.Exception e)
            {
                Debugger.LogError("write /outLog.txt exception:" + e.StackTrace);
                Object.Destroy(this);
            }
        }

        void OnDestroy()
        {
#if !UNITY_EDITOR
            //Debugger.Log("------------------------------------------------------------------end log--------------------------------------------------------------------");   
#endif
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            mWriteTxt.Add(logString);
#if !UNITY_EDITOR
            mWriteTxt.Add(stackTrace);
#endif
            Update();
#if !UNITY_EDITOR
            /*if (type == LogType.Error || type == LogType.Exception || type == LogType.Log)
            {
                Log(logString);
                Log(stackTrace);
            }*/
#endif
        }

#if !UNITY_EDITOR
        /*
        //这里我把错误的信息保存起来，用来输出在手机屏幕上
        static public void Log(params object[] objs)
        {
            string text = "";
            for (int i = 0; i < objs.Length; ++i)
            {
                if (i == 0)
                {
                    text += objs[i].ToString();
                }
                else
                {
                    text += ", " + objs[i].ToString();
                }
            }
            if (Application.isPlaying)
            {
                if (mLines.Count > 20)
                {
                    mLines.RemoveAt(0);
                }
                mLines.Add(text);
            }
        }

        void OnGUI()
        {
            if (logScreen)
            {
                GUI.color = Color.red;
                if (mLines.Count > 0)
                {
                    if (GUI.Button(new Rect(800, 0, 80, 30), "clear"))
                    {
                        mLines.Clear();
                    }
                    for (int i = mLines.Count - 1; i >= 0; i--)
                    {
                        GUILayout.Label(mLines[i]);
                    }
                }
            }
        }
         */
#endif
    }
}