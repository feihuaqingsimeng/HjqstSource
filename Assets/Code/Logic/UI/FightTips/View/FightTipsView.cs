using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Util;
using Common.ResMgr;
using Logic.Net.Controller;
using Logic.Game.Controller;
using Logic.Game;
using Logic.UI.FightTips.Controller;
using Logic.Enums;
using Logic.VIP.Model;
using Common.Localization;
using Common.GameTime.Controller;
using Logic.Character.Controller;


namespace Logic.UI.FightTips.View
{
    public class FightTipsView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/fight_tips/fight_tips_view";
        #region ui
        public GameObject core;
        public Image pauseImage;
        public Image autoFightImage;
        public Image fightDoubleSpeedImage;
        public Image fightTripleSpeedImage;
        public Image moveImage;
        public Image endImage;
        public Image[] splitImages;
        public Text timeText;
        public GameObject comboSliderGO;
        public Image comboSlider;
        #endregion
        private int _teamCount, _currentTeamIndex = 0;
        private float _moveTime = 1.5f;
        private const float TIPS_TIME = 15f;
        private bool _autoFight = false;
        private bool _lastAutoFight = false;
        private bool autoFight
        {
            set
            {
                _autoFight = value;
                Fight.Controller.FightController.instance.autoFight = value;
            }
            get
            {
                return _autoFight;
            }
        }
        public GameSpeedMode gameSpeedMode = GameSpeedMode.Normal;
        private float _lastTickTime;
        private bool _tick;
        private bool tick
        {
            get
            {
                return _tick;
            }
            set
            {
                _tick = value;
                if (value)
                {
                    _lastTickTime = Time.realtimeSinceStartup;
                    StartCoroutine("TimeTickCoroutine");
                }
                else
                    StopCoroutine("TimeTickCoroutine");
            }
        }

        private float _maxTime = 0;

        public float maxTime
        {
            set
            {
                _maxTime = value;
            }
            get
            {
                return _maxTime;
            }
        }

        private float _costTime;
        public float costTime
        {
            get
            {
                return _costTime;
            }
            set
            {
                _costTime = value;
                int result = (int)(maxTime - _costTime);
                if (result >= 0)
                {
                    timeText.text = TimeUtil.FormatSecondToMinute((int)_costTime);
                    if (result <= TIPS_TIME && _costTime > 0)
                        timeText.color = Color.red;
                }
                else
                {
                    if (FightTipsController.instance.fightOverHandler != null)
                        FightTipsController.instance.fightOverHandler();
                    tick = false;
                }
            }
        }

        private float _lastTime;
        public float lastTime
        {
            get
            {
                return _lastTime;
            }
            set
            {
                _lastTime = value;
                int result = (int)(_lastTime);
                if (result >= 0)
                {
                    timeText.text = TimeUtil.FormatSecondToMinute(result);
                    if (result <= TIPS_TIME)
                        timeText.color = Color.red;
                }
                else
                {
                    if (FightTipsController.instance.fightOverHandler != null)
                        FightTipsController.instance.fightOverHandler();
                    tick = false;
                }
                _costTime = maxTime - _lastTime;
            }
        }

        private int teamCount
        {
            get
            {
                return _teamCount;
            }
            set
            {
                _teamCount = value;
                switch (_teamCount)
                {
                    case 1:
                        for (int i = 0, count = splitImages.Length; i < count; i++)
                            splitImages[i].gameObject.SetActive(false);
                        break;
                    case 2:
                        splitImages[0].gameObject.SetActive(false);
                        splitImages[2].gameObject.SetActive(false);
                        break;
                    case 3:
                        splitImages[1].gameObject.SetActive(false);
                        break;
                }
            }
        }

        public bool pause { get; set; }

        void OnDestroy()
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.SkillDisplay:
                case FightType.ConsortiaFight:
                case FightType.FirstFight:
                    break;
                case FightType.Arena:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                    Fight.Controller.FightController.instance.autoFight = _lastAutoFight;
                    break;
            }
        }

        void Start()
        {
            pause = false;
            comboSliderGO.gameObject.SetActive(false);
            _autoFight = Fight.Controller.FightController.instance.autoFight;
            _lastAutoFight = _autoFight;
            gameSpeedMode = Game.GameSetting.instance.lastFightSpeedMode;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case Enums.FightType.PVE:
                    costTime = 0f;
                    lastTime = maxTime;
                    teamCount = Logic.Fight.Model.FightProxy.instance.CurrentTeamCount;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    break;
                case Enums.FightType.DailyPVE:
                    lastTime = maxTime;
                    teamCount = Logic.Fight.Model.FightProxy.instance.CurrentTeamCount;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    break;
                case Enums.FightType.WorldTree:
                    costTime = 0f;
                    lastTime = maxTime;
                    teamCount = Logic.Fight.Model.FightProxy.instance.CurrentTeamCount;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    break;
                case Enums.FightType.Arena:
                    costTime = 0f;
                    teamCount = 1;
                    lastTime = maxTime;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    autoFight = true;
                    autoFightImage.GetComponent<Button>().interactable = false;
                    break;
                case Enums.FightType.Expedition:
                    costTime = 0f;
                    teamCount = 1;
                    lastTime = maxTime;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    break;
                case FightType.PVP:
                    costTime = 0f;
                    teamCount = 1;
                    lastTime = maxTime;
                    pauseImage.gameObject.SetActive(false);
                    fightDoubleSpeedImage.gameObject.SetActive(false);
                    fightTripleSpeedImage.gameObject.SetActive(false);
                    break;
                case FightType.FriendFight:
                    costTime = 0f;
                    teamCount = 1;
                    lastTime = maxTime;
                    autoFight = true;
                    autoFightImage.GetComponent<Button>().interactable = false;
                    break;
                case Enums.FightType.WorldBoss:
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    costTime = 0f;
                    break;
                case FightType.FirstFight:
                    pauseImage.gameObject.SetActive(false);
                    fightDoubleSpeedImage.gameObject.SetActive(false);
                    fightTripleSpeedImage.gameObject.SetActive(false);
                    autoFightImage.gameObject.SetActive(false);
                    Game.GameSetting.instance.speedMode = Game.GameSpeedMode.Normal;
                    costTime = 0f;
                    break;
                case FightType.ConsortiaFight:
                    fightDoubleSpeedImage.gameObject.SetActive(false);
                    fightTripleSpeedImage.gameObject.SetActive(false);
                    autoFightImage.gameObject.SetActive(false);
                    costTime = 0f;
                    break;
                case FightType.MineFight:
                    costTime = 0f;
                    teamCount = 1;
                    lastTime = maxTime;
                    autoFight = true;
                    autoFightImage.GetComponent<Button>().interactable = false;
                    Game.GameSetting.instance.speedMode = gameSpeedMode;
                    break;
                case FightType.SkillDisplay:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    Game.GameSetting.instance.speedMode = Game.GameSpeedMode.Normal;
                    break;
            }
            UpdateAutoFightImage();
            UpdateFightSpeedImage();

            Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
        }

        public void ComboWating(float comboWaitTime, FightStatus fightStatus, bool isPlayer)
        {
            if (isPlayer)
            {
                comboSliderGO.gameObject.SetActive(true);
                comboSlider.fillAmount = 1f;
            }
            StartCoroutine(ComboWatingCoroutine(comboWaitTime, fightStatus, isPlayer));
        }

        private IEnumerator ComboWatingCoroutine(float comboWaitTime, FightStatus fightStatus, bool isPlayer)
        {
            float time = Time.realtimeSinceStartup;
            float currentTime = time;
            float passTime = Time.realtimeSinceStartup - time;
            float halfComboWaitTime = comboWaitTime / 2f;
            if (isPlayer)
            {
                while (passTime < comboWaitTime)
                {
                    if (passTime >= halfComboWaitTime && fightStatus == FightStatus.FloatWaiting)
                    {
                        if (!PlayerController.instance.HasCanOrderFloatHeros())
                            break;
                    }
                    if (passTime >= halfComboWaitTime && fightStatus == FightStatus.TumbleWaiting)
                    {
                        if (!PlayerController.instance.HasCanOrderTumbleHeros())
                            break;
                    }
                    float progress = passTime / comboWaitTime;
                    comboSlider.fillAmount = 1 - progress;
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                    passTime = Time.realtimeSinceStartup - time;
                }
            }
            else
            {
                while (passTime < comboWaitTime)
                {
                    if (passTime >= halfComboWaitTime && fightStatus == FightStatus.FloatWaiting)
                    {
                        if (!EnemyController.instance.HasCanOrderFloatEnemies())
                            break;
                    }
                    if (passTime >= halfComboWaitTime && fightStatus == FightStatus.TumbleWaiting)
                    {
                        if (!EnemyController.instance.HasCanOrderTumbleEnemies())
                            break;
                    }
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                    passTime = Time.realtimeSinceStartup - time;
                }
            }
            if (FightTipsController.instance.comboWaitingOverHandler != null)
                FightTipsController.instance.comboWaitingOverHandler(fightStatus);
            if (isPlayer)
                comboSliderGO.gameObject.SetActive(false);
        }

        public void FightStart()
        {
            tick = true;
        }

        public void FightOver()
        {
            tick = false;
        }

        private IEnumerator TimeTickCoroutine()
        {
            float delay = 0f;
            while (true)
            {
                if (tick)
                {
                    delay = Time.realtimeSinceStartup - _lastTickTime;
                    delay = delay * Game.GameSetting.instance.speed;
                    if (delay > 0.4f)
                    {
                        if (!TimeController.instance.playerPause && !pause)
                        {
                            switch (Fight.Controller.FightController.instance.fightType)
                            {
                                case Enums.FightType.PVE:
                                    //costTime += delay;
                                    lastTime -= delay;
                                    break;
                                case Enums.FightType.DailyPVE:
                                    lastTime -= delay;
                                    break;
                                case Enums.FightType.WorldTree:
                                    //costTime += delay;
                                    lastTime -= delay;
                                    break;
                                case Enums.FightType.Arena:
                                case Enums.FightType.Expedition:
                                case FightType.PVP:
                                case FightType.FriendFight:
                                    //costTime += delay;
                                    lastTime -= delay;
                                    break;
                                case Enums.FightType.WorldBoss:
                                    costTime += delay;
                                    break;
                                case FightType.FirstFight:
                                    costTime += delay;
                                    break;
                                case FightType.ConsortiaFight:
                                    costTime += delay;
                                    break;
                                case FightType.MineFight:
                                    lastTime -= delay;
                                    break;
                            }
                        }
                        _lastTickTime = Time.realtimeSinceStartup;
                    }
                }
                yield return null;
            }
        }

        public void NextTeam()
        {
            _currentTeamIndex++;
            if (_currentTeamIndex > _teamCount) return;
            switch (teamCount)
            {
                case 1:
                    LeanTween.moveLocalX(moveImage.gameObject, endImage.transform.localPosition.x, _moveTime);
                    break;
                case 2:
                    switch (_currentTeamIndex)
                    {
                        case 1:
                            LeanTween.moveLocalX(moveImage.gameObject, splitImages[1].transform.localPosition.x, _moveTime);
                            break;
                        case 2:
                            LeanTween.moveLocalX(moveImage.gameObject, endImage.transform.localPosition.x, _moveTime);
                            break;
                    }
                    break;
                case 3:
                    switch (_currentTeamIndex)
                    {
                        case 1:
                            LeanTween.moveLocalX(moveImage.gameObject, splitImages[0].transform.localPosition.x, _moveTime);
                            break;
                        case 2:
                            LeanTween.moveLocalX(moveImage.gameObject, splitImages[2].transform.localPosition.x, _moveTime);
                            break;
                        case 3:
                            LeanTween.moveLocalX(moveImage.gameObject, endImage.transform.localPosition.x, _moveTime);
                            break;
                    }
                    break;
            }
        }

        public void ResetAutoFight()
        {
            if (autoFight)
                ClickAutoFightBtnHandler();
        }

        private void UpdateAutoFightImage()
        {
            autoFightImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/" + (autoFight ? "btn_auto_selected" : "btn_auto_normal")));
        }

        private void UpdateFightSpeedImage()
        {
            switch (gameSpeedMode)
            {
                case GameSpeedMode.Normal:
                    fightDoubleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_double_normal"));
                    fightTripleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_three_normal"));
                    break;
                case GameSpeedMode.Double:
                    fightDoubleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_double_selected"));
                    fightTripleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_three_normal"));
                    break;
                case GameSpeedMode.Triple:
                    fightDoubleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_double_normal"));
                    fightTripleSpeedImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_three_selected"));
                    break;
            }
        }
        #region 事件

        public void ClickPauseBtnHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FightPause, true))
                return;
            if (Fight.Controller.FightController.instance.isComboing || Fight.Controller.FightController.instance.isWaitingCombo) return;
            bool pause = TimeController.instance.playerPause;
            if (!pause && GameController.instance.pause) return;//非玩家(技能暂停，连击等等)暂停忽略
            TimeController.instance.playerPause = !pause;
            UIMgr.instance.Open(FightPause.View.FightPauseView.PREFAB_PATH);
        }

        public void ClickAutoFightBtnHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.AutoFight, true))
                return;
            autoFight = !autoFight;
            Fight.Controller.FightController.instance.autoFight = autoFight;
            UpdateAutoFightImage();
        }

        public void ClickFightDoubleSpeedHandler()
        {
#if UNITY_EDITOR
            if (!Fight.Controller.FightController.imitate)
            {
#endif
                if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.DoubleSpeed, true))
                {
                    return;
                }
#if UNITY_EDITOR
            }
#endif
            //if (Fight.Controller.FightController.instance.fightStatus != FightStatus.Normal) return;
            switch (gameSpeedMode)
            {
                case GameSpeedMode.Normal:
                case GameSpeedMode.Triple:
                    gameSpeedMode = GameSpeedMode.Double;
                    break;
                case GameSpeedMode.Double:
                    gameSpeedMode = GameSpeedMode.Normal;
                    break;
            }
            GameSetting.instance.speedMode = gameSpeedMode;
            UpdateFightSpeedImage();
        }

        public void ClickFightTripleSpeedHandler()
        {
            VIPData currentVipData = VIPData.GetVIPData(VIPProxy.instance.VIPLevel);
            if (!currentVipData.three_speed)
            {
                Tips.View.CommonAutoDestroyTipsView.Open(LocalizationController.instance.Get("ui.fight_tips.view.vip_level_less"));
                return;
            }
            switch (gameSpeedMode)
            {
                case GameSpeedMode.Normal:
                case GameSpeedMode.Double:
                    gameSpeedMode = GameSpeedMode.Triple;
                    break;
                case GameSpeedMode.Triple:
                    gameSpeedMode = GameSpeedMode.Normal;
                    break;
            }
            GameSetting.instance.speedMode = gameSpeedMode;
            UpdateFightSpeedImage();
        }
        #endregion

    }
}