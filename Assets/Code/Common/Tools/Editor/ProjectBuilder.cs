using UnityEngine;
using System.IO;
using Common.ResMgr;
using UnityEditor;
using System.Text;
using Common.Util;
using System.Collections.Generic;
using Logic.Game;
using System.Diagnostics;
using UnityEditor.Animations;

public enum PlatformType
{
    None = 0,
    ShunWang = 1,
    // Qihoo360 = 2,
    // VIVO = 3,
    // UC = 4,
    //  OPPO = 5,
    //  Huawei = 6,
    //  Tencent = 7,
    //  Xiaomi = 8,
    shunwan = 2,
    OnlyCSharpCode = 99,
    Pure = 100,
    U8 = 101,
    
}

public class ProjectBuilder
{
    static string root;
    static string projectPath;
    static string playerPath;
    static string resGitPath;
    static string csvMD5Path;
    static string luaMD5Path;
    static string abPath;
    public static string androidProjectPath;
    public static string streamingAssetsPath;
    private static ManifestInfo manifest = null;
    static ProjectBuilder()
    {
        string datapath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;
        //倒数第二个/ 为root
        int i = datapath.LastIndexOf('/');
        if (i > 0)
        {
            datapath = datapath.Substring(0,i);
            int j = datapath.LastIndexOf('/');
            if (j > 0)
            {
                root = datapath.Substring(0,j);
            }
        }
        else
        {
            root = datapath.Replace("game-client/Assets", string.Empty);
        }
        projectPath = root + "/game-client";
        playerPath = root + "/player/";
        resGitPath = root + "/game-client-res/";
        csvMD5Path = "config/csv/config.md5";
        luaMD5Path = "lua/lua.md5";
        abPath = resGitPath + "ABs/";
        androidProjectPath = root + "/game-client-sdk/";
    }

    public static void ProjectGitPull()
    {
        //VersionUtil.ResetGit(projectPath);
        VersionUtil.PullGit(projectPath);
    }

    public static void ResGitPull()
    {
        //VersionUtil.ResetGit(projectPath);
        VersionUtil.PullGit(resGitPath);
    }

    private static int SortVersions(string x, string y)
    {
        int[] xs = x.ToArray<int>('.');
        int[] ys = y.ToArray<int>('.');
        if (xs[0] > ys[0]) return -1;
        if (xs[0] < ys[0]) return 1;
        if (xs[1] > ys[1]) return -1;
        if (xs[1] < ys[1]) return 1;
        if (xs[2] > ys[2]) return -1;
        if (xs[2] < ys[2]) return 1;
        return 0;
    }

    #region build assetbundle

    [MenuItem("Assets/Clear AB Tags", false, 1)]
    [MenuItem("Tools/Clear AB Tags", false, 200)]
    static void ClearABTags()
    {
        Object[] objs = Selection.objects;
        if (objs == null || objs.Length == 0)
        {
            Debugger.Log("please select a object");
            return;
        }
        List<string> paths = new List<string>();
        for (int i = 0, length = objs.Length; i < length; i++)
        {
            System.Type type = objs[i].GetType();
            string path = AssetDatabase.GetAssetPath(objs[i]);
            if (FilterType(type))
            {
                paths.Add(path);
            }
            else if (type == typeof(DefaultAsset))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    string p = "Assets" + fullName.Substring(Application.dataPath.Length);
                    Object obj = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                    if (obj != null && FilterType(obj.GetType()))
                        paths.Add(p);
                }
            }
        }
        for (int i = 0, length = paths.Count; i < length; i++)
        {
            string p = paths[i];
            AssetImporter assetImport = AssetImporter.GetAtPath(p);
            if (assetImport)
                assetImport.assetBundleName = string.Empty;
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debugger.Log("Clear AB Tags finish!");
    }


    [MenuItem("Assets/Set AB Tags", false, 1)]
    [MenuItem("Tools/Set AB Tags", false, 200)]
    static void SetABTags()
    {
        Object[] objs = Selection.objects;
        if (objs == null || objs.Length == 0)
        {
            Debugger.Log("please select a object");
            return;
        }
        List<string> paths = new List<string>();
        for (int i = 0, length = objs.Length; i < length; i++)
        {
            System.Type type = objs[i].GetType();
            Debugger.Log(type.ToString());
            string path = AssetDatabase.GetAssetPath(objs[i]);
            if (FilterType(type))
            {
                paths.Add(path);
            }
            else if (type == typeof(DefaultAsset))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    string p = "Assets" + fullName.Substring(Application.dataPath.Length);
                    Object obj = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                    if (obj != null && FilterType(obj.GetType()))
                        paths.Add(p);
                }
            }
        }
        for (int i = 0, length = paths.Count; i < length; i++)
        {
            string p = paths[i];
            Debugger.Log("final path:" + p);
            AssetImporter assetImport = AssetImporter.GetAtPath(p);
            if (assetImport)
            {
                string extension = p.Substring(p.LastIndexOf("."));
                //Debugger.LogError(extension);
                string assetBundleName = p.Replace(@"Assets/Res/", string.Empty);
                assetBundleName = assetBundleName.Replace(extension, string.Empty);
                assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                assetImport.assetBundleName = assetBundleName;
                Debugger.Log("assetBundleName:" + assetBundleName);
                assetImport.assetBundleName = assetBundleName;
                assetImport.assetBundleVariant = ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static bool FilterType(System.Type type)
    {
        return type == typeof(GameObject) || type == typeof(Texture2D) || type == typeof(AnimationClip) || type == typeof(AudioClip) || type == typeof(Material) ||
            type == typeof(Common.ResMgr.AssetPacker) || type == typeof(Font) || type == typeof(TextAsset) || type == typeof(AnimatorController) || type == typeof(Shader);
    }

    /* static void BuildABsForAndroid(string version, AssetBundleBuild[] abbs)
     {
         if (string.IsNullOrEmpty(version)) return;
         string path = abPath + version + "/" + EResPlatform.android.ToString() + "/";
         if (Directory.Exists(path))
             Directory.Delete(path, true);
         if (!Directory.Exists(path))
             Directory.CreateDirectory(path);
         if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
         {
             if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android))
             {
                 BuildPipeline.BuildAssetBundles(path, abbs, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
                 manifest = new ManifestInfo();
                 //if (WriteManifest(BuildTarget.Android, path, version))
                 {
                     Debugger.Log("Android打包成功! " + path);
                 }
             }
         }
         else
         {
             BuildPipeline.BuildAssetBundles(path, abbs, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
             manifest = new ManifestInfo();
             //if (WriteManifest(BuildTarget.Android, path, version))
             {
                 Debugger.Log("Android打包成功! " + path);
             }
         }
         VersionUtil.AddGit(resGitPath);
         VersionUtil.CommitGit(resGitPath, "AssetBundleForAndroid version:" + version);
         VersionUtil.PullGit(resGitPath);
         VersionUtil.PushGit(resGitPath);
         Debugger.Log("git:push to git server success!");
     }*/

    public static void BuildAssetBundle()
    {
        ProjectGitPull();
        #region generate AssetBundle version auto
        string version = string.Empty;
        string dirName = GetLastestResVersion();
        if (string.IsNullOrEmpty(dirName))
            version = "1.0.1";
        else
        {
            string[] versions = dirName.ToArray('.');
            int numVersion = int.Parse(versions[2]);
            numVersion++;
            versions[2] = numVersion.ToString();
            version = versions.ToCustomString('.');
        }
        #endregion
        BuildAssetBundleForAndroid(version);
        BuildAssetBundleForIOS(version);
    }

    public static void BuildAssetBundleForAndroid(string version)
    {
        if (string.IsNullOrEmpty(version)) return;
        //VersionUtil.ResetGit(projectPath);
        string path = abPath + version + "/" + EResPlatform.android.ToString() + "/";
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android))
            {
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
                manifest = new ManifestInfo();
                if (WriteManifest(BuildTarget.Android, path, version))
                {
                    Debugger.Log("Android打包成功! " + path);
                }
            }
        }
        else
        {
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
            manifest = new ManifestInfo();
            if (WriteManifest(BuildTarget.Android, path, version))
            {
                Debugger.Log("Android打包成功! " + path);
            }
        }
        VersionUtil.AddGit(resGitPath);
        VersionUtil.CommitGit(resGitPath, "AssetBundleForAndroid version:" + version);
        VersionUtil.PullGit(resGitPath);
        VersionUtil.PushGit(resGitPath);
        Debugger.Log("git:push to git server success!");
    }

    public static void BuildAssetBundleForIOS(string version)
    {
        if (string.IsNullOrEmpty(version)) return;
        //VersionUtil.ResetGit(projectPath);
        string path = abPath + version + "/" + EResPlatform.iOS.ToString() + "/";
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS))
            {
                //BuildAssetBundleOptions.DeterministicAssetBundle确保文件的唯一性
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
                manifest = new ManifestInfo();
                if (WriteManifest(BuildTarget.iOS, path, version))
                {
                    Debugger.Log("ios打包成功! " + path);
                }
            }
        }
        else
        {
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
            manifest = new ManifestInfo();
            if (WriteManifest(BuildTarget.iOS, path, version))
            {
                Debugger.Log("ios打包成功! " + path);
            }
        }
        VersionUtil.AddGit(resGitPath);
        VersionUtil.CommitGit(resGitPath, "AssetBundleForIOS version:" + version);
        VersionUtil.PullGit(resGitPath);
        VersionUtil.PushGit(resGitPath);
        Debugger.Log("git:push to git server success!");
    }

    public static void BuildAssetBundleForPC(string version)
    {
        if (string.IsNullOrEmpty(version)) return;
        //VersionUtil.ResetGit(projectPath);
        string path = abPath + version + "/" + EResPlatform.standalonewindows.ToString() + "/";
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64))
            {
                //BuildAssetBundleOptions.DeterministicAssetBundle确保文件的唯一性
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
                manifest = new ManifestInfo();
                if (WriteManifest(BuildTarget.StandaloneWindows64, path, version))
                {
                    Debugger.Log("PC打包成功! " + path);
                }
            }
        }
        else
        {
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
            manifest = new ManifestInfo();
            if (WriteManifest(BuildTarget.StandaloneWindows64, path, version))
            {
                Debugger.Log("PC打包成功! " + path);
            }
        }
        VersionUtil.AddGit(resGitPath);
        VersionUtil.CommitGit(resGitPath, "AssetBundleForPC version:" + version);
        VersionUtil.PullGit(resGitPath);
        VersionUtil.PushGit(resGitPath);
        Debugger.Log("git:push to git server success!");
    }

    private static bool WriteManifest(BuildTarget bt, string path, string version)
    {
        string platformPath = string.Empty;
        switch (bt)
        {
            case BuildTarget.Android:
                platformPath = EResPlatform.android.ToString();
                break;
            case BuildTarget.iOS:
                platformPath = EResPlatform.iOS.ToString();
                break;
            case BuildTarget.StandaloneWindows64:
                platformPath = EResPlatform.standalonewindows.ToString();
                break;
        }
        AssetInfo assetInfo = null;
        DirectoryInfo abDir = new DirectoryInfo(path);
        foreach (var ab in abDir.GetFiles("*" + ResUtil.ASSET_BUNDLE_SUFFIX, SearchOption.AllDirectories))
        {
            //if (ab.FullName.Contains(@"shadow"))
            //    continue;
            //Debugger.LogError(ab.FullName);
            string fileName = ab.FullName.Replace(ResUtil.ASSET_BUNDLE_SUFFIX, string.Empty);
            //Debugger.LogError(abDir.FullName);
            fileName = fileName.Substring(fileName.IndexOf(abDir.FullName) + abDir.FullName.Length);
            fileName = fileName.Replace(@"\", @"/");
            //Debugger.LogError(fileName);
            assetInfo = new AssetInfo(fileName);
            assetInfo.FilePathList = new List<string>() { assetInfo.SubPath };//只打包一个文件
            assetInfo.CreateDate = ab.CreationTime.Ticks;
            byte[] datas = File.ReadAllBytes(ab.FullName);
            string encodeMD5 = EncryptUtil.Bytes2MD5(datas);
            assetInfo.md5 = encodeMD5;
            assetInfo.Length = ab.Length;
            assetInfo.Suffix = ResUtil.ASSET_BUNDLE_SUFFIX;
            manifest.assetDic.Add(assetInfo.SubPath, assetInfo);
        }

        //assetbundle manifest
        assetInfo = new AssetInfo(platformPath);
        assetInfo.FilePathList = new List<string>() { assetInfo.SubPath };
        //Debugger.Log(abPath+ResConf.eResPlatform.ToString());
        FileInfo assetbundleManifest = new FileInfo(path + platformPath);
        byte[] platformDatas = File.ReadAllBytes(assetbundleManifest.FullName);
        string encodePlatformMD5 = EncryptUtil.Bytes2MD5(platformDatas);
        assetInfo.md5 = encodePlatformMD5;
        assetInfo.CreateDate = assetbundleManifest.CreationTime.Ticks;
        assetInfo.Length = assetbundleManifest.Length;
        manifest.assetDic.Add(assetInfo.SubPath, assetInfo);

        manifest.version = version;
        Debugger.Log(manifest.version);
        byte[] bytes = ResUtil.GetBytesFromManifest(manifest);
        File.WriteAllBytes(path + ResUtil.MANIFESTNAME, bytes);
        ManifestInfo m = ResUtil.GetManifestFromBytes(File.ReadAllBytes(path + ResUtil.MANIFESTNAME));
        Debugger.Log(m.version);
        return true;
    }

    public static string GetLastestResVersion()
    {
        string ResVersion = string.Empty;
        if (!Directory.Exists(abPath))
            Directory.CreateDirectory(abPath);
        DirectoryInfo abDir = new DirectoryInfo(abPath);
        List<int> list = new List<int>();
        foreach (var d in abDir.GetDirectories())
        {
            string dirName = d.Name;
            string[] subVersion = dirName.ToArray('.');
            int numVersion = int.Parse(subVersion[2]);
            list.Add(numVersion);
        }
        if (list.Count > 0)
        {
            list.Sort(SortType.Asc);
            ResVersion = "1.0." + list.Last();
        }
        else
            ResVersion = "1.0.1";
        return ResVersion;
    }
    #endregion

    #region build player
    public static void BuildAndroid(bool delFiles, bool isAndroidProject = false, PlatformType platformType = PlatformType.ShunWang)
    {
        string bundleVersion = PlayerSettings.bundleVersion;
        string[] subVersion = bundleVersion.ToArray('.');
        int numVersion = int.Parse(subVersion[2]);
        numVersion++;
        subVersion[2] = numVersion.ToString();
        bundleVersion = subVersion.ToCustomString('.');
        PlayerSettings.bundleVersion = bundleVersion;
        Logic.Game.Model.InfoData infoData = Logic.Game.Model.InfoData.GetInfoData();
        infoData.version = bundleVersion;
        Logic.Game.Model.InfoData.SaveCSV();
        AssetDatabase.SaveAssets();
        VersionUtil.PullGit(projectPath);
        VersionUtil.AddGit(projectPath);
        VersionUtil.CommitGit(projectPath, "bundleVersion:" + bundleVersion.ToString() + "  " + platformType.ToString());
        VersionUtil.PullGit(projectPath);
        VersionUtil.PushGit(projectPath);
        Debugger.Log("git:push to git server " + projectPath + " success!");
        //DelMotionFBXs();
        if (delFiles)
            DelFilesOnBuild();

        if (isAndroidProject)
        {
            switch (platformType)
            {
                case PlatformType.ShunWang:
                    PlayerSettings.bundleIdentifier = "com.dowan.hjqst.shunwang";
                    break;
                //case PlatformType.Qihoo360:
                //    break;
                //case PlatformType.VIVO:
                //    break;
                //case PlatformType.UC:
                //    PlayerSettings.bundleIdentifier = "com.dowan.hjqst.uc";
                //    break;
                //case PlatformType.OPPO:
                //    PlayerSettings.bundleIdentifier = "com.hjqst.nearme.gamecenter";
                //    break;
                //case PlatformType.Huawei:
                //    PlayerSettings.bundleIdentifier = "com.hjqst.huawei";
                //    break;
                //case PlatformType.Tencent:
                //    PlayerSettings.bundleIdentifier = "com.tencent.tmgp.hjqst";
                //    break;
                //case PlatformType.Xiaomi:
                //    PlayerSettings.bundleIdentifier = "com.dowan.hqjst.mi";
                //    break;
                case PlatformType.Pure:
                    PlayerSettings.bundleIdentifier = "com.dowan.hjqst.shunwang";
                    break;
                case PlatformType.U8:
					PlayerSettings.bundleIdentifier = "com.dowan.hjqst.shunwang";
                    break;
                case PlatformType.shunwan:
                    PlayerSettings.bundleIdentifier = "com.dowan.hjqst.sw";
                    break;
                default:
                    PlayerSettings.bundleIdentifier = "com.dowan.hjqst";
                    break;
            }
            PlayerSettings.productName = platformType.ToString();
            string path = androidProjectPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] backPaths = new string[] { "AndroidManifest.xml", "res/values/strings.xml" };
            List<string> backFullPaths = new List<string>();
            foreach (var p in backPaths)
            {
                backFullPaths.Add(path + platformType.ToString() + "/" + p);
            }

            foreach (var p in backFullPaths)
            {
                if (File.Exists(p))
                    File.Copy(p, p + ".bak", true);
            }


            BuildPipeline.BuildPlayer(new string[] { "Assets/main.unity"/*, "Assets/main_1.unity" */}, path, BuildTarget.Android, BuildOptions.ConnectWithProfiler | BuildOptions.AcceptExternalModificationsToPlayer);
            //string gameNameConfigPath = path + platformType.ToString() + "/res/values/strings.xml";
            //string content = File.ReadAllText(gameNameConfigPath, Encoding.UTF8);
            //content = content.Replace(platformType.ToString(), "幻姬骑士团");
            //File.WriteAllText(gameNameConfigPath, content, Encoding.UTF8);

            foreach (var p in backFullPaths)
            {
                if (File.Exists(p + ".bak"))
                {
                    File.Copy(p + ".bak", p, true);
                    File.Delete(p + ".bak");
                }
            }

            List<string> changeList = VersionUtil.GetChangeList(resGitPath);
            if (platformType == PlatformType.OnlyCSharpCode)
            {
                string targetFile1 = "/assets/bin/Data/Managed/Assembly-CSharp.dll";
                string targetFile2 = "/assets/bin/Data/Managed/Assembly-CSharp-firstpass.dll";
                string targetFile3 = "/assets/bin/Data/Managed/Assembly-UnityScript.dll";
                foreach (PlatformType p in System.Enum.GetValues(typeof(PlatformType)))
                {
                    if (p == PlatformType.None || p == platformType ) continue;
                    string originalPath = androidProjectPath + platformType.ToString() + targetFile1;
                    File.Copy(androidProjectPath + platformType.ToString() + targetFile1, androidProjectPath + p.ToString() + targetFile1, true);
                    File.Copy(androidProjectPath + platformType.ToString() + targetFile2, androidProjectPath + p.ToString() + targetFile2, true);
                    File.Copy(androidProjectPath + platformType.ToString() + targetFile3, androidProjectPath + p.ToString() + targetFile3, true);
                }

                //StringBuilder sb = new StringBuilder();
                //foreach (var item in changeList)
                //{
                //    sb.AppendLine(item);
                //}
                //Debugger.Log(sb.ToString());

                /*foreach (PlatformType p in System.Enum.GetValues(typeof(PlatformType)))
                {
                    if (p == PlatformType.None || p == platformType || p == PlatformType.Tencent) continue;
                    for (int i = 0, count = changeList.Count; i < count; i++)
                    {
                        string originalPath = changeList[i].Trim();                        
                        bool needContinue = false;
                        for (int j = 0, jCount = backPaths.Length; j < jCount; j++)
                        {
                            if (originalPath.Contains(backPaths[j]))
                            {
                                needContinue = true;
                                break;
                            }
                        }
                        if (needContinue) continue;
                        string targetPaht = originalPath.Replace(platformType.ToString(), p.ToString());
                        Debugger.Log("original path:" + gitPath + originalPath);
                        Debugger.Log("target path:" + gitPath + targetPaht);
                        if (File.Exists(gitPath + originalPath))
                            File.Copy(gitPath + originalPath, gitPath + targetPaht, true);
                    }
                }*/
            }


            Copy2AndroidAssets(platformType);

            VersionUtil.AddGit(androidProjectPath);
            VersionUtil.CommitGit(androidProjectPath, "platformType:" + platformType.ToString());
            VersionUtil.PullGit(androidProjectPath);
            VersionUtil.PushGit(androidProjectPath);
            Debugger.Log("git:push to git server success!");

            /*
            if (platformType == PlatformType.OnlyCSharpCode)
            {
                foreach (PlatformType p in System.Enum.GetValues(typeof(PlatformType)))
                {
                    if (p == PlatformType.None || p == platformType || p == PlatformType.Tencent) continue;
                    for (int i = 0, count = changeList.Count; i < count; i++)
                    {
                        string originalPath = changeList[i];
                        bool needContinue = false;
                        for (int j = 0, jCount = backPaths.Length; j < jCount; j++)
                        {
                            if (originalPath.Contains(backPaths[j]))
                            {
                                needContinue = true;
                                break;
                            }
                        }
                        if (needContinue) continue;
                        string targetPaht = originalPath.Replace(platformType.ToString(), p.ToString());
                        if (File.Exists(gitPath + originalPath))
                            File.Copy(gitPath + originalPath, gitPath + targetPaht, true);
                    }
                }
                VersionUtil.AddGit(gitPath);
                VersionUtil.CommitGit(gitPath, "copy OnlyCSharpCode to each platform");
                VersionUtil.PullGit(gitPath);
                VersionUtil.PushGit(gitPath);
                Debugger.Log("git:push to copy OnlyCSharpCode to each platform git server success!");
            }*/
        }
        else
        {
            PlayerSettings.productName = "幻姬骑士团";
            PlayerSettings.bundleIdentifier = "com.dowan.hjqst.shunwang";
            string androidPath = playerPath + EResPlatform.android.ToString() + "/";
            if (!Directory.Exists(androidPath))
                Directory.CreateDirectory(androidPath);
            BuildPipeline.BuildPlayer(new string[] { "Assets/main.unity" }, androidPath + "huanjiqishituan" + bundleVersion + ".apk", BuildTarget.Android, BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging);
        }

        VersionUtil.ResetGit(projectPath);
        AssetDatabase.Refresh();
    }

    public static void Copy2AndroidAssets(PlatformType platformType)
    {
        VersionUtil.PullGit(resGitPath);
        string targetDir = string.Format(@"{0}/{1}/assets/{2}", androidProjectPath, platformType.ToString(), EResPlatform.android.ToString());
        string csvSourceDir = resGitPath + "config/" + GetLastestCSVVersion() + "/" + EResPlatform.android.ToString();
        string luaSourceDir = resGitPath + "lua/" + GetLastestLuaVersion() + "/" + EResPlatform.android.ToString();
        if (Directory.Exists(targetDir))
            Directory.Delete(targetDir, true);
       
        Common.ResMgr.ResUtil.CopyFiles(csvSourceDir, targetDir, "*.md5");
        //Common.ResMgr.ResUtil.CopyFiles(csvSourceDir, targetDir, "*.csv");
        Common.ResMgr.ResUtil.CopyFiles(luaSourceDir, targetDir, "*.md5");
        //Common.ResMgr.ResUtil.CopyFiles(luaSourceDir, targetDir, "*.txt");
        //改为生成压缩包
        Directory.CreateDirectory(targetDir);
        CompressionAsync.打压缩包(csvSourceDir, targetDir + "/config.zip");
        CompressionAsync.打压缩包(luaSourceDir, targetDir + "/lua.zip");
    }
    public static void Copy2StreamingAssetsPath(bool isAndroid)
    {
        VersionUtil.PullGit(resGitPath);
		string platformString = isAndroid ? EResPlatform.android.ToString() : EResPlatform.iOS.ToString();
        string targetDir = string.Format(@"{0}/{1}", streamingAssetsPath, platformString);
        string csvSourceDir = resGitPath + "config/" + GetLastestCSVVersion() + "/" + platformString;
        string luaSourceDir = resGitPath + "lua/" + GetLastestLuaVersion() + "/" + platformString;
        if (Directory.Exists(targetDir))
            Directory.Delete(targetDir, true);

		string delDir = string.Format(@"{0}/{1}", streamingAssetsPath, isAndroid ? EResPlatform.iOS.ToString() : EResPlatform.android.ToString());
		if (Directory.Exists(delDir))
			Directory.Delete(delDir, true);

		string delMetaFilePath = string.Format(@"{0}/{1}.meta", streamingAssetsPath, isAndroid ? EResPlatform.iOS.ToString() : EResPlatform.android.ToString());
		if (File.Exists(delMetaFilePath))
			File.Delete(delMetaFilePath);

        Common.ResMgr.ResUtil.CopyFiles(csvSourceDir, targetDir, "*.md5");
        //Common.ResMgr.ResUtil.CopyFiles(csvSourceDir, targetDir, "*.csv");
        Common.ResMgr.ResUtil.CopyFiles(luaSourceDir, targetDir, "*.md5");
        //Common.ResMgr.ResUtil.CopyFiles(luaSourceDir, targetDir, "*.txt");
        //改为生成压缩包
        Directory.CreateDirectory(targetDir);
        CompressionAsync.打压缩包(csvSourceDir, targetDir + "/config.zip");
        CompressionAsync.打压缩包(luaSourceDir, targetDir + "/lua.zip");

		AssetDatabase.Refresh();
    }

    public static void BuildIOS(bool delFiles)
    {
        VersionUtil.PullGit(projectPath);
        Logic.Game.Model.InfoData infoData = Logic.Game.Model.InfoData.GetInfoData();
        PlayerSettings.bundleVersion = infoData.version;
        AssetDatabase.SaveAssets();
        string iosPath = playerPath + EResPlatform.iOS.ToString() + "/";
        string targetDir = iosPath + infoData.version;
        if (Directory.Exists(targetDir))
            DelAll(new DirectoryInfo(targetDir), null);
        Directory.CreateDirectory(targetDir);
        DelMotionFBXs();
        if (delFiles)
            DelFilesOnBuild();

        DirectoryInfo assetsPath = new DirectoryInfo(projectPath + "/Assets");
        foreach (var file in assetsPath.GetFiles("*.*", SearchOption.AllDirectories))
        {
            string fullName = file.FullName;
            string targetPath = fullName.Substring(fullName.IndexOf(@"Assets"));
            targetPath = targetDir + "/" + targetPath.Replace(@"\", "/");
            string tDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
            if (!Directory.Exists(tDir))
                Directory.CreateDirectory(tDir);
            File.Copy(file.FullName, targetPath, true);
        }

        DirectoryInfo projectSettingsPath = new DirectoryInfo(projectPath + "/ProjectSettings");
        foreach (var file in projectSettingsPath.GetFiles("*.*", SearchOption.AllDirectories))
        {
            string fullName = file.FullName;
            string targetPath = fullName.Substring(fullName.IndexOf(@"ProjectSettings"));
            targetPath = targetDir + "/" + targetPath.Replace(@"\", "/");
            string tDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
            if (!Directory.Exists(tDir))
                Directory.CreateDirectory(tDir);
            File.Copy(file.FullName, targetPath, true);
        }
        VersionUtil.ResetGit(projectPath);
        AssetDatabase.Refresh();
        Debugger.Log("pre build ios success !!");
    }

    public static void BuildForAndroid()
    {
        BuildAndroid(true, false);
    }

    public static void BuildForIOS()
    {
        BuildIOS(true);
    }

    [MenuItem("Tools/DelFilesOnBuild", false, 200)]
    public static void DelFilesOnBuild()
    {
        #region del un use res
        string[] dirs = new string[] { 
           "Assets/Template",
           //"Assets/Res/Model/hero", "Assets/Res/Model/player", "Assets/Res/Model/pet","Assets/Res/Resources/animcontroller", "Assets/Res/Resources/character" ,
           "Assets/Res/Resources/texture","Assets/Res/Resources/config/csv","Assets/Res/Resources/languages","Assets/ToLua/Lua","Assets/Res/Resources/sprite",
           "Assets/Res/Atlas","Assets/Res/Resources/map","Assets/Res/Resources/material","Assets/Res/Resources/ui_textures",
           "Assets/Res/Resources/fonts",
           //"Assets/Res/Resources/shader",
           "Assets/Res/Resources/ui",
           "Assets/Res/Resources/audio",
           "Assets/Res/Resources/effects"
        };

        string[] ignores = new string[] { @"Assets\Res\Resources\ui\load_game", @"Assets\Res\Atlas\always", @"Assets\Res\Resources\ui\launch", @"Assets\Res\Resources\ui\wifi_tips", 
                                        @"Assets\Res\Resources\ui_textures\loading_textures",@"Assets\Res\Resources\fonts\FZY3JW_Dynamic", @"Assets\Res\Resources\ui\net_error_tips"};
        foreach (var d in dirs)
        {
            if (Directory.Exists(d))
                DelAll(new DirectoryInfo(d), ignores);
        }
        AssetDatabase.Refresh();

        string[] files = new string[] { "Assets/Res/Resources/config/info.csv", "Assets/Res/Resources/config/lua.csv", "Assets/Res/Resources/config/names.csv" };
        foreach (var f in files)
        {
            if (File.Exists(f))
                File.Delete(f);
        }
        AssetDatabase.Refresh();
        #endregion
    }

    private static void DelAll(DirectoryInfo dir, string[] ignores)
    {
        if (ignores != null)
        {
            foreach (var ig in ignores)
            {
                if (dir.FullName.Contains(ig))
                {
                    return;
                }
            }
        }
        foreach (var d in dir.GetDirectories())
        {
            DelAll(d, ignores);
        }
        foreach (var f in dir.GetFiles("*.*"))
        {
            bool ignore = false;
            foreach (var ig in ignores)
            {
                if (f.FullName.Contains(ig))
                {
                    ignore = true;
                    break;
                }
            }
            if (ignore) continue;
            f.Delete();
        }
        if (dir.GetDirectories().Length == 0 && dir.GetFiles().Length == 0)
            dir.Delete();
    }

    [MenuItem("Tools/DelMotionFBXs", false, 200)]
    private static void DelMotionFBXs()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Res/Model");
        foreach (var f in dir.GetFiles("*.FBX", SearchOption.AllDirectories))
        {
            if (f.Name.Contains("@"))
                f.Delete();
        }
        AssetDatabase.Refresh();
    }
    #endregion

    #region build config
    public static string[] GetCSVVersions()
    {
        List<string> result = new List<string>();
        string configRoot = resGitPath + "config/";
        if (!Directory.Exists(configRoot))
            Directory.CreateDirectory(configRoot);
        DirectoryInfo configDir = new DirectoryInfo(configRoot);
        foreach (var d in configDir.GetDirectories())
        {
            result.Add(d.Name);
        }
        result.Sort(SortVersions);
        return result.ToArray();
    }

    public static string GetLastestCSVVersion()
    {
        string csvVersion = string.Empty;
        string configRoot = resGitPath + "config/";
        if (!Directory.Exists(configRoot))
            Directory.CreateDirectory(configRoot);
        DirectoryInfo configDir = new DirectoryInfo(configRoot);
        List<int> list = new List<int>();
        foreach (var d in configDir.GetDirectories())
        {
            string dirName = d.Name;
            string[] subVersion = dirName.ToArray('.');
            int numVersion = int.Parse(subVersion[2]);
            list.Add(numVersion);
        }
        if (list.Count > 0)
        {
            list.Sort(SortType.Asc);
            csvVersion = "1.0." + list.Last();
        }
        else
            csvVersion = "1.0.1";
        return csvVersion;
    }

    public static void BuildCSV(string csvVersion, string gitMessage = "")
    {
        #region generate csv version auto
        if (string.IsNullOrEmpty(csvVersion))
        {
            string dirName = GetLastestCSVVersion();
            if (string.IsNullOrEmpty(dirName))
                csvVersion = "1.0.1";
            else
            {
                string[] versions = dirName.ToArray('.');
                int numVersion = int.Parse(versions[2]);
                numVersion++;
                versions[2] = numVersion.ToString();
                csvVersion = versions.ToCustomString('.');
            }
        }
        #endregion
        string androidPath = resGitPath + "config/" + csvVersion + "/" + EResPlatform.android.ToString() + "/" + csvMD5Path;
        string iosPath = resGitPath + "config/" + csvVersion + "/" + EResPlatform.iOS.ToString() + "/" + csvMD5Path;
        string pcPath = resGitPath + "config/" + csvVersion + "/" + EResPlatform.standalonewindows.ToString() + "/" + csvMD5Path;
        string csvDirPath = projectPath + "/Assets/Res/Resources/config";
        DirectoryInfo csvDir = new DirectoryInfo(csvDirPath);

        string androidDir = androidPath.Replace("/config.md5", string.Empty);
        if (!Directory.Exists(androidDir))
            Directory.CreateDirectory(androidDir);
        string iOSDir = iosPath.Replace("/config.md5", string.Empty);
        if (!Directory.Exists(iOSDir))
            Directory.CreateDirectory(iOSDir);
        string pcDir = pcPath.Replace("/config.md5", string.Empty);
        if (!Directory.Exists(pcDir))
            Directory.CreateDirectory(pcDir);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("version:" + csvVersion);
        foreach (var csv in csvDir.GetFiles("*.csv", SearchOption.AllDirectories))
        {
            string content = File.ReadAllText(csv.FullName);
            string encrypt = EncryptUtil.DESEncryptString(content, GameConfig.encryptKey);
            string encodeMD5 = EncryptUtil.String2MD5(encrypt);
            sb.AppendLine(csv.Name + ":" + encodeMD5);
            File.WriteAllText(androidDir + "/" + csv.Name, encrypt);
            File.WriteAllText(iOSDir + "/" + csv.Name, encrypt);
            File.WriteAllText(pcDir + "/" + csv.Name, encrypt);
        }
        string langauge = File.ReadAllText(projectPath + "/Assets/Res/Resources/languages/Chinese.txt");

        Debugger.Log(langauge.Substring(0, 50));
        string langaugeEncrypt = EncryptUtil.DESEncryptString(langauge, GameConfig.encryptKey);
        string langaugeMD5 = EncryptUtil.String2MD5(langaugeEncrypt);
        sb.AppendLine("Chinese.csv:" + langaugeMD5);
        File.WriteAllText(androidDir + "/Chinese.csv", langaugeEncrypt);
        File.WriteAllText(iOSDir + "/Chinese.csv", langaugeEncrypt);
        File.WriteAllText(pcDir + "/Chinese.csv", langaugeEncrypt);
        Debugger.Log(sb.ToString());

        File.WriteAllText(androidPath, sb.ToString());
        File.WriteAllText(iosPath, sb.ToString());
        File.WriteAllText(pcPath, sb.ToString());
        CopyCSV2ABs();
        VersionUtil.AddGit(resGitPath);
        VersionUtil.CommitGit(resGitPath, "csv version:" + csvVersion + "  " + gitMessage);
        VersionUtil.PullGit(resGitPath);
        VersionUtil.PushGit(resGitPath);
        Debugger.Log("git:csv md5 file push to git server success!");
    }

    public static void BuildForCSV()
    {
        BuildCSV(string.Empty);
    }
    #endregion

    #region build lua
    public static string[] GetLuaVersions()
    {
        List<string> result = new List<string>();
        string luaRoot = resGitPath + "lua/";
        if (!Directory.Exists(luaRoot))
            Directory.CreateDirectory(luaRoot);
        DirectoryInfo luaDir = new DirectoryInfo(luaRoot);
        foreach (var d in luaDir.GetDirectories())
        {
            result.Add(d.Name);
        }
        result.Sort(SortVersions);
        return result.ToArray();
    }

    public static string GetLastestLuaVersion()
    {
        string luaVersion = string.Empty;
        string luaRoot = resGitPath + "lua/";
        if (!Directory.Exists(luaRoot))
            Directory.CreateDirectory(luaRoot);
        DirectoryInfo luaDir = new DirectoryInfo(luaRoot);
        List<int> list = new List<int>();
        foreach (var d in luaDir.GetDirectories())
        {
            string dirName = d.Name;
            string[] subVersion = dirName.ToArray('.');
            int numVersion = int.Parse(subVersion[2]);
            list.Add(numVersion);
        }
        if (list.Count > 0)
        {
            list.Sort(SortType.Asc);
            luaVersion = "1.0." + list.Last();
        }
        else
            luaVersion = "1.0.1";
        return luaVersion;
    }

    public static void BuildLua(string luaVersion, string gitMessage = "", bool isEncrypt = true)
    {
        #region generate lua version auto
        if (string.IsNullOrEmpty(luaVersion))
        {
            string dirName = GetLastestLuaVersion();
            if (string.IsNullOrEmpty(dirName))
                luaVersion = "1.0.1";
            else
            {
                string[] versions = dirName.ToArray('.');
                int numVersion = int.Parse(versions[2]);
                numVersion++;
                versions[2] = numVersion.ToString();
                luaVersion = versions.ToCustomString('.');
            }
        }
        #endregion

        #region generate lua txt
        Common.Tools.Editor.HotKey.LuaToTxt();
        #endregion
        string androidPath = resGitPath + "lua/" + luaVersion + "/" + EResPlatform.android.ToString() + "/lua";
        string iosPath = resGitPath + "lua/" + luaVersion + "/" + EResPlatform.iOS.ToString() + "/" + luaMD5Path;
        string pcPath = resGitPath + "lua/" + luaVersion + "/" + EResPlatform.standalonewindows.ToString() + "/" + luaMD5Path;
        string luaDirPath = projectPath + "/Assets/ToLua/Resources/Lua";
        DirectoryInfo luaDir = new DirectoryInfo(luaDirPath);
        StringBuilder sb = new StringBuilder();
        string dir = string.Empty;
        if (isEncrypt)
            BuildLuaToResources("version:" + luaVersion, androidPath);
        else
        {
            dir = androidPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            sb.AppendLine("version:" + luaVersion);
            foreach (var txt in luaDir.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                string fullName = txt.FullName;
                string targetPath = fullName.Substring(fullName.IndexOf(@"Assets\ToLua\Resources\Lua\"));
                targetPath = targetPath.Replace(@"Assets\ToLua\Resources\Lua\", string.Empty);
                targetPath = targetPath.Replace(@"\", "/");
                byte[] bytes = File.ReadAllBytes(txt.FullName);
                string encodeMD5 = EncryptUtil.Bytes2MD5(bytes);
                //Debugger.Log(targetPath);
                sb.AppendLine(targetPath + ":" + encodeMD5);
                targetPath = dir + "/" + targetPath;
                string targetDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                File.WriteAllBytes(targetPath, bytes);
            }
            File.WriteAllText(androidPath + "/lua.md5", sb.ToString());
            sb.Remove(0, sb.Length);
        }

        sb.AppendLine("version:" + luaVersion);
        dir = iosPath.Replace("/lua.md5", string.Empty);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        foreach (var txt in luaDir.GetFiles("*.txt", SearchOption.AllDirectories))
        {
            string fullName = txt.FullName;
            string targetPath = fullName.Substring(fullName.IndexOf(@"Assets\ToLua\Resources\Lua\"));
            targetPath = targetPath.Replace(@"Assets\ToLua\Resources\Lua\", string.Empty);
            targetPath = targetPath.Replace(@"\", "/");
            byte[] bytes = File.ReadAllBytes(txt.FullName);
            byte[] encrypt = null;
            if (isEncrypt)
                encrypt = EncryptUtil.PlusExcursionBytes(bytes, GameConfig.excursion);
            else
                encrypt = bytes;
            string encodeMD5 = EncryptUtil.Bytes2MD5(encrypt);
            //Debugger.Log(targetPath);
            sb.AppendLine(targetPath + ":" + encodeMD5);
            targetPath = dir + "/" + targetPath;
            string targetDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);
            File.WriteAllBytes(targetPath, encrypt);
        }
        File.WriteAllText(iosPath, sb.ToString());
        sb.Remove(0, sb.Length);
        sb.AppendLine("version:" + luaVersion);
        dir = pcPath.Replace("/lua.md5", string.Empty);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        foreach (var txt in luaDir.GetFiles("*.txt", SearchOption.AllDirectories))
        {
            string fullName = txt.FullName;
            string targetPath = fullName.Substring(fullName.IndexOf(@"Assets\ToLua\Resources\Lua\"));
            targetPath = targetPath.Replace(@"Assets\ToLua\Resources\Lua\", string.Empty);
            targetPath = targetPath.Replace(@"\", "/");
            byte[] bytes = File.ReadAllBytes(txt.FullName);
            byte[] encrypt = null;
            if (isEncrypt)
                encrypt = EncryptUtil.PlusExcursionBytes(bytes, GameConfig.excursion);
            else
                encrypt = bytes;
            string encodeMD5 = EncryptUtil.Bytes2MD5(encrypt);
            //Debugger.Log(targetPath);
            sb.AppendLine(targetPath + ":" + encodeMD5);
            targetPath = dir + "/" + targetPath;
            string targetDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);
            File.WriteAllBytes(targetPath, encrypt);
        }
        File.WriteAllText(pcPath, sb.ToString());
        CopyLua2ABs();
        Directory.Delete(projectPath + "/Assets/ToLua/Resources", true);
        VersionUtil.AddGit(resGitPath);
        VersionUtil.CommitGit(resGitPath, "lua version:" + luaVersion + "  " + gitMessage);
        VersionUtil.PullGit(resGitPath);
        VersionUtil.PushGit(resGitPath);
        Debugger.Log("git:lua md5 file push to git server success!");
        AssetDatabase.Refresh();
    }

    public static void BuildForLua()
    {
        BuildLua(string.Empty);
    }
    #endregion

    [MenuItem("Tools/Copy csv 2 abs")]
    private static bool CopyCSV2ABs()
    {
        string csvDirName = GetLastestCSVVersion();
        string absDirName = GetLastestResVersion();
        string sourceDir = resGitPath + "config/" + csvDirName;
        string targetDir = abPath + absDirName;
        if (!Directory.Exists(sourceDir)) return false;
        DirectoryInfo csvDir = new DirectoryInfo(sourceDir);
        foreach (var d in csvDir.GetDirectories())
        {
            foreach (var f in d.GetFiles("*.csv", SearchOption.AllDirectories))
            {
                string dir = targetDir + "/" + d.Name + "/config/csv";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(f.FullName, dir + "/" + f.Name, true);
            }
        }
        foreach (var d in csvDir.GetDirectories())
        {
            foreach (var f in d.GetFiles("*.md5", SearchOption.AllDirectories))
            {
                string dir = targetDir + "/" + d.Name + "/config/csv";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(f.FullName, dir + "/" + f.Name, true);
            }
        }
        Debugger.Log("copy csv 2 abs success !");
        return true;
    }

    [MenuItem("Tools/Copy lua 2 abs")]
    private static bool CopyLua2ABs()
    {
        string luaDirName = GetLastestLuaVersion();
        string absDirName = GetLastestResVersion();
        string sourceDir = resGitPath + "lua/" + luaDirName;
        string targetDir = abPath + absDirName;
        if (!Directory.Exists(sourceDir)) return false;
        DirectoryInfo luaDir = new DirectoryInfo(sourceDir);
        foreach (var txt in luaDir.GetFiles("*.*", SearchOption.AllDirectories))
        {
            string fullName = txt.FullName;
            string targetPath = fullName.Substring(fullName.IndexOf(@"game-client-res\lua\" + luaDirName));
            targetPath = targetPath.Replace(@"game-client-res\lua\" + luaDirName, string.Empty);
            targetPath = targetDir + "/" + targetPath.Replace(@"\", "/");
            string tDir = targetPath.Substring(0, targetPath.LastIndexOf("/") + 1);
            if (!Directory.Exists(tDir))
                Directory.CreateDirectory(tDir);
            File.Copy(txt.FullName, targetPath, true);
        }

        Debugger.Log("copy lua 2 abs success !");
        return true;
    }

    public static void BuildLuaToResources(string version, string destDir)
    {
        string tempDir = Application.streamingAssetsPath + "/Lua";
        if (!Directory.Exists(tempDir))
            Directory.CreateDirectory(tempDir);
        string path = Application.dataPath.Replace('\\', '/');
        path = path.Substring(0, path.LastIndexOf('/'));
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);
        else
            Directory.Delete(destDir, true);
        File.Copy(path + "/Luajit/Build.bat", tempDir + "/Build.bat", true);
        CopyLuaBytesFiles(LuaConst.luaDir, tempDir, false);
        Process proc = Process.Start(tempDir + "/Build.bat");
        proc.WaitForExit();
        CopyLuaBytesFiles(tempDir + "/Out/", destDir, false, "*.bytes");

        Directory.Delete(tempDir, true);
        AssetDatabase.Refresh();

        //RecursivePath(destDir, ".txt");
        CopyLua2Txt(destDir, ".txt");
        DeleteDir(destDir, ".bytes");

        //获取md5值
        DirectoryInfo di = new DirectoryInfo(destDir);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(version);
        foreach (var txt in di.GetFiles("*.txt", SearchOption.AllDirectories))
        {
            string fullName = txt.FullName;
            string targetPath = fullName.Replace(@"\", "/");
            targetPath = targetPath.Replace(destDir.Replace("//", "/") + "/", string.Empty);
            byte[] bytes = File.ReadAllBytes(txt.FullName);
            //Debugger.Log(targetPath);
            string encodeMD5 = EncryptUtil.Bytes2MD5(bytes);
            sb.AppendLine(targetPath + ":" + encodeMD5);
        }
        File.WriteAllText(destDir + "/lua.md5", sb.ToString());
        UnityEngine.Debug.Log("finish generated");
    }

    static void CopyLua2Txt(string dir, string ext)
    {
        DirectoryInfo di = new DirectoryInfo(dir);
        foreach (var f in di.GetFiles("*.*", SearchOption.AllDirectories))
        {
            string targetPath = f.FullName.Replace(@".lua.bytes", ".txt");
            f.CopyTo(targetPath, true);
        }
    }

    static void RecursivePath(string dir, string ext)
    {
        var files = Directory.GetFiles(dir);
        for (int i = 0; i < files.Length; i++)
        {
            var fn = files[i];
            var nfn = fn.Substring(0, fn.IndexOf('.')) + ext;
            Debugger.Log(fn);
            File.Copy(fn, nfn, true);
        }
        var dirs = Directory.GetDirectories(dir);
        foreach (var item in dirs)
        {
            RecursivePath(item, ext);
        }
    }
    static void DeleteDir(string dir, string ext)
    {
        var files = Directory.GetFiles(dir);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(ext))
                File.Delete(files[i]);
        }
        var dirs = Directory.GetDirectories(dir);
        foreach (var item in dirs)
        {
            DeleteDir(item, ext);
        }
    }

    static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(sourceDir))
        {
            return;
        }

        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
        int len = sourceDir.Length;

        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
        {
            --len;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string str = files[i].Remove(0, len);
            string dest = destDir + "/" + str;
            if (appendext) dest += ".bytes";
            string dir = Path.GetDirectoryName(dest);
            Directory.CreateDirectory(dir);
            File.Copy(files[i], dest, true);
        }
    }
}