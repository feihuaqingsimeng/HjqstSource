using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Logic.Protocol.Model;
using Logic.UI.FightResult.Model;


namespace Common.Tools.Editor
{
    public class ChatWindow : EditorWindow
    {
        private string content = string.Empty;
        private static string contents = string.Empty;
        private static List<string> cacheStr = new List<string>();
        private static bool isSave = false;
        private static bool isOpen = false;
		[MenuItem("Tools/GM界面 ",false,201)]
		public static void OpenChatView()
		{
			Logic.UI.Chat.View.GMView.Open();
		}

        [MenuItem("Tools/GM聊天窗口 &d", false, 200)]
        public static void OpenChatWindow()
        {
            ChatWindow win = EditorWindow.GetWindow<ChatWindow>(true);
            contents = EditorPrefs.GetString("contents");
            string[] strs = contents.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            cacheStr.Clear();
            cacheStr = new List<string>(strs);
            if (!isOpen)
            {
                isOpen = true;
                win.Show();
            }
            else
            {
                isOpen = false;
                win.Close();
            }
        }

        GUIContent lable = new GUIContent();
        void OnGUI()
        {
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("内容:", GUILayout.Width(40));
            bool isAddContent = false;
            content = EditorGUILayout.TextField(lable, content);
            content = content.Trim();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            isSave = GUILayout.Toggle(isSave, "保存");
            if ((GUILayout.Button("发送") || (Event.current.keyCode == KeyCode.Return)) && Application.isPlaying)
            {
                if (string.IsNullOrEmpty(content)) return;
                Logic.Protocol.Model.GMReq gm = new Logic.Protocol.Model.GMReq();
				if (content == "skip") 
				{
					GuideReq guidReq = new GuideReq();
					guidReq.no = 100;
					Logic.Protocol.ProtocolProxy.instance.SendProtocol(guidReq);
					//FightResultProxy.instance.GotoMainScene(Logic.Enums.FightResultQuitType.Go_MainView,FightResultProxy.instance.QuitPveCallback);
					Logic.Game.Controller.GameController.instance.ReLoadMainScene();
				}else
				{
					gm.command = content;
					Logic.Protocol.ProtocolProxy.instance.SendProtocol(gm);
				}
                
                if (isSave)
                {
                    if (!cacheStr.Contains(content))
                    {
                        isAddContent = true;
                        cacheStr.Add(content);
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (var str in cacheStr)
                    {
                        sb.AppendLine(str);
                    }
                    contents = sb.ToString();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("历史记录:", GUILayout.Width(200));
            string temp = EditorGUILayout.TextArea(contents, GUILayout.MinWidth(300), GUILayout.MinHeight(200));
            if (temp != contents || isAddContent)
            {
                string[] strs = temp.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                cacheStr.Clear();
                cacheStr = new List<string>(strs);
                contents = temp;
                EditorPrefs.SetString("contents", contents);
            }
        }
    }
}