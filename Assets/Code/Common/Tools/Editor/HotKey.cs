using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Common.Util;
using Logic.Skill.Model;
namespace Common.Tools.Editor
{
    #region quick window
    [InitializeOnLoad]
    public class Appload
    {
        static Appload()
        {
            int isShow = PlayerPrefs.GetInt("ShowQuickWindow", 1);
            if (isShow == 1)
            {
                EditorApplication.update += Update;
            }
            bool linkMaterialAndTexture = PlayerPrefs.GetInt("linkMaterialAndTexture", 1) == 1;
            if (linkMaterialAndTexture)
                EditorApplication.update += LinkMaterialAndTexture;

            bool saveBorderInfo = PlayerPrefs.GetInt("saveBorderInfo", 0) == 1;
            if (saveBorderInfo)
                EditorApplication.update += SaveBorderInfo;

        }

        static void Update()
        {
            bool isSuccess = EditorApplication.ExecuteMenuItem("CEngine/快捷设置窗口");
            if (isSuccess) EditorApplication.update -= Update;
        }

        static void LinkMaterialAndTexture()
        {
            bool isSuccess = EditorApplication.ExecuteMenuItem("Tools/LinkMaterialWithTexture");
            if (isSuccess)
            {
                EditorApplication.update -= LinkMaterialAndTexture;
                Debugger.Log("贴图关联材质球成功！");
            }
        }

        static void SaveBorderInfo()
        {
            bool isSuccess = EditorApplication.ExecuteMenuItem("Tools/SaveBorderInfo");
            if (isSuccess)
            {
                EditorApplication.update -= SaveBorderInfo;
                Debugger.Log("保存图集border信息成功！");
            }
        }
    }

    public class QuickWindow : EditorWindow
    {
        [UnityEditor.MenuItem("CEngine/快捷设置窗口", false)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<QuickWindow>(true, "快捷设置窗口");
        }

        bool flag = false, linkMaterialAndTexture = false, saveBorderInfo = false;

        void OnGUI()
        {
            flag = PlayerPrefs.GetInt("ShowQuickWindow", 1) == 1;
            flag = EditorGUILayout.Toggle("开始时候显示对话框", flag);
            if (flag)
            {
                PlayerPrefs.SetInt("ShowQuickWindow", 1);
            }
            else
            {
                PlayerPrefs.SetInt("ShowQuickWindow", 0);
            }
            linkMaterialAndTexture = PlayerPrefs.GetInt("linkMaterialAndTexture", 1) == 1;
            linkMaterialAndTexture = EditorGUILayout.Toggle("贴图自动关联材质球", linkMaterialAndTexture);
            if (linkMaterialAndTexture)
            {
                PlayerPrefs.SetInt("linkMaterialAndTexture", 1);
            }
            else
            {
                PlayerPrefs.SetInt("linkMaterialAndTexture", 0);
            }

            saveBorderInfo = PlayerPrefs.GetInt("saveBorderInfo", 0) == 1;
            saveBorderInfo = EditorGUILayout.Toggle("保存图集border信息", saveBorderInfo);
            if (saveBorderInfo)
            {
                PlayerPrefs.SetInt("saveBorderInfo", 1);
            }
            else
            {
                PlayerPrefs.SetInt("saveBorderInfo", 0);
            }
        }
    }
    #endregion

    public static class HotKey
    {
        /// <summary>
        /// 场景对象快速显示隐藏 Alt+s
        /// </summary>
        [MenuItem("CEngine/QuickShowHide &s")]
        public static void QuickShowHide()
        {
            Transform[] trans = Selection.transforms;
            if (trans.Length == 0)
            {
                Debugger.LogError("u select nothing !!");
                return;
            }
            foreach (Transform father in trans)
            {
                father.gameObject.SetActive(!father.gameObject.activeSelf);
            }
        }


        static bool isScreenshot = false;
        /// <summary>
        /// 截图 Ctrl+Alt+Q
        /// </summary>
        [MenuItem("CEngine/Screenshot %&q")]
        public static void Screenshot()
        {
            //Debugger.LogError("Screenshot");
            if (isScreenshot) return;
            isScreenshot = true;
            Texture2D screenShot = null;
            try
            {
                screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                screenShot.Apply();

                DateTime date = DateTime.Now;
                string fileName = date.Year + "_" + date.Month + "_" + date.Day + "_" + date.Hour + "_" + date.Minute + "_" + date.Second + ".png";

                string dir = @"D:\Screenshot";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllBytes(dir + @"\" + fileName, screenShot.EncodeToPNG());

                Debugger.Log("save successed! file:" + dir + @"\" + fileName);
            }
            catch (Exception e)
            {
                Debugger.LogError("save fail!" + e.Message + " " + e.ToString());
            }
            finally
            {
                isScreenshot = false;
                GameObject.DestroyImmediate(screenShot);
            }
        }

        [MenuItem("CEngine/快速设置label为--")]
        static void ResetLabelText()
        {
            Transform[] selectObj = Selection.transforms;
            if (selectObj.Length == 0)
            {
                Debugger.Log("selected nothing!");
                return;
            }
            foreach (Transform trans in selectObj)
            {
                Text[] texts = trans.GetComponentsInChildren<Text>(true);
                foreach (Text t in texts)
                    t.text = "--";
            }
        }

        [MenuItem("CEngine/快速查找BoxCollider")]
        static void FindBoxCollider()
        {
            Transform[] selectObj = Selection.transforms;
            if (selectObj.Length == 0)
            {
                Debugger.Log("selected nothing!");
                return;
            }
            foreach (Transform trans in selectObj)
            {
                BoxCollider[] bcs = trans.GetComponentsInChildren<BoxCollider>(true);
                foreach (BoxCollider bc in bcs)
                    Debugger.Log(bc.name);
            }
        }

        [MenuItem("CEngine/TimeScaleSwitch")]
        public static void TimeScaleSwitch()
        {
            if (Application.isPlaying)
            {
                if (Time.timeScale != 0)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
        }

        [MenuItem("CEngine/GC")]
        public static void GC()
        {
            if (Application.isPlaying)
            {
                System.GC.Collect();
                Resources.UnloadUnusedAssets();
                LuaInterface.LuaScriptMgr.Instance.LuaGC();
            }
        }

        [MenuItem("Tools/LinkMaterialWithTexture", false, 300)]
        public static void LinkMatWithTexture()
        {
            DirectoryInfo modelDI = new DirectoryInfo("Assets/Res/Model");
            foreach (FileInfo fi in modelDI.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                if (fi.Name.Contains("p_face_") || fi.Name.Contains("p_hair_"))
                    continue;
                string matPath = fi.FullName;
                matPath = matPath.Substring(matPath.IndexOf(@"Assets\"));
                //Debugger.Log(matPath);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                string matName = fi.Name.Replace(".mat", string.Empty);
                DirectoryInfo tempDI = fi.Directory.Parent;
                Texture texture = null;
                foreach (var file in tempDI.GetFiles())
                {
                    //Debugger.LogError(file.Extension + "  " + file.Name + "   " + fi.Name);
                    if (file.Extension == ".png" || file.Extension == ".tga")
                    {
                        string textureName = file.Name.Replace(".png", string.Empty).Replace(".tga", string.Empty);
                        //Debugger.LogError(textureName+"   "+matName);
                        if (textureName.StartsWith(matName) && !textureName.Contains("_info") && !textureName.Contains("_wp"))
                        {
                            string texturePath = file.FullName;
                            texturePath = texturePath.Substring(file.FullName.IndexOf(@"Assets\"));
                            texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                            if (texture != null)
                            {
                                mat.mainTexture = texture;
                            }
                        }
                        if (textureName.Contains("_info"))
                        {
                            string texturePath = file.FullName;
                            texturePath = texturePath.Substring(file.FullName.IndexOf(@"Assets\"));
                            texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                            if (texture != null)
                            {
                                mat.SetTexture("_InfoTex", texture);
                            }
                        }
                        if (textureName.Contains("_wp") && mat.name.Contains("_wp"))
                        {
                            string texturePath = file.FullName;
                            texturePath = texturePath.Substring(file.FullName.IndexOf(@"Assets\"));
                            texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                            if (texture != null)
                            {
                                mat.mainTexture = texture;
                            }
                        }
                    }
                }
                Texture reflTexture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Res/Model/reflection.png");
                if (reflTexture)
                    mat.SetTexture("_GlossTex", reflTexture);
            }
        }

        [MenuItem("Tools/查找未设置Font的Text路径")]
        public static void CheckNotSetFontText()
        {
            Debugger.LogError("[Cehcking Not Set Font Text:Start]");
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                List<Text> notSetFontTextList = new List<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].font == null)
                    {
                        notSetFontTextList.Add(texts[i]);
                    }
                }

                if (notSetFontTextList.Count > 0)
                {
                    Debugger.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
                    Transform parent = null;
                    for (int i = 0; i < notSetFontTextList.Count; i++)
                    {
                        string textPath = string.Empty;
                        parent = notSetFontTextList[i].transform.parent;
                        while (parent != null)
                        {
                            textPath = parent.name + "/" + textPath;
                            parent = parent.parent;
                        }
                        textPath += notSetFontTextList[i].name + " not set font!";
                        Debugger.LogWarning(textPath);
                    }
                }
            }
            Debugger.LogError("[Cehcking Not Set Font Text:End]");
        }

        [MenuItem("Tools/查找使用了动态字体的Text路径")]
        public static void CheckUseDynamicFontText()
        {
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                List<Text> useDynamicFontTextList = new List<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].font != null && texts[i].font.dynamic)
                    {
                        useDynamicFontTextList.Add(texts[i]);
                    }
                }

                if (useDynamicFontTextList.Count > 0)
                {
                    Debugger.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
                    Transform parent = null;
                    for (int i = 0; i < useDynamicFontTextList.Count; i++)
                    {
                        string textPath = string.Empty;
                        parent = useDynamicFontTextList[i].transform.parent;
                        while (parent != null)
                        {
                            textPath = parent.name + "/" + textPath;
                            parent = parent.parent;
                        }
                        textPath += useDynamicFontTextList[i].name + " <====================> [" + useDynamicFontTextList[i].font.name + "]";
                        Debugger.LogWarning(textPath);
                    }
                }
            }
        }

        [MenuItem("Tools/查找使用了FZZDH粗体字体的Text路径")]
        public static void CheckUseBoldFontText()
        {
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                List<Text> useFZZDHBoldFontFontTextList = new List<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].font != null && texts[i].font.name == "FZZDH")
                    {
                        useFZZDHBoldFontFontTextList.Add(texts[i]);
                    }
                }

                if (useFZZDHBoldFontFontTextList.Count > 0)
                {
                    Debugger.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
                    Transform parent = null;
                    for (int i = 0; i < useFZZDHBoldFontFontTextList.Count; i++)
                    {
                        string textPath = string.Empty;
                        parent = useFZZDHBoldFontFontTextList[i].transform.parent;
                        while (parent != null)
                        {
                            textPath = parent.name + "/" + textPath;
                            parent = parent.parent;
                        }
                        textPath += useFZZDHBoldFontFontTextList[i].name + " <====================> [" + useFZZDHBoldFontFontTextList[i].font.name + "]";
                        Debugger.LogWarning(textPath);
                    }
                }
            }
        }

        [MenuItem("Tools/将所有使用动态FZZDH粗体字体的Text的字体替换为FZY3JW字体")]
        public static void ReplaceFZZDHWithStaticFZY3JW()
        {
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                List<Text> useFZZDHBoldFontFontTextList = new List<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].font != null && texts[i].font.name == "FZZDH")
                    {
                        useFZZDHBoldFontFontTextList.Add(texts[i]);
                    }
                }

                if (useFZZDHBoldFontFontTextList.Count > 0)
                {
                    Debugger.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
                    Transform parent = null;
                    for (int i = 0; i < useFZZDHBoldFontFontTextList.Count; i++)
                    {
                        string textPath = string.Empty;
                        parent = useFZZDHBoldFontFontTextList[i].transform.parent;
                        while (parent != null)
                        {
                            textPath = parent.name + "/" + textPath;
                            parent = parent.parent;
                        }
                        textPath += useFZZDHBoldFontFontTextList[i].name + " <====================> [" + useFZZDHBoldFontFontTextList[i].font.name + "]";

                        int fontSize = useFZZDHBoldFontFontTextList[i].fontSize;
                        Font fzy3jwFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Resources/fonts/FZY3JW.ttf");
                        useFZZDHBoldFontFontTextList[i].font = fzy3jwFont;
                        useFZZDHBoldFontFontTextList[i].fontSize = fontSize;
                        useFZZDHBoldFontFontTextList[i].fontStyle = FontStyle.Normal;
                        Debugger.LogWarning(textPath);
                    }
                }
                EditorUtility.SetDirty(gameObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Tools/将所有使用动态Arial的Text的字体替换为静态Arial")]
        public static void ReplaceDynamicArialWithStaticArial()
        {
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                List<Text> useDynamicArialFontTextList = new List<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].font != null && texts[i].font.dynamic && texts[i].font.name == "Arial")
                    {
                        useDynamicArialFontTextList.Add(texts[i]);
                    }
                }

                if (useDynamicArialFontTextList.Count > 0)
                {
                    Debugger.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
                    Transform parent = null;
                    for (int i = 0; i < useDynamicArialFontTextList.Count; i++)
                    {
                        string textPath = string.Empty;
                        parent = useDynamicArialFontTextList[i].transform.parent;
                        while (parent != null)
                        {
                            textPath = parent.name + "/" + textPath;
                            parent = parent.parent;
                        }
                        textPath += useDynamicArialFontTextList[i].name + " <====================> [" + useDynamicArialFontTextList[i].font.name + "]";

                        int fontSize = useDynamicArialFontTextList[i].fontSize;
                        FontStyle fontStyle = useDynamicArialFontTextList[i].fontStyle;
                        Font staticArialFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Resources/fonts/Arial.ttf");
                        Font staticArialItalicFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Resources/fonts/Arial_Italic.ttf");
                        if (fontStyle == FontStyle.Normal)
                        {
                            useDynamicArialFontTextList[i].font = staticArialFont;
                        }
                        else if (fontStyle == FontStyle.Italic)
                        {
                            useDynamicArialFontTextList[i].font = staticArialItalicFont;
                        }
                        useDynamicArialFontTextList[i].fontSize = fontSize;
                        useDynamicArialFontTextList[i].fontStyle = FontStyle.Normal;
                        Debugger.LogWarning(textPath);
                    }
                }
                EditorUtility.SetDirty(gameObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Tools/检测MainUI中或许未使用的Sprite并输出至Console")]
        public static void CheckUnusedSpritesInMainUI()
        {
            Debugger.LogError("========== Check Unused sprite in main ui ==========");
            Dictionary<string, int> unusedMainUISpriteNamesDic = new Dictionary<string, int>();
            TextAsset mainUIAtlasInfoTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Res/Atlas/main_ui.txt");
            string[] mainUISpriteInfoStrings = mainUIAtlasInfoTextAsset.text.Split('\n');
            for (int i = 0, count = mainUISpriteInfoStrings.Length; i < count; i++)
            {
                unusedMainUISpriteNamesDic.TryAdd(mainUISpriteInfoStrings[i].Split(';')[0], 0);
            }

            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                List<Image> images = new List<Image>();
                gameObject.GetComponentsInChildren<Image>(true, images);
                for (int i = 0, count = images.Count; i < count; i++)
                {
                    if (images[i].sprite != null
                        && images[i].sprite.texture.name.Equals("main_ui")
                        && unusedMainUISpriteNamesDic.ContainsKey(images[i].sprite.name))
                    {
                        unusedMainUISpriteNamesDic.Remove(images[i].sprite.name);
                    }
                }
            }

            foreach (string spriteName in unusedMainUISpriteNamesDic.Keys)
            {
                Debugger.LogWarning(spriteName);
            }
            Debugger.LogError("========== Check Unused sprite in main ui ==========");
        }

        //[MenuItem("Tools/SwitchIP", false, 100)]
        //public static void SwitchIP()
        //{
        //    bool isMainServer = Logic.Game.GameConfig.instance.innerGameServerHost.Equals("192.168.3.114");
        //    Debugger.Log(Logic.Game.GameConfig.instance.innerGameServerHost);
        //    isMainServer = !isMainServer;
        //    Logic.Game.GameConfig.instance.innerGameServerHost = isMainServer ? "192.168.3.114" : "192.168.3.201";
        //    Logic.Game.GameConfig.instance.innerGameServerPort = isMainServer ? 80 : 8080;
        //    Debugger.Log("switch ip to " + Logic.Game.GameConfig.instance.innerGameServerHost);
        //    Logic.Protocol.ProtocolProxy.instance.Connect(Logic.Game.GameConfig.instance.innerGameServerHost, Logic.Game.GameConfig.instance.innerGameServerPort);
        //    AssetDatabase.SaveAssets();
        //}

        //[MenuItem("Tools/LuaToTxt", false, 100)]
        public static void LuaToTxt()
        {
            DirectoryInfo originalDI = new DirectoryInfo("Assets/ToLua/Lua");
            //if (Directory.Exists(@"Assets\ToLua\Resources\Lua\user"))
            //    Directory.Delete(@"Assets\ToLua\Resources\Lua\user", true);
            if (Directory.Exists(@"Assets\ToLua\Resources\Lua"))
                Directory.Delete(@"Assets\ToLua\Resources\Lua", true);
            Directory.CreateDirectory(@"Assets\ToLua\Resources\Lua");
            foreach (FileInfo fi in originalDI.GetFiles("*.lua", SearchOption.AllDirectories))
            {
                //if (fi.FullName.Contains(@"Lua\cjso") || fi.FullName.Contains(@"Lua\math") || fi.FullName.Contains(@"Lua\misc") || fi.FullName.Contains(@"Lua\protobuf") ||
                //    fi.FullName.Contains(@"Lua\socket") || fi.FullName.Contains(@"Lua\system") || fi.FullName.Contains(@"Lua\u3d") || fi.Name.Contains("tolua"))
                //    continue;
                string name = fi.FullName;
                name = name.Replace(@"Assets\ToLua\Lua", @"Assets\ToLua\Resources\Lua");
                string fileDir = name.Substring(0, name.IndexOf(".") + 1);
                fileDir = fileDir.Substring(0, fileDir.LastIndexOf("\\"));
                //Debugger.Log(fileDir);
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                fi.CopyTo(name, true);
            }
            string dirName = @"Assets\ToLua\Resources\Lua\";
            DirectoryInfo newDI = new DirectoryInfo(dirName);
            StringBuilder sb = new StringBuilder();
            foreach (FileInfo fi in newDI.GetFiles("*.lua", SearchOption.AllDirectories))
            {
                string path = fi.FullName;
                string[] contents = File.ReadAllLines(path, Encoding.UTF8);
                if (sb.Length > 0)
                    sb.Remove(0, sb.Length);
                foreach (string s in contents)
                {
                    string content = s;
                    //if (StringUtil.ContainsChinese(content))
                    //{
                    //    foreach (var c in content)
                    //    {
                    //        if (StringUtil.ContainsChinese(c.ToString()))
                    //            content = content.Replace(c.ToString(), "");
                    //    }
                    //}
                    sb.AppendLine(content);
                }
                //string content = File.ReadAllText(path, Encoding.UTF8);
                string targetPath = path.Replace(".lua", ".txt");
                if (File.Exists(targetPath))
                    File.Delete(targetPath);
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                //File.WriteAllText(targetPath, content, Encoding.UTF8);
                File.WriteAllBytes(targetPath, bytes);
                //fi.CopyTo(targetPath, true);
                //Debugger.Log(fi.FullName);
                //string value = fi.FullName.Substring(fi.FullName.IndexOf(dirName) + dirName.Length);
                //files.Add(key.Replace(@"\", @".").Replace(".lua", string.Empty), key);
                //string key = value.Replace(@"\", @"/");
                fi.Delete();
            }
            GenerateLuaCSV();
            AssetDatabase.Refresh();
            Debugger.Log("copy finished");
        }

        [MenuItem("Tools/Generate Lua CSV", false, 100)]
        public static void GenerateLuaCSV()
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            DirectoryInfo dir = new DirectoryInfo("Assets/ToLua/Lua");
            string dirName = @"Assets\ToLua\Lua\";
            foreach (FileInfo fi in dir.GetFiles("*.lua", SearchOption.AllDirectories))
            {
                //string name = fi.FullName;
                string value = fi.FullName.Substring(fi.FullName.IndexOf(dirName) + dirName.Length);
                string key = value.Replace(@"\", @"/");
                files.Add(key.Replace(".lua", string.Empty), key);
            }
            string path = Application.dataPath + "/Res/Resources/config/lua.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("name,");
            sb.Append("path");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            foreach (var kvp in files)
            {
                sb.Append(kvp.Key + ",");
                //Debugger.Log(kvp.Value);
                sb.Append(kvp.Value);
                sb.Append(CSVUtil.SYMBOL_LINE[0]);
            }
            sw.WriteLine(sb.ToString());
            #endregion
            sw.Close();
            fs.Close();
            files.Clear();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/ReLoadLua", false, 100)]
        public static void ReLoadLua()
        {
            if (Logic.Game.Controller.GameController.instance)
                Logic.Game.Controller.GameController.instance.InitLua();
        }

        [MenuItem("Tools/Find Extensions", false, 100)]
        private static void FindExtensions()
        {
            List<string> extensions = new List<string>();
            ////extensions.Add("*.gif");
            ////extensions.Add("*.jpeg");
            //extensions.Add("*.psd");
            //extensions.Add("*.mingw");
            //extensions.Add("*.html");
            //extensions.Add("*.xml");
            List<string> result = new List<string>();
            List<string> filePaths = new List<string>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            if (extensions.Count > 0)
            {
                foreach (var ex in extensions)
                {
                    foreach (FileInfo f in di.GetFiles(ex, SearchOption.AllDirectories))
                    {
                        string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                        string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                        filePaths.Add(assetsPath);
                    }
                    Debugger.Log("{0}  Extension:-------------------------------------------------------------------", ex);
                    foreach (var path in filePaths)
                    {
                        Debugger.Log(path);
                    }
                    filePaths.Clear();
                }
            }
            else
            {
                foreach (FileInfo f in di.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (!result.Contains(f.Extension))
                        result.Add(f.Extension);
                }
                Debugger.Log("All Extension:-------------------------------------------------------------------");
                foreach (var path in result)
                {
                    Debugger.Log(path);
                }
                result.Clear();
            }
        }

        [MenuItem("CEngine/快速胜利 &y")]
        static void ForceFinshFight()
        {
            if (Application.isPlaying)
            {
                if (Logic.Fight.Controller.FightController.instance.fightStatus != Logic.Enums.FightStatus.GameOver)
                    Logic.Net.Controller.DataMessageHandler.DataMessage_ForceFightFinished(true, Logic.Enums.FightOverType.ForceOver);
            }
        }

        [MenuItem("CEngine/查找中文文件")]
        static void FindChineseFile()
        {
            List<string> filePaths = new List<string>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (StringUtil.ContainsChinese(f.FullName))
                {
                    string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                    filePaths.Add(assetsPath);
                }
            }
            foreach (var path in filePaths)
            {
                Debugger.Log(path);
            }
            filePaths.Clear();
        }

        [MenuItem("Tools/Replace All of font in ui prefab", false, 300)]
        public static void ReplaceAllSprites()
        {
            //string uiPath = "Assets/Res/Resources/ui";
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Resources/ui"))
                    resPaths.Add(path);
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a ui prefab or a contain ui prefab of folder !");
                return;
            }
            Font font = Resources.Load<Font>("fonts/FZY3JW");
            Debugger.Log(font.name);
            foreach (string assetPath in resPaths)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) as GameObject;
                if (!go) continue;
                Text[] texts = go.transform.GetComponentsInChildren<Text>(true);
                foreach (var txt in texts)
                {
                    if (txt.font.name == "Arial")
                        txt.font = font;
                }
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Tools/查找命名包含空格", false, 300)]
        public static void FindWhiteSpaceCharacterInName()
        {
            FindWhiteSpaceCharacterCountInName();
        }

        public static int FindWhiteSpaceCharacterCountInName()
        {
            string path = Application.dataPath;
            DirectoryInfo dir = new DirectoryInfo(path);
            int i = 0;
            foreach (var f in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string name = f.Name;
                if (name.EndsWith(".meta"))
                    continue;
                if (StringUtil.ContainsWhiteSpace(name))
                {
                    string assetPath = f.FullName.Replace(path, "Assets/");
                    Debugger.Log(assetPath);
                    i++;
                }
            }
            Debugger.Log("find space character in name total count:{0}", i);
            return i;
        }



        [MenuItem("Tools/Del Empty Dir", false, 300)]
        public static void DelEmptyDir()
        {
            string path = Application.dataPath;
            DelEmptyDir(new DirectoryInfo(path));
            AssetDatabase.Refresh();
        }

        private static void DelEmptyDir(DirectoryInfo dir)
        {
            foreach (var d in dir.GetDirectories())
            {
                DelEmptyDir(d);
            }
            if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
            {
                dir.Delete();
                Debugger.Log("del empty dir {0}", dir.FullName);
            }
        }

        [MenuItem("Tools/Enable所有UI Prefab上的Canvas Adapter脚本")]
        public static void EnableAllCanvasAdapter()
        {
            Debugger.LogError("==================== [EnableAllCanvasAdapter] ====================");
            DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
            foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Common.Canvases.CanvasAdapter[] canvasAdapter = gameObject.GetComponentsInChildren<Common.Canvases.CanvasAdapter>(true);
                foreach (Common.Canvases.CanvasAdapter c in canvasAdapter)
                {
                    if (!c.enabled)
                    {
                        c.enabled = true;
                        Debugger.LogError(prefabPath);
                    }
                    else
                    {
                        Debugger.LogWarning(prefabPath);
                    }
                }
                EditorUtility.SetDirty(gameObject);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debugger.LogError("==================== [EnableAllCanvasAdapter] ====================");
        }

        [MenuItem("CEngine/ClearLocalCache", false, 300)]
        public static void ClearLocalCache()
        {
            PlayerPrefs.DeleteKey("speedMode");
            PlayerPrefs.DeleteKey("autoFight");
        }


        [MenuItem("CEngine/ClearAllLocalCache", false, 300)]
        public static void ClearAllLocalCache()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("CEngine/CheckSkillTable", false, 300)]
        public static void CheckSkillTable()
        {
            StringBuilder sb = new StringBuilder();
            List<uint> list = new List<uint>();
            Dictionary<uint, SkillData> skillDatas = SkillData.GetSkillDatas();
            foreach (var kvp in skillDatas)
            {
                SkillData skillData = kvp.Value;
                AnimationData animationData = AnimationData.GetAnimationDataById(kvp.Key);
                if (skillData.timeline.Count > 0)
                {
                    if (skillData.timeline.Last().Key > animationData.length)
                    {
                        list.Add(kvp.Key);
                    }
                }
            }

            if (list.Count > 0)
            {
                sb.AppendLine("技能效果时间超过技能长度：");
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    sb.AppendLine(list[i].ToString());
                }
            }
            else
            {
                sb.AppendLine("无");
            }
            Debugger.Log(sb.ToString());
        }

        [MenuItem("CEngine/ReloadScene", false, 300)]
        public static void ReloadScene()
        {
            Logic.Game.Controller.GameController.instance.ReLoadMainScene();
        }

        [MenuItem("CEngine/TestEncrypt", false, 300)]
        public static void TestEncrypt()
        {
            string str = "test";
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(str);
            string key = "12345678";
            byte[] results = null;
            byte[] dedata = null;
            string destr = string.Empty;
            /*Debugger.Log("-------------------------DES-----------------------------");
            results = EncryptUtil.DESEncryptBytes(data, key);
            Debugger.Log("results length:" + results.Length);
            Debugger.Log("results:" + results.ToCustomString());
            //StringBuilder sb = new StringBuilder();
            //foreach (byte b in results)
            //{
            //    sb.AppendFormat("{0:X2}", b);
            //}
            //Debugger.Log("result:" + sb.ToString());
            //string result = Encoding.Unicode.GetString(results);
            //Debugger.Log("result:" + result);
            dedata = EncryptUtil.DESDecryptBytes(results, key);
            destr = Encoding.UTF8.GetString(dedata);
            Debugger.Log("destr:" + destr);
            */
            /*Debugger.Log("-------------------------AES-----------------------------");
            key = "1234567812345678";
            results = EncryptUtil.AESEncryptBytes(data, key);
            Debugger.Log("results length:" + results.Length);
            Debugger.Log("results:" + results.ToCustomString());
            StringBuilder sb = new StringBuilder();
            foreach (byte b in results)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            Debugger.Log("result:" + sb.ToString());
            //result = Encoding.Unicode.GetString(results);
            //Debugger.Log("result:" + result);
            dedata = EncryptUtil.AESDecryptBytes(results, key);
            destr = Encoding.UTF8.GetString(dedata);
            Debugger.Log("destr:" + destr);
            */
            Debugger.Log("-------------------------blow fish-----------------------------");
            key = "12345678";
            results = EncryptUtil.BlowFishEncryptBytes(data, key);
            Debugger.Log("results length:" + results.Length);
            Debugger.Log("results:" + results.ToCustomString());
            Debugger.LogError(Convert.ToBase64String(results));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in results)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            Debugger.Log("result:" + sb.ToString());
            //result = Encoding.Unicode.GetString(results);
            //Debugger.Log("result:" + result);
            dedata = EncryptUtil.BlowFishDecryptBytes(results, key);
            destr = Encoding.UTF8.GetString(dedata);
            Debugger.Log("destr:" + destr);

            Debugger.Log("-------------------------Triple DES ECB-----------------------------");
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            key = "12345678123456781234567812345678";
            byte[] keyByte = Convert.FromBase64String(key);
            results = EncryptUtil.TripleDesECBEncryptBytes(data, iv, keyByte);
            Debugger.Log("results length:" + results.Length);
            Debugger.Log("results:" + results.ToCustomString());
            //result = Encoding.Unicode.GetString(results);
            //Debugger.Log("result:" + result);
            dedata = EncryptUtil.TripleDesECBDecryptBytes(results, iv, keyByte);
            destr = Encoding.UTF8.GetString(dedata);
            Debugger.Log("destr:" + destr);
        }

        [MenuItem("CEngine/MD5", false, 300)]
        public static void MD5()
        {
            string key = "1c7d2ad19e9176c555b6149f3c8f1f0f#678b227765944e3413de1e017a5d5c2f";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(key);
            string result = EncryptUtil.Bytes2MD5(bytes);
            Debugger.Log(result.Length.ToString());
            Debugger.Log(result.ToLower());
        }
    }
}