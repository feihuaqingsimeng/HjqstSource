using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Activity.Model;
using Logic.Fight.Controller;
using Logic.FunctionOpen.Model;
using Logic.Enums;
using Common.Localization;
using Logic.VIP.Model;
using Logic.UI.Tips.View;
using Logic.Activity.Controller;
using Logic.UI.FightResult.Model;
using Logic.ConsumeTip.Model;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonWinView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/daily_dungeon/daily_dungeon_win_view";

		public enum Multiple
		{
			One = 1,
			Double = 2,
			Triple = 3,
		}
		private Multiple multiple = Multiple.One;

		private List<GameResData> _rewardGameResDataList = new List<GameResData>();

		#region UI components
		public List<DailyDungeonRewardCard> dailyDungeonRewardCardList;
		public GameObject drawMultipleRewardRootGameObject;
		public Text rechargeText;
		public Text vipLevelText;

		public Text drawDoubleRewardDiamondCost;
		public Text drawTripleRewardDiamondCost;
		public Text drawDoubleRewardVIPLevelText;
		public Text drawTripleRewardVIPLevelText;
		#endregion UI components

		public static void Open ()
		{
			UIMgr.instance.Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
		}

		void Awake ()
		{
			VIPData currentVIPData = VIPProxy.instance.VIPData;

			rechargeText.text = VIPProxy.instance.TotalRechargeDiamond.ToString();
			vipLevelText.text = currentVIPData.ShortName;

			VIPData drawDoubleRewardVIPData = VIPData.GetVIPData(GlobalData.GetGlobalData().dailyDungeonAwardVIPLevelList[0]);
			VIPData drawTripleRewardVIPData = VIPData.GetVIPData(GlobalData.GetGlobalData().dailyDungeonAwardVIPLevelList[1]);

			drawDoubleRewardDiamondCost.text = GlobalData.GetGlobalData().dailyDungeonAwardCostList[0].count.ToString();
			drawTripleRewardDiamondCost.text = GlobalData.GetGlobalData().dailyDungeonAwardCostList[1].count.ToString();

			drawDoubleRewardVIPLevelText.text = string.Format(Localization.Get("ui.daily_dungeon_win_view.vip_privilege_title"), drawDoubleRewardVIPData.ShortName);
			drawTripleRewardVIPLevelText.text = string.Format(Localization.Get("ui.daily_dungeon_win_view.vip_privilege_title"), drawTripleRewardVIPData.ShortName);

			for (int i = 0, count = ActivityProxy.instance.fixedRewardGameResDataList.Count; i < count; i++)
			{
				_rewardGameResDataList.Add(new GameResData(ActivityProxy.instance.fixedRewardGameResDataList[i]));
			}

			for (int i = 0, count = dailyDungeonRewardCardList.Count; i < count; i++)
			{
				if (i < ActivityProxy.instance.fixedRewardGameResDataList.Count)
				{
					dailyDungeonRewardCardList[i].gameObject.SetActive(true);
					dailyDungeonRewardCardList[i].SetGameResData(_rewardGameResDataList[i]);
				}
				else
				{
					dailyDungeonRewardCardList[i].gameObject.SetActive(false);
				}
			}
		}

		void Start ()
		{
			ActivityProxy.instance.onActivityAwardMultipledDelegate += OnActivityAwardMultipledHandler;
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}

		void OnDestroy ()
		{
			ActivityProxy.instance.onActivityAwardMultipledDelegate -= OnActivityAwardMultipledHandler;
		}
		
		#region proxy callback
		public void OnActivityAwardMultipledHandler ()
		{
			for (int i = 0, count = ActivityProxy.instance.fixedRewardGameResDataList.Count; i < count; i++)
			{
				if (multiple == Multiple.Double)
				{
					_rewardGameResDataList[i].count *= 2;
				}
				else if (multiple == Multiple.Triple)
				{
					_rewardGameResDataList[i].count *= 3;
				}
			}
			
			for (int i = 0, count = dailyDungeonRewardCardList.Count; i < count; i++)
			{
				if (i < ActivityProxy.instance.fixedRewardGameResDataList.Count)
				{
					dailyDungeonRewardCardList[i].gameObject.SetActive(true);
					dailyDungeonRewardCardList[i].SetGameResData(_rewardGameResDataList[i]);
				}
				else
				{
					dailyDungeonRewardCardList[i].gameObject.SetActive(false);
				}
			}
		}
		#endregion proxy callback

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UI.UIMgr.instance.Close(PREFAB_PATH);
			FightResultProxy.instance.GotoMainScene(FightResultQuitType.Go_Activity,FightResultProxy.instance.QuitActivityCallback);
		}
		
		public void ClickDrawDoubleRewardButtonHandler ()
		{
			multiple = Multiple.Double;

			if (VIPProxy.instance.VIPLevel < GlobalData.GetGlobalData().dailyDungeonAwardVIPLevelList[0])
			{
				CommonErrorTipsView.Open(Localization.Get("ui.daily_dungeon_win_view.vip_level_not_enough_tips"));
				return;
			}

			if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondDrawMutipleReward))
			{
				ConsumeTipData consumeTipData = ConsumeTipData.GetConsumeTipDataByType(ConsumeTipType.DiamondDrawMutipleReward);
				ConfirmCostTipsView.Open(GlobalData.GetGlobalData().dailyDungeonAwardCostList[0], Localization.Get(consumeTipData.des), null, ConfirmDrawDoubleReward, ConsumeTipType.DiamondDrawMutipleReward);
			}
			else
			{
				ConfirmDrawDoubleReward();
			}
		}

		public void ConfirmDrawDoubleReward ()
		{
			if (GameProxy.instance.BaseResourceDictionary[BaseResType.Diamond] < GlobalData.GetGlobalData().dailyDungeonAwardCostList[0].count)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.daily_dungeon_win_view.diamond_not_enough_tips"));
				return;
			}
			
			ActivityController.instance.CLIENT2LOBBY_ACTIVITY_PVE_AWARD_REQ(1);
			drawMultipleRewardRootGameObject.SetActive(false);
		}

		public void ClickDrawTripleRewardButtonHandler ()
		{
			multiple = Multiple.Triple;

			if (VIPProxy.instance.VIPLevel < GlobalData.GetGlobalData().dailyDungeonAwardVIPLevelList[1])
			{
				CommonErrorTipsView.Open(Localization.Get("ui.daily_dungeon_win_view.vip_level_not_enough_tips"));
				return;
			}
			
			if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondDrawMutipleReward))
			{
				ConsumeTipData consumeTipData = ConsumeTipData.GetConsumeTipDataByType(ConsumeTipType.DiamondDrawMutipleReward);
				ConfirmCostTipsView.Open(GlobalData.GetGlobalData().dailyDungeonAwardCostList[1], Localization.Get(consumeTipData.des), null, ConfirmDrawTripleReward, ConsumeTipType.DiamondDrawMutipleReward);
			}
			else
			{
				ConfirmDrawTripleReward();
			}
		}

		public void ConfirmDrawTripleReward ()
		{
			if (GameProxy.instance.BaseResourceDictionary[BaseResType.Diamond] < GlobalData.GetGlobalData().dailyDungeonAwardCostList[1].count)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.daily_dungeon_win_view.diamond_not_enough_tips"));
				return;
			}
			
			ActivityController.instance.CLIENT2LOBBY_ACTIVITY_PVE_AWARD_REQ(2);
			drawMultipleRewardRootGameObject.SetActive(false);
		}
		#endregion UI event handlers
	}
}
