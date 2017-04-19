using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Hero.Model;
using Logic.Game.Model;
using Common.ResMgr;
using Logic.Player.Model;
using Common.Localization;
using Common.Util;
using Logic.UI.Pvp.Model;
using Logic.UI.Pvp.Controller;
using Logic.UI.Tips.View;
using Logic.Protocol.Model;
using Logic.VIP.Model;
using LuaInterface;
using Logic.Enums;
using Common.UI.Components;
using Logic.Formation.Model;

namespace Logic.UI.Pvp.View
{
    public class PvpView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/pvp/pvp_view";

        public static PvpView Open()
        {
            PvpView view = UIMgr.instance.Open<PvpView>(PREFAB_PATH);
            return view;
        }


        #region ui component
        public Text textPowerNum;
        public Text textRankNum;
        public Text textChangeFighterRemindTime;
		public Text textName;
		public Transform formationRoot;
		public ScrollContent scrollContent;
		private bool isFirstEnter = true;
		public Transform Panel;
        #endregion

        private string _canGetRewardString = string.Empty;
        void Awake()
        {
            CommonTopBar.View.CommonTopBarView view = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(Panel);
            view.SetAsCommonStyle(Localization.Get("ui.pvp_view.textTitle"), OnClickBackButtonHandler, false, false, true, true, true);
        }
        void Start()
        {
            Init();
            BindDelegate();
        }
        private void OnDestroy()
        {
            UnbindDelegate();
        }
        private void Init()
        {
            PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REQ();
            _canGetRewardString = Localization.Get("ui.pvp_view.canGetReward");
        }
        private void BindDelegate()
        {
            PvpProxy.instance.OnUpdateRefreshFighterTimeDelegate += UpdateUIRefreshFighterTime;
            PvpProxy.instance.OnUpdatePvpInfoDelegate += UpdatePvpInfoByProtocol;
            PvpProxy.instance.onUpdateTopHundredRankingSuccessDelegate += UpdateTopHundredRankingByProtocol;
            PvpProxy.instance.OnUpdateFighterDelegate += RefreshFighter;

			Observers.Facade.Instance.RegisterObserver("OnFormationChange", OnFormationChangeHandler);
        }
        private void UnbindDelegate()
        {
            PvpProxy.instance.OnUpdateRefreshFighterTimeDelegate -= UpdateUIRefreshFighterTime;
            PvpProxy.instance.OnUpdatePvpInfoDelegate -= UpdatePvpInfoByProtocol;
            PvpProxy.instance.onUpdateTopHundredRankingSuccessDelegate -= UpdateTopHundredRankingByProtocol;
            PvpProxy.instance.OnUpdateFighterDelegate -= RefreshFighter;

			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnFormationChange", OnFormationChangeHandler);
			}
        }

        private void InitText()
        {
            //	textTitle.text = Localization.Get("ui.pvp_view.textTitle");
//            textPowerDes.text = Localization.Get("ui.pvp_view.textPowerDes");
//            textRankDes.text = Localization.Get("ui.pvp_view.textRankDes");
//            textVictoryDes.text = Localization.Get("ui.pvp_view.textVictoryDes");
//            textRankReward.text = Localization.Get("ui.pvp_view.textRankReward");
//            textMyInfomation.text = Localization.Get("ui.pvp_view.textMyInfomation");
//            textBattleReport.text = Localization.Get("ui.pvp_view.textBattleReport");
//            textHundredRank.text = Localization.Get("ui.pvp_view.textHundredRank");
//            textChangeFighter.text = Localization.Get("ui.pvp_view.textChangeFighter");
//            textFormation.text = Localization.Get("ui.pvp_view.textFormation");
        }
        void Update()
        {
            PvpProxy.instance.UpdateChangeFighterTime();
            //PvpProxy.instance.UpdateMyPvpRewardTime();

        }
        private void Refresh()
        {
            RefreshInformation();
            UpdateUIRefreshFighterTime();
            RefreshFighter();
        }
        private void RefreshInformation()
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
            PvpInfo pvpInfo = PvpProxy.instance.PvpInfo;
			textPowerNum.text = PvpFormationProxy.instance.GetPower().ToString();
            textRankNum.text = pvpInfo.rankNo.ToString();
			textName.text = GameProxy.instance.AccountName;
			FormationTeamInfo teamInfo = PvpFormationProxy.instance.FormationTeamInfo;
			TransformUtil.ClearChildren(formationRoot,true);

			foreach(var value in teamInfo.teamPosDictionary)
			{
				CommonHeroIcon.View.CommonHeroIcon icon = CommonHeroIcon.View.CommonHeroIcon.Create(formationRoot);
				if(value.Value == playerInfo.instanceID)
				{
					icon.SetRoleInfo(playerInfo);
				}else
				{
					icon.SetRoleInfo(HeroProxy.instance.GetHeroInfo(value.Value));
				}

			}

        }
		private bool isFirstInit = true;
        private void RefreshFighter()
        {
			if (isFirstInit)
				scrollContent.Init(PvpProxy.instance.PvpInfo.fighterInfoList.Count,true);
			else
				scrollContent.RefreshAllContentItems();
			isFirstInit = false;
        }

        private void UpdateUIRefreshFighterTime()
        {
            PvpProxy pvpProxy = PvpProxy.instance;
            int count = pvpProxy.PvpInfo.remainRefreshTimes;
            int time = (int)pvpProxy.PvpInfo.refreshTimesCountDown;

            if (pvpProxy.IsChangeFighterRecoverOver)
            {
                textChangeFighterRemindTime.text = string.Format(Localization.Get("common.value/max"), count, pvpProxy.PvpInfo.refreshMaxCount);

            }
            else
            {
                textChangeFighterRemindTime.text = TimeUtil.FormatSecondToMinute((int)(PvpProxy.instance.PvpInfo.refreshTimesCountDown / 1000f));

            }
        }
		private void RefreshPower()
		{
			textPowerNum.text = PvpFormationProxy.instance.GetPower().ToString();
		}

		public void OnResetItemHandler(GameObject go,int index)
		{
			PvpFighterButton fightBtn = go.GetComponent<PvpFighterButton>();
			fightBtn.SetPvpFighterInfo(PvpProxy.instance.PvpInfo.fighterInfoList[index]);
		}

        #region update from server
        private void UpdatePvpInfoByProtocol()
        {
            Debugger.Log("lastRankNo:{0},curRank:{1}", PvpProxy.instance.PvpInfo.lastRankNo, PvpProxy.instance.PvpInfo.rankNo);
			if (isFirstEnter && PvpProxy.instance.PvpInfo.lastRankNo > 0 && PvpProxy.instance.PvpInfo.lastRankNo < PvpProxy.instance.PvpInfo.rankNo) 
            {
                LuaTable arena_controller = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "arena_controller")[0];
                arena_controller.GetLuaFunction("OpenRankChangeTipView").Call(PvpProxy.instance.PvpInfo.rankNo,PvpProxy.instance.PvpInfo.lastRankNo);
            }
			isFirstEnter = false;
            Refresh();
        }
        //private void UpdateGainRewardByProtocol()
        //{
        //    CommonRewardAutoDestroyTipsView.Open(PvpArenaPrizeData.GetDataByRank(PvpProxy.instance.PvpInfo.lastRankNo).bonusList);
        //    PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REQ();//刷界面
        //}
        private void UpdateTopHundredRankingByProtocol(List<PvpFighterInfo> fighterList)
        {
            PvpTopHundredRankView.Open(fighterList);
        }

		public bool OnFormationChangeHandler (Observers.Interfaces.INotification note)
		{
			RefreshInformation();
			return true;
		}
        #endregion

        #region ui event handler

        public void OnClickPvpFighterButtonHandler(PvpFighterButton fighterBtn)
        {
			if (GameProxy.instance.PvpAction <= 0)
			{
				ConfirmTipsView.Open(Localization.Get("ui.pvp_formation_view.notEnoughChallengeTimesAndGoToBuy"), GoToBuyAction);
				return;
			}

            PvpProxy.instance.ChallengeFighter = fighterBtn.pvpFighterInfo;

            //			string id = "";
            //			for(int i = 0,count= fighterBtn.pvpFighterInfo.heroTeamDataList.Count;i<count;i++)
            //			{
            //				HeroTeamProtoData data = fighterBtn.pvpFighterInfo.heroTeamDataList[i];
            //				id = string.Format("{0}{1},",id,data.id);
            //
            //			}
			Debugger.Log(string.Format("挑战对手{0}：playerid:{1}",fighterBtn.pvpFighterInfo.id,fighterBtn.pvpFighterInfo.playerInfo.instanceID));
            //PvpFormationView.Open(true);
            //PVPEmbattleView.Open(true);
			PvpController.instance.CLIENT2LOBBY_RANK_ARENA_CHANLLENGE_REQ(PvpProxy.instance.ChallengeFighter);
			//LuaTable friendCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","friend_controller")[0];
			//friendCtrlLua.GetLuaFunction("RoleInfoLookUpReq").Call(fighterBtn.pvpFighterInfo.id,(int)FunctionOpenType.FightCenter_Arena);
        }

		public void GoToBuyAction ()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
		}

		public void OnClickChangeFormationHandler()
		{
			PVPEmbattleView.Open(false);
		}
        public void OnClickBackButtonHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        public void OnClickBattleReportBtnHandler()
        {
            PvpBattleReportView.Open();


        }
        public void OnClickTopHundredRankBtnHandler()
        {
            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "ranking_controller")[0];
            table.GetLuaFunction("OpenRankingView").Call(4);
        }
        public void OnClickMyRewardBtnHandler()
        {
			LuaTable arenaCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","arena_controller")[0];
			arenaCtrlLua.GetLuaFunction("OpenChallengeRewardView").Call();
//            if (PvpProxy.instance.PvpInfo.lastRankNo > 0)
//            {
//                PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REWARD_REQ();
//            }
//            else
//            {
//                CommonErrorTipsView.Open(Localization.Get("ui.pvp_view.timeNotArrive"));
//            }
        }
        public void OnClickRankRewardBtnHandler()
        {
            PvpRewardRankView.Open();
        }
        public void OnClickChangeFighterBtnHandler()
        {
            if (PvpProxy.instance.PvpInfo.remainRefreshTimes == 0)
            {
                CommonErrorTipsView.Open(Localization.Get("ui.pvp_view_refreshNotEnough"));
                return;
            }
            if (!PvpProxy.instance.IsChangeFighterRecoverOver)
            {
                CommonErrorTipsView.Open(Localization.Get("ui.pvp_view.refreshCD"));
                return;
            }

            PvpController.instance.CLIENT2LOBBY_REFRESH_OPPONENTS_REQ();

        }
        public void OnClickFormationBtnHandler()
        {
            //			PvpFormationView.Open(false);
            PVPEmbattleView.Open(false);
        }
        #endregion
    }


}
