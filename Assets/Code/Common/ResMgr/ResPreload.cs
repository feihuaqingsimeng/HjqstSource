using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common.ResMgr
{
    public sealed class ResPreload : SingletonMono<ResPreload>
    {
        public delegate void CheckedVersionHandler(ResPreload target, long totalLength, int totalNum);
        public event CheckedVersionHandler checkedVersionHandler;

        public delegate void ProgressHandler(ResPreload target, float progress, int totalNum, int currentNum);
        public event ProgressHandler progressHandler;

        public delegate void CompleteHandler(ResPreload target);
        public event CompleteHandler completeHandler;

        public delegate void ErrorHandler(ResPreload target);
        public event ErrorHandler errorHandler;

        private Dictionary<string, AssetInfo> _diffAssetDic;
        private int _totalNum;
        private int _currentNum;
        private long _totalLength;

        public long TotalLength
        {
            get { return _totalLength; }
        }

        private List<Regex> _excludeSubpathRegexList;//被排掉的子路径

        private bool IsExclude(string subpath)
        {
            if (_excludeSubpathRegexList == null)
                return false;
            foreach (Regex regex in _excludeSubpathRegexList)
            {
                if (regex.IsMatch(subpath))
                    return true;
            }
            return false;
        }

        private bool _checkedVersion = false;
        public void CheckVersion(List<Regex> excludeSubpathRegexList)
        {
            if (_checkedVersion)
                return;
            _excludeSubpathRegexList = excludeSubpathRegexList;
            _checkedVersion = true;

            LoadRemoteManifest();
        }

        private void LoadLocalManifest()
        {
            ResMgr.instance.localManifest = ResUtil.GetManifestFromLocal();
            if (ResMgr.instance.localManifest == null)
                ResMgr.instance.localManifest = new ManifestInfo();
        }

        private bool _isPreloading = false;
        private const byte MAX_TRY_NUMBER = 10;

        public void Preload()
        {
            if (_isPreloading)
                return;
            _isPreloading = true;
            ResMgr.instance.CompleteAction += CompleteAction;
            ResMgr.instance.ProgressAction += ProgressAction;
            ResMgr.instance.ErrorAction += ErrorAction;
            Dictionary<string, AssetInfo>.Enumerator e = _diffAssetDic.GetEnumerator();
            while (e.MoveNext())
            {
                KeyValuePair<string, AssetInfo> kvp = e.Current;
                ResMgr.instance.PushRes(new ResLoaderObj(kvp.Value.SubPath, 0, MAX_TRY_NUMBER, kvp.Value.Suffix));
            }
            e.Dispose();
            //ResMgr.instance.TryLoadRes();
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            LoadLocalManifest();
        }

        private void OnDestroy()
        {
            _diffAssetDic = null;

            progressHandler = null;
            completeHandler = null;
            checkedVersionHandler = null;
            if (ResMgr.instance != null)
            {
                ResMgr.instance.CompleteAction -= CompleteAction;
                ResMgr.instance.ProgressAction -= ProgressAction;
                ResMgr.instance.ErrorAction -= ErrorAction;
            }
            StopAllCoroutines();
        }

        private void LoadRemoteManifest()
        {
            StopCoroutine("LoadRemoteManifestCoroutine");
            StartCoroutine("LoadRemoteManifestCoroutine");
        }

        //=============================================www加载有bug==========================start
        private float _currentProgress;
        private float _lastProgressTime;
        private const float ERR_TIMEOUT = 60f;
        private bool IsErrorProgress(float value)
        {
            if (_currentProgress != value)
            {
                _lastProgressTime = Time.realtimeSinceStartup;
                _currentProgress = value;
                return false;
            }
            if (Time.realtimeSinceStartup - _lastProgressTime > ERR_TIMEOUT)
            {
                _currentProgress = -1;
                return true;
            }
            return false;
        }
        //=============================================www加载有bug==========================end

        private System.Collections.IEnumerator LoadRemoteManifestCoroutine()
        {
            WWW www = new WWW(ResUtil.GetRemoteManifestPath() + Logic.Game.GameConfig.instance.param);
            while (!www.isDone)
            {
                if (IsErrorProgress(www.progress))
                {
                    Debugger.Log("manifest加载失败:" + www.url);
                    www.Dispose();
                    www = null;
                    _isPreloading = false;
                    LoadRemoteManifest();
                    yield break;
                }
                yield return 1;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                Debugger.Log("manifest加载失败:" + www.url);
                www.Dispose();
                www = null;
                yield return new WaitForSeconds(0.2f);
                _isPreloading = false;
                LoadRemoteManifest();
            }
            else
            {
                Debugger.Log("manifest加载成功:" + www.url);
                ResMgr.instance.remoteManifest = ResUtil.GetManifestFromBytes(www.bytes);
                if (ResMgr.instance.remoteManifest == null)
                    ResMgr.instance.remoteManifest = new ManifestInfo();

                www.Dispose();
                www = null;
                CheckDifference();
            }
        }

        private void CheckDifference()
        {
            _totalLength = 0;
            _diffAssetDic = new Dictionary<string, AssetInfo>();
            AssetInfo localAsset;
            if (ResMgr.instance.localManifest.version != ResMgr.instance.remoteManifest.version)
            {
                ResMgr.instance.localManifest.version = ResMgr.instance.remoteManifest.version;
                //ResMgr.instance.localManifest.assetDic.Clear();
                //ResUtil.DelAllLocalAbRes();
            }

            string tmpStr;
            foreach (var kvp in ResMgr.instance.remoteManifest.assetDic)
            {
                if (IsExclude(kvp.Key))
                {
                    continue;
                }

                if (ResMgr.instance.localManifest.assetDic.TryGetValue(kvp.Key, out localAsset))
                {
                    if (!localAsset.Equals(kvp.Value) || !ResUtil.ExistsInLocal(kvp.Key + kvp.Value.Suffix, out tmpStr))
                    {
                        _diffAssetDic.Add(kvp.Key, kvp.Value);
                        _totalLength += kvp.Value.Length;
                    }
                }
                else
                {
                    _diffAssetDic.Add(kvp.Key, kvp.Value);
                    _totalLength += kvp.Value.Length;
                }
            }

            _totalNum = _diffAssetDic.Count;
            _currentNum = 1;
            Debugger.Log(string.Format("检查更新结束_totalLength:{0},_totalNum{1}", _totalLength, _totalNum));
            if (checkedVersionHandler != null)
                checkedVersionHandler(this, _totalLength, _totalNum);

            if (_totalNum == 0)
            {
                int total = ResMgr.instance.remoteManifest.assetDic.Count;
                if (progressHandler != null)
                    progressHandler(this, 1, total, total);
                Debugger.Log("资源同步完成");
                if (completeHandler != null)
                    completeHandler(this);
                UnityEngine.Object.Destroy(this);
            }
            else
            {
                Preload();//检查完之后手动去调用
            }
        }

        private void CompleteAction(string subPath)
        {
            //ResMgr.instance.RemoveLoadedRes(subPath);
            _diffAssetDic.Remove(subPath);
            _currentNum++;

            if (_diffAssetDic.Count == 0)
            {
                if (ResMgr.instance != null)
                {
                    ResMgr.instance.CompleteAction -= CompleteAction;
                    ResMgr.instance.ProgressAction -= ProgressAction;
                    ResMgr.instance.ErrorAction -= ErrorAction;
                    ResMgr.instance.RemoveResLoader();
                }

                Debugger.Log("资源同步完成");
                if (completeHandler != null)
                    completeHandler(this);

                UnityEngine.Object.Destroy(this);
            }
        }

        private void ProgressAction(string subPath, float progress)
        {
            float totalProgress = (float)_currentNum / (float)_totalNum;
            if (progressHandler != null)
                progressHandler(this, totalProgress, _currentNum, _totalNum);
        }

        private void ErrorAction(string subPath, System.Object res)
        {
            //ResLoaderObj resLoaderObj = res as ResLoaderObj;
            ////重新加入列表
            //Debugger.LogError(string.Format("{0}加载失败,重新加入等待列表", resLoaderObj.SubPath));
            //ResMgr.instance.PushRes(resLoaderObj);
            if (errorHandler != null)
                errorHandler(this);
        }
    }
}
