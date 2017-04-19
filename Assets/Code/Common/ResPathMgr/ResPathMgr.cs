/*
 * Author       :yeluo
 * Create Time  :2014-12-29 09:32
 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 资源路径管理器
/// </summary>
public class ResPathMgr
{

    #region === 资源路径管理器私有属性集合
    
    /// <summary>
    /// 资源安装包路径
    /// </summary>
    private static string ResPackageBasePath;
    /// <summary>
    /// 资源安全沙箱路径
    /// </summary>
    private static string ResPersistentBasePath;
    /// <summary>
    /// 平台文件夹
    /// </summary>
    private static string PlatformFolder;
    /// <summary>
    /// 音效资源的路径
    /// </summary>
    public static string AudioPath;
    /// <summary>
    /// 文本资源的路径
    /// </summary>
    public static string TextPath;

    #endregion


    #region === GetResPath(string typeFolder, string resName) + 获取资源路径
    /// <summary>
    /// 获取资源路径
    /// </summary>
    /// <param name="typeFolder">文件夹的类型</param>
    /// <param name="resName">资源名称</param>
    public static string GetResPath(string typeFolder, string resName)
    {
        return PlatformFolder + typeFolder + resName;
    } 
    #endregion


    #region === Init() + 资源路径初始化
    /// <summary>
    /// 资源路径初始化
    /// </summary>
    public static void Init()
    {
        if (((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) || (Application.platform == RuntimePlatform.OSXEditor))
        {
            ResPackageBasePath = "file:///" + Application.dataPath + "/../../res/";
            ResPersistentBasePath = "file:///" + Application.persistentDataPath + "/res/";
            PlatformFolder = "android/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ResPackageBasePath = "file:///" + Application.dataPath + "/res/";
            ResPersistentBasePath = "file:///" + Application.persistentDataPath + "/res/";
            PlatformFolder = "ios/";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            ResPackageBasePath = "jar:file://" + Application.dataPath + "!/assets/res/";
            ResPersistentBasePath = "file:///" + Application.persistentDataPath + "/res/";
            PlatformFolder = "android/";
        }
        else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
        {
            ResPackageBasePath = Application.dataPath + "/";
            ResPersistentBasePath = ResPackageBasePath;
            PlatformFolder = "webplayer/";
        }
        AudioPath = "audio/";
        TextPath = "text/";
    } 
    #endregion


    #region === GetPersistentFolder() + 获取安全沙箱的文件夹
    /// <summary>
    /// 获取安全沙箱的文件夹
    /// </summary>
    public static string GetPersistentFolder()
    {
        string path = string.Empty;
        if (((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) || (Application.platform == RuntimePlatform.OSXEditor))
        {
            path = Application.persistentDataPath + "/res/webplayer";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Application.persistentDataPath + "/res/ios";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/res/android";
        }
        return path;
    } 
    #endregion


}
