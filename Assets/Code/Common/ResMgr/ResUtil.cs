using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Common.ResMgr
{
    public static class ResUtil
    {
        public const string MANIFESTNAME = "manifest.bin";
        public const string ASSET_BUNDLE_SUFFIX = ".ab";//ab只是用于磁盘文件上的标记
        private const string LOCAL_WWW_PREFIX = "file:///";
        public const string CSV_DIR = "config/csv";
        public const string CSV_MD5 = "config.md5";
        public const string LUA_DIR = "lua";
        public const string LUA_MD5 = "lua.md5";
        public const string CONFIG_DIRECTORY = "config";
        public const string LUA_DIRECTORY = "lua";
        public const string CSV_ZIP= "config.zip";
        public const string LUA_ZIP= "lua.zip";

        public static List<string> RecursiveGetFiles(string path)
        {
            List<string> result = new List<string>();
            if (!System.IO.Directory.Exists(path))
            {
                if (System.IO.File.Exists(path))
                    result.Add(path);
                return result;
            }
            result.AddRange(System.IO.Directory.GetFiles(path));
            foreach (string dir in System.IO.Directory.GetDirectories(path))
            {
                result.AddRange(RecursiveGetFiles(dir));
            }
            return result;
        }

        public static string GetDirectoryByPath(string path)
        {
            return path.Substring(0, path.LastIndexOf("/"));
        }

        private static string GetFullPath(string basePath, string subPath)
        {
            return string.Format(@"{0}/{1}/{2}", basePath, ResConf.eResPlatform, subPath);
        }

        public static string GetRemotePathRoot(string subPath)
        {
            if (ResMgr.instance == null)
            {
                throw new System.Exception("you must run game first");
            }
            return string.Format(@"{0}{1}", Logic.Game.GameConfig.instance.url, subPath);
        }

        //本地ab路径
        public static string GetLocalPath(string subPath)
        {
            return GetFullPath(Application.persistentDataPath, subPath);
        }

        //Streaming assets路径
        public static string GetStreamingAssetsPath(string subPath)
        {
            return GetFullPath(Application.streamingAssetsPath, subPath);
        }

        public static string GetStreamingAssetsWWWPath(string subPath)
        {
            string path = GetStreamingAssetsPath(subPath);
#if UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_ANDROID

#elif UNITY_IPHONE
            path = "file://" + path;
#endif
            return path;
        }

        public static string GetLocalWWWPath(string subPath)
        {
            return string.Format(@"{0}{1}", LOCAL_WWW_PREFIX, subPath);
        }

        //远程静态路径
        public static string GetRemoteStaticPath(string subPath)
        {
            return GetFullPath(Logic.Game.GameConfig.instance.staticUrl, subPath);
        }
		//远程静态路径
		public static string GetRemoteStaticPathByCdn(string subPath)
		{
			return GetFullPath(Logic.Game.GameConfig.instance.staticUrl, subPath)+"?v="+DateTime.Now.Ticks;
		}
        //远程ab路径
        public static string GetRemotePath(string subPath)
        {
            if (ResMgr.instance == null)
            {
                throw new System.Exception("you must run game first");
            }
            return GetFullPath(Logic.Game.GameConfig.instance.url, subPath);
        }

        //本地manifest路径
        public static string GetLocalManifestPath()
        {
            return GetLocalPath(ResUtil.MANIFESTNAME);
        }

        //远程manifest路径
        public static string GetRemoteManifestPath()
        {
            return GetRemotePath(ResUtil.MANIFESTNAME);
        }

        //本地assetbundle manifest路径
        public static string GetLocalAssetBundleManifestPath()
        {
            return GetLocalPath(ResConf.eResPlatform.ToString());
        }

        ////远程assetbundle manifest路径
        //public static string GetRemoteAssetBundleManifestPath()
        //{
        //    return GetRemotePath(ResConf.eResPlatform.ToString() + "/" + ResConf.eResPlatform.ToString());
        //}

        public static bool ExistConfigMD5InLocal()
        {
            return ExistsInLocal(string.Format("{0}/{1}", CSV_DIR, CSV_MD5));
        }

        public static bool ExistsInLocal(string subPath, out string localPath)
        {
            return ExistsInStreamOrLocal(subPath, false, out localPath);
        }
        public static bool ExistsInStreamingAssets(string subPath, out string streamPath)
        {
            return ExistsInStreamOrLocal(subPath, true, out streamPath);
        }
        /// <summary>
        /// 本地（指定读写目录或者母包内）是否存在
        /// </summary>
        /// <param name="isStreamPath"> true 就stream地址，false 是persistentData地址</param>
        private static bool ExistsInStreamOrLocal(string subPath, bool isStreamPath,out string outPath)
        {
            outPath = isStreamPath ? GetStreamingAssetsPath(subPath) : GetLocalPath(subPath);
            if (System.IO.File.Exists(outPath))
            {
                return true;
            }
            outPath = null;
            return false;
        }
        public static bool ExistsInStreamingAssets(string subPath)
        {
            return ExistsInStreamOrLocal(subPath, true);
        }
        public static bool ExistsInLocal(string subPath)
        {
            return ExistsInStreamOrLocal(subPath, false);
        }
        /// <summary>
        /// 本地（指定读写目录或者母包内）是否存在
        /// </summary>
        /// <param name="isStreamPath"> true 就stream地址，false 是persistentData地址</param>
        private static bool ExistsInStreamOrLocal(string subPath, bool isStreamPath)
        {

            string localPath = isStreamPath ? GetStreamingAssetsPath(subPath):GetLocalPath(subPath);
            if (System.IO.File.Exists(localPath))
            {
                return true;
            }
            return false;
        }

        public static void CopyFiles(string sourceDir, string destDir, string searchPattern = "*.txt", SearchOption option = SearchOption.AllDirectories)
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
                string dir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(files[i], dest, true);
            }
        }

        public static void Save2Local(string subPath, byte[] bytes)
        {
            string path = GetLocalPath(subPath);
            Save2LocalFullPath(path, bytes);
        }
        public static void Save2LocalFullPath(string path, byte[] bytes)
        {
            try
            {
               
                string dir = ResUtil.GetDirectoryByPath(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                fs.SetLength(0);
                if (bytes != null)
                    fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
                Debugger.Log("Save2Local:{0}", path);
            }
            catch (Exception e)
            {
                Debugger.LogError("Save2Local:{0} error {1}!", path, e.ToString());
            }
        }

        //public static void Save2Local(string subPath, string text)
        //{
        //    try
        //    {
        //        string path = GetLocalPath(subPath);
        //        string dir = ResUtil.GetDirectoryByPath(path);
        //        if (!System.IO.Directory.Exists(dir))
        //            System.IO.Directory.CreateDirectory(dir);
        //        System.IO.File.WriteAllText(path, text);
        //        Debugger.Log(path);
        //    }
        //    catch (Exception e)
        //    {
        //        Debugger.LogError("Save2Local:" + e);
        //    }
        //}
        //public static bool DelLocalAbRes(string subPath)
        //{
        //    string path = GetLocalPath(subPath);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //        return true;
        //    }
        //    return false;
        //}

        public static bool DelAllLocalAbRes()
        {
            try
            {
                string s = GetLocalPath(string.Empty);
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(s);
                foreach (var d in dir.GetDirectories())
                {
                    if (d.Name.Contains(CONFIG_DIRECTORY) || d.Name.Contains(LUA_DIRECTORY))
                        continue;
                    d.Delete(true);
                }
                return true;
            }
            catch (Exception e)
            {
                Debugger.LogError("DelAllLocalAbRes:" + e.StackTrace);
            }
            return false;
        }

        public static bool DelLocalFile(string subPath)
        {
            try
            {
                string s = GetLocalPath(subPath);
                Debugger.Log(string.Format("del file:{0} from localization.", s));
                if (File.Exists(s))
                    File.Delete(s);
                return true;
            }
            catch (Exception e)
            {
                Debugger.LogError("DelLocalFile:" + subPath + " fail," + e.StackTrace);
            }
            return false;
        }

        #region Manifest文件处理相关

        //获取本地的manifest文件信息
        public static ManifestInfo GetManifestFromLocal()
        {
            try
            {
                string path = GetLocalManifestPath();
                //Debugger.Log("path:"+path);
                if (!System.IO.File.Exists(path))
                    return null;
                return GetManifestFromBytes(System.IO.File.ReadAllBytes(path));
            }
            catch (Exception e)
            {
                Debugger.LogError("GetManifestFromLocal:" + e);
            }
            finally
            {
            }
            return null;
        }

        //把manifest文件信息保存在本地
        public static void SaveManifest2Local(ManifestInfo manifestInfo)
        {
            try
            {
                string path = GetLocalManifestPath();
                string dir = ResUtil.GetDirectoryByPath(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                System.IO.File.WriteAllBytes(path, GetBytesFromManifest(manifestInfo));
            }
            catch (Exception e)
            {
                Debugger.LogError("SaveManifest2Local:" + e);
            }
            finally
            {
            }
        }

        //把字节流转为manifest对象
        public static ManifestInfo GetManifestFromBytes(byte[] bytes)
        {
            ManifestInfo manifestInfo = null;
            try
            {
                //System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes.Uncompress());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
                System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
                br.BaseStream.Position = 0;
                manifestInfo = new ManifestInfo();
                manifestInfo.version = br.ReadString();
                uint num = br.ReadUInt32();
                AssetInfo assetInfo;
                while (num-- > 0)
                {
                    List<string> fileNameList = new List<string>();

                    string subPath = br.ReadString();
                    long length = br.ReadInt64();
                    long createDate = br.ReadInt64();
                    string md5 = br.ReadString();
                    string suffix = br.ReadString();
                    uint fileNameNum = br.ReadUInt32();
                    while (fileNameNum-- > 0)
                    {
                        fileNameList.Add(br.ReadString());
                    }
                    assetInfo = new AssetInfo(subPath, length, createDate, md5, suffix, fileNameList);
                    manifestInfo.assetDic.Add(assetInfo.SubPath, assetInfo);
                }
                br.Close();
                ms.Close();
                ms.Dispose();
                return manifestInfo;
            }
            catch (Exception e)
            {
                Debugger.LogError("GetManifestFromBytes:" + e);
            }
            finally
            {
            }
            return null;
        }

        //把manifest对象转为字节流
        public static byte[] GetBytesFromManifest(ManifestInfo manifestInfo)
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
                bw.Write(manifestInfo.version);
                bw.Write(manifestInfo.assetDic.Count);
                foreach (var kv in manifestInfo.assetDic)
                {
                    bw.Write(kv.Value.SubPath);
                    bw.Write(kv.Value.Length);
                    bw.Write(kv.Value.CreateDate);
                    bw.Write(kv.Value.md5);
                    bw.Write(kv.Value.Suffix);
                    bw.Write(kv.Value.FilePathList.Count);
                    for (int i = 0, count = kv.Value.FilePathList.Count; i < count; i++)
                    {
                        bw.Write(kv.Value.FilePathList[i]);
                    }
                }
                ms.Position = 0;
                int len = (int)ms.Length;//manifest文件不能超2GB
                byte[] bytes = new byte[len];
                ms.Read(bytes, 0, len);
                bw.Close();
                ms.Close();
                ms.Dispose();
                //return bytes.Compress();
                return bytes;
            }
            catch (Exception e)
            {
                Debugger.LogError("GetBytesFromManifest:" + e);
            }
            finally
            {

            }
            return null;
        }
        #endregion

        public static int LoadAssetBundleSort(LoadAssetbundle x, LoadAssetbundle y)
        {
            if (x.Prior > y.Prior)
                return -1;
            if (x.Prior < y.Prior)
                return 1;
            return 0;
        }
    }
}
