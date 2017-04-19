using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Enums;
using Logic.Game.Model;
using Logic.UI.GoodsJump.Model;
using Logic.Dungeon.Model;
using System.Collections.Generic;
using Common.Localization;
using Logic.Drop.Model;
using System.Text;
using System.IO;

namespace Logic.UI.GoodsJump.Editor
{
	public class ParseConfigLuaEditor : EditorWindow
	{
		
		private static string resultNameTip = string.Empty;
		Vector2 scrollPos = Vector2.zero;

		void OnGUI()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos,false,false,GUILayout.Width(400),GUILayout.Height(500));
			EditorGUILayout.TextArea(resultNameTip);
			EditorGUILayout.EndScrollView();
		}

		public static void TransformLuaDataAddAutoId(string path,bool isStringId = false,bool saveToFile = true)
		{
			Debugger.Log("transform lua data path:"+path);
			if(!path.EndsWith(".lua") || !path.Contains("Lua/user/config"))
			{
				Debugger.Log("please select a config dir lua data!!! ");
				return;
			}
			StringBuilder sb = new StringBuilder();
			
			StreamReader sr = new StreamReader(path,Encoding.UTF8);
			string line = "";
			bool find = false;
			int index = 1;
			while(true)
			{
				line = sr.ReadLine();
				if(!find)
				{
					sb.Append(line);
					sb.Append("\n");
				}
				if(line == "t.t = {")
				{
					find = true;
					continue;
				}
				if(find)
				{
					int leftIndex = line.IndexOf("[");
					int rightIndex = line.IndexOf("]",leftIndex+1);
					if(leftIndex >= 0 && rightIndex > leftIndex)
					{
						if(isStringId)
						{
							line = line.Insert(leftIndex+1,"'");
							line = line.Insert(rightIndex+1,"'");
						}else{
							line = line.Substring(0,leftIndex+1)+index.ToString()+line.Substring(rightIndex);
						}
						

						sb.Append(line);
						sb.Append("\n");
						index ++;
					}else if(line == "}")
					{
						find = false;
						sb.Append(line);
						sb.Append("\n");
					}
					
				}
				if(sr.EndOfStream)
					break;
			}
			sr.Close();
			if(saveToFile)
			{
				StreamWriter sw = new StreamWriter(path,false,Encoding.UTF8);
				sw.WriteLine(sb.ToString());
				sw.Close();
			}else
			{
				resultNameTip = sb.ToString();
				
				EditorWindow.GetWindow<ParseConfigLuaEditor>(true,"代码");
			}

		}


		[MenuItem("Assets/config目录 lua文件/解析数据", false, 0)]
		public static void TransformLuaData()
		{


			string path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if(!path.EndsWith(".lua") || !path.Contains("Lua/user/config"))
			{
				Debugger.Log("please select a config dir lua data!!! ");
				return;
			}

			string name =path.Substring( path.LastIndexOf("/")+1);

			name = name.Substring(0,name.IndexOf("."));

			string tableName = "t";
			string tableItemName = "item";

			//string newFileName = string.Format("{0}{1}.lua",path.Substring(0,path.LastIndexOf("/")+1),tableName);
			//Debugger.Log("create file:"+newFileName);

			StringBuilder sb = new StringBuilder();
			//sb.Append(string.Format("local name = '{0}'\n",tableName)); 

			sb.Append(string.Format("local {0} = {1}\n",tableName,"{}"));
			sb.Append(string.Format("{0}.data = {1}\n",tableName,"{}"));
			sb.Append(string.Format("local {0} = {1}\n",tableItemName,"{}"));
			sb.Append(string.Format("{0}.__index = {1}\n",tableItemName,tableItemName));
			//get col name
			StreamReader sr = new StreamReader(path,Encoding.UTF8);
			string data = sr.ReadToEnd();
			sr.Close();
			int start = data.IndexOf("indexs={");
			int end  = data.IndexOf("}",start);
			string head = data.Substring(start+8,end-(start+8));

			string[] colName = head.Split(',');
			List<string> colNameList = new List<string>();
			for(int i = 0,count = colName.Length;i<count;i++)
			{
				string cn = colName[i];
				if(!string.IsNullOrEmpty(cn))
				{
					int starI = cn.IndexOf("'");
					int endI = cn.LastIndexOf("'");
					if(starI >= 0 && endI > starI)
						colNameList.Add( cn.Substring(starI+1,endI-starI-1));

				}
			}
			//new item
			sb.Append(string.Format( "\nfunction {0}.New(table)\n",tableItemName));
			sb.Append("\tlocal o = {}\n");
			sb.Append(string.Format("\tsetmetatable(o,{0})\n",tableItemName));
			for(int i = 0,count2 = colNameList.Count;i<count2;i++)
			{
				sb.Append(string.Format("\to.{0} = table.{1}\n",colNameList[i],colNameList[i]));
				
			}
			sb.Append("\treturn o\n");
			sb.Append("end\n");
			//GetDataById
			sb.Append(string.Format("\nfunction {0}.GetDataById(id)\n\treturn {1}.data[id]\nend\n",tableName,tableName));

			//start
			sb.Append("\nlocal function Start()\n");
			sb.Append(string.Format("\tlocal origin = dofile('{0}')\n",name));
			sb.Append("\torigin.ForEach(function(id,table)\n");
			sb.Append(string.Format("\t\tlocal newItem = {0}.New(table)\n",tableItemName));
			sb.Append(string.Format("\t\t{0}.data[id] = newItem\n",tableName));
			sb.Append("\tend)\n");
			sb.Append("end\n");

			//end
			sb.Append(string.Format("\nStart()\nreturn {0}",tableName));
		
			resultNameTip = sb.ToString();

			EditorWindow.GetWindow<ParseConfigLuaEditor>(true,"代码");
		}

		[MenuItem("Assets/config目录 lua文件/解析成class", false, 0)]
		public static void TransformLuaDataToClass()
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			
			if(!path.EndsWith(".lua") || !path.Contains("Lua/user/config"))
			{
				Debugger.Log("please select a config dir lua data!!! ");
				return;
			}
			
			string name =path.Substring( path.LastIndexOf("/")+1);
			
			name = name.Substring(0,name.IndexOf("."));
			string tableName = "t";
			
			string newFileName = string.Format("{0}{1}.lua",path.Substring(0,path.LastIndexOf("/")+1),tableName);
			Debugger.Log("create file:"+newFileName);
			StringBuilder sb = new StringBuilder();
			//sb.Append(string.Format("local name = '{0}'\n",tableName)); 
			
			sb.Append(string.Format("local {0} = {1}\n",tableName,"{}"));

			//start
			sb.Append("\nlocal function Start()\n");
			sb.Append(string.Format("\tlocal origin = dofile('{0}')\n",name));
			//parse item
			StreamReader sr = new StreamReader(path,Encoding.UTF8);
			string line = "";
			bool find = false;
			int index = 1;
			while(true)
			{
				line = sr.ReadLine();
				if(line == "t.t = {")
				{
					find = true;
					continue;
				}
				if(find)
				{
					int leftIndex = line.IndexOf("{");
					int rightIndex = line.IndexOf("}",leftIndex+1);
					if(leftIndex >= 0 && rightIndex > leftIndex)
					{

						line = line.Substring(leftIndex+1,rightIndex-leftIndex-1);
						string[] kv = line.Split(',');
						string k = kv[0].Substring(1,kv[0].Length-2);
						sb.Append(string.Format("\t{0}.{1} = origin.t[{2}][2] \n",tableName,k,kv[0]));
						//sb.Append(string.Format("\tprint('{0}', {1}.{2}) \n",k,tableName,k));
						index ++;
					}else if(line == "}")
					{
						find = false;
						break;
					}
					
				}
				if(sr.EndOfStream)
					break;
			}
			sr.Close();
			//start
			sb.Append("end\n");
			
			//end
			sb.Append(string.Format("\nStart()\nreturn {0}",tableName));
			
			resultNameTip = sb.ToString();
			
			EditorWindow.GetWindow<ParseConfigLuaEditor>(true,"代码");
		}
	}
}

