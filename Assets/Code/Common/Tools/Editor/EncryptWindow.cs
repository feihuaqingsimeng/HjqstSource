using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using Common.Util;
using Logic.Game;

namespace Common.Tools.Editor
{
    public class EncryptWindow : EditorWindow
    {
        private string content = string.Empty;
        private static string result = string.Empty;
        private static bool isOpen = false;
        [MenuItem("Tools/加密", false, 200)]
        public static void OpenEncryptWindow()
        {
            EncryptWindow win = EditorWindow.GetWindow<EncryptWindow>(true);
            result = string.Empty;
        }

        GUIContent lable = new GUIContent();
        void OnGUI()
        {
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("内容:", GUILayout.Width(40));
            content = EditorGUILayout.TextArea(content, GUILayout.MinWidth(300), GUILayout.MinHeight(200));
            content = content.Trim();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("DES解密") || (Event.current.keyCode == KeyCode.Return))
            {
                if (string.IsNullOrEmpty(content)) return;
                result = EncryptUtil.DESDecryptString(content, GameConfig.encryptKey);
                Debugger.Log(result);
            }
            if (GUILayout.Button("DES加密") || (Event.current.keyCode == KeyCode.Return))
            {
                if (string.IsNullOrEmpty(content)) return;
                result = EncryptUtil.DESEncryptString(content, GameConfig.encryptKey);
                Debugger.Log(result);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("移位解密") || (Event.current.keyCode == KeyCode.Return))
            {
                if (string.IsNullOrEmpty(content)) return;
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(content);
                bytes = EncryptUtil.MinusExcursionBytes(bytes, GameConfig.excursion);
                result = UTF8Encoding.UTF8.GetString(bytes);
                Debugger.Log(result);
            }
            if (GUILayout.Button("移位加密") || (Event.current.keyCode == KeyCode.Return))
            {
                if (string.IsNullOrEmpty(content)) return;
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(content);
                bytes = EncryptUtil.PlusExcursionBytes(bytes, GameConfig.excursion);
                result = ASCIIEncoding.ASCII.GetString(bytes);
                Debugger.Log(result);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("结果:", GUILayout.Width(40));
            result = EditorGUILayout.TextArea(result, GUILayout.MinWidth(300), GUILayout.MinHeight(200));
            GUILayout.EndHorizontal();
        }
    }
}
