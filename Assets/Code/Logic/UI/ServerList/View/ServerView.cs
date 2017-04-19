//#define banshu
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.ServerList.Model;
using Common.Localization;
using Logic.UI.Login.Controller;
using Logic.UI.Login.Model;
using LitJson;
using Logic.Game;
using Common.ResMgr;
using Logic.Enums;

namespace Logic.UI.ServerList.View
{
    public class ServerView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/server_list/server_view";
        public static ServerView Open()
        {
            ServerView view = UIMgr.instance.Open<ServerView>(PREFAB_PATH);
            return view;
        }

        public Text currentServerText;
        public GameObject core;
        public RawImage bgRawImg;
        public string bgRawImgPath;
        private ServerInfo _currentServerInfo;
        private bool _isClickStarted;
        void Awake()
        {
            ServerListProxy.instance.lastServerId = PlayerPrefs.GetInt("lastLoginServerId");
            BindDelegate();
#if UNITY_ANDROID && !UNITY_EDITOR
			//RefreshServer();
			//StartCoroutine(ServerListProxy.instance.GetServerListFromRemote());
			Refresh();
#else
            Refresh();
#endif
            bgRawImg.texture = Common.ResMgr.ResMgr.instance.Load<Texture>(bgRawImgPath);
        }
        void Start()
        {
            if (!GameDataCenter.instance.isSdkLogin)
            {
                PlatformProxy.instance.ShowSdkLogin(PayCallbackData.GetPayCallbackDataByID(100).callback);
            }
            else
            {
                GameDataCenter gdc = GameDataCenter.instance;
                LoginSuccessHandler(gdc.sdkId, gdc.u8id, gdc.token, gdc.platformId);
            }
#if UNITY_ANDROID && !UNITY_EDITOR
			StartCoroutine(ShowNoticeView());
#elif UNITY_IOS && !UNITY_EDITOR
			StartCoroutine(ShowNoticeView());
#endif
        }

        void OnDestroy()
        {
            UnbindDelegate();
        }

        void BindDelegate()
        {
            ServerListProxy.instance.changeServerDelegate += ChangeServerHandler;
            PlatformResultProxy.instance.onLoginSuccessDelegate += LoginSuccessHandler;
            ServerListProxy.instance.getServerListFromRemoteSucDelegate += GetServerListFromRemoteSucHandler;
        }

        void UnbindDelegate()
        {
            ServerListProxy.instance.changeServerDelegate -= ChangeServerHandler;
            PlatformResultProxy.instance.onLoginSuccessDelegate -= LoginSuccessHandler;
            ServerListProxy.instance.getServerListFromRemoteSucDelegate -= GetServerListFromRemoteSucHandler;

        }

        private void Refresh()
        {
            int id = ServerListProxy.instance.lastServerId;
            _currentServerInfo = ServerListProxy.instance.ServerListDictionary.GetValue(id);
            if (_currentServerInfo == null)
                _currentServerInfo = ServerListProxy.instance.ServerListDictionary.Last().Value;
            RefreshServer();
        }

        private void RefreshServer()
        {
            if (_currentServerInfo != null)
                currentServerText.text = _currentServerInfo.description;
            else
                currentServerText.text = "";
        }

        private void ChangeServerHandler(ServerInfo info)
        {
            _currentServerInfo = info;
            RefreshServer();
            core.SetActive(true);
        }

        private void GetServerListFromRemoteSucHandler()
        {
            int id = ServerListProxy.instance.lastServerId;
            _currentServerInfo = ServerListProxy.instance.ServerListDictionary.GetValue(id);
            if (_currentServerInfo == null)
                _currentServerInfo = ServerListProxy.instance.ServerListDictionary.Last().Value;
            RefreshServer();
        }

        private IEnumerator EnableClickCoroutine()
        {
            _isClickStarted = true;
            yield return new WaitForSeconds(5);
            _isClickStarted = false;
        }

        private void Login()
        {
            //UIMgr.instance.Close(PREFAB_PATH);
            //UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            LoginController.instance.Login(LoginProxy.instance.cachedAccount, LoginProxy.instance.cachedPassword);
        }

        public void ClickStartBtnHandler()
        {
            if (_isClickStarted)
            {
                return;
            }
            if (_currentServerInfo == null)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.serverListView.noSelectServer"));
                return;
            }
            if (_currentServerInfo.state == ServerState.Maintain)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.serverListView.selectServerMaintain"));
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
			if(!GameDataCenter.instance.isSdkLogin)
			{				
                PlatformProxy.instance.ShowSdkLogin(PayCallbackData.GetPayCallbackDataByID(100).callback);
				return;
			}else
			{
				Debugger.LogError("isSdkLogin:"+GameDataCenter.instance.isSdkLogin.ToString());
			}
#elif UNITY_IOS && !UNITY_EDITOR
			if(!GameDataCenter.instance.isSdkLogin)
			{				
				PlatformProxy.instance.ShowSdkLogin(PayCallbackData.GetPayCallbackDataByID(100).callback);
				Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.common_tips.please_login_gamecenter"), null, EUISortingLayer.TopTips);
				return;
			}else
			{
				Debugger.LogError("isSdkLogin:"+GameDataCenter.instance.isSdkLogin.ToString());
			}
#endif
            StartCoroutine(EnableClickCoroutine());

            ServerListProxy.instance.lastServerId = _currentServerInfo.serverId;
#if banshu
            _currentServerInfo.port = 70;
#endif
            Logic.Game.Controller.GameController.instance.ConnectGameServer(_currentServerInfo.host, _currentServerInfo.port, Login);
            Game.GameConfig.gameServerHost = _currentServerInfo.host;
            Game.GameConfig.gameServerPort = _currentServerInfo.port;

#if UNITY_EDITOR
            Game.GameConfig.instance.innerGameServerHost = _currentServerInfo.host;
            Game.GameConfig.instance.innerGameServerPort = _currentServerInfo.port;
#else
            Game.GameConfig.gameServerHost = _currentServerInfo.host;
            Game.GameConfig.gameServerPort = _currentServerInfo.port;
#endif
        }

        public void ClickServerListHandler()
        {
            if (_isClickStarted)
            {
                return;
            }
            if (_currentServerInfo == null)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.serverListView.noServer"));
                return;
            }
            ServerListView.Open();
            core.SetActive(false);
        }

		private void LoginSuccessHandler(string sdkId, string u8id, string token, int platformId)
		{
			Game.Model.GameProxy.instance.SetLoginSuccessData(sdkId, u8id, token, platformId);
		}

        private IEnumerator ShowNoticeView()
        {
            int day = PlayerPrefs.GetInt("tipLoginNoticeBoardDay");
            day = System.DateTime.Now.DayOfYear - day;
            if (GameDataCenter.instance.isTipLoginNotice && day > 0)
            {
				WWW www = new WWW(ResUtil.GetRemoteStaticPathByCdn("notice.txt"));
                yield return www;

                string noticeStr = www.text;
                Logic.UI.LoginNoticeBoard.View.LoginNoticeBoardView.Open(noticeStr);
                GameDataCenter.instance.isTipLoginNotice = false;
                www.Dispose();
                www = null;
            }
        }

    }

}

