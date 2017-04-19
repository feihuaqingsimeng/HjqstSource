using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class DuplicateWindow : EditorWindow {

	List<string> m_Strings = new List<string>();
	static bool m_Init = false;
	static bool m_Found = false;

	[MenuItem("Tools/检查文件名是否重复")]
	static void  Init()
	{
		DuplicateWindow window = (DuplicateWindow)EditorWindow.GetWindow(typeof(DuplicateWindow));
		window.minSize = new Vector2(200,300);
		window.Show();
		m_Init = false;
		m_Found = false;
	}

	void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.wordWrap = true;
		style.normal.textColor  = Color.white;

		GUILayout.BeginVertical();
		if (GUILayout.Button("log dupes"))
		{
			compareAssetList(UsedAssets.GetAllAssets());
			m_Init = true;
		}
		if(m_Init && !m_Found)
		{
			style.normal.textColor = Color.green;
			GUILayout.Label("\nNo dumplicates found :-D\n\n",style);
			style.normal.textColor = Color.white;
		}else if(m_Init && m_Found)
		{
			style.normal.textColor = Color.red;
			string s = "";
			foreach(string t in m_Strings)
			
			{
				s = s + t + "\n";
			}
			GUILayout.Label("\nduplicates found !!!\n\n" + s,style);
			style.normal.textColor = Color.white;
		}
		GUILayout.EndVertical();
	}
	private void compareAssetList(string[] assetList)
	{
		m_Strings.Clear();
		List<string> filenames = new List<string>();
		for(int i = 0;i<assetList.Length;i++)
		{
			string s = assetList[i].Substring(assetList[i].LastIndexOf("/")+1).ToLower();
			int idx = s.IndexOf(".");
			if(idx > 0)
			{
				string type = s.Substring(idx);
				if(idx>0 && (type == ".cs" || type == ".bcc" || type == ".js"))
				{
					s = s.Substring(0,idx);
					filenames.Add(s);
				}
			}
		}
		filenames.Sort();
		for(int i  = 1;i<filenames.Count;i++)
		{
			if(filenames[i] == filenames[i-1])
			{
				m_Found = true;
				if (!m_Strings.Contains(filenames[i]))
				{
					m_Strings.Add(filenames[i]);
				}
			}
		}
	}
}
