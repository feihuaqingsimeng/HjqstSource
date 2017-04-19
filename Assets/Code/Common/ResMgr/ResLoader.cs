#define compress
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Common.ResMgr
{
    public class ResLoaderObj
    {
        private string _subPath;//资源路径

        public string SubPath
        {
            get { return _subPath; }
        }

        private byte _prior;//资源加载优先级，越大越先加载。
        public byte Prior
        {
            get { return _prior; }
        }

        private byte _maxTryLoadNum;

        public byte MaxTryLoadNum//最多尝试下载的次数。0为无限制尝试
        {
            get { return _maxTryLoadNum; }
        }

        private string _suffix;//文件后缀
        public string Suffix
        {
            get { return _suffix; }
        }
        public ResLoaderObj(string subPath, byte prior, byte maxTryLoadNum, string suffix)
        {
            _subPath = subPath;
            _prior = prior;
            _maxTryLoadNum = maxTryLoadNum;
            _suffix = suffix;
        }

        public ResLoaderObj(string subPath, string suffix) : this(subPath, 0, 0, suffix) { }
        public ResLoaderObj(string subPath, byte prior, string suffix) : this(subPath, prior, 0, suffix) { }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("ResLoaderObj:[");
            sb.Append("_subPath:");
            sb.Append(_subPath);
            sb.Append(",_prior:");
            sb.Append(_prior);
            sb.Append(",_maxTryLoadNum:");
            sb.Append(_maxTryLoadNum);
            sb.Append("]");
            return sb.ToString();
        }
    }

    public enum EResLoaderState
    {
        READY,
        Loading,
        Complete,
        Error,
    }

    /// <summary>
    /// 只允许加载AssetBundle,其它资源应该打包成AssetBundle
    /// 依赖说ResObj.CanLoadFromLocal是否尝试从本地加载，如果加载不到，从远程加载，并保存到本地
    /// 加载失败后会无限尝试再次加载
    /// 加载进度不变化(ERR_TIMEOUT 10秒)的时候会加载失败，无限尝试再次加载
    /// </summary>
    public sealed class ResLoader : MonoBehaviour
    {
        public delegate void StartHandler(ResLoader target);
        public event StartHandler startHandler;

        public delegate void ProgressHandler(ResLoader target, float progress);
        public event ProgressHandler progressHandler;

        public delegate void CompleteHandler(ResLoader target);
        public event CompleteHandler completeHandler;

        public delegate void ErrorHandler(ResLoader target);
        public event ErrorHandler errorHandler;


        private ResLoaderObj _resObj;

        public ResLoaderObj ResObj
        {
            get { return _resObj; }
            set { _resObj = value; }
        }

        //public AssetBundle assetBundle
        //{
        //    get { return _assetBundle; }
        //}

        //private AssetBundle _assetBundle;

        private bool _formLocal = false;//是否从本地加载
        public bool FormLocal
        {
            get { return _formLocal; }
        }

        private int _reLoadNum = 1;//尝试加载的次数
        public int ReLoadNum
        {
            get { return _reLoadNum; }
        }

        private EResLoaderState _state = EResLoaderState.READY;//当前的状态
        public EResLoaderState State
        {
            get { return _state; }
        }

        private float _currentProgress;

        private float _lastProgressTime;
        private const float ERR_TIMEOUT = 60f;
        public float CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                if (_currentProgress != value)
                {
                    _lastProgressTime = Time.realtimeSinceStartup;
                    _currentProgress = value;
                    if (progressHandler != null)
                        progressHandler(this, _currentProgress);
                }
                else
                {
                    if (Time.realtimeSinceStartup - _lastProgressTime > ERR_TIMEOUT)
                    {
                        _state = EResLoaderState.Error;
                    }
                }
            }
        }

        public void Load()
        {
            if (_resObj == null)
                throw new System.Exception("ResLoader:: you must set ResObj before Load");
            if (_state != EResLoaderState.READY)
                throw new System.Exception("ResLoader:: is loading you can not reload it");
            _state = EResLoaderState.Loading;
            _reLoadNum++;
            _currentProgress = 0f;
            _lastProgressTime = Time.realtimeSinceStartup;
            StopCoroutine("LoadFileCoroutine");
            StartCoroutine("LoadFileCoroutine");
        }

        private void OnDestroy()
        {
            _resObj = null;
            startHandler = null;
            progressHandler = null;
            completeHandler = null;
            errorHandler = null;
            StopAllCoroutines();
        }

        private System.Collections.IEnumerator LoadFileCoroutine()
        {
            if (startHandler != null)
                startHandler(this);
            string localPath;
            AssetInfo localAssetInfo;
            AssetInfo remoteAssetInfo;
            if (ResMgr.instance.localManifest.assetDic.TryGetValue(_resObj.SubPath, out localAssetInfo)
                && ResMgr.instance.remoteManifest.assetDic.TryGetValue(_resObj.SubPath, out remoteAssetInfo)
                && localAssetInfo.Equals(remoteAssetInfo) && ResUtil.ExistsInLocal(_resObj.SubPath + _resObj.Suffix, out localPath))
            {
                _formLocal = true;
                byte[] bytes = System.IO.File.ReadAllBytes(localPath);
                if (bytes == null)
                {
                    if (errorHandler != null)
                        errorHandler(this);
                    Debugger.LogError(string.Format("{0}读取ab文件失败，可能本地文件已损坏", _resObj));
                    UnityEngine.Object.Destroy(this);
                }
                AssetBundleCreateRequest abcr = AssetBundle.CreateFromMemory(bytes);
                while (!abcr.isDone)
                {
                    if (_state == EResLoaderState.Error)
                        break;
                    CurrentProgress = abcr.progress;
                    yield return null;
                }
                if (_state == EResLoaderState.Error)
                {
                    abcr = null;

                    if (_resObj.MaxTryLoadNum > 0 && _reLoadNum > _resObj.MaxTryLoadNum)
                    {
                        if (errorHandler != null)
                            errorHandler(this);
                        Debugger.LogError(string.Format("{0}加载失败…{1}次尝试加载，可能本地文件已损坏", _resObj, _reLoadNum));
                        //#if !UNITY_EDITOR
                        UnityEngine.Object.Destroy(this);
                        //#endif
                    }
                    else
                    {
                        Debugger.LogError(string.Format("{0}加载本地文件失败，可能本地文件已损坏，直接删除本地文件，重新下载", _resObj));
                        System.IO.File.Delete(localPath);
                        yield return new WaitForSeconds(1f);
                        _state = EResLoaderState.READY;
                        Load();
                    }
                }
                else
                {
                    //_assetBundle = abcr.assetBundle;
                    //if (_assetBundle == null)
                    //    Debugger.LogError("you must load assetBundle file with resloader" + _resObj);
                    _state = EResLoaderState.Complete;
                    CurrentProgress = 1f;
                    if (completeHandler != null)
                        completeHandler(this);
                    //#if !UNITY_EDITOR
                    UnityEngine.Object.Destroy(this);
                    //#endif
                }
            }
            else
            {
                WWW www = new WWW(ResUtil.GetRemotePath(_resObj.SubPath + _resObj.Suffix) + Logic.Game.GameConfig.instance.param);
                Debugger.Log(www.url);
                www.threadPriority = ThreadPriority.High;
                while (!www.isDone)
                {
                    if (_state == EResLoaderState.Error)
                        break;
                    CurrentProgress = www.progress;
                    yield return null;
                }
                if (!string.IsNullOrEmpty(www.error))
                    _state = EResLoaderState.Error;
                if (_state == EResLoaderState.Error)
                {
                    Debugger.LogError(string.Format("{0}加载失败", www.url));
                    www.Dispose();
                    www = null;
                    if (_resObj.MaxTryLoadNum > 0 && _reLoadNum > _resObj.MaxTryLoadNum)
                    {
                        if (errorHandler != null)
                            errorHandler(this);
                        //#if !UNITY_EDITOR
                        UnityEngine.Object.Destroy(this);
                        //#endif
                    }
                    else
                    {
                        Debugger.LogError(string.Format("{0}加载失败…第{1}次尝试加载", _resObj, _reLoadNum));
                        yield return new WaitForSeconds(1f);
                        _state = EResLoaderState.READY;
                        Load();
                    }
                }
                else
                {
                    ResUtil.Save2Local(_resObj.SubPath + _resObj.Suffix, www.bytes);

                    if (ResMgr.instance.remoteManifest.assetDic.TryGetValue(_resObj.SubPath, out remoteAssetInfo))
                    {
                        ResMgr.instance.localManifest.assetDic[_resObj.SubPath] = remoteAssetInfo;
                        ResUtil.SaveManifest2Local(ResMgr.instance.localManifest);//同步远程和本地的manifest
                    }
                    _state = EResLoaderState.Complete;
                    //_assetBundle = www.assetBundle;
                    www.Dispose();
                    www = null;
                    //if (_assetBundle == null)
                    //    Debugger.LogError("you must load assetBundle file with resloader");
                    //if(_resObj)
                    //_assetBundle.Unload(true);
                    CurrentProgress = 1f;
                    if (completeHandler != null)
                        completeHandler(this);
                    //#if !UNITY_EDITOR
                    UnityEngine.Object.Destroy(this);
                    //#endif
                }
            }
        }
    }
}