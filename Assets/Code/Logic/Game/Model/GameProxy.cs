using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol.Model;
using Logic.UI;
using Logic.Player.Model;
using Logic.Dungeon.Model;
using Logic.Game.Controller;
using LuaInterface;
using System.Collections;
using Logic.UI.Main.View;
using Logic.UI.Login.Model;
using LitJson;
using Common.Localization;
using Logic.UI.ServerList.Model;
using Logic.VIP.Model;

namespace Logic.Game.Model
{
    public class GameProxy : SingletonMono<GameProxy>
    {
        private LuaTable _gameModelLuaTable;
        public LuaTable GameModelLuaTable
        {
            get
            {
                if (_gameModelLuaTable == null)
                    _gameModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "game_model")[0];
                return _gameModelLuaTable;
            }
        }

        private int _serverId;
        public int serverId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }

        private string _accountName;
        public string AccountName
        {
            set
            {
                _accountName = value;
            }
            get
            {
                return _accountName;
            }
        }

        private int _accountLevel;
        public int AccountLevel
        {
            get
            {
                return _accountLevel;
            }
        }
        public int AccountMaxLevel
        {
            get
            {
                return GlobalData.GetGlobalData().accountLevelMax;
            }
        }
        public bool IsMaxAccountLevel
        {
            get
            {
                return _accountLevel == AccountMaxLevel;
            }
        }
        private int _accountExp;
        public int AccountExp
        {
            get
            {
                return _accountExp;
            }
        }
        public float AccountExpPercent
        {
            get
            {
                AccountExpData data = AccountExpData.GetAccountExpDataByLv(_accountLevel);
                if (data == null)
                    return 0;
                return (GameProxy.instance.AccountExp + 0.0f) / data.exp;
            }
        }
        public int AccountId
        {
            get;
            set;
        }
        private string _accountHeadIcon;
        public string AccountHeadIcon
        {
            get
            {
                if (string.IsNullOrEmpty(_accountHeadIcon))
                {
                    return PlayerInfo.HeadIcon;
                }
                return _accountHeadIcon;
            }
            set
            {
                _accountHeadIcon = value;
            }
        }

        private int _pveAction;
        public int PveAction
        {
            get
            {
                return _pveAction;
            }
        }

        private int _pveActionMax;
        public int PveActionMax
        {
            get
            {
                return _pveActionMax;
            }
        }

        private float _pveActionNextRecoverTime;
        public float PveActionNextRecoverTime
        {
            get
            {
                return _pveActionNextRecoverTime;
            }
        }
        private int _pvpAction;
        public int PvpAction
        {
            get
            {
                return _pvpAction;
            }
            set
            {
                _pvpAction = value;
            }

        }
        private int _pvpActionMax;
        public int PvpActionMax
        {
            get
            {
                return _pvpActionMax;
            }
            set
            {
                _pvpActionMax = value;
            }
        }
        //		//回复倒计时（ms）
        //		private float _pvpActionNextRecoverTime;
        //		public float PvpActionNextRecoverTime
        //		{
        //			get
        //			{
        //				return _pvpActionNextRecoverTime;
        //			}
        //		}
        //		//回复时间点（ms）
        //		private long _pvpActionNextRecoverTimePoint;

        public int HeroCellNum
        {
            get
            {
                LuaFunction getHeroCellNum = GameModelLuaTable.GetLuaFunction("GetHeroCellNum");
                int heroCellNum = getHeroCellNum.Call(null)[0].ToString().ToInt32();
                return heroCellNum;
            }
        }

        public int HeroCellBuyNum
        {
            get
            {
                LuaFunction getHeroCellBuyNum = GameModelLuaTable.GetLuaFunction("GetHeroCellBuyNum");
                int heroCellBuyNum = getHeroCellBuyNum.Call(null)[0].ToString().ToInt32();
                return heroCellBuyNum;
            }
        }

        public int EquipCellNum
        {
            get
            {
                LuaFunction getEquipCellNum = GameModelLuaTable.GetLuaFunction("GetEquipCellNum");
                int equipCellNum = getEquipCellNum.Call(null)[0].ToString().ToInt32();
                return equipCellNum;
            }
        }

        public int EquipCellBuyNum
        {
            get
            {
                LuaFunction getEquipCellBuyNum = GameModelLuaTable.GetLuaFunction("GetEquipCellBuyNum");
                int heroEquipCellBuyNum = getEquipCellBuyNum.Call(null)[0].ToString().ToInt32();
                return heroEquipCellBuyNum;
            }
        }

        private Dictionary<BaseResType, int> _baseResourceDictionary;
        public Dictionary<BaseResType, int> BaseResourceDictionary
        {
            get
            {
                if (_baseResourceDictionary == null)
                {
                    _baseResourceDictionary = new Dictionary<BaseResType, int>();
                }
                return _baseResourceDictionary;
            }
        }

        private PlayerInfo _playerInfo;
        public PlayerInfo PlayerInfo
        {
            get
            {
                return _playerInfo;
            }
            set
            {
                _playerInfo = value;
            }
        }
        public bool CheckPackFull(bool isCheckHero = true, bool isCheckEquip = true)
        {
            object o = GameModelLuaTable.GetLuaFunction("CheckPackFull").Call(isCheckHero, isCheckEquip)[0];
            return o.ToString().ToBoolean();
        }
        public bool IsPlayer(uint instanceID)
        {
            return instanceID == _playerInfo.instanceID;
        }

        public List<DungeonInfo> allDungeonInfoList = new List<DungeonInfo>();
        private DungeonInfo _currentSelectDungeonInfo;
        public DungeonInfo CurrentSelectDungeonInfo
        {
            get
            {
                return _currentSelectDungeonInfo;
            }
            set
            {
                _currentSelectDungeonInfo = value;
            }
        }

        #region delegates
        public delegate void OnAccountInfoUpdateDelegate();
        public OnAccountInfoUpdateDelegate onAccountInfoUpdateDelegate;

        public delegate void OnPveActionInfoUpdateDelegate();
        public OnPveActionInfoUpdateDelegate onPveActionInfoUpdateDelegate;

        public delegate void OnPveActionNextRecoverTimeUpdateDelegate();
        public OnPveActionNextRecoverTimeUpdateDelegate onPveActionNextRecoverTimeUpdateDelegate;

        public System.Action onPvpActionInfoUpdateDelegate;
        public System.Action onPvpActionNextRecoverTimeUpdateDelegate;

        public delegate void OnBaseResourcesUpdateDelegate();
        public OnBaseResourcesUpdateDelegate onBaseResourcesUpdateDelegate;

        public delegate void OnHeroCellNumUpdateDelegate();
        public OnHeroCellNumUpdateDelegate onHeroCellNumUpdateDelegate;

        public delegate void OnEquipCellNumUpdateDelegate();
        public OnEquipCellNumUpdateDelegate onEquipCellNumUpdateDelegate;
        #endregion delegates

        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            LeanTween.delayedCall(1, UpdatePveAction).setIgnoreTimeScale(true);
            //StartCoroutine (UpdateAction());
        }
        //		private IEnumerator UpdateAction ()
        //		{
        //			while (true) 
        //			{
        //				UpdatePvpAction();
        //				yield return new WaitForSeconds(1);
        //			}
        //		}

		public void SetLoginSuccessData(string sdkId, string u8id, string token, int platformId)
        {
            if (PlatformProxy.instance.GetPlatformId() == 0) return;
			LoginProxy.instance.cachedAccount = sdkId;
            LoginProxy.instance.cachedPassword = string.Empty;
            LoginProxy.instance.cachedToken = token;
            LoginProxy.instance.cachedPlatformId = platformId;
			LoginProxy.instance.cachedU8userID = u8id;
			Logic.TalkingData.Controller.TalkingDataController.instance.TDGAAccountSetAccount(sdkId);
            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "game_model")[0];
            table.GetLuaFunction("SetChannelId").Call(platformId);
			table.GetLuaFunction("SetPlatformAccountId").Call(sdkId);
        }

        public void SendGameInfo()
        {
            JsonData data = new JsonData();
            data["roleId"] = GameProxy.instance.AccountId;
            data["roleName"] = GameProxy.instance.AccountName;
            data["roleLevel"] = GameProxy.instance.AccountLevel;
            data["serverId"] = GameProxy.instance.serverId;
            data["vip"] = VIPProxy.instance.VIPLevel;
            data["serverName"] = Localization.Get(ServerListProxy.instance.ServerListDictionary[ServerListProxy.instance.lastServerId].name);
            PayCallbackData payData = PayCallbackData.GetPayCallbackDataByID(LoginProxy.instance.cachedPlatformId);
            if (payData != null)
                data["payCallback"] = payData.callback;
            PlatformProxy.instance.SendGameInfo(data.ToJson());

        }

        private void UpdatePveAction()
        {
            if (_pveAction < _pveActionMax)
            {
                if (_pveActionNextRecoverTime > 0)
                {
                    _pveActionNextRecoverTime = Mathf.Max(_pveActionNextRecoverTime - 1, 0);
                    OnPveActionNextRecoverTimeUpdate();
                    GameModelLuaTable.GetLuaFunction("OnPveActionInfoUpdate").Call(_pveAction, _pveActionMax, _pveActionNextRecoverTime < 0 ? 0 : _pveActionNextRecoverTime);
                    if (_pveActionNextRecoverTime <= 0)
                    {
                        RequestPveInfo();
                    }
                }
            }
            LeanTween.delayedCall(1, UpdatePveAction).setIgnoreTimeScale(true);
        }
        //		private void UpdatePvpAction()
        //		{
        //			if (_pvpAction < _pvpActionMax)
        //			{
        //				if (_pvpActionNextRecoverTime > 0)
        //				{
        //					_pvpActionNextRecoverTime = Mathf.Max(_pvpActionNextRecoverTimePoint - Common.GameTime.Controller.TimeController.instance.ServerTimeTicksSecond*1000, 0);
        //				
        //					if(onPvpActionNextRecoverTimeUpdateDelegate != null)
        //					{
        //						onPvpActionNextRecoverTimeUpdateDelegate();
        //					}
        //					if (_pvpActionNextRecoverTime <= 0)
        //					{
        //						RequestPvpInfo();
        //					}
        //				}
        //			}
        //		}
        public void UpdateAccountLevelAndExp(int accountLevel, int accountExp)
        {
            int oldLevel = this._accountLevel;
            this._accountLevel = accountLevel;
            this._accountExp = accountExp;
            UpdateAccountInfo();
            Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.Init();
            Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.UpdateSoftGuide();
            if (oldLevel != 0 && oldLevel < accountLevel && UIMgr.instance.Get(MainView.PREFAB_PATH) != null)
            {
                LevelUpTipsView.Open();
            }
        }
        public void UpdateAccountInfo()
        {
            if (onAccountInfoUpdateDelegate != null)
            {
                onAccountInfoUpdateDelegate();
            }
        }
        public int GetExpandHeroBagCost()
        {
            int cost = (HeroCellBuyNum + 1) * GlobalData.GetGlobalData().heroPackageBuyA + GlobalData.GetGlobalData().heroPackageBuyB;
            return cost;
        }
        private void RequestPveInfo()
        {
            GameController.instance.CLIENT2LOBBY_SYNC_PVE_ACTION_REQ();
        }
        private void RequestPvpInfo()
        {
            GameController.instance.CLIENT2LOBBY_SYNC_PVP_ACTION_REQ();
        }
        public void OnPveActionInfoUpdate(SynPveActionResp syncPveActionResp)
        {
            _pveAction = syncPveActionResp.pveAction;
            _pveActionMax = syncPveActionResp.pveActionMax;
            _pveActionNextRecoverTime = syncPveActionResp.nextRecoverTime;

            if (onPveActionInfoUpdateDelegate != null)
            {
                onPveActionInfoUpdateDelegate();
            }

            if (_pveAction < _pveActionMax && _pveActionNextRecoverTime <= 0)
            {
                Invoke("RequestPveInfo", 1);
            }
            GameModelLuaTable.GetLuaFunction("OnPveActionInfoUpdate").Call(_pveAction, _pveActionMax, _pveActionNextRecoverTime < 0 ? 0 : _pveActionNextRecoverTime);

        }
        public void OnPvpActionInfoUpdate(int pvpAction, int pvpActionMax, long nextRecoverTimePoint)
        {
            _pvpAction = pvpAction;
            _pvpActionMax = pvpActionMax;
            //_pvpActionNextRecoverTimePoint = nextRecoverTimePoint;
            //_pvpActionNextRecoverTime = Mathf.Max(_pvpActionNextRecoverTimePoint - Common.GameTime.Controller.TimeController.instance.ServerTimeTicksSecond*1000, 0);
            onPvpActionInfoUpdateDelegateByProtocol();

            //			if(_pvpAction >= _pvpActionMax)
            //			{
            //				if(onPvpActionNextRecoverTimeUpdateDelegate!=null)
            //				{
            //					onPvpActionNextRecoverTimeUpdateDelegate();
            //				}
            //			}
            //			if(_pvpAction < _pvpActionMax && _pvpActionNextRecoverTime <= 0)
            //			{
            //				Invoke("RequestPvpInfo",1);
            //			}
        }

        public void OnPveActionNextRecoverTimeUpdate()
        {
            if (onPveActionNextRecoverTimeUpdateDelegate != null)
            {
                onPveActionNextRecoverTimeUpdateDelegate();
            }
        }

        public void UpdateHeroCellNum(int heroCellNum, int heroCellBuyNum)
        {

            if (onHeroCellNumUpdateDelegate != null)
            {
                onHeroCellNumUpdateDelegate();
            }
        }

        public void UpdateEquipcellNum(int equipCellNum, int equipCellBuyNum)
        {

            if (onEquipCellNumUpdateDelegate != null)
            {
                onEquipCellNumUpdateDelegate();
            }
        }

        public void OnBaseResourcesUpdate(List<BaseResourceInfo> baseResourceInfos)
        {
            int baseResourceInfoCount = baseResourceInfos.Count;
            for (int i = 0; i < baseResourceInfoCount; i++)
            {
                BaseResourceInfo baseResourceInfo = baseResourceInfos[i];
                BaseResType baseResType = (BaseResType)baseResourceInfo.type;
                if (!BaseResourceDictionary.ContainsKey(baseResType))
                {
                    BaseResourceDictionary.Add(baseResType, baseResourceInfo.value);
                }
                else
                {
                    BaseResourceDictionary[baseResType] = baseResourceInfo.value;
                }
            }

            if (onBaseResourcesUpdateDelegate != null)
            {
                onBaseResourcesUpdateDelegate();
            }
        }
        public void onPvpActionInfoUpdateDelegateByProtocol()
        {
            if (onPvpActionInfoUpdateDelegate != null)
            {
                onPvpActionInfoUpdateDelegate();
            }
        }

    }
}
