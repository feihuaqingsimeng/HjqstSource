using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


public static class CompressionAsync
{
    private static int _codePage = 65001;//uft-8//54936;
    private static int _currentCount = 0;
    #region 打压缩包
    /// <summary>
    ///path= "Assets\\Resources\\ziptest", targetPath="C://ziptest.zip"
    /// </summary>
    /// <param name="path"></param>
    /// <param name="targetPath"></param>
    public static void 打压缩包(string path, string targetPath)
    {
        _currentCount = 0;
        Debug.Log("开始打压缩包" + targetPath);

        ZipDirectory(path, targetPath, string.Empty, true, string.Empty /*"(?!(manifest|meta|zip))$"*/, string.Empty, true, null);//太难匹配"非"了
        Debug.Log("完成打压缩包" + targetPath);
        ////ZipDirectory("Assets\\Resources\\ziptest", "C://ziptest.zip");/.*(?!/.txt)$/


        //string writePath = targetPath + "/resourceszip.config";
        //int bundleVersion = 0;
        //string zipInfo = "";
        //var d = GetCompressionConfig(writePath);
        //if (d != null && d.Count > 0d && d.ContainsKey("bundleVersion"))
        //{
        //    bundleVersion = int.Parse(d["bundleVersion"]);
        //}
        //Dictionary<string, string> dSet = new Dictionary<string, string>();
        //dSet.Add("bundleVersion", (bundleVersion + 1).ToString());
        //dSet.Add("size", (StaticAPI.ReadFile(targetPath + "/assetbundles.zip").Length / 1024 / 1024).ToString());
        //SetCompressionConfig(writePath, dSet);
    }


    //public static Dictionary<string, string> GetCompressionConfig(string filePath)
    //{
    //    Dictionary<string, string> list = new Dictionary<string, string>();
    //    //StopWatch sw=new StopWatch();
    //    //sw.Start();
    //    var d = StaticAPI.ReadFile(filePath);
    //    //Debug.Log(filePath);
    //    //Debug.Log(sw.TotalElapsed);
    //    if (d == null) return null;
    //    string zipInfo = new UTF8Encoding(true).GetString(d);
    //    if (!string.IsNullOrEmpty(zipInfo))
    //    {
    //        var zs = zipInfo.Split(',');
    //        foreach (var s in zs)
    //        {
    //            var dp = s.Split(':');
    //            if (dp.Length == 2)
    //                list[dp[0]] = dp[1];
    //        }
    //    }
    //    //Debug.Log(sw.TotalElapsed);
    //    return list;
    //}
    public static Dictionary<string, string> GetCompressionConfig(byte[] bytes)
    {
        //StopWatch sw = new StopWatch();
        //sw.Start();
        Dictionary<string, string> list = new Dictionary<string, string>();
        string zipInfo = new UTF8Encoding(true).GetString(bytes);
        //Debug.Log(sw.TotalElapsed);
        if (!string.IsNullOrEmpty(zipInfo))
        {
            var zs = zipInfo.Split(',');
            foreach (var s in zs)
            {
                var dp = s.Split(':');
                if (dp.Length == 2)
                    list[dp[0]] = dp[1];
            }
        }
        //Debug.Log(sw.TotalElapsed);
        return list;
    }
    //public static void SetCompressionConfig(string filePath, Dictionary<string, string> dData)
    //{
    //    string zipInfo = "";

    //    foreach (var s in dData)
    //    {
    //        zipInfo += s.Key + ":" + s.Value + ",";
    //    }

    //    var d = new UTF8Encoding(true).GetBytes(zipInfo);
    //    StaticAPI.WriteFile(d, filePath);
    //}
    public static void ProgressHandler(object sender, ProgressEventArgs e)
    {
        _currentCount++;
        //string s =string.Format(" _currentCount:{5},e.ContinueRunning:{0},\n e.Name:{1},\n e.PercentComplete:{2},\n e.Processed:{3},\n e.Target:{4}",e.ContinueRunning, e.Name, e.PercentComplete, e.Processed, e.Target, _currentCount);

        //string writePath ="AssetBundles/ResourcesZip.config";
        //using (FileStream fileStream = File.OpenWrite(writePath) )
        //{
        //    StreamWriter sw = new StreamWriter(fileStream);
        //    byte[] info = new UTF8Encoding(true).GetBytes(s);
        //    sw.WriteLine(info);
        //}
        //Debug.Log(s);
    }
    public static void CompletedFileHandler(object sender, ScanEventArgs e)
    {

    }

    #endregion

    //private static void UnZip(Stream inputStream, string targetDirectory, FastZipEvents zipEvents)
    //{
    //    MonoBehaviourAPIBridge.Instance.StartCoroutine(UnZipFile(inputStream, targetDirectory, string.Empty, string.Empty, null, zipEvents));
    //}

    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    public static void ZipDirectory(string folderToZip, string zipedFileName, FastZipEvents zipEvents)
    {
        ZipDirectory(folderToZip, zipedFileName, string.Empty, true, string.Empty, string.Empty, true, zipEvents);
    }
    public static void ZipDirectory(string folderToZip, string zipedFileName)
    {
        ZipDirectory(folderToZip, zipedFileName, string.Empty, true, string.Empty, string.Empty, true, null);
    }

    public static void ZipDirectory(string folderToZip, string zipedFileName, string password)
    {
        ZipDirectory(folderToZip, zipedFileName, password, true, string.Empty, string.Empty, true, null);
    }

    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="folderToZip">需要压缩的文件夹</param>
    /// <param name="zipedFileName">压缩后的Zip完整文件名（如D:\test.zip）</param>
    /// <param name="isRecurse">如果文件夹下有子文件夹，是否递归压缩</param>
    /// <param name="password">解压时需要提供的密码</param>
    /// <param name="fileRegexFilter">文件过滤正则表达式</param>
    /// <param name="directoryRegexFilter">文件夹过滤正则表达式</param>
    /// <param name="isCreateEmptyDirectories">是否压缩文件中的空文件夹</param>
    /// <param name="zipEvents">进度事件</param>
    public static void ZipDirectory(string folderToZip, string zipedFileName, string password, bool isRecurse, string fileRegexFilter, string directoryRegexFilter, bool isCreateEmptyDirectories, FastZipEvents zipEvents)
    {
        ZipConstants.DefaultCodePage = _codePage;
        FastZip fastZip = new FastZip(zipEvents);
        fastZip.CreateEmptyDirectories = isCreateEmptyDirectories;
        fastZip.Password = password;
        fastZip.UseZip64 = UseZip64.On;
        fastZip.RestoreDateTimeOnExtract = false;
        fastZip.RestoreAttributesOnExtract = false;
        //fastZip.CreateEmptyDirectories = true;
        fastZip.CreateZip(zipedFileName, folderToZip, isRecurse, fileRegexFilter, directoryRegexFilter);
    }

    private static void ProcessFileHandler(object sender, ProgressEventArgs e)
    {
        Debug.Log(string.Format(" sender:{0},e:{1}  ,Name:{2} ", sender, e.PercentComplete, e.Name));
    }

    private static FastZipEvents _FastZipEvents = new FastZipEvents();
    public static void UnZipFile(string zipedFileName, string targetDirectory)
    {
        UnZipFile(zipedFileName, targetDirectory, string.Empty, string.Empty);
    }

    public static void UnZipFile(string zipedFileName, string targetDirectory, string password)
    {
        UnZipFile(zipedFileName, targetDirectory, password, string.Empty);
    }

    /// <summary>
    /// 解压缩文件
    /// </summary>
    /// <param name="zipedFileName">Zip的完整文件名（如D:\test.zip）</param>
    /// <param name="targetDirectory">解压到的目录</param>
    /// <param name="password">解压密码</param>
    /// <param name="fileFilter">文件过滤正则表达式</param>
    public static void UnZipFile(string zipedFileName, string targetDirectory, string password, string fileFilter)
    {
        ZipConstants.DefaultCodePage = _codePage;
        FastZip fastZip = new FastZip();
        fastZip.Password = password;
        fastZip.ExtractZip(zipedFileName, targetDirectory, fileFilter);
    }

    /// <summary>
    /// 解压缩文件
    /// </summary>
    /// <param name="inputStream">解压来源.流</param>
    /// <param name="targetDirectory">解压到的目录</param>
    /// <param name="password">解压密码</param>
    /// <param name="fileFilter">文件过滤正则表达式</param>
    /// <param name="zipEvents">进度事件</param>
    public static IEnumerator UnZipFile(MonoBehaviour mono, Stream inputStream, string targetDirectory, string password, string fileFilter, Action<long,long, string> actionProgress, FastZipEvents zipEvents = null)
    {
        ZipConstants.DefaultCodePage = _codePage;
        //Debug.Log("ZipConstants.DefaultCodePage: " + ZipConstants.DefaultCodePage);
        FastZip fastZip = new FastZip(zipEvents);
        fastZip.Password = password;
        fastZip.UseZip64 = UseZip64.On;
        fastZip.RestoreDateTimeOnExtract = false;
        fastZip.RestoreAttributesOnExtract = false;
        //fastZip.CreateEmptyDirectories = true;
        yield return mono.StartCoroutine(fastZip.ExtractZipIEnumerator(inputStream, targetDirectory, FastZip.Overwrite.Always, null, fileFilter, string.Empty,false, true, 10, (i,count, s) => actionProgress(i,count, s)));

        //同步
        ////fastZip.ExtractZip(inputStream, targetDirectory, FastZip.Overwrite.Always, null, fileFilter,string.Empty,true,true);
        ////yield break;
    }
}
