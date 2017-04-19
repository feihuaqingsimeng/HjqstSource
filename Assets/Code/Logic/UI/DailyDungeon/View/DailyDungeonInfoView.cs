using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Common.Util;
using Logic.Enums;
using Logic.UI.Tips.View;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.UI.CommonReward.View;
using Logic.Dungeon.Model;
using Logic.Activity.Model;
using Logic.Activity.Controller;
using Logic.UI.CommonTopBar.View;
using Logic.FunctionOpen.Model;
using Logic.UI.Description.View;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonInfoView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/daily_dungeon/daily_dungeon_info_view";
		public static DailyDungeonInfoView Open(ActivityInfo info)
		{
			DailyDungeonInfoView view = UIMgr.instance.Open<DailyDungeonInfoView>(PREFAB_PATH);
			view.SetActivityInfo(info);
			return view;
		}

		private ActivityInfo _activityInfo;
		private int _currentSelectLevel = 0;
		private DungeonData CurrentSelectDungeonData
		{
			get
			{
				return _activityInfo.ActivityData.GetDungeonDataOfLevel(_currentSelectLevel);
			}
		}

		#region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text dailyDungeonNameText;
		public Image dailyDungeonIconImage;

		public Transform enemiesRootTransform;
		public Transform lootRootTransform;

		public Text enemiesPreviewText;
		public Text clickEnemyIconForFetailText;
		public Text dailyDungeonlootText;
		public Text embattleText;
		public Text remainChallengeTimesText;
		public GameObject buyChallengeTimeBtnGO;
		public Button startButton;
		public Text startText;
		public Text costText;
		public GameObject bottomButtonsRootGameObject;
		public Text closedTipesText;

		public Text currentLevelNameText;

		public Button previousDungeonButton;
		public Button nextDungeonButton;
		public GameObject newLevelUnlockedFXRootGameObject;
		#endregion UI components

		void Awake ()
		{
			string title = Localization.Get("ui.daily_dungeon_info_view.daily_dungeon_info_title");
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, true);

			enemiesPreviewText.text = Localization.Get("ui.daily_dungeon_info_view.enemies_preview");
			clickEnemyIconForFetailText.text = Localization.Get("ui.daily_dungeon_info_view.click_enemy_icon_for_detail");
			dailyDungeonlootText.text = Localization.Get("ui.daily_dungeon_info_view.daily_dungeon_loot");
			embattleText.text = Localization.Get("ui.daily_dungeon_info_view.embattle");
			startText.text = Localization.Get("ui.daily_dungeon_info_view.start");
		}

		public void SetActivityInfo (ActivityInfo activityInfo)
		{
			_activityInfo = activityInfo;
			dailyDungeonNameText.text = Localization.Get(_activityInfo.ActivityData.name);
			dailyDungeonIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetActivityIconPath(_activityInfo.ActivityData.pic)));
			ResetLevel(_activityInfo.LastChallengedLevel);
			ResetTimesAndStartButton();

			string openDaysString = UIUtil.GetWeekdayListString(_activityInfo.ActivityData.openDayList);
			closedTipesText.text = string.Format(Localization.Get("ui.daily_dungeon_view.open_date"), openDaysString);
			if (_activityInfo.isOpen)
			{
				bottomButtonsRootGameObject.SetActive(true);
				closedTipesText.gameObject.SetActive(false);
			}
			else
			{
				bottomButtonsRootGameObject.SetActive(false);
				closedTipesText.gameObject.SetActive(true);
			}
		}

		private void ResetLevel (int level)
		{
			int dungeonID = _activityInfo.ActivityData.GetDungeonIDOfLevel(level);
			DungeonData dungeonData = DungeonData.GetDungeonDataByID(dungeonID);

			if (GameProxy.instance.AccountLevel < dungeonData.unlockLevel)
			{
				string accountLevelNotEnoughTipsString = string.Format(Localization.Get("ui.daily_dungeon_info_view.account_level_not_enough"), dungeonData.unlockLevel);
				CommonAutoDestroyTipsView.Open(accountLevelNotEnoughTipsString);
				return;
			}

			_currentSelectLevel = level;
			currentLevelNameText.text = string.Format(Localization.Get("ui.daily_dungeon_info_view.dungeon_difficulty_title"), level + 1);
			ResetEnemies(dungeonData);
			ResetLoot(dungeonData);
			ResetCost();
			RefreshPreviousLevelAndNextLevelButtons();
		}

		private void ResetEnemies (DungeonData dungeonData)
		{
			TransformUtil.ClearChildren(enemiesRootTransform, true);
			List<HeroInfo> heroPresentList = dungeonData.heroPresentList;
			int heroCount = heroPresentList.Count;
			for (int i = 0; i < heroCount; i++)
			{
				CommonHeroIcon.View.CommonHeroIcon commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.Create(enemiesRootTransform);
				commonHeroIcon.SetHeroInfo(heroPresentList[i]);
				RoleDesButton.Get(commonHeroIcon.gameObject).SetRoleInfo(heroPresentList[i],ShowDescriptionType.click);
			}
		}

		private void ResetLoot (DungeonData dungeonData)
		{
			TransformUtil.ClearChildren(lootRootTransform, true);
			List<GameResData> lootList = dungeonData.eachLootPresent;
			int lootCount = lootList.Count;
			for (int i = 0; i < lootCount; i++)
			{
				CommonRewardIcon commonRewardIcon = CommonRewardIcon.Create(lootRootTransform);
				commonRewardIcon.SetGameResData(lootList[i]);
				commonRewardIcon.HideCount();
				commonRewardIcon.SetDesButtonType(ShowDescriptionType.click);
			}
		}

		private void ResetCost ()
		{
			DungeonData dungeonData = _activityInfo.ActivityData.GetDungeonDataOfLevel(_currentSelectLevel);
			costText.text = string.Format(Localization.Get("ui.daily_dungeon_info_view.cost"), dungeonData.actionNeed);
		}

		private void ResetTimesAndStartButton ()
		{
			buyChallengeTimeBtnGO.SetActive(_activityInfo.remainChallengeTimes <= 0);
			remainChallengeTimesText.text = string.Format(Localization.Get("ui.daily_dungeon_info_view.remain_challenge_times"), _activityInfo.remainChallengeTimes, _activityInfo.ActivityData.count);
		}

		private void RefreshPreviousLevelAndNextLevelButtons ()
		{
			previousDungeonButton.gameObject.SetActive(_currentSelectLevel > _activityInfo.ActivityData.MinDungeonIDIndex);
			nextDungeonButton.gameObject.SetActive(_currentSelectLevel < _activityInfo.ActivityData.MaxDungeonIDIndex);
			newLevelUnlockedFXRootGameObject.SetActive(_activityInfo.ChallengedHighestLevel < _activityInfo.UnlockedHighestLevel
			                                           && _currentSelectLevel < _activityInfo.UnlockedHighestLevel);
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickEmbattleHandler ()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
		}

		public void ClickBuyChanllengeBtnHandler()
		{
			if (GameProxy.instance.PveAction < CurrentSelectDungeonData.actionNeed)
			{
				
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.not_enough_pveAction"));
				return;
			}
			if(_activityInfo.remainBuyActivityTimes > 0)
			{
				int index = _activityInfo.buyActivityChanllengeTimes;
				if(_activityInfo.buyActivityChanllengeTimes >= GlobalData.GetGlobalData().daily_dungeon_buy_List.Count)
					index = GlobalData.GetGlobalData().daily_dungeon_buy_List.Count-1;
				GameResData data = GlobalData.GetGlobalData().daily_dungeon_buy_List[index];
				ConfirmCostTipsView.Open(data, Localization.Get("ui.daily_dungeon_info_view.buy_challenge_times_tips_title"), Localization.Get("ui.daily_dungeon_info_view.buy_challenge_times_tips_content"), null, ExpendCostToBuyChallengeHandler);
				return;
			}
			if(_activityInfo.hasBuyActivityTimesByMaxVip)
			{
				ConfirmTipsView.Open(Localization.Get("ui.daily_dungeon_info_view.upgrade_vip_tips"), OpenRechargeHandler);
				return ;
			}
			CommonErrorTipsView.Open(Localization.Get("ui.daily_dungeon_info_view.out_of_challenge_time"));
		}

		public void ExpendCostToBuyChallengeHandler()
		{
			int index = _activityInfo.buyActivityChanllengeTimes;
			if(_activityInfo.buyActivityChanllengeTimes >= GlobalData.GetGlobalData().daily_dungeon_buy_List.Count)
				index = GlobalData.GetGlobalData().daily_dungeon_buy_List.Count-1;
			GameResData data = GlobalData.GetGlobalData().daily_dungeon_buy_List[index];
			int own = GameProxy.instance.BaseResourceDictionary.GetValue(data.type);
			if(own < data.count)
			{
				bool isEnough = GameProxy.instance.GameModelLuaTable.GetLuaFunction("CheckBaseResEnoughByType").Call((int)data.type,data.count)[0].ToString().ToBoolean();
				if (!isEnough) 
				{
					return;
				}
			}
			if (_currentSelectLevel > _activityInfo.ChallengedHighestLevel)
			{
				_activityInfo.ChallengedHighestLevel = _currentSelectLevel;
			}
			_activityInfo.LastChallengedLevel = _currentSelectLevel;
			ActivityController.instance.CLIENT2LOBBY_ACTIVITY_PVE_CHALLENGE_REQ(CurrentSelectDungeonData.dungeonID, true);
		}

		private void OpenRechargeHandler()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
		}

		public void ClickPreviousDungeonButtonHandler ()
		{
			int previousDungeonIDIndex = Mathf.Max(_currentSelectLevel - 1, _activityInfo.ActivityData.MinDungeonIDIndex);
			ResetLevel(previousDungeonIDIndex);
		}

		public void ClickNextDungeonButtonHandler ()
		{
			int nextDungeonIDIndex = Mathf.Min(_currentSelectLevel + 1, _activityInfo.ActivityData.MaxDungeonIDIndex);
			ResetLevel(nextDungeonIDIndex);
		}

		public void ClickNextHandler ()
		{
			DungeonData dungeonData = _activityInfo.ActivityData.GetDungeonDataOfLevel(_currentSelectLevel);
			if (_activityInfo.remainChallengeTimes <= 0)
			{
				ClickBuyChanllengeBtnHandler();
				return;
			}
			if (GameProxy.instance.PveAction < dungeonData.actionNeed)
			{
				string tipString = Localization.Get("ui.common_tips.not_enough_pve_action_and_go_to_buy");
				ConfirmTipsView.Open(tipString, GoToBuyAction);
				return;
			}
			if (_currentSelectLevel > _activityInfo.ChallengedHighestLevel)
			{
				_activityInfo.ChallengedHighestLevel = _currentSelectLevel;
			}
			_activityInfo.LastChallengedLevel = _currentSelectLevel;
			ActivityController.instance.CLIENT2LOBBY_ACTIVITY_PVE_CHALLENGE_REQ(dungeonData.dungeonID, false);
		}

		public void GoToBuyAction ()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
		}
		#endregion UI event handlers
	}
}
