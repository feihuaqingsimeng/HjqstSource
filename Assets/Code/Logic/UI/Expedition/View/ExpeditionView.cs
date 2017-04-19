using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.Expedition.Model;
using Logic.UI.Expedition.Controller;
using Logic.UI.Tips.View;
using Logic.Game.Model;
using Logic.VIP.Model;
using Logic.Enums;
using Logic.UI.Shop.View;
using Logic.FunctionOpen.Model;
using Logic.ConsumeTip.Model;
using LuaInterface;


namespace Logic.UI.Expedition.View
{
	public class ExpeditionView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/expedition/expedition_view";
		public static ExpeditionView Open()
		{
			ExpeditionView view = UIMgr.instance.Open<ExpeditionView>(PREFAB_PATH);
			return view;
		}
		#region ui component
		public Text resetText;
		public Text buyVipTipText;
		public Transform panel;
		public Transform mapRoot;
		public ExpeditionMapView mapView;
		#endregion
		void Awake()
		{
			CommonTopBar.View.CommonTopBarView commonTopBarView = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(panel);
			commonTopBarView.SetAsCommonStyle(Localization.Get("ui.expedition_view.title"),OnClickBackBtnHandler,true,true,true,false,false,true);

			BindDelegate();

		}
		void Start()
		{
			resetText.text = string.Format( Localization.Get("ui.expedition_view.resetCount"),ExpeditionProxy.instance.resetCount);
			ExpeditionController.instance.CLIENT2LOBBY_Expedition_REQ();
		}
		void OnDestroy()
		{
			UnbindDelegate();
		}
		private void BindDelegate()
		{
			ExpeditionProxy.instance.onUpdateResetSucDelegate += Refresh;
			ExpeditionProxy.instance.onUpdateGetRewardSucDelegate += UpdateGetRewardSuccess;
			VIPProxy.instance.onVIPInfoUpdateDelegate += OnVIPInfoUpdateHandler;
		}
		private void UnbindDelegate()
		{
			ExpeditionProxy.instance.onUpdateResetSucDelegate -= Refresh;
			ExpeditionProxy.instance.onUpdateGetRewardSucDelegate -= UpdateGetRewardSuccess;
			VIPProxy.instance.onVIPInfoUpdateDelegate -= OnVIPInfoUpdateHandler;
		}
		private void Refresh()
		{
			mapView.Refresh();
			RefreshTimes();
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}
		private void RefreshTimes()
		{
			resetText.text = string.Format( Localization.Get("ui.expedition_view.resetCount"),ExpeditionProxy.instance.resetCount);
			buyVipTipText.gameObject.SetActive( ExpeditionProxy.instance.resetCount == 0);
			buyVipTipText.text = string.Format(Localization.Get("ui.expedition_view.remindBuy"),ExpeditionProxy.instance.expeditionRemaindBuyTimes);
		}
		#region ui event handler
		public void OnClickBackBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnClickRuleBtnHandler()
		{
			Logic.UI.Tips.View.CommonRuleTipsView.Open(Localization.Get("ui.expedition_view.ruleTitle"),Localization.Get("ui.expedition_view.rule"));
		}
		public void OnClickShopBtnHandler()
		{

		}
		public void OnClickFormationBtnHandler()
		{
			//Logic.UI.Expedition.View.ExpeditionFormationView.Open(false);
			ExpeditionEmbattleView.Open(false);
		}
		public void OnClickResetMapBtnHandler()
		{
			if(ExpeditionProxy.instance.resetCount==0)
			{
				if(ExpeditionProxy.instance.expeditionRemaindBuyTimes > 0)
				{
					if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondResetExpeditionCount))
						ConfirmCostTipsView.Open(GlobalData.GetGlobalData().faraway_buy_ResData,Localization.Get( "ui.expedition_view.resetNotEnough"),Localization.Get( "ui.expedition_view.canResetOnce"),null,ExpendCostToResetMapHandler,ConsumeTipType.DiamondResetExpeditionCount);
					else
						ExpendCostToResetMapHandler();
					return;
				}
				int nextVip = ExpeditionProxy.instance.nextVipExtraBuyTimes;
				if(nextVip != 0)
				{
					ConfirmTipsView.Open(string.Format(Localization.Get( "ui.expedition_view.improveVip"),nextVip),OpenRechargeHandler);
					return;
				}
				CommonErrorTipsView.Open(Localization.Get("ui.expedition_view.resetCountDown"));
				return;
			}else
			{
				ConfirmTipsView.Open(Localization.Get("ui.expedition_view.confirmResetTip"),ClickConfirmResetMapHandler);
			}


		}
		private void ClickConfirmResetMapHandler()
		{
			ExpeditionController.instance.CLIENT2LOBBY_Expedition_Reset_REQ(false);
			Refresh();
		}
		
		private void ExpendCostToResetMapHandler()
		{
			GameResData data = GlobalData.GetGlobalData().faraway_buy_ResData;
			int own = GameProxy.instance.BaseResourceDictionary.GetValue(data.type);
			if(own < data.count)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.expedition_view.notEnoughDiamond"));
				return;
			}
			ExpeditionController.instance.CLIENT2LOBBY_Expedition_Reset_REQ(true);
		}
		private void OpenRechargeHandler()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
		}
		#endregion
		#region update by server
		private void UpdateGetRewardSuccess()
		{
			ExpeditionData data = ExpeditionData.GetExpeditionDataByID(ExpeditionProxy.instance.selectExpeditionDungeonInfo.id);
			LuaTable tips_model = LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","tips_model")[0] as LuaTable;
			LuaTable tip_view =tips_model.GetLuaFunction("GetTipView").Call("common_reward_tips_view")[0] as LuaTable;
			tip_view.GetLuaFunction("CreateByCSharpGameResDataList").Call(data.rewardList);
		}
		private void OnVIPInfoUpdateHandler(int vipLevel, int totalRecharge, List<int> hasReceivedGiftVIPLevelList)
		{
			ExpeditionController.instance.CLIENT2LOBBY_Expedition_REQ();
		}
		#endregion
	}
}

