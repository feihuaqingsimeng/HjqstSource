using UnityEngine;
using System.Collections.Generic;
using Common.GameTime.Controller;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.UI.Login.Model;
using LuaInterface;

namespace Logic.UI.Login.Controller
{
    public class LoginController : SingletonMono<LoginController>
    {
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.LoginReq).ToString(), LOBBY2CLIENT_LOGIN_FAILED_Handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.LoginResp).ToString(), LOBBY2CLIENT_LOGIN_SUCCESS_Handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PalyerRoleResp).ToString(), LOBBY2CLIENT_PLAYER_ROLE_RESP_Handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RoleHeadResp).ToString(), LOBBY2CLIENT_ROLE_HEAD_RESP_Handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RoleNameResp).ToString(), LOBBY2CLIENT_ROLE_NAME_RESP_Handler);
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.LoginReq).ToString(), LOBBY2CLIENT_LOGIN_FAILED_Handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.LoginResp).ToString(), LOBBY2CLIENT_LOGIN_SUCCESS_Handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PalyerRoleResp).ToString(), LOBBY2CLIENT_PLAYER_ROLE_RESP_Handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RoleHeadResp).ToString(), LOBBY2CLIENT_ROLE_HEAD_RESP_Handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RoleNameResp).ToString(), LOBBY2CLIENT_ROLE_NAME_RESP_Handler);
            }
        }

        public void Login(string account, string password)
        {
            LoginProxy.instance.cachedAccount = account;
            //LuaInterface.ToLuaPb.Login(account, password, UnityEngine.SystemInfo.deviceUniqueIdentifier);
            Logic.Protocol.Model.LoginReq req = new Logic.Protocol.Model.LoginReq();
            req.account = account;
            req.password = password;
            req.token = Logic.UI.Login.Model.LoginProxy.instance.cachedToken;
            req.devicesId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            req.partnerId = LoginProxy.instance.cachedPlatformId;
            req.u8token = Logic.UI.Login.Model.LoginProxy.instance.cachedToken;
			req.u8userID = LoginProxy.instance.cachedU8userID;
            req.isU8 = PlatformProxy.instance.isU8;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }

        public void Loginout()
        {
            if (Logic.Protocol.ProtocolProxy.instance != null)
            {
                UserLogoutReq req = new UserLogoutReq();
                req.type = 3;
                Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
            }
        }

        //修改头像
        public void CLIENT2LOBBY_ROLE_HEAD_REQ(int headNo)
        {
            RoleHeadReq req = new RoleHeadReq();
            req.headNo = headNo;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        //修改名字
        public void CLIENT2LOBBY_ROLE_NAME_REQ(string name)
        {
            RoleNameReq req = new RoleNameReq();
            req.roleName = name;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        private bool LOBBY2CLIENT_PLAYER_ROLE_RESP_Handler(Observers.Interfaces.INotification note)
        {
            //            Logic.Protocol.Model.PlayerRoleResp resp = note.Body as Logic.Protocol.Model.PlayerRoleResp;
            //			Debugger.Log(resp.ToString());
            //			UIMgr.instance.CloseAll();
            //			UIMgr.instance.Open(UI.CreateRole.View.SelectRoleView.PREFAB_PATH);
            //			return true;

            // new test
            Logic.Protocol.Model.PlayerRoleResp resp = note.Body as Logic.Protocol.Model.PlayerRoleResp;
            Debugger.Log(resp.ToString());
            UI.UIMgr.instance.Close(EUISortingLayer.MainUI);
            List<PlayerData> basicPlayerDataList = PlayerData.GetBasicPlayerDataList();
            Logic.UI.CreateRole.Controller.CreateRoleController.instance.SelectRole(basicPlayerDataList[0].Id);
            UIMgr.instance.Open(Logic.UI.CreateRole.View.CreateRoleView.PREFAB_PATH);
            if (resp.isNeedCDKey)
            {
                LuaTable loginCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "login_controller")[0];
                loginCtrlLua.GetLuaFunction("OpenLoginVerifyCDKeyView").Call();
            }

            return true;
            // new test
        }

        private bool LOBBY2CLIENT_LOGIN_SUCCESS_Handler(Observers.Interfaces.INotification note)
        {
            UI.UIMgr.instance.CloseAll();
            UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            Logic.Protocol.Model.LoginResp resp = note.Body as Logic.Protocol.Model.LoginResp;
            //Debugger.Log(resp.account);
            //Debugger.Log(resp.ToString());

            //            PlayerProxy.instance.SetPlayerName(resp.roleName);
            GameProxy.instance.AccountName = resp.roleName;
            List<PlayerInfo> playerInfoList = PlayerProxy.instance.PlayerInfoDictionary.GetValues();
            int playerInfoCount = playerInfoList.Count;
            //            for (int i = 0; i < playerInfoCount; i++)
            //            {
            //                playerInfoList[i].name = PlayerProxy.instance.PlayerName;
            //            }
            //            GameProxy.instance.PlayerInfo.name = PlayerProxy.instance.PlayerName;
            GameProxy.instance.serverId = resp.serverId;
            GameProxy.instance.AccountId = resp.roleId;
            GameProxy.instance.AccountHeadIcon = UIUtil.ParseHeadIcon(resp.headNo);
            //GameProxy.instance.UpdateAccountLevelAndExp(resp.lv, resp.exp);
            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "game_model")[0];
            table.GetLuaFunction("UpdateAccountLevelAndExp").Call(resp.lv, resp.exp);
            table.GetLuaFunction("SetAccountName").Call(resp.roleName);
            table.GetLuaFunction("SetAccountHeadNo").Call(resp.headNo);
            table.GetLuaFunction("SetLastServerId").Call(resp.serverId);
            table.GetLuaFunction("SetAccountId").Call(resp.account);
            table.GetLuaFunction("SetServerName").Call(ServerList.Model.ServerListProxy.instance.GetCurrentServerInfo().name);
            TimeController.instance.login = true;
            TimeController.instance.ServerTimeTicksMillisecond = resp.serverTime;

            LuaInterface.LuaTable onlineGiftController = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "online_gift_controller")[0];
            onlineGiftController.GetLuaFunction("OnlineGiftSynReq").Call();
            Protocol.ProtocolProxy.instance.SendProtocol(new ClientActiveReq());

            PlayerPrefs.SetString("account", resp.account);
            PlayerPrefs.SetInt("lastLoginServerId", ServerList.Model.ServerListProxy.instance.lastServerId);
            PlayerPrefs.Save();
#if UNITY_IOS
            PlatformProxy.instance.InitIOSPayment();
			Game.Model.GameProxy.instance.SetLoginSuccessData(resp.account, string.Empty,string.Empty, PlatformProxy.instance.GetPlatformId());
#endif
            GameProxy.instance.SendGameInfo();
            
            //if(FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Enums.FunctionOpenType.MainView_Activity))
            //    LuaInterface.ToLuaPb.SetFromLua((int)MSG.ActivityListReq);
            //if (FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Enums.FunctionOpenType.MainView_Chat))
            //    LuaInterface.ToLuaPb.SetFromLua((int)MSG.ChatInfoReq);
            //            Logic.Game.Controller.GameController.instance.LoadGame();
            //UIMgr.instance.Close(Logic.UI.ServerList.View.ServerView.PREFAB_PATH);
            Logic.Game.Controller.GameController.instance.LoadAllGame();

            if (CreateRole.Controller.CreateRoleController.instance.isCreatingNewRole)
            {
                AdTracking.Controller.AdTrackingController.instance.AdTracking_OnRegister(resp.account);
                Logic.Game.Controller.GameController.instance.SendExternalData(Logic.Enums.ExtraDataType.CreateRole);
                CreateRole.Controller.CreateRoleController.instance.isCreatingNewRole = false;
            }
            AdTracking.Controller.AdTrackingController.instance.AdTracking_OnLogin(resp.account);

            Logic.Game.Controller.GameController.instance.SendExternalData(Logic.Enums.ExtraDataType.EntryGame);
            return true;
        }

        private bool LOBBY2CLIENT_LOGIN_FAILED_Handler(Observers.Interfaces.INotification note)
        {
            return true;
        }

        //private bool LOBBY2CLIENT_BASE_RESOURCES_SYNC(Observers.Interfaces.INotification note)
        //{
        //BaseResourceSyn baseResourceSyn = note.Body as BaseResourceSyn;
        //List<BaseResourceInfo> baseResourcesInfos = baseResourceSyn.resourceInfos;
        //return true;
        //}

        private bool LOBBY2CLIENT_ROLE_HEAD_RESP_Handler(Observers.Interfaces.INotification note)
        {
            RoleHeadResp resp = note.Body as RoleHeadResp;
            GameProxy.instance.AccountHeadIcon = UIUtil.ParseHeadIcon(resp.headNo);
            //GameProxy.instance.UpdateAccountInfo();
            LuaTable gameModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "game_model")[0];
            gameModel.GetLuaFunction("SetAccountHeadNo").Call(resp.headNo);
            gameModel.GetLuaFunction("UpdateAccountLevelAndExpDelegateByProtocol").Call();
            return true;
        }
        private bool LOBBY2CLIENT_ROLE_NAME_RESP_Handler(Observers.Interfaces.INotification note)
        {
            RoleNameResp resp = note.Body as RoleNameResp;
            //            GameProxy.instance.PlayerInfo.name = resp.roleName;
            GameProxy.instance.AccountName = resp.roleName;
            GameProxy.instance.UpdateAccountInfo();
            return true;
        }
    }
}
