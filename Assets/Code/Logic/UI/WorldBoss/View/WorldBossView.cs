using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Logic.Protocol.Model;
using Logic.WorldBoss.Model;
using Logic.WorldBoss.Controller;
using Logic.Hero.Model;
using Logic.Character;
using Logic.Game.Model;
using Common.GameTime.Controller;
using Logic.ConsumeTip.Model;
using Logic.Enums;

namespace Logic.UI.WorldBoss.View
{
    public class WorldBossView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/world_boss/world_boss_view";

        public enum WorldBossViewState
        {
            WorldBossDidNotArrive = 0,
            CanChallenge = 1,
            PlayerDeadState = 2,
            WorldBossActivityFinished = 3,
        }

        private WorldBossViewState _currentWorldBossViewState;
        public bool manualSwitchState = false;
        public WorldBossViewState worldBossViewState;
        private CharacterEntity _characterEntity;

		private int _cachedBossHP = 0;

        #region UI components
        public GameObject core;

        public GameObject damageRankingFrame;
        public GameObject functionButtonsFrame;
        public GameObject worldBossStatusFrame;
        public GameObject worldBossInfoRoot;

        public Button inspireButton;
        public Button challengeButton;
        public Button reviveButton;

        public Transform worldBossInfoMiddleAnchor;
        public Transform worldBossInfoRightAnchor;

        public Text worldBossLevelText;
        public Text worldBossNameText;

        public Transform bossModelRoot;
        public GameObject bossRemainTimeRoot;
        public Text bossHPText;
        public Slider bossHpSlider;

        public Text damageRankingTitleText;
        public Transform damageRankingItemRoot;
        public WorldBossDamageRankingItem worldBossDamageRankingItemPrefab;
        public Text myAttackTitleText;
        public Text myAttackText;
		public Text myHurtPercentText;
        public Text myAttackRankingTitleText;
        public Text myAttackRankingText;
        public Text formationText;
        public Text trainText;
        public Text equipmentsText;
        public Text rewardText;
        public Text remainTimeTitleText;
        public Text remainTimeText;
        public Text bossDetailText;
        public Text inspireEffectText;
        public Text inspireCostText;
        public Text inspireText;
        public Text challengeText;
        public Text reviveCountDownTimeText;
        public Text reviveCostText;
        public Text reviveText;
        public Text worldBossStatusText;
        #endregion UI components

        void Awake()
        {
            CommonTopBar.View.CommonTopBarView commonTopBarView = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(core.transform);
            commonTopBarView.SetAsCommonStyle(Localization.Get("ui.world_boss_view.title"), ClickCloseHandler, true, true, true, false);

            damageRankingTitleText.text = Localization.Get("ui.world_boss_view.damage_ranking");
            myAttackTitleText.text = Localization.Get("ui.world_boss_view.my_attack");
            myAttackRankingTitleText.text = Localization.Get("ui.world_boss_view.my_attack_ranking");
            formationText.text = Localization.Get("ui.world_boss_view.formation");
            trainText.text = Localization.Get("ui.world_boss_view.train");
            equipmentsText.text = Localization.Get("ui.world_boss_view.equipments");
            rewardText.text = Localization.Get("ui.world_boss_view.reward");
            remainTimeTitleText.text = Localization.Get("ui.world_boss_view.remain_time");
            bossDetailText.text = Localization.Get("ui.world_boss_view.boss_detail");
            inspireCostText.text = GlobalData.GetGlobalData().inspireCost.count.ToString();
            inspireText.text = Localization.Get("ui.world_boss_view.encourage");
            challengeText.text = Localization.Get("ui.world_boss_view.challenge");
            reviveCostText.text = GlobalData.GetGlobalData().bossFightRebornCost.count.ToString();
            reviveText.text = Localization.Get("ui.world_boss_view.revive");

            worldBossDamageRankingItemPrefab.gameObject.SetActive(false);
            CheckAndResetViewState();
        }

        void Start()
        {
            WorldBossProxy.instance.onWorldBossStatusChangedDelegate += OnWorldBossStatusChangedHandler;
            WorldBossProxy.instance.onWorldBossInfoUpdateDelegate += OnWorldBossInfoUpdateHandler;
            WorldBossProxy.instance.onWorldBossInspireTimesUpdateDelegate += OnWorldBossInspireTimesUpdateHandler;
            WorldBossProxy.instance.onWorldBossOpenCountDownTimeUpdateDelegate += OnWorldBossOpenCountDownTimeUpdateHandler;
            WorldBossProxy.instance.onWorldBossOverCountDownTimeUpdateDelegate += OnWorldBossOverCountDownTimeUpdateHandler;
//            WorldBossController.instance.CLIENT2LOBBY_WorldBossReq();
			RequestWorldBossInfo();
			PlayHurtEffect();
        }

        void OnDestroy()
        {
			LeanTween.cancel(gameObject);
            DespawnCharacter();
            WorldBossProxy.instance.onWorldBossStatusChangedDelegate -= OnWorldBossStatusChangedHandler;
            WorldBossProxy.instance.onWorldBossInfoUpdateDelegate -= OnWorldBossInfoUpdateHandler;
            WorldBossProxy.instance.onWorldBossInspireTimesUpdateDelegate -= OnWorldBossInspireTimesUpdateHandler;
            WorldBossProxy.instance.onWorldBossOpenCountDownTimeUpdateDelegate -= OnWorldBossOpenCountDownTimeUpdateHandler;
            WorldBossProxy.instance.onWorldBossOverCountDownTimeUpdateDelegate -= OnWorldBossOverCountDownTimeUpdateHandler;
        }

        private int _cachedReviveCountDownTime = 0;
        void Update()
        {
#if UNITY_EDITOR
            if (manualSwitchState)
            {
                if (_currentWorldBossViewState != worldBossViewState)
                {
                    SetAs(worldBossViewState);
                }
            }
#endif
            int reviveCountDownTime = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.ReviveCoolingEndTime);
            if (reviveCountDownTime >= 0)
            {
                reviveCountDownTimeText.text = string.Format(Localization.Get("ui.world_boss_view.revive_count"), Common.Util.TimeUtil.FormatSecondToMinute(reviveCountDownTime));
            }
            if (_cachedReviveCountDownTime > 0 && reviveCountDownTime <= 0)
            {
                WorldBossController.instance.CLIENT2LOBBY_WorldBossReq();
            }
            _cachedReviveCountDownTime = reviveCountDownTime;
        }

        private void SetAsBossDidNotArriveState()
        {
            damageRankingFrame.gameObject.SetActive(false);
            functionButtonsFrame.gameObject.SetActive(false);
            worldBossStatusFrame.gameObject.SetActive(true);
            worldBossInfoRoot.gameObject.SetActive(true);
            worldBossInfoRoot.transform.position = worldBossInfoMiddleAnchor.position;
            inspireButton.gameObject.SetActive(false);
            challengeButton.gameObject.SetActive(false);
            reviveButton.gameObject.SetActive(false);
            bossRemainTimeRoot.SetActive(false);

			remainTimeTitleText.gameObject.SetActive(false);
			remainTimeText.gameObject.SetActive(false);
        }

        private void SetAsCanChallengeState()
        {
            damageRankingFrame.gameObject.SetActive(true);
            functionButtonsFrame.gameObject.SetActive(true);
            worldBossStatusFrame.gameObject.SetActive(false);
            worldBossInfoRoot.gameObject.SetActive(true);
            worldBossInfoRoot.transform.position = worldBossInfoRightAnchor.position;
            inspireButton.gameObject.SetActive(true);
            challengeButton.gameObject.SetActive(true);
            reviveButton.gameObject.SetActive(false);
            bossRemainTimeRoot.SetActive(true);

			remainTimeTitleText.gameObject.SetActive(true);
			remainTimeText.gameObject.SetActive(true);
        }

        private void SetAsPlayerDeadState()
        {
            damageRankingFrame.gameObject.SetActive(true);
            functionButtonsFrame.gameObject.SetActive(true);
            worldBossStatusFrame.gameObject.SetActive(false);
            worldBossInfoRoot.gameObject.SetActive(true);
            worldBossInfoRoot.transform.position = worldBossInfoRightAnchor.position;
            inspireButton.gameObject.SetActive(true);
            challengeButton.gameObject.SetActive(false);
            reviveButton.gameObject.SetActive(true);
            bossRemainTimeRoot.SetActive(true);

			remainTimeTitleText.gameObject.SetActive(true);
			remainTimeText.gameObject.SetActive(true);
        }

        private void SetAsWorldBossActivityFinishedState()
        {
            damageRankingFrame.gameObject.SetActive(true);
            functionButtonsFrame.gameObject.SetActive(false);
            worldBossStatusFrame.gameObject.SetActive(true);
            worldBossInfoRoot.gameObject.SetActive(true);
            worldBossInfoRoot.transform.position = worldBossInfoRightAnchor.position;
            inspireButton.gameObject.SetActive(false);
            challengeButton.gameObject.SetActive(false);
            reviveButton.gameObject.SetActive(false);
            bossRemainTimeRoot.SetActive(true);

			remainTimeTitleText.gameObject.SetActive(false);
			remainTimeText.gameObject.SetActive(false);
        }

        private void SetAs(WorldBossViewState worldBossViewState)
        {
            switch (worldBossViewState)
            {
                case WorldBossViewState.WorldBossDidNotArrive:
                    SetAsBossDidNotArriveState();
                    break;
                case WorldBossViewState.CanChallenge:
                    SetAsCanChallengeState();
                    break;
                case WorldBossViewState.PlayerDeadState:
                    SetAsPlayerDeadState();
                    break;
                case WorldBossViewState.WorldBossActivityFinished:
                    SetAsWorldBossActivityFinishedState();
                    break;
                default:
                    break;
            }
            _currentWorldBossViewState = worldBossViewState;
        }

        private void RefreshDamageRankingInfo()
        {
            TransformUtil.ClearChildren(damageRankingItemRoot, true);
            List<WorldBossHurtRankProto> worldBossHurtRankProtoList = WorldBossProxy.instance.WorldBossHurtRankProtoList;
            int worldBossHurtRankProtoCount = worldBossHurtRankProtoList.Count;
            for (int i = 0; i < worldBossHurtRankProtoCount; i++)
            {
                WorldBossDamageRankingItem worldBossDamageRankingItem = GameObject.Instantiate<WorldBossDamageRankingItem>(worldBossDamageRankingItemPrefab);
                worldBossDamageRankingItem.SetInfo(worldBossHurtRankProtoList[i]);
                worldBossDamageRankingItem.transform.SetParent(damageRankingItemRoot, false);
                worldBossDamageRankingItem.gameObject.SetActive(true);
            }
            myAttackText.text = WorldBossProxy.instance.TotalHurt.ToString();
			string totalHurtPercentValueString = string.Format("{0:F2}", (float)(WorldBossProxy.instance.HurtPercent) / 100);
			myHurtPercentText.text = string.Format(Localization.Get("ui.world.boss_view.total_hurt_percent"), totalHurtPercentValueString);
            myAttackRankingText.text = WorldBossProxy.instance.HurtRankNo.ToString();
        }

        private void RefreshInspireEffect()
        {
            inspireEffectText.text = string.Format(Localization.Get("ui.world_boss_view.inspire_effect"), WorldBossProxy.instance.InspireTimes * GlobalData.GetGlobalData().inspireEffect);
        }

        private void CheckAndResetViewState()
        {
            if (WorldBossProxy.instance.IsOpen)
            {
                float reviveDiffTimeWithServer = TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.ReviveCoolingEndTime);
                if (reviveDiffTimeWithServer > 0)
                {
                    SetAs(WorldBossViewState.PlayerDeadState);
                }
                else
                {
                    SetAs(WorldBossViewState.CanChallenge);
                }
            }
            else
            {
				long openTimeDiffWithServerTime = TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OpenTime);
				long overTimeDiffWithServerTime = TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
				if (openTimeDiffWithServerTime >= 0 && openTimeDiffWithServerTime <= GlobalData.GetGlobalData().bossInBefore)
				{
					SetAs(WorldBossViewState.WorldBossDidNotArrive);
				}
				else if (overTimeDiffWithServerTime <= 0 && overTimeDiffWithServerTime >= -GlobalData.GetGlobalData().bossInLast)
				{
					SetAs(WorldBossViewState.WorldBossActivityFinished);
				}
				else
				{
					SetAs(WorldBossViewState.WorldBossDidNotArrive);
				}
            }
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }
		
		public void RequestWorldBossInfo ()
		{
			WorldBossController.instance.CLIENT2LOBBY_WorldBossReq();
			if (WorldBossProxy.instance.IsOpen)
			{
				LeanTween.delayedCall(gameObject, 10, RequestWorldBossInfo);
			}
		}

		private void OnUpdateBossHPBarColor (Color color)
		{
			Image fillImage = bossHpSlider.fillRect.GetComponent<Image>();
			fillImage.CrossFadeColor(color, 0, true, false);
		}

		private void PlayHurtEffect ()
		{
			if (WorldBossProxy.instance.IsOpen)
			{
				LTDescr ltDescr = LeanTween.value(gameObject, Color.white, Color.red, 0.1f);
				ltDescr.setOnUpdate(OnUpdateBossHPBarColor);
				ltDescr.setRepeat(4);
				ltDescr.setLoopPingPong();
			}
			LeanTween.delayedCall(gameObject, 1, PlayHurtEffect);
		}

        #region proxy callback
        private void OnWorldBossStatusChangedHandler()
        {
            CheckAndResetViewState();
        }

		private bool _isBossModelLoaded = false;
        private void OnWorldBossInfoUpdateHandler()
        {
            HeroData bossData = HeroData.GetHeroDataByID(WorldBossProxy.instance.BossID);

			if (!_isBossModelLoaded)
			{
				DespawnCharacter();
				TransformUtil.ClearChildren(bossModelRoot, true);
				_characterEntity = CharacterEntity.CreateHeroEntityAsUIElement(bossData.modelNames[bossData.starMax - 1], bossModelRoot, false, true);
				_isBossModelLoaded = true;
			}
            worldBossLevelText.text = string.Format(Localization.Get("ui.world_boss_view.boss_level"), WorldBossProxy.instance.BossLevel.ToString());
            worldBossNameText.text = Localization.Get(bossData.name);
            bossHPText.text = WorldBossProxy.instance.BossCurrHP + "/" + WorldBossProxy.instance.BossHPUpperLimit;
            bossHpSlider.value = WorldBossProxy.instance.BossCurrHP * 1.0f / WorldBossProxy.instance.BossHPUpperLimit;

            RefreshDamageRankingInfo();
            RefreshInspireEffect();
            CheckAndResetViewState();
        }

        void OnWorldBossInspireTimesUpdateHandler()
        {
            RefreshInspireEffect();
        }

        private void OnWorldBossOpenCountDownTimeUpdateHandler(int second)
        {
            string countDownTimeString = TimeUtil.FormatSecondToHour(second);
            worldBossStatusText.text = string.Format(Localization.Get("ui.world_boss_view.until_boss_arrive_tips"), countDownTimeString);
        }

        private void OnWorldBossOverCountDownTimeUpdateHandler(int second)
        {
			string countDownTimeString;
			if (WorldBossProxy.instance.IsOpen)
			{
				countDownTimeString = TimeUtil.FormatSecondToHour(second);
			}
			else
			{
				countDownTimeString = TimeUtil.FormatSecondToHour(WorldBossProxy.instance.OpenDiffTimeWithServerTimeInSecond);
			}
			worldBossStatusText.text = string.Format(Localization.Get("ui.world_boss_view.until_boss_arrive_tips"), countDownTimeString);
			remainTimeText.text = countDownTimeString;
        }
        #endregion proxy callback

        #region UI event handler
        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickFormationHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.PveEmbattleView);
        }

        public void ClickTrainHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.RoleInfoView);
        }

        public void ClickEquipmentsHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.Equipment_View);
        }

        public void ClickRewardHandler()
        {
            UIMgr.instance.Open(WorldBossRewardView.PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
        }

        public void ClickBossDetailHandler()
        {
            UIMgr.instance.Open(WorldBossDetailView.PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
        }

		private void GoToBuyDiamond ()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
		}

        public void ClickInspireHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.WorldBoss_inspireBtn, false))
			{
				FunctionOpen.Model.FunctionData worldBossInspireFunctionOpenData = FunctionOpen.Model.FunctionData.GetFunctionDataByID((int)FunctionOpenType.WorldBoss_inspireBtn);
				Logic.VIP.Model.VIPData vipData = Logic.VIP.Model.VIPData.GetVIPData(worldBossInspireFunctionOpenData.vip);
				string tipString = Localization.Format("ui.world_boss_view.tips.inspire_locked_tips", vipData.ShortName);
				Logic.UI.Tips.View.ConfirmTipsView.Open(tipString, GoToBuyDiamond);
                return;
			}
            if (WorldBossProxy.instance.InspireTimes >= GlobalData.GetGlobalData().inspireMax)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.world_boss_view.tips.reach_inspire_text"));
                return;
            }
            if (GameProxy.instance.BaseResourceDictionary[Logic.Enums.BaseResType.Diamond] < GlobalData.GetGlobalData().inspireCost.count)
            {
                Logic.UI.Tips.View.ConfirmTipsView.Open(Localization.Get("ui.world_boss_view.tips.diamond_not_enough"), ClickGoShopBuyDiamondButtonHandler);
                return;
            }
            //是否弹提示框
            if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondBuyWorldBossInspire))
            {
                Logic.UI.Tips.View.ConfirmTipsView.Open(string.Format(Localization.Get("ui.world_boss_view.tips.confirm_inspire_tips"), GlobalData.GetGlobalData().inspireCost.count), ClickConfirmInspireHandler, ConsumeTipType.DiamondBuyWorldBossInspire);

            }
            else
            {
                ClickConfirmInspireHandler();
            }
        }

        public void ClickGoShopBuyDiamondButtonHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.Shop_Diamond);
        }

        public void ClickConfirmInspireHandler()
        {
            WorldBossController.instance.CLIENT2LOBBY_WorldBossInspireReq();
        }

        public void ClickChallengeHandler()
        {
            WorldBossController.instance.CLIENT2LOBBY_WorldBossChallengeReq();
        }

        public void ClickReviveButtonHandler()
        {
            if (GameProxy.instance.BaseResourceDictionary[Logic.Enums.BaseResType.Diamond] < GlobalData.GetGlobalData().bossFightRebornCost.count)
            {
                Logic.UI.Tips.View.ConfirmTipsView.Open(Localization.Get("ui.world_boss_view.tips.diamond_not_enough"), ClickGoShopBuyDiamondButtonHandler);
                return;
            }
			if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.WorldBossRevive))
			{
				Logic.UI.Tips.View.ConfirmTipsView.Open(string.Format(Localization.Get("ui.world_boss_view.tips.confirm_revive_tips"), GlobalData.GetGlobalData().bossFightRebornCost.count), ClickConfirmReviveButtonHandler, ConsumeTipType.WorldBossRevive);
			}
			else
			{
				ClickConfirmReviveButtonHandler();
			}

        }

        public void ClickConfirmReviveButtonHandler()
        {
            WorldBossController.instance.CLIENT2LOBBY_WorldBossReviveReq();
        }
        #endregion UI event handler
    }
}