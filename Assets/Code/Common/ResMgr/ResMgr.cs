//#define compress

using System.Diagnostics;
using System.Threading;
using Common.Util;
using Logic.Game;
using Logic.Pool.Controller;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Common.ResMgr
{
    public struct LoadAssetbundle
    {
        public string assetbundlePath;
        public string path;
        public System.Action<string, Object> loadAction;
        public System.Type type;
        private byte _prior;//资源加载优先级，越大越先加载。
        public byte Prior
        {
            get { return _prior; }
            set { _prior = value; }
        }
    }

    public sealed class ResMgr : SingletonMono<ResMgr>
    {
        private const string ATLAS_ASSET_SUFFIX = "_asset";

        [NoToLua]
        public System.Action<string> CompleteAction;
        [NoToLua]
        public System.Action<string, float> ProgressAction;
        [NoToLua]
        public System.Action<string, ResLoaderObj> ErrorAction;

        [NoToLua]
        public ManifestInfo remoteManifest;
        [NoToLua]
        public ManifestInfo localManifest;

        [NoToLua]
        public AssetBundleManifest assetBundleManifest;

        private byte NUM_LOADING_MAX = 1;//同时可以启用的www下载个数
        private const byte TIMEOUT = 60;
        private const byte LOAD_ASSETBUNDLE_MANIFEST_MAX = 5;//加载assetBundleManifest最大次数
        private byte loadAssetbundleManifestCount = 0;
        private GameObject _loaderRoot;//把加载中的ResLoader放到这个节点下面

        private List<ResLoader> _waitingList;//等待队列
        private List<ResLoader> _loadingList;
        private List<string> _loadedRes;//加载完成资源列表
        private Dictionary<string, AssetsObj> _resDic;//资源列表
        private Dictionary<string, AssetBundleObj> _assetBundleDic;
        private List<LoadAssetbundle> _waitingABs = new List<LoadAssetbundle>();
        private List<LoadAssetbundle> _loadingABs = new List<LoadAssetbundle>();
        private const int MAX_LOAD_AB_NUM = 1;
        private int _loadFailCount = 0;
        private List<string> _ignorePaths = new List<string>();
        private bool _loadIgnorePaths = false;
        private List<string> ignorePaths
        {
            get
            {
                if (!_loadIgnorePaths)
                {
                    _loadIgnorePaths = true;
                    LuaTable lt = LuaScriptMgr.Instance.GetLuaTable("gamedataTable.ignorePaths");
                    string[] strs = lt.ToArray<string>();
                    for (int i = 0, length = strs.Length; i < length; i++)
                    {
                        _ignorePaths.Add(strs[i]);
                    }
                }
                return _ignorePaths;
            }
        }

        private List<string> _preFightPaths = new List<string>();
        private bool _loadPreFightPaths = false;
        public List<string> preFightPaths
        {
            get
            {
                if (!_loadPreFightPaths)
                {
                    _loadPreFightPaths = true;
                    LuaTable lt = LuaScriptMgr.Instance.GetLuaTable("gamedataTable.preFightPaths");
                    string[] strs = lt.ToArray<string>();
                    for (int i = 0, length = strs.Length; i < length; i++)
                    {
                        _preFightPaths.Add(strs[i]);
                    }
                    //_preFightPaths.AddRange(strs);
                }
                return _preFightPaths;
            }
        }

#if UNITY_EDITOR
        [NoToLua]
        public List<string> loadedRes
        {
            get { return _loadedRes; }
        }

        [NoToLua]
        public Dictionary<string, AssetsObj> resDic
        {
            get { return _resDic; }
        }

        [NoToLua]
        public Dictionary<string, AssetBundleObj> assetBundleDic
        {
            get { return _assetBundleDic; }
        }
        [NoToLua]
        public List<ResLoader> waitingList { get { return _waitingList; } }
        [NoToLua]
        public List<ResLoader> loadingList { get { return _loadingList; } }
        [NoToLua]
        public int loadFailCount { get { return _loadFailCount; } }
        [NoToLua]
        public List<LoadAssetbundle> waitingABs { get { return _waitingABs; } }
        [NoToLua]
        public List<LoadAssetbundle> loadingABs { get { return _loadingABs; } }
#endif

        private void Awake()
        {
            instance = this;
            _loaderRoot = new GameObject("_loaderRoot");
            _loaderRoot.transform.parent = transform;
            _waitingList = new List<ResLoader>();
            _loadingList = new List<ResLoader>();
            _loadedRes = new List<string>();
            _resDic = new Dictionary<string, AssetsObj>();
            //_frequentlyResDic = new Dictionary<string, Object>();
            //_fightResDic = new Dictionary<string, Object>();
            _assetBundleDic = new Dictionary<string, AssetBundleObj>();
        }

        void OnDestroy()
        {
            RemoveAllRes();
        }

        #region 公开方法

        public string LoadStreamingAssetText(string path)
        {
            string fullPath = ResUtil.GetStreamingAssetsPath(path);
            string content = string.Empty;
            //try
            //{

            //Stopwatch sw = new Stopwatch();
            //sw.Start();


            var bytes = System.IO.File.ReadAllBytes(fullPath);
            //this.DelayAction(1, () =>
            //{
            string toPath = ResUtil.GetLocalPath(path);
            Thread t = new Thread(() => ResUtil.Save2LocalFullPath(toPath, bytes)) { IsBackground = true };
                t.Start();
           // });
           
            //content = System.IO.File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
            content = System.Text.Encoding.UTF8.GetString(bytes);
           // UnityEngine.Debug.LogError("------sw:" + sw.ToString() + "    path:" + path + "   sw.ElapsedMilliseconds:" + sw.ElapsedMilliseconds);
            //}
            //catch (System.Exception e)
            //{
            //    Debugger.LogError("read {0} error,{1}", path, e.StackTrace);
            //}
            return EncryptUtil.DESDecryptString(content, GameConfig.encryptKey);
        }
        public string LoadText(string path)
        {
            string fullPath = ResUtil.GetLocalPath(path);
            string content = string.Empty;
            //try
            //{
            content = System.IO.File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
            //}
            //catch (System.Exception e)
            //{
            //    Debugger.LogError("read {0} error,{1}", path, e.StackTrace);
            //}
            return EncryptUtil.DESDecryptString(content, GameConfig.encryptKey);
        }

        public byte[] LoadBytes(string path)
        {
            byte[] bytes = default(byte[]);
            string content = string.Empty;

            content = LoadText(path);
            //try
            //{
            bytes = System.Text.Encoding.UTF8.GetBytes(content);
            //}
            //catch (System.Exception e)
            //{
            //    Debugger.LogError("read {0} error,{1}", path, e.StackTrace);
            //}
            return bytes;
        }

        public Object Load(string path)
        {
            return Load<Object>(path);
        }

        public Object LoadMaterial(string path)
        {
            return Load<UnityEngine.Material>(path);
        }

        /// <summary>
        /// 普通资源加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        [NoToLua]
        public T Load<T>(string path) where T : Object
        {
            //Debugger.Log("load  " + path);
            T t = default(T);
            if (localManifest == null)
                t = LoadLocal<T>(path);
            else
            {
                string assetPath = string.Empty;
                if (typeof(T) == typeof(Sprite))
                {
                    assetPath = GetSpriteAssetPath(path);
                }
                else
                    assetPath = path;
                string assetBundlePath = localManifest.GetAssetBundlePathByRealPath(assetPath);
                if (string.IsNullOrEmpty(assetBundlePath))
                    t = LoadLocal<T>(path);
                else
                {
                    if (typeof(T) == typeof(Sprite))
                        t = GetSprite(assetBundlePath, GetResName(path)) as T;
                    else
                        t = GetRes<T>(assetBundlePath, GetResName(path));
                }
            }
            return t;
        }

        public Sprite LoadSprite(string path)
        {
            return Load<Sprite>(path);
        }

        public void Load(string path, System.Action<Object> loadAction)
        {
            Load<Object>(path, loadAction, 0);
        }

        [NoToLua]
        public void Load<T>(string path, System.Action<T> loadAction, byte prior) where T : Object
        {
            if (localManifest == null)
            {
                T t = LoadLocal<T>(path);
                if (loadAction != null)
                    loadAction(t);
            }
            else
            {
                string assetPath = string.Empty;
                if (typeof(T) == typeof(Sprite))
                {
                    assetPath = GetSpriteAssetPath(path);
                }
                else
                    assetPath = path;
                string assetBundlePath = localManifest.GetAssetBundlePathByRealPath(assetPath);
                if (string.IsNullOrEmpty(assetBundlePath))
                {
                    T t = LoadLocal<T>(path);
                    if (loadAction != null)
                        loadAction(t);
                }
                else
                {
                    #region add to abs load list
                    Debugger.Log(path);
                    LoadAssetbundle loadAssetbundle = new LoadAssetbundle();
                    loadAssetbundle.assetbundlePath = assetBundlePath;
                    loadAssetbundle.path = path;
                    loadAssetbundle.type = typeof(T);
                    loadAssetbundle.Prior = prior;
                    loadAssetbundle.loadAction = (abPath, result) =>
                    {
                        T t = result as T;
                        if (t == null)
                            _loadFailCount++;
                        if (loadAction != null)
                            loadAction(t);
                        for (int i = 0, count = _loadingABs.Count; i < count; i++)
                        {
                            LoadAssetbundle lab = _loadingABs[i];
                            if (lab.assetbundlePath == abPath)
                            {
                                _loadingABs.RemoveAt(i);
                                break;
                            }
                        }

                        if (_loadingABs.Count < MAX_LOAD_AB_NUM + _loadFailCount && _waitingABs.Count > 0)
                            LoadRes();
                    };
                    _waitingABs.Add(loadAssetbundle);
                    _waitingABs.Sort(ResUtil.LoadAssetBundleSort);
                    if (_loadingABs.Count < MAX_LOAD_AB_NUM + _loadFailCount)
                        LoadRes();
                    #endregion
                }
            }
        }

        public void LoadSprite(string path, System.Action<Sprite> loadAction)
        {
            Load<Sprite>(path, loadAction, 0);
        }

        #region load sprite
        private Sprite LoadLocalSprite(string path)
        {
            //            Debugger.Log("load sprite {0} from asset!", path);
            Sprite result = null;
            string assetPath = string.Concat(GetSpriteAssetPath(path), ATLAS_ASSET_SUFFIX);
            string spriteName = GetResName(path);
            AssetPacker assetPacker = Resources.Load<AssetPacker>(assetPath);
            if (assetPacker)
                result = assetPacker.GetSprite(spriteName);
            return result;
        }

        private Sprite GetSprite(string subPath, string resName)
        {
            UnityEngine.Object asset = null;
            if (ExistInMemory(subPath))//是否存在判断是为了避免重复加载ab
            {
                asset = GetResFromMemory(subPath, resName);
                if (asset != null)
                    return asset as Sprite;
                return null;
            }
            if (ExistInAssetBundle(subPath))
            {
                asset = GetResFromAssetBundle(subPath, resName);
                if (asset != null)
                {
                    return asset as Sprite;
                }
                return null;
            }
            AssetsObj ao = GetResFromDisk(subPath);
            if (ao != null)
            {
                asset = ao.GetAssetByName(resName);
                if (asset != null)
                {
                    return asset as Sprite;
                }
            }
            return null;
        }

        private string GetSpriteAssetPath(string fullPath)
        {
            int index = fullPath.LastIndexOf("/");
            if (index > 0)
                return fullPath.Substring(0, index);
            return "invalid:" + fullPath;
        }

        private string GetResName(string fullPath)
        {
            int index = fullPath.LastIndexOf("/");
            return fullPath.Substring(index + 1);
        }
        #endregion

        private T LoadLocal<T>(string path) where T : Object
        {
            string assetName = path.Substring(path.LastIndexOf("/") + 1);
            T t = GetResFromMemory(path, assetName) as T;
            if (t != null)
                return t;
            // T t = default(T);
            if (typeof(T) == typeof(Sprite))
                t = LoadLocalSprite(path) as T;
            else
                t = Resources.Load<T>(path);
            if (t != null)
            {
                AssetsObj ao = new AssetsObj(new List<Object>() { t });
                if (!_resDic.ContainsKey(path))
                    _resDic.Add(path, ao);
            }
            return t;
        }

        [NoToLua]
        public Object[] LoadAll(string path)
        {
            return Resources.LoadAll(path);
        }

        #region 获取资源方法
        private void LoadRes()
        {
            if (_waitingABs.Count > 0)
            {
                LoadAssetbundle loadAssetbundle = _waitingABs.First();
                _waitingABs.RemoveAt(0);
                //bool isContanis = false;
                //for (int i = 0, count = _loadingABs.Count; i < count; i++)
                //{
                //    LoadAssetbundle lab = _loadingABs[i];
                //    if (lab.assetbundlePath == loadAssetbundle.assetbundlePath)
                //    {
                //        lab.loadAction += loadAssetbundle.loadAction;
                //        isContanis = true;
                //    }
                //}
                //if (!isContanis)
                //{
                _loadingABs.Add(loadAssetbundle);
                GetRes(loadAssetbundle.assetbundlePath, GetResName(loadAssetbundle.path), loadAssetbundle.loadAction);
                //}
            }
        }

        private T GetRes<T>(string subPath, string resName) where T : Object
        {
            T t = default(T);
            UnityEngine.Object result = null;
            if (ExistInMemory(subPath))//是否存在判断是为了避免重复加载ab
            {
                result = GetResFromMemory(subPath, resName);
                if (result != null)
                    t = result as T;
                return t;
            }
            if (ExistInAssetBundle(subPath))
            {
                result = GetResFromAssetBundle(subPath, resName);
                if (result != null)
                    t = result as T;
                return t;
            }
            AssetsObj ao = GetResFromDisk(subPath);
            if (ao != null)
            {
                result = ao.GetAssetByName(resName);
                if (result != null)
                    t = result as T;
            }
            return t;
        }

        private void GetRes<T>(string subPath, string resName, System.Action<string, T> loadAction) where T : Object
        {
            UnityEngine.Object result = null;
            if (ExistInMemory(subPath))//是否存在判断是为了避免重复加载ab
            {
                result = GetResFromMemory(subPath, resName);
                //Debugger.Log(result == null);
                T t = default(T);
                if (result != null)
                    t = result as T;
                if (loadAction != null)
                    loadAction(subPath, t);
                return;
            }
            if (ExistInAssetBundle(subPath))
            {
                result = GetResFromAssetBundle(subPath, resName);
                T t = default(T);
                if (result != null)
                    t = result as T;
                if (loadAction != null)
                    loadAction(subPath, t);
                return;
            }
            //Debugger.Log("GetResObjectFromDisk:" + subPath + ":" + resName);
            GetResFromDisk(subPath, resName, loadAction);
        }

        private AssetsObj GetResFromMemory(string subPath)
        {
            AssetsObj result = null;
            if (_resDic.TryGetValue(subPath, out result))
            {
                //Debugger.Log(result.ToString());
                return result;
            }
            return result;
        }

        private UnityEngine.Object GetResFromMemory(string subPath, string resName)
        {
            AssetsObj ao = GetResFromMemory(subPath);
            if (ao == null)
                return null;
            UnityEngine.Object result = ao.GetAssetByName(resName);
            return result;
        }

        private bool ExistInMemory(string subPath)
        {
            return _resDic.ContainsKey(subPath);
        }

        private UnityEngine.Object GetResFromAssetBundle(string subPath, string resName)
        {
            //Debugger.Log(subPath + ResUtil.ASSET_BUNDLE_SUFFIX); 
            UnityEngine.Object result = null;
            try
            {
                AssetBundleObj assetBundle = null;
                if (_assetBundleDic.TryGetValue(subPath + ResUtil.ASSET_BUNDLE_SUFFIX, out assetBundle))
                {
                    if (assetBundle.assetBundle != null)
                    {
                        //result = assetBundle.assetBundle.LoadAsset(resName);
                        var namess = assetBundle.assetBundle.GetAllAssetNames();
                        Debugger.Log("namess count {0}  !", namess.Length);
                        for (int i = 0; i < namess.Length; i++)
                        {
                            Debugger.Log("LoadAllAssets:+++++++++" + namess[i]);
                            //Debugger.Log("assetBundle.assetBundle.mainAsset is {0}", ((assetBundle.assetBundle.mainAsset == null) ? "null" : "not null"));
                            //if (assetBundle.assetBundle.mainAsset)
                            //    Debugger.Log("LoadAllAssetsxxxxxxxxxx" + assetBundle.assetBundle.mainAsset.name);
                            //Debugger.Log("LoadAllAssets:---------" + assetBundle.assetBundle.LoadAsset(namess[i]).name);
                        }
                        AssetsObj ao = new AssetsObj(assetBundle.assetBundle.LoadAllAssets());
                        result = ao.GetAssetByName(resName);
                        _resDic.Add(subPath, ao);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debugger.LogError(string.Format("load {0} from assetbundle is fail!message:{1}", subPath, e.StackTrace));
            }
            return result;
        }

        private bool ExistInAssetBundle(string subPath)
        {
            return _assetBundleDic.ContainsKey(subPath + ResUtil.ASSET_BUNDLE_SUFFIX);
        }

        private void GetResFromDisk<T>(string subPath, string resName, System.Action<string, T> loadAction) where T : Object
        {
            StartCoroutine(GetResFromDiskCoroutine(subPath, (ao) =>
             {
                 T t = default(T);
                 if (ao != null)
                 {
                     UnityEngine.Object result = ao.GetAssetByName(resName);
                     t = result as T;
                     Debugger.Log("load res {0} success !", subPath);
                     if (!_resDic.ContainsKey(subPath))
                         _resDic.Add(subPath, ao);
                     if (loadAction != null)
                         loadAction(subPath, t);
                 }
                 else
                 {
                     Debugger.LogError("load res {0} fail !", subPath);
                     if (loadAction != null)
                         loadAction(subPath, null);
                 }
             }));
        }

        private IEnumerator GetResFromDiskCoroutine(string subPath, System.Action<AssetsObj> loadAssetsObjAction)
        {
            AssetsObj result = null;
            string localPath;

            if (ResUtil.ExistsInLocal(subPath + ResUtil.ASSET_BUNDLE_SUFFIX, out localPath))
            {
                string url = string.Empty;
                float time = 0f;
                #region load dependenics ab
                if (assetBundleManifest == null)
                {
                    if (loadAssetsObjAction != null)
                        loadAssetsObjAction(null);
                    yield break;
                }
                string path = subPath + ResUtil.ASSET_BUNDLE_SUFFIX;
                string[] depends = assetBundleManifest.GetAllDependencies(path);
                //Debugger.Log(depends.Length + "   " + path);
                for (int i = 0, count = depends.Length; i < count; i++)
                {
                    AssetBundleObj assetBundle = null;
                    string dependSubPath = depends[i];
                    if (!_assetBundleDic.TryGetValue(dependSubPath, out assetBundle))
                    {
                        url = ResUtil.GetLocalWWWPath(ResUtil.GetLocalPath(dependSubPath));
                        Debugger.Log("GetDependsFromDisk:{0},{1}", url, i.ToString());
                        WWW www = new WWW(url);
                        time = Time.realtimeSinceStartup;
                        while (!www.isDone)
                        {
                            if (Time.realtimeSinceStartup - time > TIMEOUT)
                            {
                                www.Dispose();
                                www = null;
                                Debugger.LogError("load depend res {0} timeout !", url);
                                if (loadAssetsObjAction != null)
                                    loadAssetsObjAction(null);
                                yield break;
                            }
                            yield return null;
                        }
                        if (!string.IsNullOrEmpty(www.error))
                        {
                            www.Dispose();
                            www = null;
                            Debugger.LogError("load depend res {0} error !", url);
                            if (loadAssetsObjAction != null)
                                loadAssetsObjAction(null);
                            yield break;
                        }
                        try
                        {
                            if (!_assetBundleDic.ContainsKey(dependSubPath))
                            {
                                assetBundle = new AssetBundleObj();
                                assetBundle.subPath = dependSubPath;
                                assetBundle.dependerPath = path;
                                if (www.bytes == null)
                                {
                                    Debugger.LogError(string.Format("read www.bytes fail {0}", dependSubPath));
                                    throw new System.Exception();
                                }
                                AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(www.bytes);
                                if (ab == null)
                                {
                                    Debugger.LogError(string.Format("ab is null read ab fail {0}", dependSubPath));
                                    throw new System.Exception();
                                }
                                assetBundle.assetBundle = ab;
                                _assetBundleDic.Add(dependSubPath, assetBundle);
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debugger.LogError(string.Format("load depend file from dependSubPath:{0} fail!message:{1}", dependSubPath, e.StackTrace));
                        }
                        www.Dispose();
                        www = null;
                        yield return null;
                        /*  ---------AssetBundleCreateRequest
                        AssetBundleCreateRequest abcr = AssetBundle.CreateFromMemory(System.IO.File.ReadAllBytes(ResUtil.GetLocalPath(dependSubPath)));
                        while (!abcr.isDone)
                        {
                            if (Time.realtimeSinceStartup - time > TIMEOUT)
                            {
                                if (loadAssetsObjAction != null)
                                    loadAssetsObjAction(null);
                                yield break;
                            }
                            yield return null;
                        }
                        if (!_assetBundleDic.ContainsKey(dependSubPath))
                        {
                            assetBundle = new AssetBundleObj();
                            assetBundle.subPath = dependSubPath;
                            assetBundle.dependerPath = path;
                            assetBundle.assetBundle = abcr.assetBundle;
                            //assetBundle.assetBundle = AssetBundle.CreateFromFile(ResUtil.GetLocalPath(subPath));
                            _assetBundleDic.Add(dependSubPath, assetBundle);
                        }*/
                    }
                }
                Debugger.Log("load depends assetbundle success !");
                #endregion

                url = ResUtil.GetLocalWWWPath(localPath);
                //Debugger.Log("GetResFromDisk:" + url);
                WWW wwwRes = new WWW(url);
                time = Time.realtimeSinceStartup;
                while (!wwwRes.isDone)
                {
                    if (Time.realtimeSinceStartup - time > TIMEOUT)
                    {
                        wwwRes.Dispose();
                        wwwRes = null;
                        Debugger.LogError("load res {0} timeout !", url);
                        if (loadAssetsObjAction != null)
                            loadAssetsObjAction(null);
                        yield break;
                    }
                    yield return null;
                }
                if (!string.IsNullOrEmpty(wwwRes.error))
                {
                    wwwRes.Dispose();
                    wwwRes = null;
                    Debugger.LogError("load res {0} error !", url);
                    if (loadAssetsObjAction != null)
                        loadAssetsObjAction(null);
                    yield break;
                }
                try
                {
                    //AssetBundle ab = wwwRes.assetBundle;
                    if (wwwRes.bytes == null)
                    {
                        Debugger.LogError(string.Format("read www.bytes fail {0}", localPath));
                        throw new System.Exception();
                    }
                    AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(wwwRes.bytes);

                    var namess = ab.GetAllAssetNames();
                    Debugger.Log("namess count {0}  !", namess.Length);
                    for (int i = 0; i < namess.Length; i++)
                    {
                        Debugger.Log("LoadAllAssets:+++++++++" + namess[i]);
                        //Debugger.Log("ab.mainAsset is {0}", ((ab.mainAsset == null) ? "null" : "not null"));
                        //if (ab.mainAsset)
                        //    Debugger.Log("LoadAllAssetsxxxxxxxxxx" + ab.mainAsset.name);
                        //Debugger.Log("LoadAllAssets:---------" + ab.LoadAsset(namess[i]).name);
                    }
                    result = new AssetsObj(ab.LoadAllAssets());
                    ab.Unload(false);
                    wwwRes.Dispose();
                    wwwRes = null;
                    //Debugger.Log("load res assetbundle success !");
                }
                catch (System.Exception e)
                {
                    Debugger.LogError(string.Format("load depend file from dependSubPath:{0} fail!message:{1}", localPath, e.StackTrace));
                }
                finally
                {
                    if (loadAssetsObjAction != null)
                        loadAssetsObjAction(result);
                }
            }
            else
            {
                Debugger.LogError("not exist res {0} !", localPath);
                if (loadAssetsObjAction != null)
                    loadAssetsObjAction(null);
            }
        }

        [NoToLua]
        public void PreLoadAssetBundleManifest()
        {
            StartCoroutine(PreLoadAssetBundleManifestCoroutine(PreLoadAssetBundleManifestHandler));
        }

        private void PreLoadAssetBundleManifestHandler(bool flag)
        {
            if (!flag)
            {
                if (loadAssetbundleManifestCount >= LOAD_ASSETBUNDLE_MANIFEST_MAX)
                {
                    Debugger.Log("loadAssetbundleManifest {0} times,fail", loadAssetbundleManifestCount);
                    Logic.UI.Tips.View.ConfirmTipsView.Open(Common.Localization.Localization.Get("ui.common.tips.relogin"), Logic.Game.Controller.GameController.instance.ReLoadMainScene);
                }
            }
        }

        private IEnumerator PreLoadAssetBundleManifestCoroutine(System.Action<bool> loadManifestAction)
        {
            loadAssetbundleManifestCount++;
            string manifestPath = ResUtil.GetLocalAssetBundleManifestPath();
            string url = ResUtil.GetLocalWWWPath(manifestPath);
            //Debugger.Log("GetManifestPathFromDisk:" + url);
            WWW www = new WWW(url);
            float time = Time.realtimeSinceStartup;
            while (!www.isDone)
            {
                if (Time.realtimeSinceStartup - time > TIMEOUT)
                {
                    www.Dispose();
                    www = null;
                    if (loadManifestAction != null)
                        loadManifestAction(false);
                    yield break;
                }
                yield return null;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                www.Dispose();
                www = null;
                if (loadManifestAction != null)
                    loadManifestAction(false);
                yield break;
            }
            AssetBundle manifestBundle = www.assetBundle;
            if (manifestBundle == null)
            {
                www.Dispose();
                www = null;
                if (loadManifestAction != null)
                    loadManifestAction(false);
                yield break;
            }
            assetBundleManifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
            manifestBundle.Unload(false);
            www.Dispose();
            www = null;
            //Debugger.Log(assetBundleManifest == null);
            if (assetBundleManifest == null)
            {
                if (loadManifestAction != null)
                    loadManifestAction(false);
                yield break;
            }

            //Debugger.Log("load assetbundleManifest assetbundle success !");
            if (loadManifestAction != null)
                loadManifestAction(true);
        }

        private AssetsObj GetResFromDisk(string subPath)
        {
            AssetsObj result = null;
            string localPath;
            if (!ResUtil.ExistsInLocal(subPath + ResUtil.ASSET_BUNDLE_SUFFIX, out localPath))
            {
                return null;
            }
            try
            {
                if (!GetDependencies(subPath + ResUtil.ASSET_BUNDLE_SUFFIX))
                    return null;
                Debugger.Log("GetResFromDisk:" + localPath);
#if compress
                AssetBundle ab = AssetBundle.CreateFromFile(localPath);
#else
                byte[] bytes = System.IO.File.ReadAllBytes(localPath);
                if (bytes == null)
                {
                    Debugger.LogError(string.Format("read file fail {0}", localPath));
                    throw new System.Exception();
                }
                AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(bytes);
#endif
                if (ab != null)
                {
                    Debugger.Log("1 ab is not null start load objects!");
                    var namess = ab.GetAllAssetNames();
                    Debugger.Log("namess count {0}  !", namess.Length);
                    for (int i = 0; i < namess.Length; i++)
                    {
                        Debugger.Log("LoadAllAssets:+++++++++" + namess[i]);
                        //Debugger.Log("ab.mainAsset is {0}", ((ab.mainAsset == null) ? "null" : "not null"));
                        //if (ab.mainAsset)
                        //    Debugger.Log("LoadAllAssetsxxxxxxxxxx" + ab.mainAsset.name);
                        //Debugger.Log("LoadAllAssets:---------" + ab.LoadAsset(namess[i]).name);
                    }
                    result = new AssetsObj(ab.LoadAllAssets());
                    Debugger.Log("2 load objects success !");
                    ab.Unload(false);
                    Debugger.Log("3 Unload false success !");
                    //if (subPath == "sprite/skill")
                    //    Debugger.Log("subpath:-0-------------------------------");
                    _resDic.Add(subPath, result);
                    Debugger.Log("load res assetbundle success !");
                }
                else
                    Debugger.LogError(string.Format("load file from subPath:{0} fail!message:file is empty !", ResUtil.GetLocalPath(subPath)));
            }
            catch (System.Exception e)
            {
                Debugger.LogError(string.Format("load file from subPath:{0} fail!message:{1}", subPath, e.StackTrace));
            }
            return result;
        }

        private bool GetDependencies(string path)
        {
            string subPath = string.Empty;
            try
            {
                if (assetBundleManifest == null)
                {
                    string manifestPath = ResUtil.GetLocalAssetBundleManifestPath();
                    try
                    {
                        //byte[] bytes = LoadBytes(manifestPath);
                        Debugger.Log("GetManifestPathFromDisk:" + manifestPath);
#if compress
                        AssetBundle manifestBundle = AssetBundle.CreateFromFile(manifestPath);
#else
                        byte[] bytes = System.IO.File.ReadAllBytes(manifestPath);
                        if (bytes == null)
                        {
                            Debugger.LogError(string.Format("read file fail {0}", manifestPath));
                            throw new System.Exception();
                        }
                        AssetBundle manifestBundle = AssetBundle.CreateFromMemoryImmediate(bytes);
#endif
                        //Debugger.Log(manifestBundle == null);
                        if (manifestBundle == null)
                            return false;
                        assetBundleManifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
                        manifestBundle.Unload(false);
                        //Debugger.Log(assetBundleManifest == null);
                        if (assetBundleManifest == null)
                            return false;
                    }
                    catch (System.Exception e)
                    {
                        Debugger.LogError(string.Format("load file from path:{0} fail!message:{1}", manifestPath, e.StackTrace));
                        return false;
                    }
                }
                Debugger.Log("load assetbundleManifest assetbundle success !");
                string[] depends = assetBundleManifest.GetAllDependencies(path);
                for (int i = 0, count = depends.Length; i < count; i++)
                {
                    AssetBundleObj assetBundle = null;
                    subPath = depends[i];
                    if (!_assetBundleDic.TryGetValue(subPath, out assetBundle))
                    {
                        Debugger.Log("GetDependsFromDisk{0}:" + subPath, i.ToString());
                        //加载所有的依赖文件;                        
                        assetBundle = new AssetBundleObj();
                        assetBundle.subPath = subPath;
                        assetBundle.dependerPath = path;
#if compress
                        AssetBundle ab = AssetBundle.CreateFromFile(ResUtil.GetLocalPath(subPath));
#else
                        byte[] bytes = System.IO.File.ReadAllBytes(ResUtil.GetLocalPath(subPath));
                        if (bytes == null)
                        {
                            Debugger.LogError(string.Format("read bytes fail {0}", ResUtil.GetLocalPath(subPath)));
                            throw new System.Exception();
                        }
                        AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(bytes);
#endif
                        //assetBundle.assetBundle = AssetBundle.CreateFromFile(ResUtil.GetLocalPath(subPath));
                        if (ab == null)
                        {
                            Debugger.LogError(string.Format("ab is null read ab fail {0}", ResUtil.GetLocalPath(subPath)));
                            throw new System.Exception();
                        }
                        assetBundle.assetBundle = ab;
                        _assetBundleDic.Add(subPath, assetBundle);
                    }
                }
                Debugger.Log("load depends assetbundle success !");
            }
            catch (System.Exception e)
            {
                Debugger.LogError(string.Format("load file from subPath:{0} fail!message:{1}", subPath, e.StackTrace));
                return false;
            }
            return true;
        }
        #endregion

        private bool ExistInIgnorePath(string path)
        {
            for (int i = 0, count = ignorePaths.Count; i < count; i++)
            {
                if (path.Contains(ignorePaths[i]))
                    return true;
            }
            return false;
        }

        private void RemoveAssetBundles(bool ignore, List<string> excepts)
        {
            Dictionary<string, AssetBundleObj> tempDic = null;
            if (ignore)
                tempDic = new Dictionary<string, AssetBundleObj>();
            foreach (var kvp in _assetBundleDic)
            {
                if (PoolController.instance.ExsitInPool(kvp.Key))
                {
                    tempDic.Add(kvp.Key, kvp.Value);
                    continue;
                }
                if (excepts != null)
                {
                    if (excepts.Contains(kvp.Key))
                    {
                        tempDic.Add(kvp.Key, kvp.Value);
                        continue;
                    }
                }
                if (ignore)
                {
                    if (ExistInIgnorePath(kvp.Key))
                    {
                        tempDic.Add(kvp.Key, kvp.Value);
                        continue;
                    }
                }
                kvp.Value.assetBundle.Unload(true);
            }
            _assetBundleDic.Clear();
            if (ignore)
            {
                _assetBundleDic.AddRange(tempDic);
                tempDic.Clear();
                tempDic = null;
            }
        }

        [NoToLua]
        //移除战斗中的引用，特效，声音等等
        public void ClearRes(bool ignore)
        {
            PoolController.instance.ClearTemporaryPools();
            Dictionary<string, AssetsObj> tempDic = null;
            if (ignore)
                tempDic = new Dictionary<string, AssetsObj>();
            foreach (var kvp in _resDic)
            {
                if (PoolController.instance.ExsitInPool(kvp.Key))
                {
                    tempDic.Add(kvp.Key, kvp.Value);
                    continue;
                }
                if (ignore)
                {
                    if (ExistInIgnorePath(kvp.Key))
                    {
                        tempDic.Add(kvp.Key, kvp.Value);
                        continue;
                    }
                }
                AssetsObj ao;
                ao = kvp.Value;
                if (ao != null)
                {
                    ao.Clear();
                    ao = null;
                }
            }
            _resDic.Clear();
            if (ignore)
            {
                _resDic.AddRange(tempDic);
                tempDic.Clear();
                tempDic = null;
            }
            List<string> excepts = new List<string>();
            if (assetBundleManifest != null)
            {
                foreach (var kvp in _resDic)
                {
                    string[] depends = assetBundleManifest.GetAllDependencies(kvp.Key + ResUtil.ASSET_BUNDLE_SUFFIX);
                    if (depends != null && depends.Length > 0)
                    {
                        for (int i = 0, length = depends.Length; i < length; i++)
                        {
                            excepts.Add(depends[i]);
                        }
                    }
                }
            }
            RemoveAssetBundles(true, excepts);
            excepts.Clear();
            excepts = null;
        }

        //[NoToLua]
        //public void RemoveRes()
        //{
        //    //RemoveResLoader();
        //    _waitLoadQueue.Clear();
        //    ClearRes(true);
        //    RemoveAssetBundles(true);
        //}

        //清理所有资源
        [NoToLua]
        public void RemoveAllRes()
        {
            AssetsObj ao;
            //foreach (var kv in _loadedRes)
            //{
            //    ao = kv.Value;
            //    if (ao != null)
            //    {
            //        ao.Clear();
            //        ao = null;
            //    }
            //}
            _loadedRes.Clear();

            foreach (var kv in _resDic)
            {
                ao = kv.Value;
                if (ao != null)
                {
                    ao.Clear();
                    ao = null;
                }
            }
            _resDic.Clear();

            foreach (var kvp in _assetBundleDic)
            {
                kvp.Value.assetBundle.Unload(true);
            }

            RemoveResLoader();
            _waitingABs.Clear();
            _loadingABs.Clear();
        }
        #endregion

        #region 资源下载
        //加载一个资源
        [NoToLua]
        public void PushRes(ResLoaderObj resObj)
        {
            if (_loadedRes.Contains(resObj.SubPath))
            {
                Debugger.Log(string.Format("加载重复：loaded"));
                //Observers.Facade.Instance.SendNotification(resObj.SubPath, GetRes(resObj.SubPath),TYPE_COMPLETE);
                if (CompleteAction != null)
                    CompleteAction(resObj.SubPath);
                return;
            }
            ResLoader resLoader;
            int i;
            int count;
            for (i = 0, count = _waitingList.Count; i < count; i++)
            {
                resLoader = _waitingList[i];
                if (resLoader.ResObj.SubPath == resObj.SubPath)
                {
                    Debugger.Log(string.Format("加载重复：beforeLoad"));
                    return;
                }
            }

            for (i = 0, count = _loadingList.Count; i < count; i++)
            {
                resLoader = _loadingList[i];
                if (resLoader.ResObj.SubPath == resObj.SubPath)
                {
                    Debugger.Log(string.Format("加载重复：loading"));
                    return;
                }
            }
            resLoader = _loaderRoot.AddComponent<ResLoader>();
            resLoader.ResObj = resObj;
            _waitingList.Add(resLoader);
            _waitingList.Sort(CompareResLoaderPrior);
            TryLoadRes();
        }

        //尝试加载一个资源列队中的资源
        private void TryLoadRes()
        {
            if (_waitingList.Count <= 0 || _loadingList.Count >= NUM_LOADING_MAX)
                return;
            ResLoader resLoader = _waitingList[0];
            _waitingList.RemoveAt(0);
            _loadingList.Add(resLoader);
            resLoader.startHandler += OneStartHandler;
            resLoader.progressHandler += OneProgressHandler;
            resLoader.completeHandler += OneCompleteHandler;
            resLoader.errorHandler += OneErrorHandler;
            resLoader.Load();
        }

        //清理一个资源
        [NoToLua]
        public bool RemoveLoadedRes(string subPath)
        {
            if (_loadedRes.Contains(subPath))
            {
                _loadedRes.Remove(subPath);
                return true;
            }
            //AssetsObj ao = null;
            //if (_loadedRes.TryGetValue(subPath, out ao))
            //{
            //    _loadedRes.Remove(subPath);
            //    if (ao != null)
            //    {
            //        ao.Clear();
            //        ao = null;
            //        return true;
            //    }
            //}
            ResLoader resLoader;
            int i;
            int count;
            for (i = 0, count = _waitingList.Count; i < count; i++)
            {
                resLoader = _waitingList[i];
                if (resLoader.ResObj.SubPath == subPath)
                {
                    GameObject.Destroy(resLoader);
                    _waitingList.RemoveAt(i);
                    return true;
                }
            }

            for (i = 0, count = _loadingList.Count; i < count; i++)
            {
                resLoader = _loadingList[i];
                if (resLoader.ResObj.SubPath == subPath)
                {
                    GameObject.Destroy(resLoader);
                    _loadingList.RemoveAt(i);
                    TryLoadRes();//继续加载
                    return true;
                }
            }
            return false;
        }

        [NoToLua]
        public void RemoveResLoader()
        {
            ResLoader resLoader;
            int i;
            int count;
            for (i = 0, count = _waitingList.Count; i < count; i++)
            {
                resLoader = _waitingList[i];
                GameObject.Destroy(resLoader);
            }
            _waitingList.Clear();

            for (i = 0, count = _loadingList.Count; i < count; i++)
            {
                resLoader = _loadingList[i];
                GameObject.Destroy(resLoader);
            }
            _loadingList.Clear();
        }
        #endregion

        #region 单个资源的事件处理
        private void OneStartHandler(ResLoader target)
        {
            target.startHandler -= OneStartHandler;
        }

        private void OneProgressHandler(ResLoader target, float progress)
        {
            if (ProgressAction != null)
                ProgressAction(target.ResObj.SubPath, progress);
        }

        private void OneCompleteHandler(ResLoader target)
        {
            _loadingList.Remove(target);
            //AssetsObj assetsObj = new AssetsObj(target.assetBundle.LoadAllAssets());
            //_loadedRes.Add(target.ResObj.SubPath, assetsObj);
            _loadedRes.Add(target.ResObj.SubPath);
            target.progressHandler -= OneProgressHandler;
            target.completeHandler -= OneCompleteHandler;
            target.errorHandler -= OneErrorHandler;
            if (CompleteAction != null)
                CompleteAction(target.ResObj.SubPath);
            //target.assetBundle.Unload(false);
            TryLoadRes();
        }

        private void OneErrorHandler(ResLoader target)
        {
            _loadingList.Remove(target);
            target.progressHandler -= OneProgressHandler;
            target.completeHandler -= OneCompleteHandler;
            target.errorHandler -= OneErrorHandler;
            if (ErrorAction != null)
                ErrorAction(target.ResObj.SubPath, target.ResObj);
        }

        #endregion

        private static int CompareResLoaderPrior(ResLoader a, ResLoader b)
        {
            if (a.ResObj.Prior > b.ResObj.Prior) return -1;
            if (a.ResObj.Prior < b.ResObj.Prior) return 1;
            return 0;
        }

    }
}
