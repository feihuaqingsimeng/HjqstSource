using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Common.ResMgr;
using Common.Util;
using System.Text;
namespace Common.Tools.Editor
{
    [ExecuteInEditMode]
    public class PlayerBuilder : UnityEditor.EditorWindow
    {
        [MenuItem("Tools/发布打包", false, 200)]
        public static void OpenAssetBundlesBuilder()
        {
            EditorWindow.GetWindow<PlayerBuilder>();
            csvVersion = ProjectBuilder.GetLastestCSVVersion();
            luaVersion = ProjectBuilder.GetLastestLuaVersion();
            lastVersion = ProjectBuilder.GetLastestResVersion();
            csvVersions = ProjectBuilder.GetCSVVersions();
            luaVersions = ProjectBuilder.GetLuaVersions();
            version = lastVersion;
            platformTypes.Clear();
            platformTypeFlags.Clear();
            platformTypeArray = System.Enum.GetValues(typeof(PlatformType)) as PlatformType[];
            foreach (PlatformType p in platformTypeArray)
            {
                if (p == PlatformType.None) continue;
                platformTypeFlags.Add(false);
            }
            csvIndex = 0;
            luaIndex = 0;
            newCSVVersion = newLuaVersion = false;
            csvGitMessage = luaGitMessage = string.Empty;
        }
        private bool android = true, ios = true, pc = false, iosPlayer = false, androidPlayer = true, delFiles = false, androidProject = true, luaEncrypt = true;
        private static string version, lastVersion;
        private static string csvVersion, luaVersion;
        private static PlatformType[] platformTypeArray;
        private static List<PlatformType> platformTypes = new List<PlatformType>();
        private static List<bool> platformTypeFlags = new List<bool>();
        private static string[] luaVersions, csvVersions;
        private static int csvIndex, luaIndex;
        private static bool newCSVVersion, newLuaVersion;
        private static string csvGitMessage, luaGitMessage;
        void OnGUI()
        {
            #region 打包资源
            version = EditorGUILayout.TextField("资源版本号：", version, GUILayout.Width(200));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            android = EditorGUILayout.Toggle("安卓:", android);
            ios = EditorGUILayout.Toggle("IOS:", ios);
            pc = EditorGUILayout.Toggle("pc:", pc);
            if (GUILayout.Button("打包资源"))
            {
                if (lastVersion == version)
                {
                    Debugger.Log("版本号相同!");
                }
                if (HotKey.FindWhiteSpaceCharacterCountInName() > 0)
                {
                    Debugger.LogError("有空格名字！");
                    return;
                }
                ProjectBuilder.ProjectGitPull();
                if (android)
                    ProjectBuilder.BuildAssetBundleForAndroid(version);
                if (ios)
                    ProjectBuilder.BuildAssetBundleForIOS(version);
                if (pc)
                    ProjectBuilder.BuildAssetBundleForPC(version);
                this.Close();
            }
            #endregion

            #region 发布
            androidPlayer = EditorGUILayout.Toggle("安卓:", androidPlayer);
            iosPlayer = EditorGUILayout.Toggle("IOS:", iosPlayer);
            delFiles = EditorGUILayout.Toggle("是否删除资源文件:", delFiles);
            if (androidPlayer)
            {
                androidProject = EditorGUILayout.Toggle("发布安卓工程:", androidProject);
                EditorGUILayout.Space();
                VersionUtil.IsDebug = EditorGUILayout.Toggle("不提交Git", VersionUtil.IsDebug);
                EditorGUILayout.Space();
                //platformType = (PlatformType)EditorGUILayout.EnumPopup("发布平台", platformType, GUILayout.Width(300));
                if (androidProject)
                {
                    int i = 0;
                    foreach (PlatformType p in platformTypeArray)
                    {
                        if (p == PlatformType.None) continue;
                        platformTypeFlags[i] = EditorGUILayout.Toggle(p.ToString() + ":", platformTypeFlags[i]);
                        if (platformTypeFlags[i])
                        {
                            if (!platformTypes.Contains(p))
                                platformTypes.Add(p);
                        }
                        else
                        {
                            if (platformTypes.Contains(p))
                                platformTypes.Remove(p);
                        }
                        i++;
                    }
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("发布"))
            {
                //VersionUtil.IsDebug = false;
                if (androidPlayer)
                {
                    if (androidProject)
                    {
                        foreach (var p in platformTypes)
                        {
                            ProjectBuilder.BuildAndroid(delFiles, androidProject, p);
                        }
                    }
                    else
                        ProjectBuilder.BuildAndroid(delFiles, false);
                   
                }
                if (iosPlayer)
                    ProjectBuilder.BuildIOS(delFiles);
                this.Close();
            }
            if (GUILayout.Button("debug打包（不git操作）", GUILayout.Width(256)))
            {
                VersionUtil.IsDebug = true;
                if (androidPlayer)
                {
                    if (androidProject)
                    {
                        foreach (var p in platformTypes)
                        {
                            ProjectBuilder.BuildAndroid(delFiles, androidProject, p);
                        }
                    }
                    else
                        ProjectBuilder.BuildAndroid(delFiles, false);
                }
                if (iosPlayer)
                    ProjectBuilder.BuildIOS(delFiles);
                this.Close();
            }
            GUILayout.EndHorizontal();
            //if (androidPlayer)
            //{
            //    if ((GUILayout.Button("CopyCSV&Lua2AndroidAssets")))
            //    {
            //        if (androidProject)
            //        {
            //            foreach (var p in platformTypes)
            //            {
            //                ProjectBuilder.Copy2AndroidAssets(p);
            //            }
            //            VersionUtil.AddGit(ProjectBuilder.androidProjectPath);
            //            VersionUtil.CommitGit(ProjectBuilder.androidProjectPath, "CopyCSV&Lua2AndroidAssets");
            //            VersionUtil.PullGit(ProjectBuilder.androidProjectPath);
            //            VersionUtil.PushGit(ProjectBuilder.androidProjectPath);
            //            Debugger.Log("git:push to git server success!");
            //            this.Close();
            //        }
            //    }
            //}

			string copyZipFileButtonName = androidPlayer ? "CopyCSV&LuaZip2StreamingAssets[Android]" : "CopyCSV&LuaZip2StreamingAssets[iOS]";

			if ((GUILayout.Button(copyZipFileButtonName)))
            {
                ProjectBuilder.Copy2StreamingAssetsPath(androidPlayer);
                Debugger.Log("CopyCSV&LuaZip2StreamingAssets success!");
                this.Close();
            }

            #endregion

            #region 生成配置文件
            EditorGUILayout.Space();
            if (GUILayout.Button("pull res"))
            {
                ProjectBuilder.ResGitPull();
            }
            EditorGUILayout.LabelField("生成配置文件的MD5信息，并保存到文件");
            EditorGUILayout.BeginHorizontal();
            csvIndex = EditorGUILayout.Popup("csv版本列表：", csvIndex, csvVersions, GUILayout.Width(300));
            if (GUILayout.Button("new", GUILayout.Width(200)))
            {
                newCSVVersion = true;
                string v = csvVersions[0];
                string[] subVersion = v.ToArray('.');
                int numVersion = int.Parse(subVersion[2]);
                numVersion++;
                csvVersion = subVersion[0] + "." + subVersion[1] + "." + numVersion;
            }
            EditorGUILayout.EndHorizontal();
            if (!newCSVVersion)
                csvVersion = csvVersions[csvIndex];
            csvVersion = EditorGUILayout.TextField("配置文件版本号：", csvVersion, GUILayout.Width(200));
            csvGitMessage = EditorGUILayout.TextField("git message(allow empty)：", csvGitMessage);
            EditorGUILayout.Space();
            VersionUtil.IsDebug = EditorGUILayout.Toggle("不提交Git", VersionUtil.IsDebug);
            EditorGUILayout.Space();
            if (GUILayout.Button("生成配置文件MD5文件"))
            {
                ProjectBuilder.BuildCSV(csvVersion, csvGitMessage);
                this.Close();
            }
            #endregion

            #region 生成Lua文件
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("生成Lua文件的MD5信息，并保存到文件");
            EditorGUILayout.BeginHorizontal();
            luaIndex = EditorGUILayout.Popup("lua版本列表：", luaIndex, luaVersions, GUILayout.Width(300));
            if (GUILayout.Button("new", GUILayout.Width(200)))
            {
                newLuaVersion = true;
                string v = luaVersions[0];
                string[] subVersion = v.ToArray('.');
                int numVersion = int.Parse(subVersion[2]);
                numVersion++;
                luaVersion = subVersion[0] + "." + subVersion[1] + "." + numVersion;
            }
            EditorGUILayout.EndHorizontal();
            if (!newLuaVersion)
                luaVersion = luaVersions[luaIndex];
            luaVersion = EditorGUILayout.TextField("lua版本号：", luaVersion, GUILayout.Width(200));
            luaGitMessage = EditorGUILayout.TextField("git message(allow empty)：", luaGitMessage);
            luaEncrypt = EditorGUILayout.Toggle("lua加密:", luaEncrypt);
            EditorGUILayout.Space();
            VersionUtil.IsDebug = EditorGUILayout.Toggle("不提交Git", VersionUtil.IsDebug);
            EditorGUILayout.Space();
            if (GUILayout.Button("生成Lua文件MD5文件"))
            {
                ProjectBuilder.BuildLua(luaVersion, luaGitMessage, luaEncrypt);
                this.Close();
            }
            #endregion
        }
        void OnDestroy()
        {
            if (VersionUtil.IsDebug)
                VersionUtil.IsDebug = false;
            Debug.Log("关闭" + this);
        }
    }

}