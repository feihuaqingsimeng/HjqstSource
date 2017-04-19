using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using LuaInterface;
using Logic.Fight.Model;
using Logic.Player.Model;
using Logic.Protocol.Model;
using Logic.Hero.Model;
using Logic.Character;
using Logic.Character.Controller;
namespace Logic.Fight.Controller
{
    public class MockFightController : SingletonMono<MockFightController>
    {
        public List<FirstFightCharacterData> heros = new List<FirstFightCharacterData>();
        public List<FirstFightCharacterData> enemies = new List<FirstFightCharacterData>();
        private string _mapName;
        public string mapName
        {
            get
            {
                return _mapName;
            }
        }

        public bool isSkip { get; set; }

        private bool _needCombpPause = false;
        public bool needComboPause
        {
            get
            {
                return _needCombpPause;
            }
            set
            {
                _needCombpPause = value;
            }

        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            LuaScriptMgr.Instance.DoFile("user/first_fight");
            InitData();
        }

        private void InitData()
        {
            _mapName = LuaScriptMgr.Instance.GetTableValue<string>("first_fightTable", "map");

            LuaTable herosLT = LuaScriptMgr.Instance.GetLuaTable("first_fightTable.heros");
            LuaTable enemiesLT = LuaScriptMgr.Instance.GetLuaTable("first_fightTable.enemies");
            foreach (LuaTable lt in herosLT.ToArray())
            {
                FirstFightCharacterData ffcd = new FirstFightCharacterData();
                ffcd.id = lt[1].ToString().ToInt32();
                ffcd.starLevel = lt[2].ToString().ToInt32();
                ffcd.formationPosition = (FormationPosition)lt[3].ToString().ToInt32();
                ffcd.hp = lt[4].ToString().ToUInt32();
                ffcd.cd1 = lt[5].ToString().ToFloat();
                ffcd.cd2 = lt[6].ToString().ToFloat();
                ffcd.isRole = lt[7].ToString() == "1";
                heros.Add(ffcd);
            }
            foreach (LuaTable lt in enemiesLT.ToArray())
            {
                FirstFightCharacterData ffcd = new FirstFightCharacterData();
                ffcd.id = lt[1].ToString().ToInt32();
                ffcd.starLevel = lt[2].ToString().ToInt32();
                ffcd.formationPosition = (FormationPosition)lt[3].ToString().ToInt32();
                ffcd.hp = lt[4].ToString().ToUInt32();
                ffcd.cd1 = lt[5].ToString().ToFloat();
                ffcd.cd2 = lt[6].ToString().ToFloat();
                ffcd.isRole = lt[7].ToString() == "1";
                enemies.Add(ffcd);
            }
        }

        public void InitFightData()
        {
            List<FightHeroInfo> fightHeroInfoList = new List<FightHeroInfo>();
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                FirstFightCharacterData ffcd = heros[i];
                if (ffcd.isRole)
                {
                    PlayerInfo playerInfo = new PlayerInfo((uint)ffcd.id);
                    playerInfo.advanceLevel = ffcd.starLevel;
                    PlayerFightProtoData pfpd = new PlayerFightProtoData();
                    pfpd.posIndex = (int)ffcd.formationPosition;
                    pfpd.attr = new HeroAttrProtoData();
                    pfpd.attr.hp = (int)ffcd.hp;
                    pfpd.attr.hpUp = (int)ffcd.hp;
                    FightPlayerInfo fightPlayerInfo = new FightPlayerInfo(playerInfo, pfpd);
                    FightProxy.instance.SetFightPlayerInfo(fightPlayerInfo);
                }
                else
                {
                    HeroInfo heroInfo = new HeroInfo(ffcd.id);
                    heroInfo.advanceLevel = ffcd.starLevel;
                    HeroFightProtoData hfpd = new HeroFightProtoData();
                    hfpd.posIndex = (int)ffcd.formationPosition;
                    hfpd.attr = new HeroAttrProtoData();
                    hfpd.attr.hp = (int)ffcd.hp;
                    hfpd.attr.hpUp = (int)ffcd.hp;
                    FightHeroInfo fightHeroInfo = new FightHeroInfo(heroInfo, hfpd);
                    fightHeroInfoList.Add(fightHeroInfo);
                }
            }
            FightProxy.instance.SetFightHeroInfoList(fightHeroInfoList);
            List<FightHeroInfo> enemyFightHeroInfoList = new List<FightHeroInfo>();
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                FirstFightCharacterData ffcd = enemies[i];
                HeroInfo heroInfo = new HeroInfo(ffcd.id);
                heroInfo.advanceLevel = ffcd.starLevel;
                HeroFightProtoData hfpd = new HeroFightProtoData();
                hfpd.posIndex = (int)ffcd.formationPosition;
                hfpd.attr = new HeroAttrProtoData();
                hfpd.attr.hp = (int)ffcd.hp;
                hfpd.attr.hpUp = (int)ffcd.hp;
                FightHeroInfo fightHeroInfo = new FightHeroInfo(heroInfo, hfpd);
                enemyFightHeroInfoList.Add(fightHeroInfo);
            }
            FightProxy.instance.SetEnemyFightHeroInfoList(enemyFightHeroInfoList);
        }

        public void StartFirstFight()
        {
            Observers.Facade.Instance.RegisterObserver(Tutorial.Controller.TutorialController.TUTORIAL_STEP_COMPLETE_MSG, TUTORIAL_STEP_COMPLETE_MSG_HANDLER);
            //Observers.Facade.Instance.RegisterObserver("Fight::ForceFightFinishedHandler", ForceFightFinishedHandler);
            Tutorial.Model.TutorialProxy.instance.onSkipCurrentChapterDelegate += SkipFirstFight;
            Logic.Fight.Controller.FightController.instance.fightType = FightType.FirstFight;
            UI.UIMgr.instance.Close(Logic.UI.EUISortingLayer.MainUI);
            Logic.Fight.Controller.FightController.instance.PreReadyFight();
        }

        private void SkipFirstFight(int id)
        {
            Tutorial.Model.TutorialProxy.instance.onSkipCurrentChapterDelegate -= SkipFirstFight;
            //Observers.Facade.Instance.RemoveObserver("Fight::ForceFightFinishedHandler", ForceFightFinishedHandler);
            isSkip = true;
            Logic.Net.Controller.DataMessageHandler.DataMessage_ForceFightFinished(false, FightOverType.ForceOver);
        }

        public void MoveNextSkillHandler(Logic.Skill.Model.SkillInfo skillInfo)
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            if (tutorialStepData.GetExpandData("MoveNextSkilled") == "1")
                Tutorial.Controller.TutorialController.instance.ExecuteStepComplete();
        }

        private bool TUTORIAL_STEP_COMPLETE_MSG_HANDLER(Observers.Interfaces.INotification arg)
        {
            PlayNextSkill();
            return true;
        }

        public void PlayNextSkill()
        {
            StartCoroutine("PlayerNextSkillCoroutine");
        }

        private IEnumerator PlayerNextSkillCoroutine()
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            string transformStr = tutorialStepData.GetExpandData("Transform");
            string mechanicsStr = tutorialStepData.GetExpandData("Mechanics");
            string fightOver = tutorialStepData.GetExpandData("FightOver");
            float delay = tutorialStepData.delayTime;
            if (!string.IsNullOrEmpty(fightOver))
            {
                Logic.Net.Controller.DataMessageHandler.DataMessage_ForceFightFinished(false, FightOverType.ForceOver);
                Logic.Tutorial.Controller.TutorialController.instance.ExecuteStepComplete(tutorialStepData);
                yield break;
            }
            if (!string.IsNullOrEmpty(transformStr))
            {
                int transformId = 0;
                int.TryParse(transformStr, out transformId);
                if (transformId != 0)
                {
                    int mechanicsId = 0;
                    int.TryParse(mechanicsStr, out mechanicsId);
                    EnemyEntity enemy = EnemyController.instance[(uint)tutorialStepData.heroInstanceID];
                    if (enemy)
                        MechanicsController.instance.Transform(enemy, transformId, mechanicsId);
                    if (delay > 0)
                        yield return new WaitForSeconds(delay);
                }
                Tutorial.Controller.TutorialController.instance.ExecuteStepComplete();
            }
            else if (tutorialStepData.GetExpandData("startCombo") == "1")
            {
                if (delay > 0)
                    yield return new WaitForSeconds(delay);
                Common.GameTime.Controller.TimeController.instance.CancelPause();
                Logic.UI.FightTips.Controller.FightTipsController.instance.ComboWating(2, FightStatus.FloatWaiting, true);
            }
            else
            {
                uint characterId = (uint)tutorialStepData.heroInstanceID;
                if (characterId != 0)
                {
                    if (delay > 0)
                        yield return new WaitForSeconds(delay);
                    bool comboWait = tutorialStepData.GetExpandData("comboWait") == "0";
                    MockFightController.instance.needComboPause = !comboWait;
                    uint skillId = (uint)tutorialStepData.skillID;
                    Logic.Net.Controller.DataMessageHandler.DataMessage_OrderSkill(characterId, skillId, false);
                }
            }
        }

        public void ComboPause()
        {
            StartCoroutine("ComboPauseCoroutine");
        }

        private IEnumerator ComboPauseCoroutine()
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            float delay = tutorialStepData.delayTime;
            float time = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - time < delay)
                yield return null;
            Common.GameTime.Controller.TimeController.instance.Pause();
            Tutorial.Controller.TutorialController.instance.ExecuteStepComplete();
        }

        public FirstFightCharacterData GetCharacterDataById(int id)
        {
            FirstFightCharacterData result = default(FirstFightCharacterData);
            bool isFind = false;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                if (heros[i].id == id)
                {
                    result = heros[i];
                    isFind = true;
                    break;
                }
            }
            if (isFind)
                return result;
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                if (enemies[i].id == id)
                {
                    result = enemies[i];
                    break;
                }
            }
            return result;
        }

        public bool StopFightTime(Observers.Interfaces.INotification arg)
        {
            Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(true);
            return true;
        }

        public bool ReStartFightTime(Observers.Interfaces.INotification arg)
        {
            Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(false);
            return true;
        }
        public bool FightHangup(Observers.Interfaces.INotification arg)
        {
            StartCoroutine("FightHangupCoroutine");
            return true;
        }

        private IEnumerator FightHangupCoroutine()
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            float delay = tutorialStepData.delayTime;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            FightController.instance.FightHangupOrder(null);
        }

        public bool FightRegainOrder(Observers.Interfaces.INotification arg)
        {
            FightController.instance.FightRegainOrder();
            return true;
        }
        #region 1-1 chapter
        public bool FullSkillCD(Observers.Interfaces.INotification arg)
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            uint skillId = (uint)tutorialStepData.skillID;
            if (skillId != 0)
            {
                for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                {
                    HeroEntity hero = PlayerController.instance.heros[j];
                    if (hero.characterInfo.skillId1 == skillId)
                    {
                        hero.skill1CD = hero.characterInfo.skillInfo1.skillData.CD;
                        hero.skillItemBoxView.skillItemView1.SetSkillState(hero.skill1CD);
                        hero.canOrderTime = Time.time;
                        break;
                    }
                    if (hero.characterInfo.skillId2 == skillId)
                    {
                        hero.skill2CD = hero.characterInfo.skillInfo2.skillData.CD;
                        hero.skillItemBoxView.skillItemView2.SetSkillState(hero.skill2CD);
                        hero.canOrderTime = Time.time;
                        break;
                    }
                }
                for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                {
                    EnemyEntity enemy = EnemyController.instance.enemies[j];
                    if (enemy.characterInfo.skillId1 == skillId)
                    {
                        enemy.skill1CD = enemy.characterInfo.skillInfo1.skillData.CD;
                        enemy.canOrderTime = Time.time;
                        break;
                    }
                    if (enemy.characterInfo.skillId2 == skillId)
                    {
                        enemy.skill2CD = enemy.characterInfo.skillInfo2.skillData.CD;
                        enemy.canOrderTime = Time.time;
                        break;
                    }
                }
            }
            Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnFullSkillCDHandler"));
            return true;
        }
        #endregion

        #region 1-7chapter
        public bool SetNeedComboPause(Observers.Interfaces.INotification arg)
        {
            MockFightController.instance.needComboPause = true;
            FightController.instance.comboWaitSettingHandler += OnComboWaitSettingHandler;
            Observers.Facade.Instance.RegisterObserver(string.Format("Fight::{0}", "OnComboWaitingHandler"), TUTORIAL_STEP_COMPLETE_MSG_HANDLER);
            return true;
        }

        public bool ReSetNeedComboPause(Observers.Interfaces.INotification arg)
        {
            MockFightController.instance.needComboPause = false;
            FightController.instance.comboWaitSettingHandler -= OnComboWaitSettingHandler;
            Observers.Facade.Instance.RemoveObserver(string.Format("Fight::{0}", "OnComboWaitingHandler"), TUTORIAL_STEP_COMPLETE_MSG_HANDLER);
            return true;
        }

        private void OnComboWaitSettingHandler()
        {
            StartCoroutine("OnComboWaitSettingHanlderCoroutine");
        }

        private IEnumerator OnComboWaitSettingHanlderCoroutine()
        {
            Logic.Tutorial.Model.TutorialStepData tutorialStepData = Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData;
            float delay = tutorialStepData.delayTime;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnComboWaitSettingHandler"));
        }
        #endregion

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
                Observers.Facade.Instance.RemoveObserver(Tutorial.Controller.TutorialController.TUTORIAL_STEP_COMPLETE_MSG, TUTORIAL_STEP_COMPLETE_MSG_HANDLER);
        }
    }

    public struct FirstFightCharacterData
    {
        public int id;
        public int starLevel;
        public FormationPosition formationPosition;
        public uint hp;
        public float cd1;
        public float cd2;
        public bool isRole;//是否主角

        public override string ToString()
        {
            string result = string.Format("id:{0},starLevel:{1},formationPosition:{2},hp:{3},cd1:{4},cd1:{5},isRole:{6}", id, starLevel, formationPosition, hp, cd1, cd2, isRole);
            return result;
        }
    }
}
