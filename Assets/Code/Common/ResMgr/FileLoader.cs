using UnityEngine;
using System.Collections;
using Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using Logic.Game;
namespace Common.ResMgr
{
    public class FileLoader : MonoBehaviour
    {
        public delegate void CompleteHandler();
        public delegate void ProgressUpdateHandler(float progress, int currentNum, int totalNum);
        public delegate void FailHandler();
        public CompleteHandler completeHandler;
        public ProgressUpdateHandler progressUpdateHandler;
        public FailHandler failHandler;
        public bool switchUpdateConfigTips = false;
        public bool isRemote = true;
        private string _targetDir;
        private string _md5Name;
        private string _md5File;

        public string targetDir
        {
            private get { return _targetDir; }
            set { _targetDir = value; }
        }

        public string md5Name
        {
            set
            {
                _md5Name = value;
                md5File = _targetDir.IsNullOrEmpty() ? _md5Name : (_targetDir + "/" + _md5Name);
            }
        }

        private string md5File
        {
            set { _md5File = value; }
            get { return _md5File; }
        }

        private float _fromPorgress = 0f;
        private float _toProgress = 1f;
        public float fromPorgress
        {
            get
            {
                return _fromPorgress;
            }
            set
            {
                _fromPorgress = value;
            }
        }

        public float toPorgress
        {
            get
            {
                return _toProgress;
            }
            set
            {
                _toProgress = value;
            }
        }
        private float _currentProgress = 0f;

        public float currentProgress
        {
            get { return _currentProgress; }
        }
        //public string csvDir = "config/csv";
        //public string md5File = "config/csv/config.md5";
        private EResLoaderState _state = EResLoaderState.READY;
        private int _loadMD5Number = 0;
        private int _loadFileNumber = 0;
        private const int MAX_LOAD_NUMBER = 10;
        private int _toatlNumber;
        private Dictionary<string, string> _fileDic;
        private Dictionary<string, string> _localFileDic;
        private List<string> _modifiedList;
        private WWW _md5WWW, _fileWWW;
        void Awake()
        {
            _fileDic = new Dictionary<string, string>();
            _localFileDic = new Dictionary<string, string>();
            _modifiedList = new List<string>();
        }

        public void Load()
        {
            _loadMD5Number = 0;
            StartCoroutine("LoadMD5Coroutine");
        }

        private IEnumerator LoadMD5Coroutine()
        {
            _loadMD5Number++;
            string path = string.Empty;
            if (isRemote)
                path = ResUtil.GetRemotePath(md5File);
            else
                path = ResUtil.GetStreamingAssetsWWWPath(md5File);
            _md5WWW = new WWW(path + GameConfig.instance.param);
            Debugger.Log(_md5WWW.url);
            _md5WWW.threadPriority = ThreadPriority.High;
            yield return _md5WWW;
            if (!string.IsNullOrEmpty(_md5WWW.error))
                _state = EResLoaderState.Error;
            if (_state == EResLoaderState.Error)
            {
                Debugger.LogError(string.Format("{0}加载失败", _md5WWW.url));
                _md5WWW.Dispose();
                _md5WWW = null;
                Debugger.LogError(string.Format("加载失败…第{0}次尝试加载", _loadMD5Number));
                if (_loadMD5Number < MAX_LOAD_NUMBER)
                {
                    yield return new WaitForSeconds(1f);
                    _state = EResLoaderState.READY;
                    StopCoroutine("LoadMD5Coroutine");
                    StartCoroutine("LoadMD5Coroutine");
                }
                else
                {
                    Debugger.LogError("又加载失败了，心累了，不想加载了...");
                    if (failHandler != null)
                        failHandler();
                }
            }
            else
            {
                //Debugger.Log(_md5WWW.text);
                string[] remoteLineArr = _md5WWW.text.Split(CSVUtil.SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
                string remoteVersion = remoteLineArr[0].ToArray(CSVUtil.SYMBOL_COLON)[1];
                if (_md5Name.Contains(ResUtil.CONFIG_DIRECTORY))
                    GameConfig.instance.csvVersion = remoteVersion;
                else if (_md5Name.Contains(ResUtil.LUA_DIRECTORY))
                    GameConfig.instance.luaVersion = remoteVersion;
                for (int i = 1, count = remoteLineArr.Length; i < count; i++)
                {
                    KeyValuePair<string, string> kvp = remoteLineArr[i].SplitToKeyValuePair(CSVUtil.SYMBOL_COLON);
                    _fileDic.Add(kvp.Key, kvp.Value);
                }
                bool isSameVersion = false;
                string localMD5 = string.Empty;
                if (!ResUtil.ExistsInLocal(md5File, out localMD5))
                {
                    Debugger.Log("md5本地不存在");
                    ResUtil.Save2Local(md5File, _md5WWW.bytes);
                    _modifiedList.AddRange(_fileDic.GetKeys());
                    LoadFiles();
                }
                else
                {
                    string[] localLineArr = File.ReadAllText(localMD5).Split(CSVUtil.SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
                    string localVersion = localLineArr[0].ToArray(CSVUtil.SYMBOL_COLON)[1];
                    for (int i = 1, count = localLineArr.Length; i < count; i++)
                    {
                        KeyValuePair<string, string> kvp = localLineArr[i].SplitToKeyValuePair(CSVUtil.SYMBOL_COLON);
                        _localFileDic.Add(kvp.Key, kvp.Value);
                    }

                    Dictionary<string, string>.Enumerator locale = _localFileDic.GetEnumerator();
                    int delCount = 0;
                    while (locale.MoveNext())
                    {
                        KeyValuePair<string, string> kvp = locale.Current;
                        if (!_fileDic.ContainsKey(kvp.Key))
                        {
                            ResUtil.DelLocalFile(targetDir + "/" + kvp.Key);
                            delCount++;
                        }
                    }
                  
                    locale.Dispose();
                    Debug.LogFormat("remoteVersion:{0}, localVersion:{1}", remoteVersion, localVersion);
                    isSameVersion = remoteVersion == localVersion;
                    //if (isSameVersion)
                    {
                         Debugger.Log("本地跟远程版本是否相同，都比较" + md5File + "   isSameVersion:"+isSameVersion);
                        int addCount = 0;
                        Dictionary<string, string>.Enumerator e = _fileDic.GetEnumerator();
                        while (e.MoveNext())
                        {
                            bool isUnExist=false;
                            KeyValuePair<string, string> kvp = e.Current;
                            if (!CheckedMD5(kvp.Key, kvp.Value, out isUnExist))
                            {
                                _modifiedList.Add(kvp.Key);
                                if (isUnExist) addCount++;
                            }
                        }
                        e.Dispose();
                        Debugger.Log(string.Format("{0}个新增文件", addCount));
                        Debugger.Log(string.Format("{0}个本地文件MD5改变", _modifiedList.Count - addCount));
                        if (_modifiedList.Count > 0)
                        {
                            //Debugger.Log(string.Format("{0}个本地文件MD5改变", _modifiedList.Count));
                            LoadFiles();
                        }
                        else
                        {
                            if (progressUpdateHandler != null)
                                progressUpdateHandler(_toProgress, 0, _toatlNumber);

                            StartCoroutine("CompleteCoroutine");
                        }
                        if (!isSameVersion || _modifiedList.Count > 0 || delCount > 0)
                        {
                            ResUtil.Save2Local(md5File, _md5WWW.bytes);
                        }
                        Debugger.Log(string.Format("{0}个本地文件已删除", delCount));

                    }
                    //else
                    //{
                    //    Debugger.Log(string.Format("{0}个本地文件已删除", delCount));

                    //    Debugger.Log("本地跟远程版本不同，重新加载文件");
                    //    ResUtil.Save2Local(md5File, _md5WWW.bytes);
                    //    //_md5WWW.Dispose();
                    //    //_md5WWW = null;
                    //    _modifiedList.AddRange(_fileDic.GetKeys());
                    //    LoadFiles();
                    //}
                }
            }
            _md5WWW.Dispose();
            _md5WWW = null;
        }

        private bool CheckedMD5(string fileName, string md5Value,out bool isUnExist)
        {
            //#if UNITY_EDITOR
            //            return false;
            //#endif
            isUnExist = false;
            bool result = false;
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(md5Value))
            {
                isUnExist = true;
                return result;
            }
            string path = targetDir + "/" + fileName;
            string localPath = string.Empty;
            if (!ResUtil.ExistsInLocal(path, out localPath))
            {
                isUnExist = true;
                return result;
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(localPath);
                string encodeMD5 = EncryptUtil.Bytes2MD5(bytes);
                result = md5Value.Equals(encodeMD5);
                //result = true;
                if (!result)
                    Debugger.Log(string.Format("file {0}被修改了", fileName));
            }
            catch (Exception e)
            {
                Debugger.LogError(e.StackTrace);
            }
            return result;
        }

        private void LoadFiles()
        {
            _toatlNumber = _modifiedList.Count;
            _loadFileNumber = 0;
			StopCoroutine("LoadFilesCoroutine");
            StartCoroutine("LoadFilesCoroutine");
            if (switchUpdateConfigTips)
                Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2UpdateConfigTips();
        }

        private IEnumerator LoadFilesCoroutine()
        {
            int currentNum = _toatlNumber - _modifiedList.Count;
            float progress = (float)currentNum / (float)_toatlNumber;
            progress = fromPorgress + (toPorgress - fromPorgress) * progress;
            _currentProgress = progress;
            if (progressUpdateHandler != null)			
                progressUpdateHandler(progress, currentNum, _toatlNumber);

            if (_modifiedList.Count > 0)
            {
                _loadFileNumber++;
                string flieName = _modifiedList.First();
                string path = string.Empty;
                if (isRemote)
                    path = ResUtil.GetRemotePath(targetDir + "/" + flieName);
                else
                    path = ResUtil.GetStreamingAssetsWWWPath(targetDir + "/" + flieName);
                _fileWWW = new WWW(path + GameConfig.instance.param);
                Debugger.Log(path + GameConfig.instance.param);
                _fileWWW.threadPriority = ThreadPriority.High;
                yield return _fileWWW;
                if (!string.IsNullOrEmpty(_fileWWW.error))
                    _state = EResLoaderState.Error;
                if (_state == EResLoaderState.Error)
                {
                    Debugger.LogError(string.Format("{0}加载失败", _fileWWW.url));
                    Debugger.LogError(string.Format("加载失败…第{0}次尝试加载", _loadFileNumber));
                    if (_loadFileNumber < MAX_LOAD_NUMBER)
                    {
                        yield return new WaitForSeconds(1f);
                        _state = EResLoaderState.READY;
                    }
                }
                else
                {
                    string localPath = targetDir + "/" + flieName;
                    ResUtil.Save2Local(localPath, _fileWWW.bytes);
                    _modifiedList.Remove(flieName);
                    _loadFileNumber = 0;
                }
                _fileWWW.Dispose();
                _fileWWW = null;
                StartCoroutine("LoadFilesCoroutine");
            }
            else
            {
                if (completeHandler != null)
                    completeHandler();
                Debugger.Log(string.Format("所有文件加载成功!"));
                UnityEngine.Object.Destroy(this);
            }
        }

        public void LoadZip()
        {
            string zipPath = ResUtil.GetStreamingAssetsWWWPath(md5File);
            //string unzipPath = ResUtil.GetLocalPath("");
             string unzipPath = Path.Combine(Application.persistentDataPath, ResConf.eResPlatform.ToString());
            this.StartCoroutine(UnCompress(zipPath,unzipPath));
        }
        IEnumerator UnCompress(string zipPath, string unzipPath)
        {
            Debug.Log("unCompress   zipPath  : " + zipPath);
            Debug.Log("unCompress   unzipPath: " + unzipPath);
            //WWW www = new WWW("file://" + zipPath + "csv.zip");
            WWW www = new WWW(zipPath + GameConfig.instance.param);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError("压缩包读取失败www.url :" + www.url);
                Debug.LogError("压缩包读取失败www.error： " + www.error);
                if (failHandler != null)
                    failHandler();
                yield break;
            }
            Stream stream = new MemoryStream(www.bytes);
            yield return this.StartCoroutine(CompressionAsync.UnZipFile(this, stream, unzipPath, string.Empty, string.Empty,
                (index,count, s) =>
                {
                    float f = (float)index / count;
                    float  progress = fromPorgress + (toPorgress - fromPorgress) * f;
                    if (progressUpdateHandler != null)
                        progressUpdateHandler(progress, (int)index, (int)count);
                    //Debug.LogFormat("正在解压资源{0}", (int) (f*100) + "%)   ---s:"+s);
                }));
            //Compression.UnZipFile(soursePaht + "csv.config", localPath, string.Empty, string.Empty);
            stream.Dispose();
            if (completeHandler != null)
                completeHandler();
            UnityEngine.Object.Destroy(this);
        }
        public static IEnumerator CheckLocalAndStreamingVersion(MonoBehaviour mono, string path, Action<bool> resultIsNew, int time)
        {
            string StreamingPath = ResUtil.GetStreamingAssetsWWWPath(path);
            var www = new WWW(StreamingPath + GameConfig.instance.param);
            Debugger.Log(www.url);
            www.threadPriority = ThreadPriority.High;
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debugger.LogError(string.Format("{0}加载失败", www.url));
                www.Dispose();
                www = null;
                Debugger.LogError(string.Format("加载失败…第{0}次尝试加载", time));
                if (time < 10)
                {
                    yield return new WaitForSeconds(1f);
                    mono.StartCoroutine(CheckLocalAndStreamingVersion(mono, path, resultIsNew, time + 1));
                    yield break;
                }
                else
                {
                    Debugger.LogError("CheckLocalAndStreamingVersion 又加载失败了，心累了，不想加载了...");

                }
            }
            else
            {
                Debugger.LogError(string.Format("CheckLocalAndStreamingVersion：{0}加载成功！", www.url));
                //Debugger.Log(_md5WWW.text);
                string[] remoteLineArr = www.text.Split(CSVUtil.SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
                string remoteVersion = remoteLineArr[0].ToArray(CSVUtil.SYMBOL_COLON)[1];
               
                string localMD5 = string.Empty;
                if (ResUtil.ExistsInLocal(path, out localMD5))
                {
                    string[] localLineArr = File.ReadAllText(localMD5).Split(CSVUtil.SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
                    string localVersion = localLineArr[0].ToArray(CSVUtil.SYMBOL_COLON)[1];

                    if (resultIsNew != null)
                    {
                        Debug.LogFormat("remoteVersion:{0}, localVersion:{1}", remoteVersion, localVersion);
                        string[] remoteV = remoteVersion.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] localV = localVersion.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < remoteV.Length; i++)
                        {
                            int intRemote = 0;
                            int intlocalV = 0;
                            if (int.TryParse(remoteV[i], out intRemote) && int.TryParse(localV[i], out intlocalV))
                            {
                                if (intRemote > intlocalV)
                                {
                                    Debug.Log("intRemote > intlocalV");
                                    resultIsNew(true);
                                    www.Dispose();
                                    www = null;
                                    yield break;
                                }
                                else
                                    if (intRemote < intlocalV)
                                    {
                                        Debug.Log("intRemote < intlocalV");
                                        resultIsNew(false);
                                        www.Dispose();
                                        www = null;
                                        yield break;
                                    }
                            }

                        }


                    }
                }
                Debug.Log("intRemote == intlocalV");
                www.Dispose();
                www = null;
            }
            resultIsNew(false);

        }
        private IEnumerator CompleteCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            if (completeHandler != null)
                completeHandler();
            UnityEngine.Object.Destroy(this);
        }

        void OnDestroy()
        {
            if (_fileDic != null)
            {
                _fileDic.Clear();
                _fileDic = null;
            }
            if (_modifiedList != null)
            {
                _modifiedList.Clear();
                _modifiedList = null;
            }
            if (_fileWWW != null)
            {
                _fileWWW.Dispose();
                _fileWWW = null;
            }
            if (_md5WWW != null)
            {
                _md5WWW.Dispose();
                _md5WWW = null;
            }
            if (completeHandler != null)
                completeHandler = null;
            if (progressUpdateHandler != null)
                progressUpdateHandler = null;
        }
    }
}