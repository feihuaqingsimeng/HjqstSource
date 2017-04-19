using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Common.Localization;
using Logic.Enums;
using Logic.UI;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.UI.ManageHeroes.Model;
using Common.ResMgr;
using Common.GameTime.Controller;
using LuaInterface;
using System.Collections;
using Logic.PushMessage.Model;
using Common.Notificator;
using Logic.UI.LoadGame.View;
using LitJson;
using Logic.UI.ServerList.Model;
using Common.Util;
using Logic.TalkingData.Controller;
using Object = UnityEngine.Object;

namespace Logic.Game.Controller
{
    public class GameController : SingletonMono<GameController>
    {
        private GameObject _gameObject;
        public delegate void ConnectGameServerHandler();
        private ConnectGameServerHandler _connectGameServerHandler;
        private int _loadNum = 0;
        private const int MAX_LOAD_NUMBER = 5;

        private bool _isWifi = false;
        public bool isWifi
        {
            get { return _isWifi; }
        }

        void Awake()
        {
            instance = this;
            _gameObject = gameObject;
            CleanLocalNotificator();
        }

        void Start()
        {
            _loadNum = 0;
#if UNITY_EDITOR
            if (GameConfig.instance.skipLaunchProcess)
                InitGame();
            else
            {
#endif
                if (GameDataCenter.instance.loginNum == 0)
                {
                    //StartCoroutine("GetMovieConfigCoroutine");
                    Logic.UI.Launch.View.LogoView logoView = Logic.UI.Launch.View.LogoView.Open();
                    logoView.SetOnTimeOutDelegate(HealthAdvice);
                }
                else
                    InitGame();
#if UNITY_EDITOR
            }
#endif
        }


        private void HealthAdvice()
        {
//            Logic.UI.HealthAdvice.View.HealthAdviceView healthAdviceView = Logic.UI.HealthAdvice.View.HealthAdviceView.Open();
//            healthAdviceView.SetOnTimeOutDelegate(LauchMovie);
			LauchMovie();
        }


        /*private IEnumerator GetMovieConfigCoroutine()
        {
            WWW www = new WWW(ResUtil.GetRemoteStaticPath("movieConfig.txt"));
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
                Debugger.LogError(string.Format("{0}加载失败", www.url));
            else
                GameConfig.instance.playMovie = www.text == "1";
            www.Dispose();
            www = null;
        }*/

        private void LauchMovie()
        {
            if (GameConfig.instance.playMovie)
            {
                Logic.UI.Launch.View.LaunchMovieView launchMovieView = Logic.UI.Launch.View.LaunchMovieView.Open();
                launchMovieView.SetOnTimeOutDelegate(InitGame);
            }
            else
                InitGame();
        }

        private void InitGame()
        {
            _isWifi = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            InitOutLogger();
            StartCoroutine(DecompressionCsvZIP());
        }

        private void StartGame()
        {
#if UNITY_EDITOR
            LoadCSVHandler();
#else
            if (!isWifi)
                Logic.UI.WifiTips.View.WifiTipsView.Open(GetCDNVersion, CancelDownload);
            else
                GetCDNVersion();
#endif
        }
        private IEnumerator DecompressionCsvZIP()
        {
#if !UNITY_EDITOR
            var isExist = Common.ResMgr.ResUtil.ExistConfigMD5InLocal();
            bool isNew = false;
            if (isExist)
            {
                yield return this.StartCoroutine(FileLoader.CheckLocalAndStreamingVersion(this,ResUtil.CSV_DIR + "/" + ResUtil.CSV_MD5, b =>isNew = b, 0));
            }
            if (!isExist||isNew)
            {
                if (isNew)
                {
                    string unzipPath = ResUtil.GetLocalPath("");
                    if (Directory.Exists(unzipPath))
                        Directory.Delete(unzipPath, true);
                }

                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2DecompressionTips();
                loadGameView.SetDefaultBG();
                loadGameView.showRandomTips = false;
                FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
                fileLoader.isRemote = false;
                fileLoader.failHandler += StartGame;
                fileLoader.completeHandler += DecompressionLuaZIP;
                fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
                fileLoader.targetDir = "";
                fileLoader.md5Name = ResUtil.CSV_ZIP;
                fileLoader.fromPorgress = 0f;
                fileLoader.toPorgress =102f/(102+630);
                fileLoader.LoadZip();
            }
            else
                StartGame();
#else
            StartGame();
#endif
            yield break;
        }
        private void DecompressionLuaZIP()
        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//            if (!Common.ResMgr.ResUtil.ExistConfigMD5InLocal())
            {
                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2DecompressionTips();
                loadGameView.SetDefaultBG();
                loadGameView.showRandomTips = false;
                FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
                fileLoader.isRemote = false;
                fileLoader.failHandler += StartGame;
                fileLoader.completeHandler += StartGame;
                fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
                fileLoader.targetDir = "";
                fileLoader.md5Name = ResUtil.LUA_ZIP;
                fileLoader.fromPorgress = 102f/(102+630);
                fileLoader.toPorgress = 1f;
                fileLoader.LoadZip();
            }
//            else
//                StartGame();
//#else
//            StartGame();
//#endif
        }
        private void DecompressionCSV()
        {
#if !UNITY_EDITOR
            if (!Common.ResMgr.ResUtil.ExistConfigMD5InLocal())
            {
                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2DecompressionTips();
                loadGameView.SetDefaultBG();
                loadGameView.showRandomTips = false;
                FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
                fileLoader.isRemote = false;
                fileLoader.completeHandler += DecompressionLua;
                fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
                fileLoader.targetDir = ResUtil.CSV_DIR;
                fileLoader.md5Name = ResUtil.CSV_MD5;
                fileLoader.fromPorgress = 0f;
                fileLoader.toPorgress = 0.5f;
                fileLoader.Load();
            }
            else
                StartGame();
#else
            StartGame();
#endif
        }

        private void DecompressionLua()
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2DecompressionTips();
            loadGameView.SetDefaultBG();
            loadGameView.showRandomTips = false;
            FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
            fileLoader.isRemote = false;
            fileLoader.completeHandler += StartGame;
            fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
            fileLoader.targetDir = ResUtil.LUA_DIR;
            fileLoader.md5Name = ResUtil.LUA_MD5;
            fileLoader.fromPorgress = 0.5f;
            fileLoader.toPorgress = 1f;
            fileLoader.Load();
        }

        public void InitOutLogger()
        {
            int showLog = PlayerPrefs.GetInt("showLog", 0);
            Common.OutLogger.OutLogger outLogger = _gameObject.GetComponent<Common.OutLogger.OutLogger>();
            if (!outLogger)
                outLogger = _gameObject.AddComponent<Common.OutLogger.OutLogger>();
            if (showLog == 1)
            {
                outLogger.logScreen = true;
            }
            else
            {
                outLogger.logScreen = false;
            }
        }

        private void GetCDNVersion()
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            loadGameView.SetDefaultBG();
            _loadNum = 0;
            StartCoroutine("GetCDNVersionCoroutine");
        }

        private IEnumerator GetCDNVersionCoroutine()
        {
            _loadNum++;
			WWW www = new WWW(ResUtil.GetRemoteStaticPathByCdn("cdnVersion.txt"));
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debugger.LogError(string.Format("{0}加载失败", www.url));
                www.Dispose();
                www = null;
                if (_loadNum < MAX_LOAD_NUMBER)
                {
                    yield return new WaitForSeconds(1f);
                    StopCoroutine("GetCDNVersionCoroutine");
                    StartCoroutine("GetCDNVersionCoroutine");
                }
                else
                {
                    Debugger.LogError("又加载失败了，心累了，不想加载了...");
                    LoadFileFailHandler();
                }
            }
            else
            {
                GameConfig.instance.cdnVersion = www.text;
                www.Dispose();
                www = null;
				Debugger.LogError(string.Format("cdnVersion加载成功:{0}",GameConfig.instance.cdnVersion));
                yield return new WaitForSeconds(1f);
                LoadCSVHandler();
            }
        }

        private void CancelDownload()
        {
            Debugger.Log("application quit!");
            Application.Quit();
        }

        private void LoadFileFailHandler()
        {
            Logic.UI.NetErrorTips.View.NetErrorTipsView.Open(CancelDownload);
        }

        private void LoadCSVHandler()
        {
            if (Logic.Game.GameConfig.instance.loadCSVRemote)
            {
                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2CheckConfigTips();
				loadGameView.SetDefaultBG();
                loadGameView.showRandomTips = false;
                FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
                fileLoader.switchUpdateConfigTips = true;
                fileLoader.completeHandler += LoadLuaHandler;
                fileLoader.failHandler += LoadFileFailHandler;
                fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
                fileLoader.targetDir = ResUtil.CSV_DIR;
                fileLoader.md5Name = ResUtil.CSV_MD5;
                fileLoader.fromPorgress = 0f;
                fileLoader.toPorgress = 0.4f;
                fileLoader.Load();
            }
            else
                LoadLuaHandler();
        }

        private void LoadLuaHandler()
        {
            #region load lua
            if (GameConfig.instance.loadLuaRemote)
            {
                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.OpenLoadGameView();
				loadGameView.SetDefaultBG();
                FileLoader fileLoader = _gameObject.AddComponent<FileLoader>();
                fileLoader.completeHandler += PreloadConfigTable;
                fileLoader.failHandler += LoadFileFailHandler;
                fileLoader.progressUpdateHandler += loadGameView.UpdateLoadProgress;
                fileLoader.targetDir = ResUtil.LUA_DIR;
                fileLoader.md5Name = ResUtil.LUA_MD5;
                fileLoader.fromPorgress = 0.4f;
                fileLoader.toPorgress = 0.8f;
                fileLoader.Load();
            }
            else
                PreloadConfigTable();
            #endregion
        }

        private void PreloadConfigTable()
        {
            StartCoroutine("PreloadConfigTableCoroutine");
        }

        private IEnumerator PreloadConfigTableCoroutine()
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.OpenLoadGameView();
			loadGameView.SetDefaultBG();
            loadGameView.SetCompleteHandler(PreloadRes);
            float progress = 0.8f;
            float interval = 0.2f / 7f;
            Logic.Hero.Model.HeroData.GetHeroDatas();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.Dungeon.Model.DungeonData.GetDungeonDatas();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.Team.Model.TeamData.GetTeamDatas();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.FunctionOpen.Model.FunctionData.GetFunctionDatas();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.Task.Model.TaskData.GetTaskDataDicionary();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.Item.Model.ItemData.GetItemDatas();
            yield return null;
            progress += interval;
            loadGameView.UpdateLoadProgress(progress);
            Logic.Formation.Model.FormationData.GetFormationDatas();
            yield return null;
            progress = 1f;
            loadGameView.UpdateLoadProgress(progress);
        }

        private void PreloadRes()
        {
            if (Game.GameConfig.assetBundle)
            {
                UI.UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
                Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2LoadResTips();
                loadGameView.showRandomTips = false;
                Common.ResMgr.ResPreload resPreload = _gameObject.AddComponent<Common.ResMgr.ResPreload>();//preload res
                resPreload.completeHandler += ResPreloadCompleteHandler;
                resPreload.progressHandler += loadGameView.UpdateLoadProgress;
                resPreload.errorHandler += ResPreloadErrorHandler;
                resPreload.CheckVersion(null);
            }
            else
            {
                Init();
            }
        }

        private void ResPreloadCompleteHandler(Common.ResMgr.ResPreload target)
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Get<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            target.completeHandler -= ResPreloadCompleteHandler;
            target.progressHandler -= loadGameView.UpdateLoadProgress;
            target.errorHandler -= ResPreloadErrorHandler;
            ResMgr.instance.PreLoadAssetBundleManifest();
            StartCoroutine("InitCoroutine");
        }

        private void ResPreloadErrorHandler(Common.ResMgr.ResPreload target)
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Get<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            target.completeHandler -= ResPreloadCompleteHandler;
            target.progressHandler -= loadGameView.UpdateLoadProgress;
            target.errorHandler -= ResPreloadErrorHandler;
            UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.common.tips.relogin"), ReLoadMainScene, EUISortingLayer.TopTips);
        }

        private System.Collections.IEnumerator InitCoroutine()
        {
            yield return null;//等两帧就不会crash?
            yield return null;
            Init();
        }

        public void InitLua()
        {
            new LuaScriptMgr().Start();
        }

        public void ReLoadMainScene()
        {
            GameDataCenter.instance.ReLoadMainScene();
        }

        //private System.Collections.IEnumerator LoadMainScene()
        //{
        //    UI.UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
        //    AsyncOperation ao = Application.LoadLevelAsync("main");
        //    Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
        //    while (!ao.isDone)
        //    {
        //        yield return null;
        //        loadGameView.UpdateLoadProgress(ao.progress, 0, 1);
        //    }
        //}

        public void GC(float delay, System.Action<float> updateProgressAction, System.Action finishedAction)
        {
            StartCoroutine(GCCoroutine(delay, updateProgressAction, finishedAction));
            Debugger.Log("GC");
        }

        private IEnumerator GCCoroutine(float delay, System.Action<float> updateProgressAction, System.Action finishedAction)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            System.GC.Collect();
            LuaScriptMgr.Instance.LuaGC();
            while (!ao.isDone)
            {
                yield return null;
                if (updateProgressAction != null)
                    updateProgressAction(ao.progress);
                Debugger.Log("gc progress:{0}", ao.progress);
            }
            if (updateProgressAction != null)
                updateProgressAction(1f);
            if (finishedAction != null)
                finishedAction();
        }

        private void Init()
        {
            try
            {
                InitLua();
            }
            catch (System.Exception e)
            {
                Debugger.LogError("init lua fail! " + e.StackTrace);
            }
            _gameObject.AddComponent<Common.Localization.LocalizationController>();
            Debugger.EnableLog = GameConfig.ableDebug;
            UI.UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            _gameObject.AddComponent<Logic.Game.Model.GameProxy>();
            _gameObject.AddComponent<Common.GameTime.Controller.TimeController>();

            _gameObject.AddComponent<Logic.Tutorial.Model.TutorialProxy>();
            _gameObject.AddComponent<Logic.Tutorial.Controller.TutorialController>();

            _gameObject.AddComponent<Logic.UI.Login.Model.LoginProxy>();
            _gameObject.AddComponent<Logic.UI.Login.Controller.LoginController>();
            _gameObject.AddComponent<Logic.UI.RandomName.Model.RandomNameProxy>();
            _gameObject.AddComponent<Logic.UI.CreateRole.Model.CreateRoleProxy>();
            _gameObject.AddComponent<Logic.UI.CreateRole.Controller.CreateRoleController>();

            _gameObject.AddComponent<Logic.Fight.Controller.FightController>();
            _gameObject.AddComponent<Logic.Fight.Controller.ConsortiaFightController>();
            _gameObject.AddComponent<Logic.Fight.Controller.PassiveSkillController>();
            _gameObject.AddComponent<Logic.Fight.Controller.MechanicsController>();
            _gameObject.AddComponent<Logic.Effect.Controller.EffectController>();
            _gameObject.AddComponent<Logic.Net.Controller.VirtualServer>();
            _gameObject.AddComponent<Logic.Net.Controller.VirtualServerController>();
            _gameObject.AddComponent<Logic.Action.Controller.ActionController>();
            _gameObject.AddComponent<Logic.Character.Controller.PlayerController>();
            _gameObject.AddComponent<Logic.Character.Controller.EnemyController>();
            _gameObject.AddComponent<Logic.UI.SkillBar.Controller.SkillBarController>();
            _gameObject.AddComponent<Logic.Judge.Controller.JudgeController>();
            _gameObject.AddComponent<Logic.Scene.Controller.SceneController>();

            _gameObject.AddComponent<Logic.Player.Model.PlayerProxy>();
            _gameObject.AddComponent<Logic.Player.Controller.PlayerController>();

            _gameObject.AddComponent<Logic.Hero.Model.HeroProxy>();
            _gameObject.AddComponent<Logic.Hero.Controller.HeroController>();

            _gameObject.AddComponent<Logic.Item.Controller.ItemController>();
            _gameObject.AddComponent<Logic.Item.Model.ItemProxy>();

            _gameObject.AddComponent<Logic.Equipment.Model.EquipmentProxy>();
            _gameObject.AddComponent<Logic.Equipment.Controller.EquipmentController>();

            _gameObject.AddComponent<Logic.UI.EquipmentsStrengthen.Model.EquipmentStrengthenProxy>();

            _gameObject.AddComponent<Logic.UI.ManageHeroes.Model.ManageHeroesProxy>();
            _gameObject.AddComponent<Logic.UI.ManageHeroes.Controller.ManageHeroesController>();

            _gameObject.AddComponent<Logic.FunctionOpen.Model.FunctionOpenProxy>();

            _gameObject.AddComponent<Logic.UI.Shop.Model.ShopProxy>();
            _gameObject.AddComponent<Logic.UI.Shop.Controller.ShopController>();

            _gameObject.AddComponent<Logic.UI.HeroCombine.Model.HeroCombineProxy>();

            _gameObject.AddComponent<Logic.UI.DungeonDetail.Model.DungeonDetailProxy>();

            _gameObject.AddComponent<Logic.Chapter.Model.ChapterProxy>();

            _gameObject.AddComponent<Logic.UI.HeroStrengthen.Model.HeroStrengthenProxy>();

            _gameObject.AddComponent<Logic.UI.HeroAdvance.Model.HeroAdvanceProxy>();

            _gameObject.AddComponent<Logic.Dungeon.Model.DungeonProxy>();
            _gameObject.AddComponent<Logic.Dungeon.Controller.DungeonController>();

            _gameObject.AddComponent<Logic.Activity.Model.ActivityProxy>();
            _gameObject.AddComponent<Logic.Activity.Controller.ActivityController>();

            _gameObject.AddComponent<Logic.Fight.Model.FightProxy>();
            _gameObject.AddComponent<Logic.UI.FightResult.Model.FightResultProxy>();

            _gameObject.AddComponent<Logic.UI.Pvp.Model.PvpProxy>();
            _gameObject.AddComponent<Logic.UI.Pvp.Controller.PvpController>();
            _gameObject.AddComponent<Logic.UI.Pvp.Model.PvpBattleReportProxy>();
            _gameObject.AddComponent<Logic.UI.Pvp.Model.PvpFormationProxy>();

            _gameObject.AddComponent<Logic.UI.Task.Model.TaskProxy>();
            _gameObject.AddComponent<Logic.UI.Task.Controller.TaskController>();

            _gameObject.AddComponent<Logic.UI.Expedition.Model.ExpeditionFormationProxy>();
            _gameObject.AddComponent<Logic.UI.Expedition.Model.ExpeditionProxy>();
            _gameObject.AddComponent<Logic.UI.Expedition.Controller.ExpeditionController>();

            _gameObject.AddComponent<Logic.UI.WorldTree.Model.WorldTreeProxy>();
            _gameObject.AddComponent<Logic.UI.WorldTree.Controller.WorldTreeController>();

            _gameObject.AddComponent<Logic.WorldBoss.Model.WorldBossProxy>();
            _gameObject.AddComponent<Logic.WorldBoss.Controller.WorldBossController>();

            _gameObject.AddComponent<Logic.UI.RedPoint.Model.RedPointProxy>();
            _gameObject.AddComponent<Logic.UI.RedPoint.Controller.RedPointController>();

            _gameObject.AddComponent<Logic.UI.BlackMarket.Model.BlackMarketProxy>();
            _gameObject.AddComponent<Logic.UI.BlackMarket.Controller.BlackMarketController>();

            _gameObject.AddComponent<Logic.UI.Chat.Model.ChatProxy>();
            _gameObject.AddComponent<Logic.UI.Chat.Controller.ChatController>();
            _gameObject.AddComponent<Logic.UI.Chat.Model.SystemNoticeProxy>();
            _gameObject.AddComponent<Logic.UI.Mail.Model.MailProxy>();
            _gameObject.AddComponent<Logic.UI.Mail.Controller.MailController>();

            _gameObject.AddComponent<Logic.UI.SoftGuide.Model.SoftGuideProxy>();

            _gameObject.AddComponent<Logic.UI.RandomGift.Model.RandomGiftProxy>();
            _gameObject.AddComponent<Logic.UI.RandomGift.Controller.RandomGiftController>();

            _gameObject.AddComponent<Logic.UI.Friend.Model.FriendProxy>();
            _gameObject.AddComponent<Logic.UI.Friend.Controller.FriendController>();

            _gameObject.AddComponent<Logic.UI.IllustratedHandbook.Model.IllustratedHandbookProxy>();
            _gameObject.AddComponent<Logic.UI.IllustratedHandbook.Controller.IllustrationController>();

            _gameObject.AddComponent<Logic.VIP.Model.VIPProxy>();
            _gameObject.AddComponent<Logic.VIP.Controller.VIPController>();

            _gameObject.AddComponent<Logic.Formation.Model.FormationProxy>();
            _gameObject.AddComponent<Logic.Formation.Controller.FormationController>();

            _gameObject.AddComponent<Logic.UI.TrainFormation.Model.TrainFormationProxy>();

            _gameObject.AddComponent<Logic.UI.Mask.Contorller.MaskController>();
            _gameObject.AddComponent<Logic.UI.FightTips.Controller.FightTipsController>();

            _gameObject.AddComponent<Logic.UI.ServerList.Model.ServerListProxy>();
            _gameObject.AddComponent<Logic.UI.ServerList.Controller.ServerListController>();
            _gameObject.AddComponent<Logic.Protocol.ProtocolConf>();

            _gameObject.AddComponent<Logic.UI.SignIn.Controller.SignInController>();
            _gameObject.AddComponent<Logic.UI.SignIn.Model.SignInProxy>();

            _gameObject.AddComponent<Logic.ConsumeTip.Model.ConsumeTipProxy>();
            _gameObject.AddComponent<Logic.UI.Main.Model.MainViewProxy>();

            _gameObject.AddComponent<Logic.Player.Model.PlayerTalentProxy>();
            _gameObject.AddComponent<Logic.Fight.Controller.MockFightController>();

            _gameObject.AddComponent<Logic.UI.Mine.Controller.MineController>();

            _gameObject.AddComponent<Logic.UI.Buff.Controller.BuffTipsController>();

            Login();

            Observers.Facade.Instance.RegisterObserver(((int)MSG.UserLogoutResp).ToString(), LOBBY2CLIENT_UUSER_LOGOUT_RESP);
            //            Observers.Facade.Instance.RegisterObserver(((int)MSG.RoleLvAndExpResp).ToString(), LOBBY2CLIENT_ROLE_LV_AND_EXP_RESP);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.PackResp).ToString(), LOBBY2CLIENT_PACK_RESP_handler);
            // Observers.Facade.Instance.RegisterObserver(((int)MSG.BaseResourceSyn).ToString(), LOBBY2CLIENT_BASE_RESOURCE_SYN_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.SynPveActionResp).ToString(), LOBBY2CLIENT_SYB_PVE_ACTION_RESP_handler);
            // Observers.Facade.Instance.RegisterObserver(((int)MSG.BuyPackCellResp).ToString(), LOBBY2CLIENT_BUY_PACK_CELL_RESP);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.ErrorResp).ToString(), LOBBY2CLIENT_ERROR_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.ServerTimeSynResp).ToString(), LOBBY2CLIENT_ServerTimeSyn_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.GetCreateTimeResp).ToString(), LOBBY2CLIENT_GetCreateTimeResp_RESP_handler);



            Observers.Facade.Instance.RegisterObserver("game_model_UpdateAccountLevelAndExpDelegateByProtocol", LOBBY2CLIENT_ROLE_LV_AND_EXP_RESP);
            Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_BASE_RESOURCE_SYN_handler", LOBBY2CLIENT_BASE_RESOURCE_SYN_handler);
            Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_PACK_RESP_handler", LOBBY2CLIENT_PACK_RESP_handler);
            Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_BUY_PACK_CELL_RESP", LOBBY2CLIENT_BUY_PACK_CELL_RESP);
            PlatformResultProxy.instance.onPlatformLogoutSuccessDelegate = PlatformLogoutSuccess_game;
            PlatformResultProxy.instance.onPlatformPaySuccess += OnChargeSuccessHandler;
            TalkingDataController.instance.TalkingDataGAOnStart(GameConfig.TALKING_DATA_APP_ID, GameConfig.CHANNEL_ID);
            AdTracking.Controller.AdTrackingController.instance.InitAdTracking();
            GameSetting.instance.pushMessage = PlayerPrefs.GetInt(GameSetting.PUSH_MSG, 1) == 1;
        }

        private void Login()
        {
            Logic.UI.Login.View.LoginView.Open();
            GameDataCenter.instance.loginNum++;
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.UserLogoutResp).ToString(), LOBBY2CLIENT_UUSER_LOGOUT_RESP);
                //                Observers.Facade.Instance.RemoveObserver(((int)MSG.RoleLvAndExpResp).ToString(), LOBBY2CLIENT_ROLE_LV_AND_EXP_RESP);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.PackResp).ToString(), LOBBY2CLIENT_PACK_RESP_handler);
                // Observers.Facade.Instance.RemoveObserver(((int)MSG.BaseResourceSyn).ToString(), LOBBY2CLIENT_BASE_RESOURCE_SYN_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.SynPveActionResp).ToString(), LOBBY2CLIENT_SYB_PVE_ACTION_RESP_handler);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.BuyPackCellResp).ToString(), LOBBY2CLIENT_BUY_PACK_CELL_RESP);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.ErrorResp).ToString(), LOBBY2CLIENT_ERROR_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.ServerTimeSynResp).ToString(), LOBBY2CLIENT_ServerTimeSyn_RESP_handler);

                Observers.Facade.Instance.RemoveObserver(((int)MSG.GetCreateTimeResp).ToString(), LOBBY2CLIENT_GetCreateTimeResp_RESP_handler);


                Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_BASE_RESOURCE_SYN_handler", LOBBY2CLIENT_BASE_RESOURCE_SYN_handler);
                Observers.Facade.Instance.RemoveObserver("game_model_UpdateAccountLevelAndExpDelegateByProtocol", LOBBY2CLIENT_ROLE_LV_AND_EXP_RESP);
                Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_BUY_PACK_CELL_RESP", LOBBY2CLIENT_BUY_PACK_CELL_RESP);
                Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_PACK_RESP_handler", LOBBY2CLIENT_PACK_RESP_handler);
            }
            PlatformResultProxy.instance.onPlatformLogoutSuccessDelegate = null;
            PlatformResultProxy.instance.onPlatformPaySuccess -= OnChargeSuccessHandler;
            LeanTween.cancelAll(false);
            if (LuaScriptMgr.Instance != null) LuaScriptMgr.Instance.Destroy();
        }

        public bool ConnectGameServer(string host, int port, ConnectGameServerHandler connectGameServerHandler)
        {
            if (_gameObject.GetComponent<Logic.Protocol.ProtocolProxy>() == null)
                _gameObject.AddComponent<Logic.Protocol.ProtocolProxy>();
            Logic.Protocol.ProtocolProxy.instance.IsLoginServer = false;
            Logic.UI.Mask.Contorller.MaskController.instance.ShowMask();
            Logic.Protocol.ProtocolProxy.instance.Connect(host, port);
            Logic.Protocol.ProtocolProxy.instance.connectedHandler += ConnectGameServerComplete;
            _connectGameServerHandler = connectGameServerHandler;
            return true;
        }

        private void ConnectGameServerComplete(bool connected)
        {
            Logic.UI.Mask.Contorller.MaskController.instance.HideMask();
            if (!connected)
            {
                Debugger.Log("连接失败");
                Logic.Protocol.ProtocolProxy.instance.connectedHandler -= ConnectGameServerComplete;
                _connectGameServerHandler = null;
                Logic.Protocol.ProtocolProxy protocolProxy = _gameObject.GetComponent<Logic.Protocol.ProtocolProxy>();
                protocolProxy.enabled = false;
                UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.common.tips.relogin"), ReLoadMainScene, EUISortingLayer.TopTips);
                //ReLoadMainScene();
            }
            else
            {
                Debugger.Log("连接成功");
                if (_connectGameServerHandler != null)
                    _connectGameServerHandler();
                _connectGameServerHandler = null;
            }
        }
        private void OtherPlaceLoginHandler()
        {
            ReLoadMainScene();
        }
        #region C2S request
        public void CLIENT2LOBBY_SYNC_PVE_ACTION_REQ()
        {
            SynPveActionReq synPveActionReq = new SynPveActionReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(synPveActionReq);
        }
        public void CLIENT2LOBBY_SYNC_PVP_ACTION_REQ()
        {
            Logic.UI.Pvp.Controller.PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REQ();
        }
        public void CLIENT2LOBBY_BUY_PACK_CELL_REQ(int packType)
        {
            BuyPackCellReq buyPackCellReq = new BuyPackCellReq();
            buyPackCellReq.packType = packType;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(buyPackCellReq);
        }
        #endregion

        #region server callback

        private void PlatformLogoutSuccess()
        {
           // ReLoadMainScene();
        }
        private void PlatformLogoutSuccess_game()
        {
            ReLoadMainScene();
        }
        private bool LOBBY2CLIENT_UUSER_LOGOUT_RESP(Observers.Interfaces.INotification note)
        {
            UserLogoutResp resp = note.Body as UserLogoutResp;
            if (resp.result == 10006)//异地登陆
            {
                Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.error.otherPlaceLogin_10006"), OtherPlaceLoginHandler);
            }
            else
            {
                ReLoadMainScene();
            }

            return true;
        }

        private bool LOBBY2CLIENT(Observers.Interfaces.INotification note)
        {
            RoleLvAndExpResp roleLvAndExpResp = note.Body as RoleLvAndExpResp;
            GameProxy.instance.UpdateAccountLevelAndExp(roleLvAndExpResp.lv, roleLvAndExpResp.exp);
            return true;
        }

        private bool LOBBY2CLIENT_PACK_RESP_handler(Observers.Interfaces.INotification note)
        {
            //            PackResp packResp = note.Body as PackResp;
            //            GameProxy.instance.UpdateHeroCellNum(packResp.heroCellNum, packResp.heroCellBuyNum);
            //            GameProxy.instance.UpdateEquipcellNum(packResp.equipCellNum, packResp.equipCellBuyNum);
            string s = note.Body.ToString();
            int[] value = s.ToArray<int>(',');
            GameProxy.instance.UpdateHeroCellNum(value[0], value[1]);
            GameProxy.instance.UpdateEquipcellNum(value[2], value[3]);
            return true;
        }

        private bool LOBBY2CLIENT_BASE_RESOURCE_SYN_handler(Observers.Interfaces.INotification note)
        {
            // BaseResourceSyn baseResourceSyn = note.Body as BaseResourceSyn;
            //   GameProxy.instance.OnBaseResourcesUpdate(baseResourceSyn.resourceInfos);
            List<BaseResourceInfo> infoList = new List<BaseResourceInfo>();
            LuaTable gameModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "game_model")[0];
            LuaTable baseResTable = (LuaTable)gameModel["baseResourceTable"];
            foreach (DictionaryEntry kvp in baseResTable.ToDictTable())
            {
                BaseResourceInfo info = new BaseResourceInfo();
                info.type = kvp.Key.ToString().ToInt32();
                info.value = kvp.Value.ToString().ToInt32();
                infoList.Add(info);
            }
            GameProxy.instance.OnBaseResourcesUpdate(infoList);
            return true;
        }

        private bool LOBBY2CLIENT_ROLE_LV_AND_EXP_RESP(Observers.Interfaces.INotification note)
        {
            // RoleLvAndExpResp roleLvAndExpResp = note.Body as RoleLvAndExpResp;
            // GameProxy.instance.UpdateAccountLevelAndExp(roleLvAndExpResp.lv, roleLvAndExpResp.exp);
            string s = (string)note.Body;
            string[] data = s.Split(',');
            int lv = data[0].ToInt32();
            int exp = data[1].ToInt32();
            int oldLevel = GameProxy.instance.AccountLevel;
            GameProxy.instance.UpdateAccountLevelAndExp(lv, exp);
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAAccountSetLevel(lv);
            if (oldLevel != 0 && oldLevel < GameProxy.instance.AccountLevel)
            {
                SendExternalData(ExtraDataType.Levelup);
            }

            return true;
        }

        private bool LOBBY2CLIENT_SYB_PVE_ACTION_RESP_handler(Observers.Interfaces.INotification note)
        {
            SynPveActionResp synPveActionResp = note.Body as SynPveActionResp;
            GameProxy.instance.OnPveActionInfoUpdate(synPveActionResp);
            return true;
        }

        private bool LOBBY2CLIENT_BUY_PACK_CELL_RESP(Observers.Interfaces.INotification note)
        {
            //            BuyPackCellResp buyPackCellResp = note.Body as BuyPackCellResp;
            //            if (buyPackCellResp.packType == (int)BagType.HeroBag)
            //            {
            //                GameProxy.instance.UpdateHeroCellNum(buyPackCellResp.cellNum, GameProxy.instance.HeroCellBuyNum + 1);
            //            }
            //            else if (buyPackCellResp.packType == (int)BagType.EquipmentBag)
            //            {
            //                GameProxy.instance.UpdateEquipcellNum(buyPackCellResp.cellNum, GameProxy.instance.EquipCellBuyNum + 1);
            //            }
            string s = note.Body.ToString();
            int[] value = s.ToArray<int>(',');
            if (value[0] == (int)BagType.HeroBag)
            {
                GameProxy.instance.UpdateHeroCellNum(value[1], GameProxy.instance.HeroCellBuyNum + 1);
            }
            else if (value[0] == (int)BagType.EquipmentBag)
            {
                GameProxy.instance.UpdateEquipcellNum(value[1], GameProxy.instance.EquipCellBuyNum + 1);
            }
            return true;
        }

        private bool LOBBY2CLIENT_ERROR_RESP_handler(Observers.Interfaces.INotification note)
        {
            ErrorResp errorResp = note.Body as ErrorResp;
            string errorStr = Localization.Get(errorResp.errorCode.ToString());
            if (errorResp.errorCode == 70208)
            {
                Logic.UI.Tips.View.CommonErrorTipsView.Open(errorStr, SpecialErrorTipCallback);
            }
            else if (errorResp.errorCode == 71502)
            {
                Logic.UI.Tips.View.ConfirmTipsView.Open(Localization.Get("common.vip_level_not_enough_tips"), () => { FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond); });
            }
            else
            {
                Logic.UI.Tips.View.CommonErrorTipsView.Open(errorStr);
                if (errorResp.errorCode == 71302)
                {
                    var chat = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("uimanager.GetView", "chat_view")[0];
                    chat.GetLuaFunction("SetFriendNotOnline");
                }
            }
            return true;
        }
        private void SpecialErrorTipCallback()
        {
            ReLoadMainScene();
        }

        private bool LOBBY2CLIENT_ServerTimeSyn_RESP_handler(Observers.Interfaces.INotification note)
        {
            ServerTimeSynResp resp = note.Body as ServerTimeSynResp;
            TimeController.instance.ServerTimeTicksMillisecond = resp.time;
            return true;
        }
        #endregion server callback

        #region 属性
        private bool _pause = false;
        public bool pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
                Time.timeScale = value ? 0 : GameSetting.instance.speed;
            }
        }
        #endregion

        #region 登陆成功，加载常用资源并载入游戏
        private List<Logic.Character.CharacterEntity> _characters = new List<Logic.Character.CharacterEntity>();
        private int _loadTotal = 0;
        private int _loadedNum = 0;
        /*public void LoadGame()
        {
            UI.UIMgr.instance.Close(EUISortingLayer.MainUI);
            Logic.UI.LoadGame.Controller.LoadGameController.instance.AddLoadTips();
            Logic.UI.LoadGame.View.LoadGameView.Open(OnResourcesLoadComplete);
            //StartCoroutine("LoadCommonUseRes");
            #region create character pool
            List<uint> heroInstanceIDList = new List<uint>(ManageHeroesProxy.instance.CurrentFormationDictionary.Values);
            _loadTotal = heroInstanceIDList.Count;
            if (!Tutorial.Model.TutorialProxy.instance.SkipSimpleIntroduction) //the first fight
            {
                Logic.Fight.Controller.MockFightController.instance.InitFightData();
                Logic.Player.Model.PlayerInfo playerInfo = Logic.Fight.Model.FightProxy.instance.fightPlayerInfo.playerInfo;
                _loadTotal++;
                List<Logic.Fight.Model.FightHeroInfo> fightHeroInfos = Logic.Fight.Model.FightProxy.instance.fightHeroInfoList;
                _loadTotal += fightHeroInfos.Count;
                List<Logic.Fight.Model.FightHeroInfo> fightEnemyHeroInfos = Logic.Fight.Model.FightProxy.instance.enemyFightHeroInfoList;
                _loadTotal += fightEnemyHeroInfos.Count;
                Logic.Character.PlayerEntity.CreatePlayerEntity(playerInfo, transform, (playerEntity) =>
                {
                    if (playerEntity)
                        _characters.Add(playerEntity);
                    UpdateProgress(playerEntity);
                });
                for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                {
                    Logic.Hero.Model.HeroInfo heroInfo = fightHeroInfos[i].heroInfo;
                    Logic.Character.HeroEntity.CreateHeroEntity(heroInfo, (heroEntity) =>
                    {
                        if (heroEntity)
                            _characters.Add(heroEntity);
                        UpdateProgress(heroEntity);
                    });
                }
                for (int i = 0, count = fightEnemyHeroInfos.Count; i < count; i++)
                {
                    Logic.Hero.Model.HeroInfo heroInfo = fightEnemyHeroInfos[i].heroInfo;
                    Logic.Character.EnemyEntity.CreateEnemyEntity(heroInfo, (enemyEntity) =>
                    {
                        if (enemyEntity)
                            _characters.Add(enemyEntity);
                        UpdateProgress(enemyEntity);
                    });
                }
            }
            for (int i = 0, count = heroInstanceIDList.Count; i < count; i++)
            {
                if (GameProxy.instance.IsPlayer(heroInstanceIDList[i]))
                {
                    Logic.Player.Model.PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                    Logic.Character.PlayerEntity.CreatePlayerEntityAsUIElement(playerInfo, transform, true, false, (playerEntity) =>
                    {
                        if (playerEntity)
                        {
                            _characters.Add(playerEntity);
                            Pool.Controller.PoolController.instance.SetPoolForever(playerInfo.playerData.model, true);
                        }
                        UpdateProgress(playerEntity);
                    }, (petEntity) =>
                    {
                        if (petEntity)
                            Pool.Controller.PoolController.instance.SetPoolForever(petEntity.name, true);
                    });
                }
                else
                {
                    Logic.Hero.Model.HeroInfo heroInfo = Logic.Hero.Model.HeroProxy.instance.GetHeroInfo(heroInstanceIDList[i]);
                    Logic.Character.HeroEntity.CreateHeroEntityAsUIElement(heroInfo, transform, true, false, (heroEntity) =>
                    {
                        if (heroEntity)
                            _characters.Add(heroEntity);
                        UpdateProgress(heroEntity);
                    });
                }
            }
            #endregion
            Effect.Model.EffectData.GetEffectDatas();//preload effect data, because it is very large.            
        }*/

        public List<string> _preloadGameUIResourcesPaths = null;
        public List<string> PreloadGameUIResourcesPaths
        {
            get
            {
                if (_preloadGameUIResourcesPaths == null)
                {
                    _preloadGameUIResourcesPaths = new List<string>();
                    LuaScriptMgr.Instance.DoFile("user/preload_resources_paths");
                    LuaTable preloadResourcesPaths = LuaScriptMgr.Instance.GetLuaTable("preload_resources_paths");
                    foreach (DictionaryEntry kvp in preloadResourcesPaths.ToDictTable())
                    {
                        _preloadGameUIResourcesPaths.Add(kvp.Value.ToString());
                        //                        Debugger.Log(kvp.Value.ToString());
                    }
                }
                return _preloadGameUIResourcesPaths;
            }
        }

        private int _preloadGameResourcesCount = 0;
        private int _currentLoadedGameResourcesCount = 0;
        public void LoadAllGame()
        {
            _preloadGameResourcesCount = PreloadGameUIResourcesPaths.Count;

            //UI.UIMgr.instance.Close(EUISortingLayer.MainUI);
            Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.LoadGame.Controller.LoadGameController.instance.Switch2PreloadResTips();
            loadGameView.showRandomTips = false;
            loadGameView.SetCompleteHandler(OnResourcesLoadComplete);
            Logic.UI.LoadGame.Controller.LoadGameController.instance.AddLoadTips();
            StartCoroutine("LoadAllGameCoroutine");
        }

        private IEnumerator LoadAllGameCoroutine()
        {
            yield return null;
            #region create character pool
            List<uint> heroInstanceIDList = new List<uint>(ManageHeroesProxy.instance.CurrentFormationDictionary.Values);
            _preloadGameResourcesCount += heroInstanceIDList.Count;
            if (!Tutorial.Model.TutorialProxy.instance.SkipSimpleIntroduction) //the first fight
            {
                Logic.Fight.Controller.MockFightController.instance.InitFightData();
                Logic.Player.Model.PlayerInfo playerInfo = Logic.Fight.Model.FightProxy.instance.fightPlayerInfo.playerInfo;
                _preloadGameResourcesCount++;
                List<Logic.Fight.Model.FightHeroInfo> fightHeroInfos = Logic.Fight.Model.FightProxy.instance.fightHeroInfoList;
                _preloadGameResourcesCount += fightHeroInfos.Count;
                List<Logic.Fight.Model.FightHeroInfo> fightEnemyHeroInfos = Logic.Fight.Model.FightProxy.instance.enemyFightHeroInfoList;
                _preloadGameResourcesCount += fightEnemyHeroInfos.Count;
                Logic.Character.PlayerEntity.CreatePlayerEntity(playerInfo, transform, (playerEntity) =>
                {
                    if (playerEntity)
                        _characters.Add(playerEntity);
                    OnLoadAllGameUpdate(playerEntity);
                });
                for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                {
                    Logic.Hero.Model.HeroInfo heroInfo = fightHeroInfos[i].heroInfo;
                    Logic.Character.HeroEntity.CreateHeroEntity(heroInfo, (heroEntity) =>
                    {
                        if (heroEntity)
                            _characters.Add(heroEntity);
                        OnLoadAllGameUpdate(heroEntity);
                    });
                }
                for (int i = 0, count = fightEnemyHeroInfos.Count; i < count; i++)
                {
                    Logic.Hero.Model.HeroInfo heroInfo = fightEnemyHeroInfos[i].heroInfo;
                    Logic.Character.EnemyEntity.CreateEnemyEntity(heroInfo, (enemyEntity) =>
                    {
                        if (enemyEntity)
                            _characters.Add(enemyEntity);
                        OnLoadAllGameUpdate(enemyEntity);
                    });
                }
            }
            for (int i = 0, count = heroInstanceIDList.Count; i < count; i++)
            {
                if (GameProxy.instance.IsPlayer(heroInstanceIDList[i]))
                {
                    Logic.Player.Model.PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                    Logic.Character.PlayerEntity.CreatePlayerEntityAsUIElement(playerInfo, transform, true, false, (playerEntity) =>
                    {
                        if (playerEntity)
                        {
                            _characters.Add(playerEntity);
                            Pool.Controller.PoolController.instance.SetPoolForever(playerInfo.playerData.model, true);
                        }
                        OnLoadAllGameUpdate(playerEntity);
                    }, (petEntity) =>
                    {
                        if (petEntity)
                            Pool.Controller.PoolController.instance.SetPoolForever(petEntity.name, true);
                    });
                }
                else
                {
                    Logic.Hero.Model.HeroInfo heroInfo = Logic.Hero.Model.HeroProxy.instance.GetHeroInfo(heroInstanceIDList[i]);
                    Logic.Character.HeroEntity.CreateHeroEntityAsUIElement(heroInfo, transform, true, false, (heroEntity) =>
                    {
                        if (heroEntity)
                            _characters.Add(heroEntity);
                        OnLoadAllGameUpdate(heroEntity);
                    });
                }
            }
            #endregion
            Effect.Model.EffectData.GetEffectDatas();//preload effect data, because it is very large.

            for (int i = 0, count = PreloadGameUIResourcesPaths.Count; i < count; i++)
            {
                ResMgr.instance.Load<Sprite>(PreloadGameUIResourcesPaths[i], OnLoadAllGameUpdate, 0);
            }
            StartCoroutine("ZeroTickCoroutine");
        }

        private void OnLoadAllGameUpdate(Object obj)
        {
            _currentLoadedGameResourcesCount++;
            if (_currentLoadedGameResourcesCount == _preloadGameResourcesCount)
                DespawnCharacter();
            LoadGameView loadGameView = UIMgr.instance.Get<LoadGameView>(LoadGameView.PREFAB_PATH);
            loadGameView.UpdateLoadProgress((float)_currentLoadedGameResourcesCount / _preloadGameResourcesCount);
        }

        #region 退出战斗加载UI资源
        private int _preloadUIResourcesCount = 0;
        private int _currentLoadedUIResourcesCount = 0;
        public void LoadUIResources(System.Action onLoadUIResourcesCompleteDelegate)
        {
            Logic.UI.LoadGame.View.LoadGameView.Open(onLoadUIResourcesCompleteDelegate);
            _preloadUIResourcesCount = PreloadGameUIResourcesPaths.Count;
            _currentLoadedUIResourcesCount = 0;
            for (int i = 0, count = PreloadGameUIResourcesPaths.Count; i < count; i++)
            {
                ResMgr.instance.Load<Sprite>(PreloadGameUIResourcesPaths[i], OnLoadUIResourcesUpdate, 0);
            }
        }

        private void OnLoadUIResourcesUpdate(Object obj)
        {
            _currentLoadedUIResourcesCount++;
            LoadGameView loadGameView = UIMgr.instance.Get<LoadGameView>(LoadGameView.PREFAB_PATH);
            loadGameView.UpdateLoadProgress((float)_currentLoadedUIResourcesCount / _preloadUIResourcesCount);
        }
        #endregion 退出战斗加载UI资源

        private void UpdateProgress(UnityEngine.Object target)
        {
            _loadedNum++;
            if (_loadedNum == _loadTotal)
                DespawnCharacter();
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UIMgr.instance.Get<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            loadGameView.UpdateLoadProgress((float)_loadedNum / _loadTotal);
        }

        private void DespawnCharacter()
        {
            int count = _characters.Count;
            for (int index = 0; index < count; index++)
            {
                Logic.Character.CharacterEntity character = _characters[index];
                if (character)
                    Pool.Controller.PoolController.instance.Despawn(character.name, character);
            }
            _characters.Clear();
        }

        #region 00:00 invoke

        private IEnumerator ZeroTickCoroutine()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(10f);
            System.DateTime lastDateTime = TimeController.instance.ServerTime;
            while (true)
            {
                System.DateTime currentDateTime = TimeController.instance.ServerTime;
                if (lastDateTime.DayOfYear < currentDateTime.DayOfYear && currentDateTime.Hour == 0 && currentDateTime.Minute == 0)
                {
                    LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.ZeroInvoke");
                    lastDateTime = TimeController.instance.ServerTime;
                }
                yield return waitForSeconds;
            }
        }
        #endregion

        private void SimleIntroduction()
        {
            Logic.UI.Launch.View.SimpleIntroductionView simpleIntroductionView = Logic.UI.Launch.View.SimpleIntroductionView.Open();
            simpleIntroductionView.SetOnTimeOutDelegate(OnSimpleIntroductionComplete);
        }

        private void OnResourcesLoadComplete()
        {
            if (Tutorial.Model.TutorialProxy.instance.SkipSimpleIntroduction)
                OnSimpleIntroductionComplete();
            else
			{
//				SimleIntroduction();
				Logic.UI.Tutorial.View.ConfirmPlayTutorialView.Open(SimleIntroduction, GoTOMainView);
			}
        }

		private static void GoToMockFight ()
		{
			Logic.Fight.Controller.MockFightController.instance.StartFirstFight();
		}

		private static void GoTOMainView ()
		{
			UI.Main.View.MainView.Open();
			UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
		}

        private static void OnSimpleIntroductionComplete()
        {
            if (Tutorial.Model.TutorialProxy.instance != null
                && Tutorial.Model.TutorialProxy.instance.CurrentTutorialChapterData != null
                && Tutorial.Model.TutorialProxy.instance.CurrentTutorialChapterData.id == 1)
            {
                if (Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData.GetExpandData("isSkip") == "1")
                {
                    Tutorial.Controller.TutorialController.instance.SkipCurrentTutorialChapter();
                    UI.Main.View.MainView.Open();
                    UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
                }
                else
				{
                    Logic.Fight.Controller.MockFightController.instance.StartFirstFight();
				}
            }
            else
            {
                UI.Main.View.MainView.Open();
                UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            }
        }
        #endregion 登陆成功，加载常用资源并载入游戏

        private void OnChargeSuccessHandler(string orderId)
        {
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAVirtualCurrencyOnChargeSuccess(orderId);
        }

        void OnApplicationPause(bool pause)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (pause)
            {
                if (!GameDataCenter.instance.isRegisterLocalNotificator)
                {
                    CleanLocalNotificator();
                    if (GameSetting.instance.pushMessage)
                        LocalNotificator();
                }
            }
            else
            {
                CleanLocalNotificator();
                GameDataCenter.instance.isRegisterLocalNotificator = false;
            }            
#endif
            if (pause)
            {
                Audio.Controller.AudioController.instance.SetBGMusicState(false);
                TalkingDataController.instance.TalkingDataGAOnEnd();
            }
            else
            {
                Audio.Controller.AudioController.instance.SetBGMusicState(Audio.Controller.AudioController.instance.isOpenAudioBg);
                TalkingDataController.instance.TalkingDataGAOnStart(GameConfig.TALKING_DATA_APP_ID, GameConfig.CHANNEL_ID);
            }
        }

        void OnApplicationQuit()
        {
            if (Logic.UI.Login.Controller.LoginController.instance != null)
                Logic.UI.Login.Controller.LoginController.instance.Loginout();
            TalkingDataController.instance.TalkingDataGAOnEnd();
            Logic.Game.Controller.GameController.instance.SendExternalData(Logic.Enums.ExtraDataType.QuitGame);
#if UNITY_IOS && !UNITY_EDITOR
            if (!GameDataCenter.instance.isRegisterLocalNotificator)
            {
                CleanLocalNotificator();
                if (GameSetting.instance.pushMessage)
                    LocalNotificator();
            }
#endif
        }

        public void LocalNotificator()
        {
            List<PushMessageData> pushMessages = PushMessageData.GetPushMessageDatas();
            if (pushMessages == null) return;
            for (int i = 0, count = pushMessages.Count; i < count; i++)
            {
                PushMessageData pushMessageData = pushMessages[i];
                if (!pushMessageData.register) continue;
#if UNITY_IOS && !UNITY_EDITOR            
                Common.Notificator.IOSLocalNotificator.NotificationMessage(Localization.Get(pushMessageData.messageInfo), pushMessageData.time.Hour, pushMessageData.time.Minute, pushMessageData.time.Second, true);            
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
                System.DateTime current = System.DateTime.Now;
                int year = current.Year;
                int month = current.Month;
                int day = current.Day;
                System.DateTime newDate = new System.DateTime(year, month, day, pushMessageData.time.Hour, pushMessageData.time.Minute, pushMessageData.time.Second);
                int repeatDays = 7;
                string title = Localization.Get(pushMessageData.title);
                string content = Localization.Get(pushMessageData.messageInfo);
                for (int j = 0; j < repeatDays; j++)
                {
                    if (newDate > current)
                    {
                        string date = newDate.Year.ToString() + (newDate.Month >= 10 ? "" : "0") + newDate.Month.ToString() + (newDate.Day >= 10 ? "" : "0") + newDate.Day.ToString();
                        PlatformProxy.instance.AddLocalNotification(title, content, date, newDate.Hour.ToString(), newDate.Minute.ToString());
                    }
                    newDate = newDate.AddDays(1);
                }
#endif
            }
#if UNITY_IOS && !UNITY_EDITOR
            GameDataCenter.instance.isRegisterLocalNotificator = true;
#endif
        }

        public void CleanLocalNotificator()
        {
#if UNITY_IOS && !UNITY_EDITOR
            Common.Notificator.IOSLocalNotificator.CleanNotification();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            ////AndroidLocalNotification.ClearNotifications();
            //List<PushMessageData> pushMessages = PushMessageData.GetPushMessageDatas();
            //for (int i = 0, count = pushMessages.Count; i < count; i++)
            //{
            //    PushMessageData pushMessageData = pushMessages[i];
            //    AndroidLocalNotification.CancelNotification(pushMessageData.id);
            //}
            PlatformProxy.instance.ClearNotifications();
#endif
        }


        private string _createTime = string.Empty;
        private List<ExtraDataType> extraDataTypeList = new List<ExtraDataType>();
        private ExtraDataType _extraDataType = ExtraDataType.None;
        public void SendExternalData(ExtraDataType extraDataType)
        {
#if UNITY_ANDROID
            //if (UI.Login.Model.LoginProxy.instance.cachedPlatformId == (int)PlatformType.uc)
            //{
            _extraDataType = extraDataType;
            if (string.IsNullOrEmpty(_createTime))
            {
                extraDataTypeList.Add(extraDataType);
                if (Protocol.ProtocolProxy.instance != null)
                    Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.GetCreateTimeReq, new GetCreateTimeReq());
            }
            else
            {
                SendExternalData(_createTime, _extraDataType);
            }
               
            //}
#endif
        }

        private bool LOBBY2CLIENT_GetCreateTimeResp_RESP_handler(Observers.Interfaces.INotification note)
        {
            StringProto resp = note.Body as StringProto;
            _createTime = resp.value;
            for (int i = 0; i < extraDataTypeList.Count; i++)
            {
                SendExternalData(_createTime, extraDataTypeList[i]);
            }
            extraDataTypeList.Clear();
            return true;
        }

        private void SendExternalData(string creatTime, ExtraDataType extraDataType)
        {
            JsonData data = new JsonData();
            data["roleId"] = GameProxy.instance.AccountId;
            data["roleName"] = GameProxy.instance.AccountName;
            data["roleLevel"] = GameProxy.instance.AccountLevel;
            data["serverId"] = ServerListProxy.instance.lastServerId;
            data["serverName"] = Localization.Get(ServerListProxy.instance.ServerListDictionary[ServerListProxy.instance.lastServerId].name);
            data["createTime"] = creatTime;
            data["diamond"] = GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Diamond);
            data["levelUpTime"] = TimeController.instance.ServerTimeTicksSecond;
            data["dataType"] = (int)extraDataType;
            PlatformProxy.instance.SendExternalData(data.ToJson());
        }
       
    }
}
