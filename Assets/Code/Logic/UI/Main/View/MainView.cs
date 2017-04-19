using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Common.Util;
using Common.Localization;
using Logic.Enums;
using Logic.Audio.Controller;
using Logic.Character;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.ManageHeroes.Model;
using Logic.UI.CommonRoleTitle.View;
using Logic.WorldBoss.Model;
using Common.GameTime.Controller;
using Logic.Player;
using Logic.FunctionOpen.Model;
using System.Collections;
using Logic.UI.CommonAnimations;
using Logic.UI.Main.Model;
using Logic.UI.SignIn.Model;
using PathologicalGames;
using LuaInterface;

namespace Logic.UI.Main.View
{
    public class MainView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/main/main_view";
        public static MainView Open()
        {
            //MainView view = UIMgr.instance.Open<MainView>(PREFAB_PATH);
           // return view;
			LuaTable gameCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","game_controller")[0];
			gameCtrlLua.GetLuaFunction("OpenMainView").Call();
			return null;
        }
        #region ui
        public GameObject core;
        private CommonTopBarView _commonTopBarView;
        public RectTransform bgRectTransform;

        public Image accountHeadIcon;
        public Text accountLevelText;
        public Text accountNameText;
        public Slider accountExpSlider;
        public Text pveActionNextRecoverTimeText;

        private Dictionary<BaseResType, Text> _baseResourceTextDictionary;

        public RectTransform formationRootRectTransform;
        public List<CommonRoleTitleView> roleTitleViewList;
        public List<GameObject> formationPositionList;
        public List<GameObject> formationPositionShadowList;

        public Image hasNewHeroHintImage;
        public Image hasNewEquipmentHintImage;

        public Transform systemNoticeRoot;

        public Button worldBossButton;
        public Button pvpRaceButton;
        public Button signInButton;
        public Text worldBossOpenTimeText;
        public Text pvpRaceOpenTimeText;

        public CommonLeftPartAnimation leftPartAnimation;
        public CommonBottomPartAnimation bottomPartAnimation;
        public CommonFadeInAnimation[] fadeInAnimation;
        public bool testRefresh;
        private List<CharacterEntity> _characters = new List<CharacterEntity>();
        private float _lastTime;
        public Text vipLevelText;
        #endregion

        void Awake()
        {
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsMainViewStyle();

            Logic.UI.Chat.View.SystemNoticeView.Create(systemNoticeRoot);

            _baseResourceTextDictionary = new Dictionary<BaseResType, Text>();

            // hasNewHeroHintImage.gameObject.SetActive(false);
            hasNewEquipmentHintImage.gameObject.SetActive(false);

            RefreshAccoubntLevel();
            RefreshPveActionNextRecoverTime();
            RefreshBaseResources();
            RefreshHeroModels();
            RefreshWorldBossButton();
            RefreshNewHeroHint();
            RefreshSignHandler();
            RefreshVIPButton();
            BindDelegate();
            AudioController.instance.PlayBGMusic(AudioController.MAINSCENE);
            AudioController.instance.SetBGMusicState(AudioController.instance.isOpenAudioBg);
        }

        void Update()
        {
            if (Time.realtimeSinceStartup - _lastTime < 1f)
                return;
            _lastTime = Time.realtimeSinceStartup;
            RefreshPVPRace();
            RefreshWorldBossButton();
            if (testRefresh)
            {
                testRefresh = false;
                InitEnterAction();
            }

            //            Vector3 dir = Vector3.zero;
            //            dir.x = Input.acceleration.x;
            //            //			dir.y = Input.acceleration.y;
            //            if (dir.sqrMagnitude > 1)
            //                dir.Normalize();
            //            dir *= Time.deltaTime * 100;
            //
            //            float idealAnchoredX = formationRootRectTransform.anchoredPosition.x + dir.x;
            //            //			float idealAnchoredY = formationRootRectTransform.anchoredPosition.y + dir.y;
            //
            //            idealAnchoredX = Mathf.Clamp(idealAnchoredX, -50, 50);
            //            //			idealAnchoredY = Mathf.Clamp(idealAnchoredY, -100, 100);
            //
            //            formationRootRectTransform.anchoredPosition = new Vector2(idealAnchoredX, formationRootRectTransform.anchoredPosition.y);
            //            bgRectTransform.anchoredPosition = new Vector2(idealAnchoredX, bgRectTransform.anchoredPosition.y);
        }

        void OnDestroy()
        {
            DespawnCharacter(false);
            UnbindDelegate();
        }

        private void BindDelegate()
        {
            GameProxy.instance.onAccountInfoUpdateDelegate += OnAccountInfoUpdateHandler;
            GameProxy.instance.onPveActionInfoUpdateDelegate += OnPveActionInfoUpdateHandler;
            GameProxy.instance.onPveActionNextRecoverTimeUpdateDelegate += OnPveActionNextRecoverTimeUpdateHandler;
            GameProxy.instance.onBaseResourcesUpdateDelegate += OnBaseResourcesUpdateHandler;
            ManageHeroesProxy.instance.onEmbattleSuccessHandler += OnEmbattleSuccessHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
            HeroProxy.instance.onHeroInfoUpdateDelegate += OnHeroInfoUpdateHandler;
            HeroProxy.instance.onHeroInfoListUpdateDelegate += OnHeroInfoListUpdateHandler;
            HeroProxy.instance.onNewHeroMarksChangedDelegate += OnNewHeroMarksChangedHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate += OnEquipmentInfoListUpdateHandler;
            EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate += OnNewEquipmentMarksChangedHandler;
            WorldBossProxy.instance.onWorldBossStatusChangedDelegate += OnWorldBossStatusChangedHandler;
            WorldBossProxy.instance.onWorldBossOpenCountDownTimeUpdateDelegate += OnWorldBossOpenCountDownTimeUpdateHandler;
            MainViewProxy.instance.UpdateEnterMainViewDelegate += InitEnterAction;
//            SignInProxy.instance.UpdateSignedTodayDelegate += RefreshSignHandler;
            Logic.VIP.Model.VIPProxy.instance.onVIPInfoUpdateDelegate += OnVIPInfoUpdateHandler;
            Observers.Facade.Instance.RegisterObserver(UIMgr.UI_MAX_LAYER_CHANGE, UIMaxLayerChange);
			Observers.Facade.Instance.RegisterObserver("OnFormationChange", FormationChangeHandler);

        }

        private bool UIMaxLayerChange(Observers.Interfaces.INotification note)
        {
            //if (note.Type == PREFAB_PATH)
            //{
            //    if (!gameObject.activeInHierarchy)
            //        gameObject.SetActive(true);
            //}
            //else
            //{
            //    if (gameObject.activeInHierarchy)
            //        gameObject.SetActive(false);
            //}
            return true;
        }

        private void UnbindDelegate()
        {
            GameProxy.instance.onAccountInfoUpdateDelegate -= OnAccountInfoUpdateHandler;
            GameProxy.instance.onPveActionInfoUpdateDelegate -= OnPveActionInfoUpdateHandler;
            GameProxy.instance.onPveActionNextRecoverTimeUpdateDelegate -= OnPveActionNextRecoverTimeUpdateHandler;
            GameProxy.instance.onBaseResourcesUpdateDelegate -= OnBaseResourcesUpdateHandler;
            ManageHeroesProxy.instance.onEmbattleSuccessHandler -= OnEmbattleSuccessHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
            HeroProxy.instance.onHeroInfoUpdateDelegate -= OnHeroInfoUpdateHandler;
            HeroProxy.instance.onHeroInfoListUpdateDelegate -= OnHeroInfoListUpdateHandler;
            HeroProxy.instance.onNewHeroMarksChangedDelegate -= OnNewHeroMarksChangedHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate -= OnEquipmentInfoListUpdateHandler;
            EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate -= OnNewEquipmentMarksChangedHandler;
            WorldBossProxy.instance.onWorldBossStatusChangedDelegate -= OnWorldBossStatusChangedHandler;
            WorldBossProxy.instance.onWorldBossOpenCountDownTimeUpdateDelegate -= OnWorldBossOpenCountDownTimeUpdateHandler;
            MainViewProxy.instance.UpdateEnterMainViewDelegate -= InitEnterAction;
//            SignInProxy.instance.UpdateSignedTodayDelegate -= RefreshSignHandler;
            Logic.VIP.Model.VIPProxy.instance.onVIPInfoUpdateDelegate -= OnVIPInfoUpdateHandler;


			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(UIMgr.UI_MAX_LAYER_CHANGE, UIMaxLayerChange);
				Observers.Facade.Instance.RemoveObserver("OnFormationChange", FormationChangeHandler);
			}
                
        }
        private void InitEnterAction()
        {
            leftPartAnimation.StartAction();
            bottomPartAnimation.StartAction();
            _commonTopBarView.GetComponent<CommonTopBarAnimation>().StartAction();
            for (int i = 0, count = fadeInAnimation.Length; i < count; i++)
            {
                fadeInAnimation[i].StartAction();
            }
        }
        private void RefreshAccoubntLevel()
        {
            accountLevelText.text = string.Format(Localization.Get("ui.main_view.account_level"), GameProxy.instance.AccountLevel);
            //            accountNameText.text = GameProxy.instance.PlayerInfo.name;
            accountNameText.text = GameProxy.instance.AccountName;
            accountExpSlider.value = GameProxy.instance.AccountExpPercent;
			accountHeadIcon.SetSprite(Common.ResMgr.ResMgr.instance.Load<Sprite>(GameProxy.instance.AccountHeadIcon));
        }

        private void RefreshPveActionNextRecoverTime()
        {
            int pveAction = GameProxy.instance.PveAction;
            int pveActionMax = GameProxy.instance.PveActionMax;
            int pveActionNextRecoverTime = (int)GameProxy.instance.PveActionNextRecoverTime;
            if (pveAction < pveActionMax)
            {
                if (!pveActionNextRecoverTimeText.gameObject.activeSelf)
                {
                    pveActionNextRecoverTimeText.gameObject.SetActive(true);
                }
                TimeSpan timeSpan = TimeSpan.FromSeconds(pveActionNextRecoverTime);
                pveActionNextRecoverTimeText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                pveActionNextRecoverTimeText.gameObject.SetActive(false);
            }
        }
        private void RefreshSignHandler()
        {
            signInButton.gameObject.SetActive(!SignInProxy.instance.isSignInToday);
        }
        private void RefreshBaseResources()
        {
            Dictionary<BaseResType, int> baseResourceDictionary = GameProxy.instance.BaseResourceDictionary;
            List<BaseResType> keys = new List<BaseResType>(baseResourceDictionary.Keys);
            int keyCount = keys.Count;
            for (int i = 0; i < keyCount; i++)
            {
                BaseResType baseResType = keys[i];
                if (_baseResourceTextDictionary.ContainsKey(baseResType))
                {
                    _baseResourceTextDictionary[baseResType].text = baseResourceDictionary[baseResType].ToString();
                }
            }
        }

        private void DespawnCharacter(bool setForever)
        {
            int count = _characters.Count;
            for (int index = 0; index < count; index++)
            {
                CharacterEntity character = _characters[index];
                if (character)
                {
                    if (setForever)
                    {
                        if (character is PlayerEntity)
                            Pool.Controller.PoolController.instance.SetPoolForever((character as PlayerEntity).petEntity.name, false);
                        Pool.Controller.PoolController.instance.SetPoolForever(character.name, false);
                    }
                    Pool.Controller.PoolController.instance.Despawn(character.name, character);
                }
            }
            _characters.Clear();
        }

        private void RefreshHeroModels()
        {
            DespawnCharacter(true);
            int formationPositionCount = formationPositionList.Count;
            for (int formationPositionIndex = 0; formationPositionIndex < formationPositionCount; formationPositionIndex++)
            {
                roleTitleViewList[formationPositionIndex].gameObject.SetActive(false);
                formationPositionShadowList[formationPositionIndex].SetActive(false);

            }
            //Debugger.Log("RefreshHeroModels");
            List<uint> heroInstanceIDList = new List<uint>(ManageHeroesProxy.instance.CurrentFormationDictionary.Values);
            // 主角要一直站在中间
            heroInstanceIDList.Remove(GameProxy.instance.PlayerInfo.instanceID);
            heroInstanceIDList.Insert(0, GameProxy.instance.PlayerInfo.instanceID);
            int heroInstanceIDCount = heroInstanceIDList.Count;
            for (int i = 0; i < heroInstanceIDCount; i++)
            {
                if (GameProxy.instance.IsPlayer(heroInstanceIDList[i]))
                {
                    PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                    //Debugger.Log("create role {0}", playerInfo.ModelName);
                    //PlayerEntity playerEntity = PlayerEntity.CreatePlayerEntityAsUIElement(playerInfo, formationPositionList[i].transform, true, false);
                    PlayerEntity.CreatePlayerEntityAsUIElement(playerInfo, formationPositionList[i].transform, true, false, (playerEntity) =>
                    {

                        _characters.Add(playerEntity);
                        Pool.Controller.PoolController.instance.SetPoolForever(playerInfo.playerData.model, true);
                    }, (petEntity) =>
                    {
                        if (petEntity)
                            Pool.Controller.PoolController.instance.SetPoolForever(petEntity.name, true);
                    });
                    formationPositionList[i].transform.localRotation = Quaternion.Euler(playerInfo.heroData.homeRotation);
                    roleTitleViewList[i].SetPlayerInfo(playerInfo);
                }
                else
                {
                    HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceIDList[i]);
                    //  Debugger.Log("create role {0}", heroInfo.ModelName);
                    //HeroEntity heroEntity = HeroEntity.CreateHeroEntityAsUIElement(heroInfo, formationPositionList[i].transform, true, false);
                    HeroEntity.CreateHeroEntityAsUIElement(heroInfo, formationPositionList[i].transform, true, false, (heroEntity) =>
                    {
                        _characters.Add(heroEntity);
                        Pool.Controller.PoolController.instance.SetPoolForever(heroInfo.ModelName, true);
                    });
                    formationPositionList[i].transform.localRotation = Quaternion.Euler(heroInfo.heroData.homeRotation);
                    roleTitleViewList[i].SetHeroInfo(heroInfo);
                }
                roleTitleViewList[i].gameObject.SetActive(true);
                formationPositionShadowList[i].SetActive(true);
            }
        }

        private void RefreshWorldBossButton()
        {
            bool shouldShowWorldBossButton = false;
            if (WorldBossProxy.instance.IsOpen)
            {
                worldBossOpenTimeText.gameObject.SetActive(false);
                shouldShowWorldBossButton = true;
            }
            else
            {
                worldBossOpenTimeText.gameObject.SetActive(true);
                //if (WorldBossProxy.instance.OpenTime > 0)
                //{
                    int diffTime = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OpenTime);
                    if (diffTime <= GlobalData.GetGlobalData().bossInBefore)
                    {
                        shouldShowWorldBossButton = true;
					}else{
					diffTime = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
					if (diffTime >= -GlobalData.GetGlobalData().bossInLast)
					{
						shouldShowWorldBossButton = true;
					}
                }
                // }
//                else if (WorldBossProxy.instance.OverTime > 0)
//                {
//                    int diffTime = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
//                    if (diffTime >= -GlobalData.GetGlobalData().bossInLast)
//                    {
//                        shouldShowWorldBossButton = true;
//                    }
//                }
            }

            if (worldBossButton.gameObject.activeSelf != shouldShowWorldBossButton)
            {
                worldBossButton.gameObject.SetActive(shouldShowWorldBossButton);
            }
        }

        private void RefreshPVPRace()
        {
            GlobalData globalData = GlobalData.GetGlobalData();
            DateTime serverTime = TimeController.instance.ServerTime;
            DateTime openTime = globalData.point_pvp_start_time;
            DateTime startTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, openTime.Hour, openTime.Minute, openTime.Second, openTime.Millisecond);
            int diffTime = TimeUtil.GetDiffTime(serverTime, startTime);
            bool show = false;
            if (diffTime <= globalData.point_pvp_open_time && diffTime > 0)
            {
                if (!pvpRaceOpenTimeText.gameObject.activeSelf)
                    pvpRaceOpenTimeText.gameObject.SetActive(true);
                pvpRaceOpenTimeText.text = TimeUtil.FormatSecondToHour(diffTime);
                show = true;
            }
            else if (diffTime <= 0 && (Mathf.Abs(diffTime) < globalData.point_pvp_keep_time))
            {
                if (pvpRaceOpenTimeText.gameObject.activeSelf)
                    pvpRaceOpenTimeText.gameObject.SetActive(false);
                show = true;
            }
            else
                show = false;
            if (pvpRaceButton.gameObject.activeSelf != show)
                pvpRaceButton.gameObject.SetActive(show);
        }

        private void RefreshNewHeroHint()
        {
            //hasNewHeroHintImage.gameObject.SetActive(HeroProxy.instance.HasNewHero());
        }

        private void RefreshVIPButton()
        {
            vipLevelText.text = Logic.VIP.Model.VIPProxy.instance.VIPLevel.ToString();
        }

        #region proxy callback handlers
        public void OnAccountInfoUpdateHandler()
        {
            RefreshAccoubntLevel();

        }

        public void OnPveActionInfoUpdateHandler()
        {
            RefreshPveActionNextRecoverTime();
        }

        public void OnPveActionNextRecoverTimeUpdateHandler()
        {
            RefreshPveActionNextRecoverTime();
        }

        public void OnBaseResourcesUpdateHandler()
        {
            RefreshBaseResources();
        }

        public void OnEmbattleSuccessHandler()
        {
            RefreshHeroModels();
        }

        public void OnPlayerInfoUpdateHandler()
        {
            RefreshHeroModels();
        }

        public void OnHeroInfoUpdateHandler(uint heroInstanceID)
        {
            RefreshHeroModels();
        }

        public void OnHeroInfoListUpdateHandler()
        {
            RefreshNewHeroHint();
        }

        public void OnNewHeroMarksChangedHandler()
        {
            RefreshNewHeroHint();
        }

        public void OnEquipmentInfoListUpdateHandler()
        {
            hasNewEquipmentHintImage.gameObject.SetActive(EquipmentProxy.instance.HasNewEquipment());
        }

        public void OnNewEquipmentMarksChangedHandler()
        {
            hasNewEquipmentHintImage.gameObject.SetActive(EquipmentProxy.instance.HasNewEquipment());
        }

        void OnWorldBossStatusChangedHandler()
        {
            RefreshWorldBossButton();
        }

        public void OnWorldBossOpenCountDownTimeUpdateHandler(int second)
        {
            worldBossOpenTimeText.text = TimeUtil.FormatSecondToHour(second);
        }

        void OnVIPInfoUpdateHandler(int vipLevel, int totalRecharge, List<int> hasReceivedGiftVIPLevelList)
        {
            RefreshVIPButton();
        }
        #endregion

        #region 事件
        public void ClickPlayerAvaterHandler()
        {
            //			UIMgr.instance.Open(Logic.UI.Player.View.PlayerView.PREFAB_PATH);
            // Logic.UI.Chat.View.GMView.Open();
            Logic.UI.AccountInfo.View.AccountInfoView.Open();
        }

        public void ClickPlayerInfoButtonHandler()
        {

            UIMgr.instance.Open(Logic.UI.Player.View.PlayerInfoView.PREFAB_PATH);
        }

        public void ClickMyHeroesButtonHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Hero);

        }

        public void ClickPVEEmbattleButtonHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Formation);
        }

        public void OpenCopyMapBtnClickHandler()
        {
            if (FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.Dungeon_SelectChapter_View))
            {
//                DungeonType lastSelectedDungeonType = Logic.Chapter.Model.ChapterProxy.instance.LastSelectedDungeonType;
//                if (lastSelectedDungeonType == DungeonType.Invalid)
//                    lastSelectedDungeonType = DungeonType.Easy;
//                int lastSelectedDungeonDataID = Logic.Dungeon.Model.DungeonProxy.instance.GetLastUnlockDungeonID(lastSelectedDungeonType);
//				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectedDungeonDataID);
				DungeonType lastSelectPVEDungoenType = Dungeon.Model.DungeonProxy.instance.LastSelectPVEDungeonType;
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectPVEDungoenType);
            }
        }


        public void ClickEquipmentsBrowseViewHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
            UIMgr.instance.Open(Logic.UI.Equipments.View.EquipmentsBrowseView.PREFAB_PATH);
        }

        public void ClickShopHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Shop);

        }

        public void ClickBlackMarketHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_BlackMarket);


        }

        public void ClickHeroCombineHandler()
        {
            UIMgr.instance.Open(Logic.UI.HeroCombine.View.HeroCombineView.PREFAB_PATH);
        }

        public void ClickWorldBossHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_WorldBoss);
        }

        public void ClickPVPRaceHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_PvpRace);
        }

        public void ClickHeroSkimBtnHandler()
        {
            //            Logic.UI.Tips.Tipser.ShowTips(UI.Tips.TipsType.UnopenFunction);
            UIMgr.instance.Open(Logic.UI.HeroesPreview.View.HeroesPreviewView.PREFAB_PATH);
        }

        public void ClickAchievementBtnHandler()
        {
            //            Logic.UI.Tips.Tipser.ShowTips(UI.Tips.TipsType.UnopenFunction);
            UIMgr.instance.Open(Logic.UI.Achievements.View.AchievementsView.PREFAB_PATH);
        }

        public void ClickSettingBtnHandler()
        {
            UI.UIMgr.instance.Close(PREFAB_PATH);
            Logic.UI.Login.View.LoginView.Open();
            //Logic.UI.Tips.Tipser.ShowTips(UI.Tips.TipsType.UnopenFunction);
        }

        public void ClickDailyDungeonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_DailyDungeon, true))
            {
                return;
            }
            UIMgr.instance.Open<Logic.UI.DailyDungeon.View.DailyDungeonView>(Logic.UI.DailyDungeon.View.DailyDungeonView.PREFAB_PATH);
        }

        public void ClickPvpBtnHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Arena, null, false, true);
        }
        public void ClickTaskBtnHandler()
        {
            Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Task, null, false, true);

        }
        public void ClickMailButtonHandler()
        {
            Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Mail, null, false, true);
        }
        public void ClickExpeditionBtnHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Expedition, null, false, true);
        }
        public void ClickMultipleFightBtnHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_FightCenter);

        }
        public void ClickIllustrationBtnHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_illustrate, null, false, true);
            //            Dialog.View.DialogView.Open(1001, null);
        }
        public void ClickFriendBtnHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Friend, null, false, true);
        }
        public void ClickSignInBtnHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_SignIn, null, false, true);
        }

        public void ClickStartTutorialButtonHandler()
        {
            Fight.Controller.ConsortiaFightController.instance.CLIENT2LOBBY_FIGHT_MATCH_REQ();
            //			Tutorial.View.TutorialView.Open(Logic.Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData);
            //SkillHead.View.SkillHeadView.Open(HeroProxy.instance.GetAllHeroInfoList()[1], SkillHeadViewShowType.Right);
        }

        public void ClickActivityButtonHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Activity);
            // LuaInterface.LuaScriptMgr.Instance.CallLuaFunctionVoid("CallActivity");
        }

        public void ClickExploreButtonHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Explore);
            //LuaInterface.LuaScriptMgr.Instance.CallLuaFunctionVoid("CallExplore");
        }

        public void ClickRankingButtonHandler()
        {
            FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Ranking, 1);
            //LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "ranking_controller")[0];
            //table.GetLuaFunction("OpenRankingView").Call(1);
        }

        public void ClickArenaPointRaceBtnHandler()
        {
            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "arena_model")[0];
            table.GetLuaFunction("OpenRaceIntroView").Call();
        }

        public void ClickConsortiaButtonHandler()
        {
            //			LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","consortia_model")[0]; 
            //			table.GetLuaFunction("OpenCreateConsortiaView").Call();

            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "consortia_controller")[0];
            table.GetLuaFunction("OpenConsortiaView").Call();
        }
        public void ClickHeroComposeHandler()
        {
            //			LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","consortia_model")[0]; 
            //			table.GetLuaFunction("OpenCreateConsortiaView").Call();

            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "hero_controller")[0];
            table.GetLuaFunction("OpenHeroComposeView").Call();
        }

        public void ClickVIPButtonHandler()
        {
            LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "vip_controller")[0];
            table.GetLuaFunction("OpenVIPView").Call();
        }

		public void ClickPackButtonHandler ()
		{
			LuaInterface.LuaTable table = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "pack_controller")[0];
			table.GetLuaFunction("OpenPackView").Call();
		}
		private bool FormationChangeHandler(Observers.Interfaces.INotification note)
		{
			RefreshHeroModels();
			return true;
		}
		
        #endregion

    }
}
