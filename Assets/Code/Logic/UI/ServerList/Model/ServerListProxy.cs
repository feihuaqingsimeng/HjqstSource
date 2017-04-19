using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using LitJson;

namespace Logic.UI.ServerList.Model
{
    public class ServerListProxy : SingletonMono<ServerListProxy>
    {

        private bool _getServerList = true;
        public bool getServerList
        {
            private set { _getServerList = value; }
            get
            {
                if (!_getServerList)
                    UI.Tips.View.CommonErrorTipsView.Open(Common.Localization.Localization.Get("ui.common.tips.get_servers_error"), Logic.Game.Controller.GameController.instance.ReLoadMainScene, EUISortingLayer.TopTips);
                return _getServerList;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public System.Action<ServerInfo> changeServerDelegate;
        public System.Action getServerListFromRemoteSucDelegate;//从远程服务器获取服务器列表成功回调

        public int lastServerId;
        private Dictionary<int, ServerInfo> _serverListDictionary = new Dictionary<int, ServerInfo>();

        public Dictionary<int, ServerInfo> ServerListDictionary
        {
            get
            {
#if !UNITY_EDITOR
                if (getServerList)
                    return _serverListDictionary;
#endif
                if (_serverListDictionary.Count == 0)
                {
                    Dictionary<int, ServerData> serverDic = ServerData.ServerDataDictionary;
                    foreach (var data in serverDic)
                    {
#if UNITY_EDITOR_WIN
                        if (data.Value.inner == 1)
                        {
                            _serverListDictionary.Add(data.Key, new ServerInfo(data.Key));
                        }
#endif
                        if (data.Value.inner != 1)
                        {
                            if (data.Value.platformIdDictionary.GetValue(-1) || data.Value.platformIdDictionary.GetValue(PlatformProxy.instance.GetPlatformId()))
                            {
                                _serverListDictionary.Add(data.Key, new ServerInfo(data.Key));
                            }
                        }

                    }
                    
                }
                return _serverListDictionary;
            }
        }

        public ServerInfo GetCurrentServerInfo()
        {
            return ServerListDictionary.GetValue(lastServerId);
        }

        public void ChangeServer(ServerInfo info)
        {
            if (changeServerDelegate != null)
                changeServerDelegate(info);
        }

        void Start()
        {
           
#if !UNITY_EDITOR
            StartCoroutine("GetServersStatusCoroutine");
#endif
        }

        private IEnumerator GetServersStatusCoroutine()
        {
            long timeStamp = Common.Util.TimeUtil.GetTimeStampBefore10();
            string sign = Common.Util.EncryptUtil.String2MD5("get_areastatus" + timeStamp + Logic.Game.GameConfig.getServerListKey);
            int platformId = PlatformProxy.instance.GetPlatformId();
            string platform = "0";
            if (platformId != 0)
                platform = platformId.ToString();
            string finalUrl = string.Format(Logic.Game.GameConfig.GetServerListUrl() + "?cmdid=get_areastatus&time={0}&sign={1}&platformId={2}", timeStamp, sign.ToLower(), platform);
            Debugger.Log("finalUrl:" + finalUrl);
            WWW www = new WWW(finalUrl);
            yield return www;
            if (www.error != null)
            {
                Debugger.LogError("-----------------从服务端获得的服务器列表 有误");
                getServerList = false;
            }
            else
            {
                _serverListDictionary.Clear();
                try
                {
                    JsonData data = JsonMapper.ToObject(www.text);
                    int count = data.Count;
                    for (int i = 0; i < count; i++)
                    {
                        ServerJsonData d = JsonMapper.ToObject<ServerJsonData>(data[i].ToJson());
                        _serverListDictionary.Add(d.id.ToInt32(), new ServerInfo(d));
                    }
                    
                    if (_serverListDictionary.Count < 0)
                    {
                        Debugger.LogError("从服务端获得的服务器列表为空，当前渠道号:" + platformId);
                        getServerList = false;
                    }
                    else
                    {
                        if (getServerListFromRemoteSucDelegate != null)
                            getServerListFromRemoteSucDelegate();
                    }
                }
                catch (System.Exception e)
                {
                    Debugger.LogError("从服务端获得的服务器列表错误,msg:" + e.StackTrace);
                    getServerList = false;
                }
            }
        }
    }

}

