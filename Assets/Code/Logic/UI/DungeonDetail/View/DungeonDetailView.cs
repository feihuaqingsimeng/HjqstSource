using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.DungeonDetail.Model;
using Logic.Dungeon.Model;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Common.Util;
using Common.Localization;
using Logic.Chapter.Model;
using Logic.UI.CommonHeroIcon.View;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.CommonReward.View;
using Common.ResMgr;
using Logic.Dungeon.Controller;
using Logic.Item.Model;
using Logic.UI.Tips.View;
using Logic.VIP.Model;
using Logic.UI.Description.View;
using Logic.FunctionOpen.Model;
using Logic.TimesCost.Model;


namespace Logic.UI.DungeonDetail.View{
	public class DungeonDetailView : MonoBehaviour {
		

		public const string PREFAB_PATH = "ui/dungeon_detail/dungeon_detail_view";

		private Sprite _unlockConditionStatusPassedSprite;
		public Sprite UnlockConditionStatusPassedSprite
		{
			get
			{
				if (_unlockConditionStatusPassedSprite == null)
				{
					_unlockConditionStatusPassedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/ui_checkin_1_sml");
				}
				return _unlockConditionStatusPassedSprite;
			}
		}

		private Sprite _unlockConditionStatusUnpassedSprite;
		public Sprite UnlockConditionStatusUnpassedSprite
		{
			get
			{
				if (_unlockConditionStatusUnpassedSprite == null)
				{
					_unlockConditionStatusUnpassedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_close_3_sml");
				}
				return _unlockConditionStatusUnpassedSprite;
			}
		}

		public static DungeonDetailView Open(DungeonInfo info)
		{
			DungeonDetailView view = UIMgr.instance.Open<DungeonDetailView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetDungeonInfo(info);
			return view;
		}
		public static DungeonDetailView Open(int id)
		{
			DungeonInfo info = DungeonProxy.instance.GetDungeonInfo(id);
			return Open (info);
		}
		#region ui components
		public Text text_dungeon_title;
		public Text text_dungeon_des;
		public Text text_dungeon_difficult;
		public Text text_reward_hero_exp;
		public Text text_reward_account_exp;
		public Text text_sweep_rule;
		public Text text_sweep_remind;
		public Text text_pveAction;
		public Text text_remainChallengeTimes;
		public Text textSweepTen;
		public Transform TranMonsterRoot;
		public Transform TranRewardRoot;
		public Image[] imgSweeps;

		public Button sweepOnceButton;
		public Button sweepTenTimesButton;

		public List<Image> starImages;
		public Text combatCapacityText;

		public Transform challengeButtonAndRemainTimesRoot;
		public Transform unlockConditionsRoot;
		public List<Text> unlockConditionTextList;
		public List<Image> unlockConditionStatusImageList;

		public Button startFightButton;
		public Button startFightBossSpeciesDungeonButton;
		#endregion

		private int _oldAccountExp;
		private int _oldAccountLv;
        private bool isClickStartFight;

		public void SetDungeonInfo(DungeonInfo info){
			DungeonDetailProxy.instance.DungeonInfo = info;
			Debugger.Log("dungeonid:"+info.id);
			init ();
		}

		void Awake(){
			BindDelegate();
		}
		void Start ()
		{
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}
		void OnDestroy()
		{
			UnBindDelegate();
		}
		private void BindDelegate()
		{
			DungeonDetailProxy.instance.onUpdateMopUpSuccessDelegate += UpdateSweepSuccessByProtocol;
			ItemProxy.instance.onItemInfoListUpdateDelegate += UpdateItemByProtocol;
			DungeonDetailProxy.instance.onBuySweepCouponsSuccessDelegate += OnBuySweepCouponsSuccessHandler;
			GameProxy.instance.onAccountInfoUpdateDelegate += OnAccountInfoUpdateHandler;
			DungeonProxy.instance.onDungeonInfosUpdateDelegate += OnDungeonInfosUpdateHandler;
		}
		private void UnBindDelegate()
		{
			DungeonDetailProxy.instance.onUpdateMopUpSuccessDelegate -= UpdateSweepSuccessByProtocol;
			ItemProxy.instance.onItemInfoListUpdateDelegate -= UpdateItemByProtocol;
			DungeonDetailProxy.instance.onBuySweepCouponsSuccessDelegate -= OnBuySweepCouponsSuccessHandler;
			GameProxy.instance.onAccountInfoUpdateDelegate -= OnAccountInfoUpdateHandler;
			DungeonProxy.instance.onDungeonInfosUpdateDelegate -= OnDungeonInfosUpdateHandler;
		}
		private void init(){
			InitText();
			//Prefab_HeroButton.SetActive(false);
			for(int i = 0;i<imgSweeps.Length;i++)
			{
				ItemDesButton.Get(imgSweeps[i].gameObject).SetItemInfo(GlobalData.GetGlobalData().sweepTicket.id);
			}
			Refresh();
		}
		private void InitText(){
			text_dungeon_title.text = GetDungeonTitleStr(); 
			//text_dungeon_des = 
			text_dungeon_difficult.text = GetDifficultStr();
			//text_reward_exp;
			//text_next_vip_need;
			//text_sweep_gain;
			//text_next_sweep_gain;
			combatCapacityText.text = DungeonDetailProxy.instance.DungeonInfo.dungeonData.combat.ToString();
			text_sweep_rule.text = Localization.Get("ui.dungeon_detail_view.text_sweep_rule_info");
			text_sweep_remind.text = ItemProxy.instance.GetItemCountByItemID(GlobalData.GetGlobalData().sweepTicket.id).ToString();
			text_pveAction.text = DungeonDetailProxy.instance.DungeonInfo.dungeonData.actionNeed.ToString();
			int count = VIPProxy.instance.VIPData.dailyWelfareSweep.count;
			VIPData nextVip =  VIPProxy.instance.VIPData.GetNextLevelVIPData();
			if(nextVip != null)
			{
				count = nextVip.dailyWelfareSweep.count;
			}

			bool canSweep = DungeonDetailProxy.instance.DungeonInfo.dungeonData.dungeonType != DungeonType.BossSubspecies;
			sweepOnceButton.gameObject.SetActive(canSweep);
			sweepTenTimesButton.gameObject.SetActive(canSweep);

			RefreshChallengeTimes();
		}


		#region refresh ui
		public void Refresh(){
			RefreshDungeon();
			RefreshStars();
			RefreshUISweep();
			RefreshChallengeTimes();
			RefreshChallengeButtonAndUnlockConditions();
		}
		public void RefreshChallengeTimes()
		{
			DungeonInfo info = DungeonDetailProxy.instance.DungeonInfo;
			if (info.dungeonData.dayChallengeTimes > 0)
			{
				text_remainChallengeTimes.text = string.Format(Localization.Get("ui.dungeon_detail_view.text_remain_challenge_times"), info.dungeonData.dayChallengeTimes -info.todayChallengedTimes, info.dungeonData.dayChallengeTimes);
				text_remainChallengeTimes.gameObject.SetActive(true);
			}
			else
			{
				text_remainChallengeTimes.gameObject.SetActive(false);
			}
		}
		public void RefreshDungeon(){

			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			text_dungeon_des.text =  Localization.Get( dungeonInfo.dungeonData.description);
			text_dungeon_title.text = GetDungeonTitleStr();

			if (dungeonInfo.dungeonData.dungeonType == DungeonType.BossSubspecies)
			{
				text_dungeon_difficult.gameObject.SetActive(false);
			}
			else
			{
				text_dungeon_difficult.text = GetDifficultStr();
				text_dungeon_difficult.gameObject.SetActive(true);
			}
			RefreshUIMonsters();
			RefreshUIDropReward();
		}

		public void RefreshUIMonsters(){
			List<HeroInfo> monsters = DungeonDetailProxy.instance.GetMonstersList();

			HeroInfo info;
			TransformUtil.ClearChildren(TranMonsterRoot,true);
			for(int i = 0,count = monsters.Count;i<count;i++){
				info = monsters[i];
				CommonHeroIcon.View.CommonHeroIcon heroIcon =CommonHeroIcon.View.CommonHeroIcon.Create(TranMonsterRoot);
				RoleDesButton.Get(heroIcon.gameObject).SetRoleInfo(info,ShowDescriptionType.click);
				heroIcon.SetHeroInfo(info);
				heroIcon.HideLevel();
			}
		}

		public void RefreshUIDropReward(){
			List<GameResData> rewards = DungeonDetailProxy.instance.GetDropRewardList();

			GameResData resData;
			TransformUtil.ClearChildren(TranRewardRoot,true);
			int accountExp = 0;
			int heroExp = 0;
			for(int i = 0,count = rewards.Count;i<count;i++){
				resData = rewards[i];
				BaseResType type = resData.type;
				if(type == BaseResType.Hero||type == BaseResType.Equipment||type == BaseResType.Item){
					CommonRewardIcon icon = CommonRewardIcon.Create(TranRewardRoot);
					icon.SetGameResData(resData);
					icon.SetDesButtonType(ShowDescriptionType.click);
				}
				else if (type == BaseResType.Account_Exp)
				{
					accountExp += resData.count;
				}
				else if(type == BaseResType.Hero_Exp)
				{
					heroExp += resData.count;
				}

			}
			text_reward_hero_exp.text = heroExp.ToString();
			text_reward_account_exp.text = accountExp.ToString();
		}

		public void RefreshStars ()
		{
			for (int i = 0, count = starImages.Count; i < count; i++)
			{
				if (i < DungeonDetailProxy.instance.DungeonInfo.star)
				{
					starImages[i].SetSprite(ResMgr.instance.LoadSprite("sprite/main_ui/icon_star2_big"));
				}
				else
				{
					starImages[i].SetSprite(ResMgr.instance.LoadSprite("sprite/main_ui/icon_star2_big_disable"));
				}
			}
		}

		public void RefreshUISweep(){

			int count = DungeonDetailProxy.instance.GetSweepTimes();
			count = count > 1 ? count : 10;
			textSweepTen.text = Localization.Get("ui.dungeon_sweep_view.sweep_"+ 10);
		}

		public string GetDifficultyNameByDungeonType (DungeonType dungeonType)
		{
			string difficultyName = "";
			if (dungeonType == DungeonType.Easy)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.easy_type");
			}
			else if (dungeonType == DungeonType.Normal)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.normal_type");
			}
			else if (dungeonType == DungeonType.Hard)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.hard_type");
			}
			return difficultyName;
		}

		public void RefreshChallengeButtonAndUnlockConditions ()
		{ 
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			if (dungeonInfo.isLock)
			{
				int accountLevel = Game.Model.GameProxy.instance.AccountLevel;
				string levelString = dungeonInfo.dungeonData.unlockLevel.ToString();
				DungeonInfo preDungeonInfo = DungeonProxy.instance.GetDungeonInfo(dungeonInfo.dungeonData.unlockDungeonIDPre1);
				string preDungeonName = GetDifficultyNameByDungeonType(preDungeonInfo.dungeonData.dungeonType) + preDungeonInfo.dungeonData.GetOrderName();
				string starCountString = "";
				starCountString = GetDifficultyNameByDungeonType(dungeonInfo.dungeonData.unlockStarDungeonType) + dungeonInfo.dungeonData.unlockStarCount.ToString();
				int ownStarCount = DungeonProxy.instance.GetTotalStarCountOfDungeonType(dungeonInfo.dungeonData.unlockStarDungeonType);

				int unlockConditionIndex = 0;
				List<string> unlockConditionStringList = new List<string>();
				string unlockConditionTemplateString = Localization.Get("ui.dungeon_detail_view.dungeon_unlock_condition_template");
				if (dungeonInfo.dungeonData.unlockLevel > 0)
				{
					unlockConditionStringList.Add(string.Format(unlockConditionTemplateString, unlockConditionIndex + 1, string.Format(Localization.Get("ui.dungeon_detail_view.dungeon_unlock_condition.account_level"), levelString)));
					unlockConditionStatusImageList[unlockConditionIndex].SetSprite(accountLevel >= dungeonInfo.dungeonData.unlockLevel ? UnlockConditionStatusPassedSprite : UnlockConditionStatusUnpassedSprite);
					unlockConditionIndex++;
				}
				if (dungeonInfo.dungeonData.unlockDungeonIDPre1 > 0)
				{
					unlockConditionStringList.Add(string.Format(unlockConditionTemplateString, unlockConditionIndex + 1, string.Format(Localization.Get("ui.dungeon_detail_view.dungeon_unlock_condition.pre_dungeon"), preDungeonName)));
					unlockConditionStatusImageList[unlockConditionIndex].SetSprite(preDungeonInfo.star > 0 ? UnlockConditionStatusPassedSprite : UnlockConditionStatusUnpassedSprite);
					unlockConditionIndex++;
				}
				if (dungeonInfo.dungeonData.unlockDungeonIDPre2 > 0)
				{
					DungeonInfo pre2DungeonInfo = DungeonProxy.instance.GetDungeonInfo(dungeonInfo.dungeonData.unlockDungeonIDPre2);
					string pre2DungeonName = GetDifficultyNameByDungeonType(pre2DungeonInfo.dungeonData.dungeonType)+ pre2DungeonInfo.dungeonData.GetOrderName();

					unlockConditionStringList.Add(string.Format(unlockConditionTemplateString, unlockConditionIndex + 1, string.Format(Localization.Get("ui.dungeon_detail_view.dungeon_unlock_condition.pre_dungeon"), pre2DungeonName)));
					unlockConditionStatusImageList[unlockConditionIndex].SetSprite(pre2DungeonInfo.star > 0 ? UnlockConditionStatusPassedSprite : UnlockConditionStatusUnpassedSprite);
					unlockConditionIndex++;
				}
				if (dungeonInfo.dungeonData.unlockStarCount > 0)
				{
					unlockConditionStringList.Add(string.Format(unlockConditionTemplateString, unlockConditionIndex + 1, string.Format(Localization.Get("ui.dungeon_detail_view.dungeon_unlock_condition.star_count"), starCountString)));
					unlockConditionStatusImageList[unlockConditionIndex].SetSprite(ownStarCount >= dungeonInfo.dungeonData.unlockStarCount ? UnlockConditionStatusPassedSprite : UnlockConditionStatusUnpassedSprite);
					unlockConditionIndex++;
				}

				for (int i = 0, count = unlockConditionTextList.Count; i < count; i++)
				{
					unlockConditionTextList[i].gameObject.SetActive(false);
				}

				for (int i = 0, count = unlockConditionStringList.Count; i < count; i++)
				{
					unlockConditionTextList[i].text = unlockConditionStringList[i];
					unlockConditionTextList[i].gameObject.SetActive(true);
				}

				unlockConditionsRoot.gameObject.SetActive(true);
				challengeButtonAndRemainTimesRoot.gameObject.SetActive(false);
			}
			else
			{
				if (dungeonInfo.dungeonData.dungeonType == DungeonType.BossSubspecies)
				{
					startFightButton.gameObject.SetActive(false);
					startFightBossSpeciesDungeonButton.gameObject.SetActive(true);
				}
				else
				{
					startFightButton.gameObject.SetActive(true);
					startFightBossSpeciesDungeonButton.gameObject.SetActive(false);
				}

				unlockConditionsRoot.gameObject.SetActive(false);
				challengeButtonAndRemainTimesRoot.gameObject.SetActive(true);
			}
		}
		#endregion

		private string GetDungeonTitleStr(){
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;

			string s = string.Format(Localization.Get("ui.dungeon_detail_view.dungeon_title"),Localization.Get( dungeonInfo.dungeonData.order_name),Localization.Get(dungeonInfo.dungeonData.name));
			return s;

		}
		private string GetDifficultStr(){
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			return Localization.Get(string.Concat("ui.dungeon_difficult_", (int)dungeonInfo.dungeonData.dungeonType));

		}

		#region ui event handle
		public void OnClickSweepOneHandle()
		{
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			if (dungeonInfo.dungeonData.dungeonType == DungeonType.BossSubspecies)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.dungeon_not_support_sweep_tips"));
				return;
			}

			if(DungeonDetailProxy.instance.CheckSweep(false))
			{
				_oldAccountLv = GameProxy.instance.AccountLevel;
				_oldAccountExp = GameProxy.instance.AccountExp;
				DungeonController.instance.CLIENT2LOBBY_PVE_MOP_UP_REQ(DungeonDetailProxy.instance.DungeonInfo.id);
			}
		}
		public void OnClickSweepTenHandle(){
//			if(!Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.Dungeon_Sweep,true))
//			{
//				return;
//			}

			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			if (dungeonInfo.dungeonData.dungeonType == DungeonType.BossSubspecies)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.dungeon_not_support_sweep_tips"));
				return;
			}

			if(DungeonDetailProxy.instance.CheckSweep(true))
			{
				_oldAccountLv = GameProxy.instance.AccountLevel;
				_oldAccountExp = GameProxy.instance.AccountExp;

				DungeonController.instance.CLIENT2LOBBY_PveTenMopUp_REQ(DungeonDetailProxy.instance.DungeonInfo.id,DungeonDetailProxy.instance.GetSweepTimes());
			}
		}
		public void OnClickPveEmbattleHandler()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
		}

		public void OnClickBuyChallengeTimesHandler ()
		{
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			if (dungeonInfo.dayRefreshTimes < VIPProxy.instance.VIPData.dayRefreshTimes)
			{
				int cost = TimesCostData.GetTimesCostData(2, dungeonInfo.dayRefreshTimes + 1).cost;
				if (GameProxy.instance.BaseResourceDictionary[BaseResType.Diamond] < cost)
				{
					ConfirmTipsView.Open(Localization.Get("ui.dungeon_detail_view.diamond_not_enough_tips_title"), OnClickGoToBuyDiamond, ConsumeTipType.None);
					return;
				}
				DungeonController.instance.CLIENT2LOBBY_RefreshDayTimesReq(dungeonInfo.dungeonData.dungeonID);
			}
			else
			{
				VIPData improveDayRefreshTimesVIPData = null;
				VIPData tempVIPData = VIPProxy.instance.VIPData;
				while (tempVIPData != null)
				{
					if (tempVIPData.dayRefreshTimes > VIPProxy.instance.VIPData.dayRefreshTimes)
					{
						improveDayRefreshTimesVIPData = tempVIPData;
						break;
					}
					tempVIPData = tempVIPData.GetNextLevelVIPData();
				}

				if (improveDayRefreshTimesVIPData != null)
				{
					string tipString = Localization.Format("ui.dungeon_detail_view.dungeon_challenge_times_not_enough_and_go_to_recharge_tips", improveDayRefreshTimesVIPData.ShortName);
					ConfirmTipsView.Open(tipString, OnClickGoToBuyDiamond, ConsumeTipType.None);
				}
				else
				{
					CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.dungeon_challenge_times_not_enough_tips"));
				}
			}
		}

		public void OnClickGoToBuyDiamond ()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
		}

		public void OnClickFightPrepareHandle(){
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			if (dungeonInfo.dungeonData.dayChallengeTimes > 0 && dungeonInfo.todayChallengedTimes >= dungeonInfo.dungeonData.dayChallengeTimes)
			{
				string tipsTitle = Localization.Get("ui.dungeon_detail_view.reset_dungeon_challenge_times_tips_title");
				string tipsContent = string.Format(Localization.Get("ui.dungeon_detail_view.reset_dungeon_challenge_times_tips_content"), VIPProxy.instance.VIPData.dayRefreshTimes - dungeonInfo.dayRefreshTimes, VIPProxy.instance.VIPData.dayRefreshTimes);
				int cost = TimesCostData.GetTimesCostData(2, dungeonInfo.dayRefreshTimes + 1).cost;
				ConfirmBuyShopItemTipsView.Open(tipsTitle, tipsContent, new GameResData(BaseResType.Diamond, 0, cost, 0), OnClickBuyChallengeTimesHandler, ConsumeTipType.None);
				return;
			}

            if (isClickStartFight) 
                return;
            StartCoroutine(EnableClickFightCoroutine());

			DungeonDetailProxy.instance.StartFight();
		}

		public void OnClickStartFightBossSpeciesDungeonHandler ()
		{
			DungeonInfo dungeonInfo = DungeonDetailProxy.instance.DungeonInfo;
			GameResData costItemGameResData = dungeonInfo.dungeonData.itemCostGameResData;
			int costItemCount = ItemProxy.instance.GetItemCountByItemID(costItemGameResData.id);

			if (costItemCount < costItemGameResData.count)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.cost_item_not_enough_tips"));
				return;
			}

			if (isClickStartFight) 
				return;
			StartCoroutine(EnableClickFightCoroutine());
			DungeonDetailProxy.instance.StartFight();
		}

        private IEnumerator EnableClickFightCoroutine() 
        {
            isClickStartFight = true;
            yield return new WaitForSeconds(2);
            isClickStartFight = false;
        }
		public void OnClickCloseHandle(){
			
			UI.UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
		#region update by protocol
		private void UpdateSweepSuccessByProtocol(int dungeonId,Dictionary<int, List<GameResData>> rewardDic)
		{
			text_sweep_remind.text = ItemProxy.instance.GetItemCountByItemID(GlobalData.GetGlobalData().sweepTicket.id).ToString();
			DungeonSweepView.Open(rewardDic,_oldAccountLv,_oldAccountExp);
			RefreshUISweep();
		}
		private void UpdateItemByProtocol()
		{
			text_sweep_remind.text = ItemProxy.instance.GetItemCountByItemID(GlobalData.GetGlobalData().sweepTicket.id).ToString();
		}
		private void OnBuySweepCouponsSuccessHandler ()
		{
			if (DungeonDetailProxy.instance.lastCheckSweepType == SweepType.Single)
				OnClickSweepOneHandle();
			else if (DungeonDetailProxy.instance.lastCheckSweepType == SweepType.Ten)
				OnClickSweepTenHandle();
		}

		void OnAccountInfoUpdateHandler ()
		{
			RefreshChallengeButtonAndUnlockConditions();
		}

		void OnDungeonInfosUpdateHandler ()
		{
			Refresh();
		}
		#endregion
	}
}

