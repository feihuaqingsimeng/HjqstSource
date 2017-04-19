using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

namespace Common.Tools.Editor
{
    public static class ProtocolGenerator
    {
        [MenuItem("Tools/ProtocolGenerator", false, 100)]
        public static void GenProtocol()
        {
            string batFilePath = UnityEngine.Application.dataPath + "/../Protobuf-net/GenerateProtocol.bat";
            FileInfo fileInfo = new FileInfo(batFilePath);
            DirectoryInfo directoryInfo = new DirectoryInfo(UnityEngine.Application.dataPath + "/../Protobuf-net/");
            List<string> protos = new List<string>();
            foreach (var file in directoryInfo.GetFiles("*.proto"))
            {
                protos.Add(file.Name.Replace(".proto", string.Empty));
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = fileInfo.Directory.FullName;
                process.StartInfo.FileName = batFilePath;
                process.StartInfo.Arguments = file.Name.Replace(".proto", string.Empty);
                process.Start();
                process.WaitForExit();
            }
            foreach (var file in directoryInfo.GetFiles("*.cs"))
            {
                string name = file.Name;
                string firstChar = name.Substring(0, 1);
                firstChar = firstChar.ToUpper();
                name = firstChar + name.Substring(1);
                string targetFilePath = UnityEngine.Application.dataPath + "/Code/Logic/Protocol/Model/" + name;
                //Debugger.Log(targetFilePath);
                string[] protocolContents = File.ReadAllLines(file.FullName, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (var c in protocolContents)
                {
                    i++;
                    string protocolContent = c;
                    if (!protocolContent.StartsWith("//"))
                    {
                        //Debugger.Log("proto." + file.Name);
                        string fileName = file.Name.Replace(".cs", string.Empty);
                        protocolContent = protocolContent.Replace("namespace " + fileName, "namespace Logic.Protocol.Model");//改命名空间
                        protocolContent = protocolContent.Replace(fileName + ".", "Logic.Protocol.Model.");//自己引用
                        foreach (var p in protos)
                        {
                            if (p == fileName) continue;
                            protocolContent = protocolContent.Replace(p + ".", "Logic.Protocol.Model.");//引用其他的包
                        }
                    }
                    sb.Append(protocolContent);
                    if (i < protocolContents.Length)
                        sb.Append("\r\n");
                }
                File.WriteAllText(targetFilePath, sb.ToString(), Encoding.UTF8);
                Debugger.Log("update proto " + targetFilePath + "  success!");
            }
            System.GC.Collect();
        }

        [MenuItem("Tools/PbLuaGenerator", false, 100)]
        static void GenPbLua()
        {

            var path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + "Protobuf-net/lua/";
            var luapath = path + "_protos/lua/";
            var batpath = path + "genpblua/build.bat";
			//copy proto to lua dictionary
			DirectoryInfo directoryInfo = new DirectoryInfo(UnityEngine.Application.dataPath + "/../Protobuf-net/");
			List<string> protos = new List<string>();
			EditorUtility.DisplayProgressBar("提示","正在复制proto文件",0);
			foreach (var file in directoryInfo.GetFiles("*.proto"))
			{
				File.Copy(file.FullName, path+"_protos/"+file.Name, true);
			}
			EditorUtility.DisplayProgressBar("提示","正在生成lua pb文件",0.3f);
            var process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.WorkingDirectory = (new FileInfo(batpath)).Directory.FullName;
            process.StartInfo.FileName = batpath;
            process.Start();
            process.WaitForExit();
            process.Dispose();

			EditorUtility.DisplayProgressBar("提示","正在复制 pb文件",0.8f);
            if (Directory.Exists(luapath))
            {
                var asset = Application.dataPath + "/ToLua/Lua/user/protocol/";
                if (!Directory.Exists(asset)) Directory.CreateDirectory(asset);
                var files = Directory.GetFiles(luapath);
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    var name = files[i].Substring(files[i].LastIndexOf('/') + 1);
                    name = asset + name;
					FileInfo info;
					CheckLuaPbLinkOtherFile(files[i], name);
                }
				for (int i = 0, length = files.Length; i < length; i++)
				{
					var name = files[i].Substring(files[i].LastIndexOf('/') + 1);
					name = asset + name;
					FileInfo info;
					File.Copy(files[i], name, true);
					File.Delete(files[i]);
				}
                Directory.Delete(luapath);
            }
            else
            {
                Debug.LogWarning("not exist:" + luapath);
            }
			EditorUtility.ClearProgressBar();
            Debug.Log("genlua success!");
            ProtobuffMSGLuaGenerator();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/ExcelLuaGenerator", false, 100)]
        static void GenExcelLua()
        {
            var path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + "document/";
            var luapath = path + "Excel/lua/";
            var batpath = path + "转换工具/build.cmd";

            var process = new System.Diagnostics.Process();
            process.StartInfo.WorkingDirectory = (new FileInfo(batpath)).Directory.FullName;
            process.StartInfo.FileName = batpath;
            process.StartInfo.Arguments = "allow-yes";
            process.Start();
            process.WaitForExit();
            process.Dispose();

            if (Directory.Exists(luapath))
            {
                var asset = Application.dataPath + "/ToLua/Lua/user/config/";
                if (!Directory.Exists(asset)) Directory.CreateDirectory(asset);
                var files = Directory.GetFiles(luapath);
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    var name = files[i].Substring(files[i].LastIndexOf('/') + 1);
                    name = asset + name;

                    File.Copy(files[i], name, true);
                    File.Delete(files[i]);
                }
                Directory.Delete(luapath);
            }
            else
            {
                Debug.LogWarning("not exist:" + luapath);
            }

            Debug.Log("genexcellua success!");
            
			//transfer special data 
            //Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/global.lua",true);
			Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/formation_attr.lua");
			Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/aggr_extraExp.lua");
			Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/aggr.lua");
			Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/timescost.lua");
			Logic.UI.GoodsJump.Editor.ParseConfigLuaEditor.TransformLuaDataAddAutoId("Assets/ToLua/Lua/user/config/roulette.lua");
			
			AssetDatabase.Refresh();
        }



        [MenuItem("Tools/ProtobuffMSGLuaGenerator", false, 100)]
        public static void ProtobuffMSGLuaGenerator()
        {
            string sourceFilePath = UnityEngine.Application.dataPath + "/ToLua/Lua/user/protocol/message_pb.lua";
            string targetFilePath = UnityEngine.Application.dataPath + "/ToLua/Lua/user/protocol/message.lua";
            string[] content = File.ReadAllLines(sourceFilePath, Encoding.Default);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int length = content.Length;
            for (; i < length; i++)
            {
                if (content[i].StartsWith(@"MSG.values = {"))
                    break;
            }
            i++;
            sb.AppendLine("MSG = {}");
            sb.AppendLine(string.Empty);
            for (; i < length; i++)
            {
                string line = content[i];
                if (string.IsNullOrEmpty(line))
                    continue;
                sb.AppendLine("MSG." + line);
            }
            File.WriteAllText(targetFilePath, sb.ToString(), Encoding.Default);
            Debug.Log("ProtobuffMSGLuaGenerator success!");
            AssetDatabase.Refresh();
        }


		public static void CheckLuaPbLinkOtherFile(string fileSrcPath,string fileDesPath)
		{
			StreamReader sr = new StreamReader(fileSrcPath,Encoding.UTF8);
			int replaceFilePathIndex = fileSrcPath.LastIndexOf("/");
			string line = "";
			int index = 1;
			try
			{
				Dictionary<string,string> findPbDic = new Dictionary<string, string>();
				while(true)
				{
					line = sr.ReadLine();
					if(sr.EndOfStream)
						break;
					if(line.Contains("_pb.") && line.Contains("message_type"))
					{
						int equalIndex = line.IndexOf("=");
						index = line.IndexOf("_pb.");
						string pbName = line.Substring(equalIndex+1,index-equalIndex + 2).Trim();

						string funcName = line.Substring(index+4).Trim();
						if (!findPbDic.ContainsKey(funcName))
						{
							findPbDic.Add(funcName,pbName);

							string replaceFilePath = fileSrcPath.Substring(0,replaceFilePathIndex+1) + pbName+".lua";
							//Debugger.Log("find "+ pbName+","+funcName +" ,it's time to change it,please wait!!! ");
							ReplaceStringInFile(replaceFilePath,replaceFilePath,"local "+funcName+" = protobuf.Descriptor();",funcName + " = protobuf.Descriptor();");
						}

					}

				}
				sr.Close();;
			}catch(Exception e)
			{
				Debugger.LogError(fileSrcPath);
				Debugger.LogError(e.StackTrace);
			}finally
			{
				if (sr != null)
				{
					sr.Close();
				}
			}
			
			
		}
		public static void ReplaceStringInFile(string fileSrcPath,string fileDesPath,string oldStr,string newStr)
		{
			StringBuilder sb = new StringBuilder();
			StreamReader sr = new StreamReader(fileSrcPath,Encoding.UTF8);
			string line = "";
			try
			{
				while(true)
				{

					line = sr.ReadLine();
					if(line == oldStr)
					{
						line = newStr;
					}
					line.Replace(oldStr,newStr);
					sb.Append(line);
					sb.AppendLine();
					if(sr.EndOfStream)
						break;
				}
				sr.Close();
				
				StreamWriter sw = new StreamWriter(fileDesPath,false,Encoding.UTF8);
				sw.WriteLine(sb.ToString());
				sw.Close();
			}catch(Exception e)
			{
				Debugger.LogError(e.StackTrace);
			}


		}
    }
}