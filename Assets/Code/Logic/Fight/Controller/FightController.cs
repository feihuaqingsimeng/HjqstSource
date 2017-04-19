using Logic.UI.FirstFightEnd.View;
#if UNITY_EDITOR
using Logic.Hero.Model;
using Logic.Player.Model;
#endif
using UnityEngine;
using System.Collections;
using Logic.Net.Controller;
using Logic.Audio.Controller;
using Logic.Character;
using Logic.Character.Controller;
using Logic.Skill.Model;
using Logic.Enums;
using Logic.Action;
using System.Collections.Generic;
using Common.ResMgr;
using Logic.Position.Model;
using Logic.Game;
using Logic.Effect.Model;
using Common.Animators;
using Logic.Action.Controller;
using Logic.UI.FightTips.Controller;
using Common.Util;
using Common.GameTime.Controller;
using LuaInterface;

namespace Logic.Fight.Controller
{
    public class FightController : SingletonMono<FightController>
    {
        #region static field
        private const float DEAD_TIME = 3f;
        public const float RUN_SCENE_TIME = 3f;
        public const float FIGHT_TRANSITION_TIME = 1f;
        public const float WAIT_FIGHT_OVER_TIME = 3f;
        public const float COMMON_CD_TIME = 5f;
        #endregion

        public delegate void FightRegainHandler();
        public FightRegainHandler fightRegainHandler;

        public delegate void FinishedSkillHandler(SkillInfo skillInfo);
        public FinishedSkillHandler finishedSkillHandler;

        public delegate void FinishedSkillMechanicsHandler(SkillInfo skillInfo);
        public FinishedSkillMechanicsHandler finishedSkillMechanicsHandler;

        public delegate void FinishedComboSkillHandler(SkillInfo skillInfo);
        public FinishedComboSkillHandler finishedComboSkillHandler;

        public delegate void FinishedComboSkillMechanicsHandler(SkillInfo skillInfo);
        public FinishedComboSkillMechanicsHandler finishedComboSkillMechanicsHandler;

        public delegate void ComboWaitSettingHandler();
        public ComboWaitSettingHandler comboWaitSettingHandler;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //_autoFight = PlayerPrefs.GetInt("autoFight", 0) == 1;
        }

        #region fight status and value
        private FightType _fightType = FightType.PVE;
        public FightType fightType
        {
            get
            {
                return _fightType;
            }
            set
            {
                _fightType = value;
            }
        }

        private FightStatus _fightStatus = FightStatus.GameOver;
        public FightStatus fightStatus
        {
            get
            {
                return _fightStatus;
            }
            set
            {
                if (_fightStatus == value) return;
                _fightStatus = value;
                switch (value)
                {
                    case FightStatus.Normal:
                        break;
                    case FightStatus.FloatWaiting:
                    case FightStatus.TumbleWaiting:
                        if (_comboer == null)
                        {
                            FinishedCombo(false);
                            return;
                        }
                        _comboerIsPlayer = _comboer.isPlayer;
                        Vector3 targetPos = _comboTarget.pos;
                        ComboWaitSetting(targetPos, comboerIsPlayer);
                        if (MockFightController.instance.needComboPause)
                        {
                            MockFightController.instance.ComboPause();
                            if (comboWaitSettingHandler != null)
                                comboWaitSettingHandler();
                            return;
                        }
                        if (!FightTipsController.instance.ComboWating(COMBO_WAIT_TIME, value, _comboer.isPlayer))
                        {
                            FinishedCombo(comboerIsPlayer);
                        }
                        break;
                    case FightStatus.FloatComboing:
                    case FightStatus.TumbleComboing:
                        PlayComboSkill(comboerIsPlayer);
                        break;
                }
            }
        }

        private bool _isCloseup;
        public bool isCloseup
        {
            get { return _isCloseup; }
            set { _isCloseup = value; }
        }

        private bool _autoFight;
        public bool autoFight
        {
            get
            {
                return _autoFight;
            }
            set
            {
                _autoFight = value;
                VirtualServerController.instance.autoFight = value;
                PlayerPrefs.SetInt("autoFight", value ? 1 : 0);
            }
        }

        private bool _tickCD;
        public bool tickCD
        {
            get
            {
                return _tickCD;
            }
            set
            {
                _tickCD = value;
                PlayerController.instance.tickCD = value;
                EnemyController.instance.tickCD = value;
            }
        }

        private bool _isFight;
        private IEnumerator UpdateFightSkillTimeCoroutine()
        {
            float lastTime = TimeController.instance.fightSkillTime = Time.time;
            while (true)
            {
                if (!TimeController.instance.playerPause)
                {
                    if (tickCD)
                    {
                        float deltaTime = Time.time - lastTime;
                        TimeController.instance.fightSkillTime += deltaTime;
                    }
                }
                lastTime = Time.time;
                yield return null;
            }
        }

        private float _lastTickTime;
        public float lastTickTime
        {
            set
            {
                _lastTickTime = value;
                PlayerController.instance.lastTickTime = value;
                EnemyController.instance.lastTickTime = value;
            }
            get
            {
                return _lastTickTime;
            }
        }

        public int fightCostTime { get; set; }

        public int deadCount { get; set; }

        public int remainderHPAverageRate { get; set; }

        public int totalDamage { get; set; }

        public int comboCount { get; set; }

        private void ResetFightStatisticsData()
        {
            comboCount = 0;
            totalDamage = 0;
            deadCount = 0;
            fightCostTime = 0;
            remainderHPAverageRate = 0;
        }

#if UNITY_EDITOR
        [NoToLua]
        public void CalcFightStatisticsDataEditor()
        {
            CalcFightStatisticsData();
        }
#endif

        private void CalcFightStatisticsData()
        {
            comboCount = (int)Judge.Controller.JudgeController.instance.comboCount;
            totalDamage = (int)Judge.Controller.JudgeController.instance.totalDamage;
            deadCount = PlayerController.instance.deadHeroDic.Count;
            fightCostTime = (int)FightTipsController.instance.CostTime;

            float result = 0f;
            List<HeroEntity> lives = PlayerController.instance.heros;
            float totalRate = 0f;
            for (int i = 0, length = lives.Count; i < length; i++)
            {
                HeroEntity hero = lives[i];
                totalRate += (float)hero.HP / hero.maxHP;
            }
            int number = lives.Count + deadCount;
            if (number > 0)
                result = totalRate / number;
            remainderHPAverageRate = (int)(result * 100);
        }

        private const float INTERVAL = 5f;
        private int _lastDamage = 0;
        private IEnumerator CalcDamageFixedTime()
        {
            float _lastTime = Time.realtimeSinceStartup;
            _lastDamage = 0;
            while (true)
            {
                while (Time.realtimeSinceStartup - _lastTime < INTERVAL)
                    yield return null;
                if (fightStatus == FightStatus.GameOver)
                    yield break;
                //int currentTotalDamage = (int)Judge.Controller.JudgeController.instance.totalDamage;
                int damage = CalcLastDamage();
                WorldBoss.Controller.WorldBossController.instance.CLIENT2LOBBY_WorldBossHurtSynReq(damage);
                _lastTime = Time.realtimeSinceStartup;
            }
        }

        private int CalcLastDamage()
        {
            int currentTotalDamage = (int)Judge.Controller.JudgeController.instance.totalDamage;
            int damage = currentTotalDamage - _lastDamage;
            _lastDamage = currentTotalDamage;
            return damage;
        }
        #region imitate
#if UNITY_EDITOR
        [NoToLua]
        public static bool imitate = false;
        [NoToLua]
        public int currentIndex = -1;
        [NoToLua]
        public int fightCount = 1;
        [NoToLua]
        public bool isDoubleSpeed = false;
        [NoToLua]
        public List<Dictionary<string, uint>> hitCounts = new List<Dictionary<string, uint>>();
        [NoToLua]
        public List<Dictionary<string, uint>> skillCounts = new List<Dictionary<string, uint>>();
        [NoToLua]
        public List<int> fightCostTimes = new List<int>();
        [NoToLua]
        public List<bool> fightResults = new List<bool>();
        [NoToLua]
        public Dictionary<FormationPosition, HeroInfo> heroInfoDic = new Dictionary<FormationPosition, HeroInfo>();
        [NoToLua]
        public Dictionary<FormationPosition, PlayerInfo> playerInfoDic = new Dictionary<FormationPosition, PlayerInfo>();
        [NoToLua]
        public Dictionary<FormationPosition, HeroInfo> enemeyHeroInfoDic = new Dictionary<FormationPosition, HeroInfo>();
        [NoToLua]
        public List<uint> enemeyBosses = new List<uint>();
        [NoToLua]
        public Dictionary<FormationPosition, PlayerInfo> enemeyPlayerInfoDic = new Dictionary<FormationPosition, PlayerInfo>();
        [NoToLua]
        public int enemyWave = 1;
        [NoToLua]
        public void InitImitateData()
        {
            currentIndex = -1;
            hitCounts.Clear();
            skillCounts.Clear();
            heroInfoDic.Clear();
            playerInfoDic.Clear();
            enemeyHeroInfoDic.Clear();
            enemeyPlayerInfoDic.Clear();
            enemeyBosses.Clear();
            fightCostTimes.Clear();
            fightResults.Clear();
        }

        [NoToLua]
        public void ClaerImitateStatisticData()
        {
            currentIndex = -1;
            hitCounts.Clear();
            skillCounts.Clear();
            fightCostTimes.Clear();
            fightResults.Clear();
        }

        private IEnumerator StartImitateFightCoroutine()
        {
            yield return new WaitForSeconds(2f);
            FightController.instance.fightType = FightType.Imitate;
            FightController.instance.PreReadyFight();
        }

        [NoToLua]
        public bool isBossImitate(uint id)
        {
            return enemeyBosses.Contains(id);
        }

        [NoToLua]
        public bool hasBossImitate
        {
            get
            {
                return enemeyBosses.Count > 0;
            }
        }
#endif
        #endregion
        #endregion

        #region combo
        public float _offset = 5f;
        private Dictionary<uint, uint> _comboSkillDic = new Dictionary<uint, uint>();
        private CharacterEntity _comboTarget;
        private CharacterEntity _comboer;
        private bool _comboerIsPlayer;
        private int _comboTimes = 0;
        private const float COMBO_WAIT_TIME = 2f;
        private GameSpeedMode _originalGameSpeedMode;

        public CharacterEntity comboer { get { return _comboer; } }

        public bool isComboing
        {
            get
            {
                return _fightStatus == FightStatus.TumbleComboing || _fightStatus == FightStatus.FloatComboing;
            }
        }

        public bool isWaitingCombo
        {
            get
            {
                return _fightStatus == FightStatus.FloatWaiting || _fightStatus == FightStatus.TumbleWaiting;
            }
        }

        public bool comboerIsPlayer
        {
            get
            {
                return _comboerIsPlayer;
            }
        }

        public void SetComboCharacter(CharacterEntity comboer, CharacterEntity comboTarget)
        {
            _comboer = comboer;
            _comboTarget = comboTarget;
        }

        private void ComboWaitingOverHandler(FightStatus fs)
        {
            switch (fs)
            {
                case FightStatus.FloatWaiting:
                    fightStatus = FightStatus.FloatComboing;
                    break;
                case FightStatus.TumbleWaiting:
                    fightStatus = FightStatus.TumbleComboing;
                    break;
            }
        }

        private void ComboWaitSetting(Vector3 targetPos, bool isPlayer)
        {
            if (isPlayer)
            {
                Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                if (skillBarView)
                    skillBarView.DesaltOnCombo(COMBO_WAIT_TIME);
            }
            Map.Controller.MapController.instance.ShowMap(false, 0.5f / GameSetting.instance.speed);
            _originalGameSpeedMode = Game.GameSetting.instance.speedMode;
            Game.GameSetting.instance.speedMode = GameSpeedMode.ComboWaiting;
            Cameras.Controller.CameraController.instance.SetMainCameraCullMask(1 << (int)LayerType.FightCombo);
            ResetComboData();
            PlayerController.instance.ShowHPBarViews(false);
            EnemyController.instance.ShowHPBarViews(false);
            if (_comboer)
            {
                Effect.Controller.EffectController.instance.SwitchEffect(_comboer, (int)LayerType.FightCombo);
                Common.Util.TransformUtil.SwitchLayer(_comboer.transform, (int)LayerType.FightCombo);
            }
            if (_comboTarget)
            {
                Common.Util.TransformUtil.SwitchLayer(_comboTarget.transform, (int)LayerType.FightCombo);
                Vector3 offset = Vector3.zero;
                Vector3 centerPos = Vector3.zero;
                if (isPlayer)
                    centerPos = PositionData.GetPos((uint)FormationPosition.Enemy_Position_2);
                else
                    centerPos = PositionData.GetPos((uint)FormationPosition.Player_Position_2);
                offset.z = _comboTarget.pos.z - centerPos.z;
                if (fightStatus == FightStatus.TumbleWaiting)
                    offset.z += _offset;
                Cameras.Controller.CameraController.instance.MainCameraComboShowTime(targetPos, offset);
            }
        }

        private void PlayComboSkill(bool isPlayer)
        {
            Game.GameSetting.instance.speedMode = _originalGameSpeedMode;
            if (isPlayer)
            {
                Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                if (skillBarView != null)
                    skillBarView.ShowAfterWaitCombo();
            }
            Effect.Controller.EffectController.instance.SwitchEffectAfterWaitCombo();
            Cameras.Controller.CameraController.instance.ResetMainCameraPos();
            Common.Util.TransformUtil.SwitchLayer(_comboer.transform, (int)LayerType.Fight);
            if (_comboSkillDic.Count == 0)
            {
                Effect.Controller.EffectController.instance.SwitchEffect(_comboer, (int)LayerType.Fight);
                FinishedCombo(isPlayer);
                return;
            }
            Effect.Controller.EffectController.instance.RemoveEffects(_comboer, (int)LayerType.FightCombo);
            _comboer.moveBroken = true;
            _comboer.pos = PositionData.GetPos(_comboer.positionId);
            AnimatorUtil.Play(_comboer.anim, AnimatorUtil.IDLE_ID, 0, 0f);
            _comboTarget.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            _comboTarget.anim.speed = Game.GameSetting.instance.slowSpeed;
            Common.GameTime.Controller.TimeController.instance.Pause();
            if (isPlayer)
            {
                Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                if (skillBarView != null)
                    skillBarView.ShowMaskStartCombo();
            }
            StartCoroutine(PlayComboSkillCoroutine(isPlayer));
            StopCoroutine("ProtectComboSystemCoroutine");
            StartCoroutine("ProtectComboSystemCoroutine");
        }

        private IEnumerator PlayComboSkillCoroutine(bool isPlayer)
        {
            List<uint> ids = _comboSkillDic.GetKeys();
            float delay = 0f, time = Time.realtimeSinceStartup;
            for (int i = 0, count = ids.Count; i < count; i++)
            {
                uint id = ids[i];
                uint skillId = _comboSkillDic[id];
                CharacterEntity character = null;
                if (isPlayer)
                    character = PlayerController.instance.GetComboHero(id);
                else
                    character = EnemyController.instance.GetComboEnemy(id);
                if (!character) continue;
                character.anim.speed = Game.GameSetting.instance.speed;
                SkillInfo skillInfo = character.GetSkillInfoById(skillId);
                CharacterEntity lastCharacter = null;
                SkillInfo lastSkillInfo = null;
                if (i > 0)
                {
                    uint lastId = ids[i - 1];
                    uint lastSkillId = _comboSkillDic[lastId];
                    if (isPlayer)
                        lastCharacter = PlayerController.instance.GetComboHero(lastId);
                    else
                        lastCharacter = EnemyController.instance.GetComboEnemy(lastId);
                    float lastTimelineEndKey = 0f;
                    float timelineFirstKey = 0f;
                    if (lastCharacter)
                    {
                        lastSkillInfo = lastCharacter.GetSkillInfoById(lastSkillId);
                        lastTimelineEndKey = lastSkillInfo.skillData.timeline.Last().Key;
                        timelineFirstKey = skillInfo.skillData.timeline.First().Key;
                    }
                    switch (skillInfo.animationData.animType)
                    {
                        case AnimType.Root:
                            if (skillInfo.skillData.flyEffectIds.Count > 0)
                            {
                                float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, _comboTarget.positionId);
                                EffectData effectData = EffectData.GetEffectDataById(skillInfo.skillData.flyEffectIds.First());
                                if (effectData != null)
                                {
                                    float CostTime = effectData.moveTime * (positionRow * GameConfig.timePercent + 1f) / 2;
                                    lastTimelineEndKey += CostTime;
                                }
                            }
                            else if (skillInfo.skillData.aoeFlyEffects.Length > 0)
                            {
                                float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, _comboTarget.positionId);
                                EffectData effectData = EffectData.GetEffectDataById(skillInfo.skillData.flyEffectIds.First());
                                if (effectData != null)
                                {
                                    float CostTime = effectData.moveTime * (positionRow * GameConfig.timePercent + 1f) / 2;
                                    lastTimelineEndKey += CostTime;
                                }
                            }
                            break;
                        case AnimType.Trace:

                            break;
                        case AnimType.Run:
                            {
                                float CostTime = GetRunCostTime(character.pos, _comboTarget.pos, skillInfo);
                                lastTimelineEndKey += CostTime;
                            }
                            break;
                    }
                    if (lastSkillInfo != null)
                    {
                        switch (lastSkillInfo.animationData.animType)
                        {
                            case AnimType.Root:
                                if (lastSkillInfo.skillData.flyEffectIds.Count > 0)
                                {
                                    float positionRow = PositionData.GetEnemyPositionLevels(lastCharacter.positionId, _comboTarget.positionId);
                                    EffectData effectData = EffectData.GetEffectDataById(lastSkillInfo.skillData.flyEffectIds.Last());
                                    if (effectData != null)
                                    {
                                        float CostTime = effectData.moveTime * (positionRow * GameConfig.timePercent + 1f) / 2;
                                        timelineFirstKey += CostTime;
                                    }
                                }
                                else if (lastSkillInfo.skillData.aoeFlyEffects.Length > 0)
                                {
                                    float positionRow = PositionData.GetEnemyPositionLevels(lastCharacter.positionId, _comboTarget.positionId);
                                    EffectData effectData = EffectData.GetEffectDataById(lastSkillInfo.skillData.flyEffectIds.Last());
                                    if (effectData != null)
                                    {
                                        float CostTime = effectData.moveTime * (positionRow * GameConfig.timePercent + 1f) / 2;
                                        timelineFirstKey += CostTime;
                                    }
                                }
                                break;
                            case AnimType.Trace:

                                break;
                            case AnimType.Run:
                                {
                                    float CostTime = GetRunCostTime(lastCharacter.pos, _comboTarget.pos, lastSkillInfo);
                                    timelineFirstKey += CostTime;
                                }
                                break;
                        }
                    }
                    delay = lastTimelineEndKey - timelineFirstKey;
                }
                delay /= GameSetting.instance.speed;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                Common.Util.TransformUtil.SwitchLayer(character.transform, (int)LayerType.FightCombo);
                List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList = new List<Dictionary<uint, List<KeyValuePair<uint, uint>>>>();
                foreach (var kvp in skillInfo.skillData.timeline)
                {
                    Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = new Dictionary<uint, List<KeyValuePair<uint, uint>>>();
                    for (int j = 0; j < kvp.Value.Count; j++)
                    {
                        mechanicsDic.Add(kvp.Value[j], new List<KeyValuePair<uint, uint>>() { new KeyValuePair<uint, uint>(_comboTarget.characterInfo.instanceID, _comboTarget.characterInfo.instanceID) });
                    }
                    timelineList.Add(mechanicsDic);
                }
                if (isPlayer)
                    PlayPlayerComboSkill(character, timelineList, skillId);
                else
                    PlayEnemyComboSkill(character, timelineList, skillId);
            }
        }

        private IEnumerator ProtectComboSystemCoroutine()
        {
            float delay = 1f;//这里不考虑时间的速度
            float time = Time.realtimeSinceStartup;
            bool isFinish = false;
            while (true)
            {
                if (fightStatus == FightStatus.GameOver)
                    yield break;
                if (_comboSkillDic.Count == 0)
                    yield break;
                if (isFinish)
                {
                    FinishedCombo(comboerIsPlayer);
                    yield break;
                }
                if (_comboSkillDic.Count == _comboTimes)
                    isFinish = true;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                }
                time = Time.realtimeSinceStartup;
            }
        }

        private void FinishedCombo(bool isPlayer)
        {
            ResetComboTargetAnimatorState();
            if (fightStatus == FightStatus.Normal) return;
            ResetComboData();
            if (isPlayer)
                PlayerController.instance.ClearComboHeros(false);
            else
                EnemyController.instance.ClearComboEnemies(false);
            Common.Util.TransformUtil.SwitchLayer(_comboTarget.transform, (int)LayerType.Fight);
            if (_comboTarget)
            {
                _comboTarget.anim.updateMode = AnimatorUpdateMode.Normal;
                _comboTarget.anim.speed = 1;
            }
            EnemyController.instance.ShowHPBarViews(true);
            PlayerController.instance.ShowHPBarViews(true);
            _comboer = null;
            _comboTarget = null;
            Game.GameSetting.instance.speedMode = FightTipsController.instance.gameSpeedMode;
            Map.Controller.MapController.instance.ShowMap(true, 1.2f / GameSetting.instance.speed);
            Cameras.Controller.CameraController.instance.ResetMainCameraMask();
            if (isPlayer)
            {
                Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                if (skillBarView != null)
                    skillBarView.ShowMaskAfterCombo();
            }
            fightStatus = FightStatus.Normal;
            Common.GameTime.Controller.TimeController.instance.CancelPause();
            StopCoroutine("ProtectComboSystemCoroutine");
        }

        private float GetRunCostTime(Vector3 startPos, Vector3 targetPos, SkillInfo skillInfo)
        {
            float delta = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            float distance = Vector3.Distance(startPos, targetPos);
            Vector3 endPos = Vector3.Lerp(startPos, targetPos, (distance + skillInfo.animationData.offset) / distance);
            float speed = skillInfo.animationData.runSpeed / (1 / delta);//帧速度
            float costTime = Vector3.Distance(startPos, endPos) / speed * delta;
            return costTime;
        }

        private void ResetComboTargetAnimatorState()
        {
            if (fightStatus == FightStatus.FloatComboing)
            {
                AnimatorUtil.SetBool(_comboTarget.anim, AnimatorUtil.FLOATING, false);
            }
            else if (fightStatus == FightStatus.TumbleComboing)
            {
                AnimatorUtil.SetBool(_comboTarget.anim, AnimatorUtil.TUMBLE, false);
            }
        }

        private void ResetComboData()
        {
            _comboTimes = 0;
            _comboSkillDic.Clear();
        }
        #endregion

        #region pause

        Dictionary<CharacterEntity, float> _pauseSpeedDic = new Dictionary<CharacterEntity, float>();
        public void Pause()
        {
            _pauseSpeedDic.Clear();
            //foreach (var kvp in _closeupCharacterDic)
            //{
            //    for (int i = 0, count = kvp.Value.Count; i < count; i++)
            //    {
            //        CharacterEntity c = kvp.Value[i];
            //        if (_pauseSpeedDic.ContainsKey(c)) continue;
            //        _pauseSpeedDic.Add(c, c.anim.speed);
            //        c.anim.speed = 0;
            //        //c.anim.updateMode = AnimatorUpdateMode.Normal;
            //    }
            //}

            if (_comboTarget)
            {
                _pauseSpeedDic.Add(_comboTarget, _comboTarget.anim.speed);
                _comboTarget.anim.speed = 0;
                //_comboTarget.anim.updateMode = AnimatorUpdateMode.Normal;
            }
            foreach (var kvp in PlayerController.instance.comboHeroDic)
            {
                _pauseSpeedDic.Add(kvp.Value, kvp.Value.anim.speed);
                kvp.Value.anim.speed = 0;
                //_comboTarget.anim.updateMode = AnimatorUpdateMode.Normal;
            }
        }

        public void CancelPause()
        {
            foreach (var kvp in _pauseSpeedDic)
            {
                kvp.Key.anim.speed = kvp.Value;
                //kvp.Key.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            _pauseSpeedDic.Clear();
        }
        #endregion

        #region fight hang up
        public void FightHangupOrder(FightRegainHandler fightRegainHandler)
        {
            this.fightRegainHandler = null;
            if (fightRegainHandler != null)
                this.fightRegainHandler += fightRegainHandler;
            DataMessageHandler.DataMessage_FightHangupOrder();
        }

        public void FightHangup()
        {
            if (fightRegainHandler != null)
                fightRegainHandler();
            fightRegainHandler = null;
            Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnFightHangupHandler"));
        }

        [ContextMenu("FightRegainOrder")]
        public void FightRegainOrder()
        {
            Debugger.Log("fight regain !");
            DataMessageHandler.DataMessage_FightRegainOrder();
            Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnFightRegainOrder"));
        }

#if UNITY_EDITOR
        [ContextMenu("FightHangupTest")]
        public void FightHangupTest()
        {
            FightHangupOrder(() =>
            {
                Debugger.Log("fight hang up !!");
            });
        }
#endif

        #endregion

        #region close up camera
        private Dictionary<CharacterEntity, List<CharacterEntity>> _closeupCharacterDic = new Dictionary<CharacterEntity, List<CharacterEntity>>();
        public void SetCloseupCharacter(CharacterEntity closeupCharacter, List<CharacterEntity> targets)
        {
            _closeupCharacterDic.Add(closeupCharacter, targets);
        }

        private void ClearCloseupCharacters()
        {
            _closeupCharacterDic.Clear();
        }

        private IEnumerator FinishCloseupCameraCoroutine(SkillInfo skillInfo, CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, bool isPlayer)
        {
            yield return new WaitForSeconds(skillInfo.animationData.closeupOverTime - skillInfo.skillData.pauseTime);
            if (FightController.instance.fightStatus == FightStatus.Normal)
            {
                //isCloseup = false;
                List<CharacterEntity> targets = null;
                if (_closeupCharacterDic.TryGetValue(character, out targets))
                {
                    foreach (var kvp in _closeupCharacterDic)
                    {
                        if (kvp.Key == character) continue;
                        for (int i = 0, count = kvp.Value.Count; i < count; i++)
                        {
                            CharacterEntity c = kvp.Value[i];
                            if (targets.Contains(c))
                                targets.Remove(c);
                        }
                    }
                    for (int i = 0, count = targets.Count; i < count; i++)
                    {
                        CharacterEntity c = targets[i];
                        if (c)
                            TransformUtil.SwitchLayer(c.transform, (int)LayerType.Fight);
                    }
                    _closeupCharacterDic.Remove(character);
                }
                //Cameras.Controller.CameraController.instance.ShowCloseupCamera(false, false, null);
                Common.Util.TransformUtil.SwitchLayer(character.transform, (int)LayerType.Fight);
                isCloseup = !Cameras.Controller.CameraController.instance.ResetCloseupCamera(character);
                if (!isCloseup)
                {
                    Cameras.Controller.CameraController.instance.ShowMainCamera(true);
                    //CharacterUtil.SwitchCharacterLayer(skillInfo, timelineList, isPlayer, (int)LayerType.Fight);
                    EnemyController.instance.ShowHPBarViews(true);
                    PlayerController.instance.ShowHPBarViews(true);
                    Logic.UI.SkillBar.Controller.SkillBarController.instance.Show(true);
                    Judge.Controller.JudgeController.instance.ShowDamageBarViewPool(true);
                }
            }
        }
        #endregion

        #region do some work when join fight and quit fight
        private bool fightResult = false;
        private int _preTotalCount = 0, _currentLoaded = 0;
        public void PreReadyFight()
        {
            PreLoadFightRes();
        }

        #region preload fight res
        private void PreLoadFightRes()
        {
            UI.UIMgr.instance.Close(Logic.UI.EUISortingLayer.MainUI);
            LuaScriptMgr.Instance.LuaGC();
            Effect.Controller.EffectController.instance.ClearEffectInFight();
            PlayerController.instance.ClearHeros();
            EnemyController.instance.ClearEnemy();
            Cameras.Controller.CameraController.instance.Reset();
            switch (fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.ConsortiaFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    ResMgr.instance.ClearRes(true);
                    break;
                case FightType.FirstFight:
                    break;
                case FightType.SkillDisplay:
                    break;
            }
#if UNITY_EDITOR
            if (GameConfig.instance.loadAssetBundle)
                Logic.UI.LoadGame.View.LoadGameView.Open(PreloadCompleteHandler);
#else
            if (GameConfig.assetBundle)
                Logic.UI.LoadGame.View.LoadGameView.Open(PreloadCompleteHandler);
#endif
            PreLoadEffects();
            //PreLoadEffects(true);
        }

        private void PreLoadEffects()
        {
            List<string> preFightPaths = null;
            SkillData.GetSkillDatas();
            AnimationData.GetAnimationDatas();
            MechanicsData.GetMechanicsDatas();
            //if (!isSkill)
            preFightPaths = new List<string>(ResMgr.instance.preFightPaths);
            //else
            //    preFightPaths = new List<string>();
            preFightPaths.AddRange(FightUtil.GetEffectsInFight(false));
            List<string> paths = FightUtil.GetEffectsInFight(true);
            for (int i = 0, count = paths.Count; i < count; i++)
            {
                string path = paths[i];
                if (!preFightPaths.Contains(path))
                    preFightPaths.Add(path);
            }
            preFightPaths.Add("effects/prefabs/" + Effect.Controller.EffectController.UI_EFFECT_01);
            //Debugger.Log((isSkill ? string.Empty : "hit skill and passive ") + "skill effects count:{0}", preFightPaths.Count);
            Debugger.Log("preFightPaths:{0}", preFightPaths.Count);
            _currentLoaded = 0;
            _preTotalCount = preFightPaths.Count;
            if (_preTotalCount == 0)
            {
                //if (!isSkill)
                Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(FIGHT_TRANSITION_TIME, PreloadCompleteHandler);
            }
            else
            {
                for (int i = 0, count = preFightPaths.Count; i < count; i++)
                {
#if UNITY_EDITOR
                    //Debugger.Log("pre fight res:{0}", preFightPaths[i]);
                    if (GameConfig.instance.loadAssetBundle)
                        ResMgr.instance.Load<Object>(preFightPaths[i], PreloadProgressHandler, 0);
                    else
                    {
                        //if (!isSkill)
                        Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(FIGHT_TRANSITION_TIME, PreloadCompleteHandler);
                        break;
                    }
#else
                    //Debugger.Log("pre fight res:{0}", preFightPaths[i]);
                    if (GameConfig.assetBundle)
                        ResMgr.instance.Load<Object>(preFightPaths[i], PreloadProgressHandler, 0);
                    else
                    {
                        //if (!isSkill)
                            Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(FIGHT_TRANSITION_TIME, PreloadCompleteHandler);
                        break;
                    }
#endif
                }
            }
        }

        public void PreLoadNumFont()
        {
            ResMgr.instance.Load<Object>("fonts/num_fight_gray", null, 0);
            ResMgr.instance.Load<Object>("fonts/num_fight_green", null, 0);
            ResMgr.instance.Load<Object>("fonts/num_fight_orange", null, 0);
        }

        private void PreloadProgressHandler(Object target)
        {
            _currentLoaded++;
            float progress = _currentLoaded / (float)_preTotalCount;
#if UNITY_EDITOR
            if (progress >= 1)
                Debugger.Log("all effects load success !");
#endif
            Logic.UI.LoadGame.View.LoadGameView loadGameView = Logic.UI.UIMgr.instance.Get<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            if (loadGameView)
                loadGameView.UpdateLoadProgress(progress);
            if (target == null)
                Debugger.LogError("load res fail !");
        }

        private void PreloadCompleteHandler()
        {
            UI.UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            ReadyFight();
        }
        #endregion

        public void ReadyFight()
        {
            _isFight = false;
            Effect.Controller.EffectController.instance.ClearEffectInFight();
            Cameras.Controller.CameraController.instance.fighting = true;
            Map.Controller.MapController.instance.fighting = true;
            MockFightController.instance.needComboPause = false;
            FightController.instance.comboWaitSettingHandler = null;
            StopAllCoroutines();
            switch (fightType)
            {
                case FightType.PVE:
                    Dungeon.Model.DungeonProxy.instance.onPveFightOverDelegate += FightResult;
                    break;
                case FightType.DailyPVE:
                    Activity.Model.ActivityProxy.instance.onActivyChallengeOverDelegate += FightResult;
                    break;
                case FightType.Arena:
                    Logic.UI.Pvp.Model.PvpProxy.instance.onPvpFightOverDelegate += FightResult;
                    break;
                case FightType.Expedition:
                    Logic.UI.Expedition.Model.ExpeditionProxy.instance.onExpeditionFightOverDelegate += FightResult;
                    break;
                case FightType.WorldTree:
                    Logic.UI.WorldTree.Model.WorldTreeProxy.instance.onWorldTreeFightOverDelegate += FightResult;
                    break;
                case FightType.WorldBoss:
                    Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossFightOverDelegate += FightResult;
                    break;
                case FightType.FirstFight:
                    //MockFightController.instance.needComboPause = true;
                    finishedComboSkillHandler += MockFightController.instance.MoveNextSkillHandler;
                    finishedSkillMechanicsHandler += MockFightController.instance.MoveNextSkillHandler;
                    break;
                case FightType.SkillDisplay:
                    break;
                case FightType.PVP:
                    Logic.UI.Pvp.Model.PvpProxy.instance.onPvpRaceFightOverDelegate += FightResult;
                    break;
                case FightType.FriendFight:
                    break;
                case FightType.ConsortiaFight:
                    break;
                case FightType.MineFight:
                    Logic.UI.Mine.Controller.MineController.instance.onMineFightOverHandler += FightResult;
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    PlayerController.instance.InitImitateData();
                    EnemyController.instance.InitImitateData();
                    foreach (var kvp in playerInfoDic)
                    {
                        PlayerController.instance.imitatePlayerInfoDic.Add(kvp.Key, kvp.Value);
                    }
                    foreach (var kvp in heroInfoDic)
                    {
                        PlayerController.instance.imitateHeroInfoDic.Add(kvp.Key, kvp.Value);
                    }
                    foreach (var kvp in enemeyPlayerInfoDic)
                    {
                        EnemyController.instance.imitateEnemyPlayerInfoDic.Add(kvp.Key, kvp.Value);
                    }
                    foreach (var kvp in enemeyHeroInfoDic)
                    {
                        EnemyController.instance.imitateEnemyHeroInfoDic.Add(kvp.Key, kvp.Value);
                    }
                    break;
#endif
            }
#if UNITY_EDITOR
            if (imitate)
                currentIndex++;
#endif
            LuaScriptMgr.Instance.CallLuaFunction("InitFightdata");
            Observers.Facade.Instance.RegisterObserver("Fight::StopFightTime", MockFightController.instance.StopFightTime);
            Observers.Facade.Instance.RegisterObserver("Fight::ReStartFightTime", MockFightController.instance.ReStartFightTime);
            Observers.Facade.Instance.RegisterObserver("Fight::FightHangup", MockFightController.instance.FightHangup);
            Observers.Facade.Instance.RegisterObserver("Fight::FightRegainOrder", MockFightController.instance.FightRegainOrder);
            Observers.Facade.Instance.RegisterObserver("Fight::FullSkillCD", MockFightController.instance.FullSkillCD);
            Observers.Facade.Instance.RegisterObserver("Fight::SetNeedComboPause", MockFightController.instance.SetNeedComboPause);
            Observers.Facade.Instance.RegisterObserver("Fight::ReSetNeedComboPause", MockFightController.instance.ReSetNeedComboPause);
            DataMessageHandler.DataMessage_ReadyFight();
        }

        public void ReadyScene()
        {
            ResetFightStatisticsData();
            Fight.Model.FightProxy.instance.InitBuffIncos();
            EnemyController.instance.enemyFloatable = false;
            AudioController.instance.StopAll();
            AudioController.instance.PlayBGMusic(fightType);
            GameSetting.instance.frameType = Logic.Game.GameFrameType.Fight;
            Fight.Model.FightProxy.instance.fightOverType = FightOverType.None;
            fightStatus = FightStatus.Normal;
            Judge.Controller.JudgeController.instance.InitFightData();
            isCloseup = false;
            //UI.UIMgr.instance.Open(Logic.UI.Reward.View.RewardView.PREFAB_PATH, Logic.UI.EUISortingLayer.MainUI, Logic.UI.UIOpenMode.Overlay);
            Logic.UI.FightTips.View.FightTipsView fightTipsView = FightTipsController.instance.OpenFightTipsView();
            switch (fightType)
            {
                case FightType.Arena:
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapArena);
                    fightTipsView.maxTime = 5 * 60f;
                    FightTipsController.instance.fightOverHandler += FightOverHandler;
                    break;
                case FightType.Expedition:
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapExpedition);
                    fightTipsView.maxTime = 5 * 60f;
                    FightTipsController.instance.fightOverHandler += FightOverHandler;
                    break;
                case FightType.PVE:
                    {
                        Dungeon.Model.DungeonData dungeonData = Fight.Model.FightProxy.instance.CurrentDungeonData;
                        fightTipsView.maxTime = 5 * 60f;
                        FightTipsController.instance.fightOverHandler += FightOverHandler;
                        Logic.Map.Controller.MapController.instance.CreateMap(dungeonData.dungeonMap);
                    }
                    break;
                case FightType.DailyPVE:
                    {
                        Dungeon.Model.DungeonData dungeonData = Fight.Model.FightProxy.instance.CurrentDungeonData;
                        fightTipsView.maxTime = 3 * 60f;
                        FightTipsController.instance.fightOverHandler += FightOverHandler;
                        Logic.Map.Controller.MapController.instance.CreateMap(dungeonData.dungeonMap);
                    }
                    break;
                case FightType.WorldTree:
                    {
                        Dungeon.Model.DungeonData dungeonData = Fight.Model.FightProxy.instance.CurrentDungeonData;
                        fightTipsView.maxTime = 3 * 60f;
                        FightTipsController.instance.fightOverHandler += FightOverHandler;
                        Logic.Map.Controller.MapController.instance.CreateMap(dungeonData.dungeonMap);
                    }
                    break;
                case FightType.WorldBoss:
                    {
                        Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapWorldBoss);
                        fightTipsView.maxTime = int.MaxValue;
                        //FightTipsController.instance.fightOverHandler += FightOverHandler;
                        WorldBoss.Model.WorldBossProxy.instance.onWorldBossActivityEndDelegate += FightOverHandler;
                    }
                    break;
                case FightType.FirstFight:
                    Logic.Map.Controller.MapController.instance.CreateMap(MockFightController.instance.mapName);
                    fightTipsView.maxTime = int.MaxValue;
                    break;
                case FightType.SkillDisplay:
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapSkillDisplay);
                    Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView illustrationSkillDisplayView = UI.UIMgr.instance.Open<Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView>(Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView.PREFAB_PATH);
                    if (illustrationSkillDisplayView)
                        illustrationSkillDisplayView.skillDisplayOverHandler += FightOverHandler;
                    break;
                case FightType.PVP:
                    EnemyController.instance.enemyFloatable = true;
                    fightTipsView.maxTime = 5 * 60f;
                    FightTipsController.instance.fightOverHandler += FightOverHandler;
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapPvp);
                    break;
                case FightType.ConsortiaFight:
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapConsortiaFight);
                    break;
                case FightType.FriendFight:
                    EnemyController.instance.enemyFloatable = true;
                    fightTipsView.maxTime = 5 * 60f;
                    FightTipsController.instance.fightOverHandler += FightOverHandler;
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapPvp);
                    break;
                case FightType.MineFight:
                    EnemyController.instance.enemyFloatable = true;
                    fightTipsView.maxTime = 5 * 60f;
                    FightTipsController.instance.fightOverHandler += FightOverHandler;
                    Logic.Map.Controller.MapController.instance.CreateMap(Logic.Game.Model.GlobalData.GetGlobalData().mapPvp);
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    Logic.Map.Controller.MapController.instance.CreateMap("map_01");
                    if (isDoubleSpeed)
                    {
                        fightTipsView.ClickFightDoubleSpeedHandler();
                    }
                    break;
#endif
            }
            FightTipsController.instance.comboWaitingOverHandler += ComboWaitingOverHandler;
            Logic.UI.SkillBanner.View.SkillBannerView.Open();
            Logic.UI.SkillHead.View.SkillHeadView.Open();
            DataMessageHandler.DataMessage_ReadySceneSuccess();
        }

        public void StartFight()
        {
            if (!_isFight)
            {
                _isFight = true;
                StartCoroutine("UpdateFightSkillTimeCoroutine");
            }
            switch (fightType)
            {
                case FightType.PVE:
                case FightType.DailyPVE:
                case FightType.WorldTree:
                    PlayerController.instance.AddFormationBuff();
                    if (Fight.Model.FightProxy.instance.CurrentTeamIndex == 0)
                        Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnFirstFightStart"));
                    else if (Fight.Model.FightProxy.instance.CurrentTeamIndex == 1)
                        Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnSecondFightStart"));
                    break;
                case FightType.WorldBoss:
                    PlayerController.instance.AddFormationBuff();
                    StopCoroutine("CalcDamageFixedTime");
                    StartCoroutine("CalcDamageFixedTime");
                    break;
                case FightType.Arena:
                case FightType.Expedition:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                    PlayerController.instance.AddFormationBuff();
                    EnemyController.instance.AddFormationBuff();
                    break;
                case FightType.ConsortiaFight:
                    break;
                case FightType.FirstFight:
                    Tutorial.Controller.TutorialController.instance.ExecuteStepComplete();
                    break;
                case FightType.SkillDisplay:
                    break;
            }
            ClearCloseupCharacters();
            PlayerController.instance.AddHaloBuff();
            EnemyController.instance.AddHaloBuff();
            EnemyController.instance.PlayBossEffect();
            FightTipsController.instance.FightStart();
            //Observers.Facade.Instance.SendNotification(string.Format("Fight::{0}", "OnFightStart"));
        }

        private void FightOverHandler()
        {
            bool result = false;
            switch (fightType)
            {
                case FightType.Arena:
                case FightType.PVE:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.Expedition:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.ConsortiaFight:
                case FightType.MineFight:
                    break;
                case FightType.DailyPVE:
                    if (Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.ActivityDamageOutput)
                        result = true;
                    break;
                case FightType.FirstFight:
                    break;
                case FightType.SkillDisplay:
                    break;
            }
            Logic.Net.Controller.DataMessageHandler.DataMessage_ForceFightFinished(result, FightOverType.Timeout);
        }

        public void FinishFight(bool isWin, FightOverType fightOverType)
        {
            switch (fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                    AudioController.instance.PlayAudio(fightResult ? AudioController.BATTLE_VICTORY_AUDIO : AudioController.BATTLE_FAIL_AUDIO, false, 0f);
                    FightTipsController.instance.fightOverHandler -= FightOverHandler;
                    break;
                case FightType.WorldBoss:
                    AudioController.instance.PlayAudio(fightResult ? AudioController.BATTLE_VICTORY_AUDIO : AudioController.BATTLE_FAIL_AUDIO, false, 0f);
                    WorldBoss.Model.WorldBossProxy.instance.onWorldBossActivityEndDelegate -= FightOverHandler;
                    break;
                case FightType.FirstFight:
                    break;
                case FightType.SkillDisplay:
                    Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView illustrationSkillDisplayView = UI.UIMgr.instance.Get<Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView>(Logic.UI.IllustratedHandbook.View.IllustrationSkillDisplayView.PREFAB_PATH);
                    if (illustrationSkillDisplayView)
                        illustrationSkillDisplayView.skillDisplayOverHandler -= FightOverHandler;
                    break;
                case FightType.ConsortiaFight:

                    break;
            }
            FightTipsController.instance.comboWaitingOverHandler -= ComboWaitingOverHandler;
            FightTipsController.instance.FightOver();
            CalcFightStatisticsData();
            FightTipsController.instance.CloseFightTipsView();
            GameSetting.instance.lastFightSpeedMode = GameSetting.instance.speedMode;
            GameSetting.instance.speedMode = GameSpeedMode.Normal;
            fightResult = isWin;
            fightStatus = FightStatus.GameOver;
            Fight.Model.FightProxy.instance.fightOverType = fightOverType;
            if (fightResult)
            {
                PlayerController.instance.Victory_Scene();
                PlayerController.instance.ShowHPBarViews(false);
            }
            else
            {
                EnemyController.instance.ShowHPBarViews(false);
            }
            _isFight = false;
            StopCoroutine("UpdateFightSkillTimeCoroutine");
            StartCoroutine("FinishFightCoroutine");
        }

        private IEnumerator FinishFightCoroutine()
        {
            if (fightType != FightType.WorldBoss)
            {
                StopCoroutine("CalcDamageFixedTime");
            }
            AudioController.instance.SetBGMusicState(false);
            if (fightType != FightType.SkillDisplay && fightType != FightType.FirstFight)
            {
#if UNITY_EDITOR
                if (fightType != FightType.Imitate)
#endif
                    AudioController.instance.PlayAudio(fightResult ? AudioController.WIN : AudioController.FAIL, false, 0f);
                if (!PlayerController.instance.playerDead)
                {
                    Logic.Audio.Model.AudioData audioData = Logic.Audio.Model.AudioData.GetAudioDataById(AudioController.ROLE_VECTORY_AUDIO_ID);
                    if (audioData != null)
                        AudioController.instance.PlayAudio(audioData.audioName, !audioData.accelerate);
                }
            }

            switch (fightType)
            {
                case FightType.PVE:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Debugger.Log("fightCostTime:" + fightCostTime + "   isWin:" + fightResult + "   totalDamage:" + totalDamage + "    deadCount:" + deadCount + "   remainderHPAverageRate:" + remainderHPAverageRate + "    comboCount:" + comboCount);
                    Dungeon.Controller.DungeonController.instance.CLIENT2LOBBY_PVE_FIGHT_OVER(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID, fightCostTime, fightResult ? 1 : 0, totalDamage, deadCount, remainderHPAverageRate, comboCount, PlayerController.instance.DeadHeroIdList);
                    break;
                case FightType.DailyPVE:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    int flag = fightResult ? 1 : 0;
                    if (Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.ActivityDamageOutput)
                        flag = 1;
                    Activity.Controller.ActivityController.instance.CLIENT2LOBBY_ACTIVITY_PVE_OVER_REQ(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID, totalDamage, flag, deadCount, fightCostTime, remainderHPAverageRate, comboCount);
                    break;
                case FightType.WorldTree:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Logic.UI.WorldTree.Controller.WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_SETTLE_REQ(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID, fightResult ? 1 : 0, PlayerController.instance.DeadHeroIdList);
                    break;
                case FightType.Arena:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Logic.UI.Pvp.Controller.PvpController.instance.CLIENT2LOBBY_RANK_ARENA_CHALLENGE_OVER_REQ(fightResult ? 1 : 0);
                    break;
                case FightType.Expedition:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Dictionary<int, int> heroDic = new Dictionary<int, int>();
                    Dictionary<int, int> enemyDic = new Dictionary<int, int>();
                    List<HeroEntity> aliveHeros = PlayerController.instance.heros;
                    for (int i = 0, count = aliveHeros.Count; i < count; i++)
                    {
                        HeroEntity hero = aliveHeros[i];
                        int rate = (int)((hero.HP / (float)hero.maxHP) * 10000);
                        heroDic.Add((int)hero.characterInfo.instanceID, rate);
                    }
                    List<HeroEntity> deadHeros = PlayerController.instance.deadHeroDic.GetValues();
                    for (int i = 0, count = deadHeros.Count; i < count; i++)
                    {
                        heroDic.Add((int)deadHeros[i].characterInfo.instanceID, 0);
                    }

                    List<EnemyEntity> aliveEnemies = EnemyController.instance.enemies;
                    for (int i = 0, count = aliveEnemies.Count; i < count; i++)
                    {
                        EnemyEntity enemy = aliveEnemies[i];
                        int rate = (int)((enemy.HP / (float)enemy.maxHP) * 10000);
                        enemyDic.Add((int)enemy.characterInfo.instanceID, rate);
                    }
                    List<EnemyEntity> deadEnemies = EnemyController.instance.deadEnemyDic.GetValues();
                    for (int i = 0, count = deadEnemies.Count; i < count; i++)
                    {
                        enemyDic.Add((int)deadEnemies[i].characterInfo.instanceID, 0);
                    }
                    Logic.UI.Expedition.Controller.ExpeditionController.instance.CLIENT2LOBBY_Expedition_Fight_Result_REQ(fightResult, heroDic, enemyDic);
                    break;
                case FightType.WorldBoss:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    WorldBoss.Model.WorldBossProxy.instance.onWorldBossKilledByOthersDelegate = null;
                    Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossCurrHPChangedDelegate = null;
                    WorldBoss.Controller.WorldBossController.instance.Mock_WorldBossSettleReq(fightResult, CalcLastDamage());
                    break;
                case FightType.PVP:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Logic.UI.Pvp.Controller.PvpController.instance.CLIENT2LOBBY_PointPvpSettleReq(fightResult ? 1 : 0);
                    break;
                case FightType.ConsortiaFight:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    FightResult();
                    break;
                case FightType.FriendFight:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    FightResult();
                    break;
                case FightType.FirstFight:
                    if (!MockFightController.instance.isSkip)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    FightResult();
                    break;
                case FightType.SkillDisplay:
                    FightResult();
                    break;
                case FightType.MineFight:
                    if (fightResult)
                        yield return new WaitForSeconds(WAIT_FIGHT_OVER_TIME);
                    Logic.UI.Mine.Controller.MineController.instance.LOBBY2SERVER_MineFightOverReq(fightResult ? 1 : 0);
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    fightCostTimes.Add(fightCostTime);
                    fightResults.Add(fightResult);
                    FightResult();
                    break;
#endif
            }
        }

        private void FightResult()
        {
            switch (fightType)
            {
                case FightType.PVE:
                    Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData);
                    Dungeon.Model.DungeonProxy.instance.onPveFightOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.DailyPVE:
                    bool flag = fightResult;
                    if (Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.ActivityDamageOutput)
                        flag = true;//win forever
                    Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData);
                    Activity.Model.ActivityProxy.instance.onActivyChallengeOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(flag, fightType);
                    break;
                case FightType.WorldTree:
                    Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData);
                    Logic.UI.WorldTree.Model.WorldTreeProxy.instance.onWorldTreeFightOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.Arena:
                    Logic.Fight.Model.FightProxy.instance.ResetArena();
                    Logic.UI.Pvp.Model.PvpProxy.instance.onPvpFightOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.Expedition:
                    Logic.Fight.Model.FightProxy.instance.ResetExpedition();
                    Logic.UI.Expedition.Model.ExpeditionProxy.instance.onExpeditionFightOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.WorldBoss:
                    Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossFightOverDelegate -= FightResult;
                    WorldBoss.Model.WorldBossProxy.instance.OnWorldBossReallyFightOver();
                    break;
                case FightType.PVP:
                    Logic.UI.Pvp.Model.PvpProxy.instance.onPvpRaceFightOverDelegate -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.ConsortiaFight:
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);//临时使用
                    break;
                case FightType.FriendFight:
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
                case FightType.FirstFight:
                    finishedComboSkillHandler -= MockFightController.instance.MoveNextSkillHandler;
                    finishedSkillMechanicsHandler -= MockFightController.instance.MoveNextSkillHandler;
                    Logic.UI.UIMgr.instance.Close(Logic.UI.Tutorial.View.TutorialView.PREFAB_PATH);
                    FirstFightEndView.Open(()=> QuitFight(true, PreQuitScene));
                    break;
                case FightType.SkillDisplay:
                    QuitFight(false, null);
                    Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_illustrate, true, true);
                    Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_illustrate_hero_detial_view);
                    break;
                case FightType.MineFight:
                    Logic.UI.Mine.Controller.MineController.instance.onMineFightOverHandler -= FightResult;
                    Logic.UI.FightResult.View.FightResultView.Open(fightResult, fightType);
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    int remind = fightCount - (currentIndex + 1);
                    Debugger.Log(string.Format("第{0}场战斗结束，还有{1}场战斗", currentIndex + 1, remind));
                    if (remind > 0)
                    {
                        Debugger.Log("2秒后战斗开始...");
                        StartCoroutine(StartImitateFightCoroutine());
                    }
                    QuitFight(true, PreQuitScene);
                    break;
#endif
            }

            fightResult = false;
            Observers.Facade.Instance.RemoveObserver("Fight::StopFightTime", MockFightController.instance.StopFightTime);
            Observers.Facade.Instance.RemoveObserver("Fight::ReStartFightTime", MockFightController.instance.ReStartFightTime);
            Observers.Facade.Instance.RemoveObserver("Fight::FightHangup", MockFightController.instance.FightHangup);
            Observers.Facade.Instance.RemoveObserver("Fight::FightRegainOrder", MockFightController.instance.FightRegainOrder);
            Observers.Facade.Instance.RemoveObserver("Fight::FullSkillCD", MockFightController.instance.FullSkillCD);
            Observers.Facade.Instance.RemoveObserver("Fight::SetNeedComboPause", MockFightController.instance.SetNeedComboPause);
            Observers.Facade.Instance.RemoveObserver("Fight::ReSetNeedComboPause", MockFightController.instance.ReSetNeedComboPause);
            Judge.Controller.JudgeController.instance.DestroyFightData();
            //Cameras.Controller.CameraController.instance.ShowCloseupCamera(false, false, null);
            Cameras.Controller.CameraController.instance.ResetCloseupCamera(null);
            Cameras.Controller.CameraController.instance.ResetMainCameraMask();
            Logic.Map.Controller.MapController.instance.ClearShadow();
            UI.UIMgr.instance.Close(Logic.UI.ComboBar.View.ComboBarView.PREFAB_PATH);
            UI.UIMgr.instance.Close(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
            //UI.UIMgr.instance.Close(Logic.UI.Reward.View.RewardView.PREFAB_PATH);
            FightTipsController.instance.CloseFightTipsView();
            UI.UIMgr.instance.Close(Logic.UI.SkillBanner.View.SkillBannerView.PREFAB_PATH);
            Fight.Model.FightProxy.instance.Clear();
            fightType = FightType.None;
            fightStatus = FightStatus.GameOver;
            LuaScriptMgr.Instance.CallLuaFunction("DestroyFightData");
            StartCoroutine("ClearFightEffectCoroutine");
        }

        private IEnumerator ClearFightEffectCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            Effect.Controller.EffectController.instance.ClearEffectInFight();
        }

        public void QuitFight(bool gc = true, System.Action callback = null)
        {
            Logic.UI.LoadGame.Controller.LoadGameController.instance.OpenLoadGameView();
            Cameras.Controller.CameraController.instance.fighting = false;
            Map.Controller.MapController.instance.fighting = false;
            GameSetting.instance.frameType = Logic.Game.GameFrameType.UI;
            Logic.Map.Controller.MapController.instance.ClearMap();
            PlayerController.instance.ClearHeros();
            EnemyController.instance.ClearEnemy();
            if (gc)
                StartCoroutine(GCCoroutine(callback));
        }

        private IEnumerator GCCoroutine(System.Action callback = null)
        {
            yield return new WaitForSeconds(0.2f);
            ResMgr.instance.ClearRes(true);
            Game.Controller.GameController.instance.GC(0f, null, callback);
        }

        private void PreQuitScene()
        {
            Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(FIGHT_TRANSITION_TIME, QuitScene);
        }

        private void QuitScene()
        {
            Logic.UI.Main.View.MainView.Open();
            AudioController.instance.PlayBGMusic(AudioController.MAINSCENE);
        }

        [ContextMenu("PlayFightStartEffect")]
        public void PlayFightStartEffect()
        {
            Effect.Controller.EffectController.instance.PlayUIEffect(Effect.Controller.EffectController.UI_EFFECT_01, Vector3.zero, Quaternion.identity, Vector3.one, 5f, 0);
        }

        [ContextMenu("PlayNextFightEffect")]
        public void PlayNextFightEffect()
        {
            Effect.Controller.EffectController.instance.PlayUIEffect(Effect.Controller.EffectController.UI_EFFECT_NEXT_FIGHT, Vector3.zero, Quaternion.identity, Vector3.one, 5f, 0, null, 2f);
        }

        [ContextMenu("PlayBossAppearEffect")]
        public void PlayBossAppearEffect()
        {
            Effect.Controller.EffectController.instance.PlayUIEffect(Effect.Controller.EffectController.UI_EFFECT_WARNING_FIGHT, Vector3.zero, Quaternion.identity, Vector3.one, 5f, 0, null, 2f);
        }

        #endregion

        public void SetAngry(float angry)
        {
            Logic.UI.SkillBar.Controller.SkillBarController.instance.SetAngry(angry);
        }

        #region animation and transform operation
        public void Victory_Scene(uint id)
        {
            CharacterEntity character = PlayerController.instance[id] as CharacterEntity;
            ActionController.instance.PlayerAnimAction(character, AnimatorUtil.VICOTRY_ID);
            if (character is PlayerEntity)
            {
                PlayerEntity playerEntity = character as PlayerEntity;
                ActionController.instance.PlayerAnimAction(playerEntity.petEntity, AnimatorUtil.VICOTRY_ID);
            }
        }

        public void RunWithoutMove_Scene(uint id)
        {
            CharacterEntity character = PlayerController.instance[id] as CharacterEntity;
            ActionController.instance.PlayerAnimAction(character, AnimatorUtil.RUN_ID);
        }

        public void EndRun_Scene(uint id, bool isPlayer = true)
        {
            CharacterEntity character = isPlayer ? PlayerController.instance[id] as CharacterEntity : EnemyController.instance[id] as CharacterEntity;
            ActionController.instance.PlayerAnimAction(character, AnimatorUtil.IDLE_ID);
        }

        public void Dead(uint id, bool isPlayer)
        {
            if (fightStatus == FightStatus.Normal)
            {
                CharacterEntity character = null;
                if (isPlayer)
                {
                    character = PlayerController.instance[id];
                    PlayerController.instance.Remove(id, DEAD_TIME);
                }
                else
                {
                    character = EnemyController.instance[id];
                    EnemyController.instance.Remove(id, DEAD_TIME);
                    if (RandomUtil.GetRandom(Logic.Game.Model.GlobalData.GetGlobalData().gold_drop_probability))
                    {
                        Effect.Model.EffectInfo effectInfo = new Effect.Model.EffectInfo(Effect.Controller.EffectController.GOLD_EFFECT_ID);
                        effectInfo.pos = character.pos + effectInfo.effectData.offset;
                        effectInfo.character = character;
                        effectInfo.target = character;
                        effectInfo.delay = effectInfo.effectData.delay;
                        Effect.Controller.EffectController.instance.PlayNoSkillEffect(effectInfo);
                    }
                }
                if (character && character.isDead && !(AnimatorUtil.isTargetState(character.anim, AnimatorUtil.DEAD_ID)
                    || AnimatorUtil.isTargetState(character.anim, AnimatorUtil.TUMBLESTART_ID)
                    || AnimatorUtil.isTargetState(character.anim, AnimatorUtil.FLOATDOWN_ID) || AnimatorUtil.isTargetState(character.anim, AnimatorUtil.FLOATSTART_ID)))
                    ActionController.instance.PlayerAnimAction(character, AnimatorUtil.DEAD_ID);

                Hero.Model.HeroData heroData = Hero.Model.HeroData.GetHeroDataByID((int)character.characterInfo.baseId);
                Logic.Audio.Model.AudioData attackAudioData = Logic.Audio.Model.AudioData.GetAudioDataById(heroData.audioDie);
                if (attackAudioData != null)
                    AudioController.instance.PlayAudio(attackAudioData.audioName, !attackAudioData.accelerate);
            }
        }

        public void Run_Scene(uint id, bool isPlayer = true, float moveTime = 1.5f)
        {
            CharacterEntity character = isPlayer ? PlayerController.instance[id] as CharacterEntity : EnemyController.instance[id] as CharacterEntity;
            Action.Controller.ActionController.instance.MoveTarget(character, PositionData.GetPos(character.positionId), moveTime);
        }

        public void Move(Transform trans, Vector3 endPos, float duration, System.Action callback = null)
        {
            StartCoroutine(MoveCoroutine(trans, endPos, duration, callback));
        }

        private IEnumerator MoveCoroutine(Transform trans, Vector3 endPos, float duration, System.Action callback = null)
        {
            Vector3 curPos = trans.localPosition;
            Vector3 normal = (endPos - curPos).normalized;
            float distance = Vector3.Distance(curPos, endPos);
            float delta = GameSetting.instance.deltaTimeFight;
            float speed = distance / (duration / delta) * 2;
            WaitForSeconds waitForSeconds = new WaitForSeconds(delta);
            //Debugger.LogError("distance:" + distance + "  speed" + speed + "   duration:" + duration + "  delta:" + delta);
            normal *= speed;
            normal.y = 0;
            if (curPos.x - endPos.x <= 0)
            {
                while (curPos.x < endPos.x)
                {
                    yield return waitForSeconds;
                    curPos += normal;
                    if (trans)
                        trans.localPosition = curPos;
                }
            }
            else
            {
                while (curPos.x > endPos.x)
                {
                    yield return waitForSeconds;
                    curPos += normal;
                    if (trans)
                        trans.localPosition = curPos;
                }
            }
            if (trans)
                trans.localPosition = endPos;
            if (callback != null)
                callback();
        }

        public void Rotate(Transform trans, Vector3 endPos, float delay)
        {
            if (delay > 0)
                StartCoroutine(RotateCoroutine(trans, endPos, delay));
            else
                Rotate(trans, endPos);
        }

        private IEnumerator RotateCoroutine(Transform trans, Vector3 endPos, float delay)
        {
            if (fightStatus == FightStatus.Normal)
                yield return new WaitForSeconds(delay);
            else
            {
                float time = Time.realtimeSinceStartup;
                delay /= GameSetting.instance.speed;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            Rotate(trans, endPos);
        }

        public Vector3 Rotate(Transform trans, Vector3 endPos)
        {
            if (!trans) return Vector3.zero;
            Vector3 curPos = trans.position;
            Vector3 originalEulerAngles = trans.eulerAngles;
            Quaternion qua = Quaternion.LookRotation(endPos - curPos);
            if (fightStatus == FightStatus.Normal)
                StartCoroutine(RotateCoroutine(trans, endPos, qua));
            else
                StartCoroutine(ComboRotateCoroutine(trans, endPos, qua));
            return originalEulerAngles - qua.eulerAngles;
        }

        private IEnumerator RotateCoroutine(Transform trans, Vector3 endPos, Quaternion qua)
        {
            int count = 0;
            while (true)
            {
                if (fightStatus == FightStatus.GameOver)
                    yield break;
                yield return new WaitForSeconds(GameSetting.instance.deltaTimeFight);
                if (count > 10)
                    break;
                if (trans)
                    trans.rotation = Quaternion.Lerp(trans.rotation, qua, 0.5f);
                count++;
            }
            if (fightStatus == FightStatus.GameOver)
                yield break;
            if (trans)
                trans.LookAt(endPos);
        }

        private IEnumerator ComboRotateCoroutine(Transform trans, Vector3 endPos, Quaternion qua)
        {
            float delta = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            float lastTime = Time.realtimeSinceStartup;
            int count = 0;
            while (true)
            {
                if (fightStatus == FightStatus.GameOver)
                    yield break;
                if (Time.realtimeSinceStartup - lastTime < delta)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    lastTime = Time.realtimeSinceStartup;
                }
                if (TimeController.instance.playerPause) continue;
                if (count > 10)
                    break;
                if (trans)
                    trans.rotation = Quaternion.Lerp(trans.rotation, qua, 0.5f);
                count++;
            }
            if (fightStatus == FightStatus.GameOver)
                yield break;
            if (trans)
                trans.LookAt(endPos);
        }
        #endregion

        #region order skill
        public void OrderComboSkill(uint id, uint skillId, bool isPlayer)
        {
            if (!_comboSkillDic.ContainsKey(id))
            {
                if (isPlayer)
                    PlayerController.instance.CloneHero(id);
                else
                    EnemyController.instance.CloneEnemy(id);
                _comboSkillDic.Add(id, skillId);
            }
            else
                Debugger.LogError("已存在该角色" + id.ToString());
        }

        public void OrderAeonSkill(uint id, uint skillId)
        {
            DataMessageHandler.DataMessage_PlayAeonSkill(id, skillId);
        }

        public void OrderPlayerSkill(uint id, uint skillId, bool forceFirst)
        {
            switch (fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.FirstFight:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    if (fightStatus == FightStatus.Normal)
                    {
                        if (PlayerController.instance.heroDic.ContainsKey(id))
                        {
                            DataMessageHandler.DataMessage_OrderSkill(id, skillId, forceFirst);
                        }
                    }
                    else if (fightStatus == FightStatus.FloatWaiting || fightStatus == FightStatus.TumbleWaiting)
                    {
                        if (_comboSkillDic.ContainsKey(id))
                        {
                            DataMessageHandler.DataMessage_OrderSkill(id, skillId, forceFirst);
                        }
                        else
                        {
                            OrderComboSkill(id, skillId, true);
                        }
                    }
                    break;
                case FightType.ConsortiaFight:
                    ConsortiaFightController.instance.CLIENT2LOBBY_SKILL_REQ(Fight.Model.FightProxy.instance.fightId, (int)id, (int)skillId);
                    break;
            }
        }

#if UNITY_EDITOR
        [NoToLua]
        public void OrderEnemySkill(uint id, uint skillId, bool forceFirst)
        {
            if (EnemyController.instance.enemyDic.ContainsKey(id))
            {
                DataMessageHandler.DataMessage_OrderSkill(id, skillId, forceFirst);
            }
        }
#endif
        #endregion

        #region play skill
        public void PlayPlayerComboSkill(CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillID)
        {
            SkillInfo skillInfo = character.GetSkillInfoById(skillID);
            try
            {
                SkillAction skillAction = new SkillAction();
                skillAction.character = character;
                skillAction.skillInfo = skillInfo;
                skillAction.timelineList = timelineList;
                skillAction.isPlayer = true;
                MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(timelineList.First().First().Key);//第一个作用效果
                switch (skillInfo.animationData.animType)
                {
                    case AnimType.Root:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(character.transform, targetPos);
                                break;
                        }
                        Rotate(character.transform, character.pos + Vector3.right * 10, skillInfo.animationData.endShowTime);
                        break;
                    case AnimType.Trace:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(character.positionId);
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                Vector3 endPos = Vector3.zero;
                                if (skillInfo.animationData.isRotate)
                                {
                                    skillInfo.rotateAngles = Rotate(character.transform, targetPos);
                                    float distance = Vector3.Distance(startPos, targetPos);
                                    endPos = Vector3.Lerp(startPos, targetPos, (distance + skillInfo.animationData.offset) / distance);
                                }
                                else
                                {
                                    endPos = targetPos + new Vector3(skillInfo.animationData.offset, 0, 0);
                                }
                                float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, targetPositionId);
                                skillAction.positionRow = positionRow;
                                skillAction.endPos = endPos;
                                CharacterMove(character, skillInfo, startPos, endPos, positionRow, () =>
                                {
                                    MechanicsController.instance.PlayerMechanics(skillInfo, character, timelineList);
                                });
                                break;
                        }
                        break;
                    case AnimType.Run:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(character.positionId);
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 endPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(character.transform, endPos);
                                CharacterRun(character, skillInfo, startPos, endPos, skillInfo.animationData.offset, skillAction, timelineList, () =>
                                {
                                    MechanicsController.instance.PlayerMechanics(skillInfo, character, timelineList);
                                });
                                break;
                        }
                        break;
                }
                character.PlaySkill(skillInfo);
                character.SetStatus(Status.Skill);
                skillAction.Execute();
            }
            catch (System.Exception e)
            {
                Debugger.LogError("hero {0} play combo skill {1} fail!", character.characterInfo.instanceID, skillInfo.skillData.skillId);
                Debugger.LogError(e.StackTrace);
            }
            finally
            {
                if (skillInfo.animationData.animType != AnimType.Run)
                    FinishedComboSkill(character, skillInfo, skillInfo.animationData.length, true);//这里不能使用endshowtime,否则会卡死
            }
        }

        public void PlayPlayerSkill(uint cid, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillID)
        {
            if (!PlayerController.instance.heroDic.ContainsKey(cid))
            {
                Debugger.LogError("找不到该角色！！");
                Logic.UI.SkillBar.Controller.SkillBarController.instance.FinishSkill(cid, skillID);
                DataMessageHandler.DataMessage_FinishSkill(skillID, cid, true);
                return;
            }
            CharacterEntity character = PlayerController.instance[cid];
            if (character.isDead || character.HP <= 0)
            {
                Logic.Net.Controller.DataMessageHandler.DataMessage_ResetSkillOrder(character, skillID, true);
                return;
            }
            SkillInfo skillInfo = character.GetSkillInfoById(skillID);
            try
            {
                if (skillInfo.skillData.skillType == SkillType.Aeon)
                {
                    PlayerController.instance.HeroTrigger(skillInfo.animationData.length - skillInfo.skillData.pauseTime);
                    character = PlayerController.instance.CreateAeon(character);//偷梁换柱啦
                }
                SkillAction skillAction = new SkillAction();
                skillAction.character = character;
                skillAction.skillInfo = skillInfo;
                skillAction.timelineList = timelineList;
                skillAction.isPlayer = true;
                MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(timelineList.First().First().Key);//第一个作用效果
                switch (skillInfo.animationData.animType)
                {
                    case AnimType.Root:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(character.transform, targetPos);
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, false);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(character, ranges);
                                break;
                        }
                        //恢复原来转向
                        Rotate(character.transform, character.pos + Vector3.right * 10, skillInfo.animationData.endShowTime);
                        break;
                    case AnimType.Trace:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(character.positionId);
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                Vector3 endPos = Vector3.zero;
                                if (skillInfo.animationData.isRotate)
                                {
                                    skillInfo.rotateAngles = Rotate(character.transform, targetPos);
                                    float distance = Vector3.Distance(startPos, targetPos);
                                    endPos = Vector3.Lerp(startPos, targetPos, (distance + skillInfo.animationData.offset) / distance);
                                }
                                else
                                {
                                    endPos = targetPos + new Vector3(skillInfo.animationData.offset, 0, 0);
                                }
                                float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, targetPositionId);
                                skillAction.positionRow = positionRow;
                                skillAction.endPos = endPos;
                                CharacterMove(character, skillInfo, startPos, endPos, positionRow, () =>
                                {
                                    MechanicsController.instance.PlayerMechanics(skillInfo, character, timelineList);
                                });
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, false);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(character, ranges);
                                break;
                        }
                        break;
                    case AnimType.Run:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(character.positionId);
                                CharacterEntity target = EnemyController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 endPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(character.transform, endPos);
                                CharacterRun(character, skillInfo, startPos, endPos, skillInfo.animationData.offset, skillAction, timelineList, () =>
                                {
                                    MechanicsController.instance.PlayerMechanics(skillInfo, character, timelineList);
                                });
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, false);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(character, ranges);
                                break;
                        }
                        break;
                }
                character.PlaySkill(skillInfo);
                character.SetStatus(Status.Skill);
                skillAction.Execute();
            }
            catch (System.Exception e)
            {
                Debugger.LogError("hero {0} play skill {1} fail!", character.characterInfo.instanceID, skillInfo.skillData.skillId);
                Debugger.LogError(e.StackTrace);
            }
            finally
            {
                if (skillInfo.animationData.animType != AnimType.Run)
                {
                    FinishedSkill(character, skillInfo, skillInfo.animationData.endShowTime - skillInfo.skillData.pauseTime, true);
                    if (GameSetting.instance.closeupCameraable)
                    {
                        if (skillInfo.animationData.closeup)
                            StartCoroutine(FinishCloseupCameraCoroutine(skillInfo, character, timelineList, true));
                    }
                }
            }
        }

        public void PlayPlayerBootSkill(uint id, uint skillId)
        {
            Debugger.Log(id + " boot skill:" + skillId);
            BootSkillAction bootSkillAction = new BootSkillAction();
            HeroEntity hero = PlayerController.instance[id];
            SkillInfo skillInfo = new SkillInfo(skillId);
            hero.SetStatus(Status.BootSkill);
            bootSkillAction.character = hero;
            bootSkillAction.skillInfo = skillInfo;
            bootSkillAction.Execute();
        }

        public void PlayEnemyComboSkill(CharacterEntity enemy, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillID)
        {
            SkillInfo skillInfo = enemy.GetSkillInfoById(skillID);
            try
            {
                SkillAction skillAction = new SkillAction();
                skillAction.character = enemy;
                skillAction.skillInfo = skillInfo;
                skillAction.timelineList = timelineList;
                skillAction.isPlayer = false;
                MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(timelineList.First().First().Key);//第一个作用效果
                switch (skillInfo.animationData.animType)
                {
                    case AnimType.Root:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, false);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(enemy.transform, targetPos);
                                break;
                        }
                        Rotate(enemy.transform, enemy.pos + Vector3.left * 10, skillInfo.animationData.endShowTime);
                        break;
                    case AnimType.Trace:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(enemy.positionId);
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, false);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                Vector3 endPos = Vector3.zero;
                                if (skillInfo.animationData.isRotate)
                                {
                                    skillInfo.rotateAngles = Rotate(enemy.transform, targetPos);
                                    float distance = Vector3.Distance(startPos, targetPos);
                                    endPos = Vector3.Lerp(startPos, targetPos, (distance + skillInfo.animationData.offset) / distance);
                                }
                                else
                                {
                                    endPos = targetPos - new Vector3(skillInfo.animationData.offset, 0, 0);
                                }
                                float positionRow = PositionData.GetEnemyPositionLevels(enemy.positionId, targetPositionId);
                                skillAction.positionRow = positionRow;
                                skillAction.endPos = endPos;
                                CharacterMove(enemy, skillInfo, startPos, endPos, positionRow, () =>
                                {
                                    MechanicsController.instance.EnemyMechanics(skillInfo, enemy, timelineList);
                                });
                                break;
                        }
                        break;
                    case AnimType.Run:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(enemy.positionId);
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, true);
                                Vector3 endPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(enemy.transform, endPos);
                                CharacterRun(enemy, skillInfo, startPos, endPos, skillInfo.animationData.offset, skillAction, timelineList, () =>
                                {
                                    MechanicsController.instance.EnemyMechanics(skillInfo, enemy, timelineList);
                                });
                                break;
                        }
                        break;
                }
                enemy.PlaySkill(skillInfo);
                enemy.SetStatus(Status.Skill);
                skillAction.Execute();
            }
            catch (System.Exception e)
            {
                Debugger.LogError("enemy {0} play combo skill {1} fail!", enemy.characterInfo.instanceID, skillInfo.skillData.skillId);
                Debugger.LogError(e.StackTrace);
            }
            finally
            {
                if (skillInfo.animationData.animType != AnimType.Run)
                    FinishedComboSkill(enemy, skillInfo, skillInfo.animationData.length, true);//这里不能使用endshowtime,否则会卡死
            }
        }


        public void PlayEnemySkill(uint id, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillId)
        {
            if (!EnemyController.instance.enemyDic.ContainsKey(id))
            {
                Debugger.LogError("找不到该角色！！");
                DataMessageHandler.DataMessage_FinishSkill(skillId, id, false);
                return;
            }
            CharacterEntity enemy = EnemyController.instance[id];
            if (enemy.isDead || enemy.HP <= 0)
            {
                Logic.Net.Controller.DataMessageHandler.DataMessage_ResetSkillOrder(enemy, skillId, false);
                return;
            }
            SkillInfo skillInfo = enemy.GetSkillInfoById(skillId);
            try
            {
                SkillAction skillAction = new SkillAction();
                skillAction.character = enemy;
                skillAction.skillInfo = skillInfo;
                skillAction.timelineList = timelineList;
                skillAction.isPlayer = false;
                MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(timelineList.First().First().Key);//第一个作用效果
                switch (skillInfo.animationData.animType)
                {
                    case AnimType.Root:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, false);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(enemy.transform, targetPos);
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, true);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(enemy, ranges);
                                break;
                        }
                        Rotate(enemy.transform, enemy.pos + Vector3.left * 10, skillInfo.animationData.endShowTime);
                        break;
                    case AnimType.Trace:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(enemy.positionId);
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, false);
                                Vector3 targetPos = PositionData.GetPos(targetPositionId);
                                Vector3 endPos = Vector3.zero;
                                if (skillInfo.animationData.isRotate)
                                {
                                    skillInfo.rotateAngles = Rotate(enemy.transform, targetPos);
                                    float distance = Vector3.Distance(startPos, targetPos);
                                    endPos = Vector3.Lerp(startPos, targetPos, (distance + skillInfo.animationData.offset) / distance);
                                }
                                else
                                {
                                    endPos = targetPos - new Vector3(skillInfo.animationData.offset, 0, 0);
                                }
                                float positionRow = PositionData.GetEnemyPositionLevels(enemy.positionId, targetPositionId);
                                skillAction.positionRow = positionRow;
                                skillAction.endPos = endPos;
                                CharacterMove(enemy, skillInfo, startPos, endPos, positionRow, () =>
                                {
                                    MechanicsController.instance.EnemyMechanics(skillInfo, enemy, timelineList);
                                });
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, true);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(enemy, ranges);
                                break;
                        }
                        break;
                    case AnimType.Run:
                        switch (mechanicsData.targetType)
                        {
                            case TargetType.Ally:

                                break;
                            case TargetType.Enemy:
                                Vector3 startPos = PositionData.GetPos(enemy.positionId);
                                CharacterEntity target = PlayerController.instance[timelineList.First().First().Value.First().Key];//第一个作用效果目标
                                uint targetPositionId = GetMoveTargetPositionId(mechanicsData, target, false);
                                Vector3 endPos = PositionData.GetPos(targetPositionId);
                                skillInfo.rotateAngles = Rotate(enemy.transform, endPos);
                                CharacterRun(enemy, skillInfo, startPos, endPos, skillInfo.animationData.offset, skillAction, timelineList, () =>
                                {
                                    MechanicsController.instance.EnemyMechanics(skillInfo, enemy, timelineList);
                                });
                                List<uint> ranges = CharacterUtil.GetTargetRangePositionIds(target, mechanicsData, true);
                                Effect.Controller.EffectController.instance.ShowTargetRangeTips(enemy, ranges);
                                break;
                        }
                        break;
                }
                enemy.PlaySkill(skillInfo);
                enemy.SetStatus(Status.Skill);
                skillAction.Execute();
            }
            catch (System.Exception e)
            {
                FinishedSkill(enemy, skillInfo, 0f, true);
                Debugger.LogError("enemy {0} play skill {1} fail!", enemy.characterInfo.instanceID, skillInfo.skillData.skillId);
                Debugger.LogError(e.StackTrace);
            }
            finally
            {
                if (skillInfo.animationData.animType != AnimType.Run)
                    FinishedSkill(enemy, skillInfo, skillInfo.animationData.endShowTime - skillInfo.skillData.pauseTime, false);
                if (GameSetting.instance.closeupCameraable)
                {
                    if (skillInfo.animationData.closeup && EnemyController.instance.isBoss(enemy.characterInfo.instanceID))
                        StartCoroutine(FinishCloseupCameraCoroutine(skillInfo, enemy, timelineList, false));
                }
            }
        }

        public void PlayEnemyBootSkill(uint id, uint skillId)
        {
            Debugger.Log(id + " boot skill:" + skillId);
            BootSkillAction bootSkillAction = new BootSkillAction();
            EnemyEntity enemy = EnemyController.instance[id];
            SkillInfo skillInfo = new SkillInfo(skillId);
            enemy.SetStatus(Status.BootSkill);
            bootSkillAction.character = enemy;
            bootSkillAction.skillInfo = skillInfo;
            bootSkillAction.Execute();
        }

        private static uint GetMoveTargetPositionId(MechanicsData mechanicsData, CharacterEntity target, bool isPlayer)
        {
            uint targetPositionId = 0;
            switch (mechanicsData.rangeType)
            {
                case RangeType.CurrentSingle:
                case RangeType.CurrentRow:
                case RangeType.All:
                case RangeType.CurrentAndBehindFirst:
                case RangeType.CurrentBehindFirst:
                case RangeType.CurrentBehindSecond:
                case RangeType.CurrentIntervalOne:
                case RangeType.CurrentAndRandomTwo:
                case RangeType.Weakness:
                case RangeType.LowestHP:
                case RangeType.RandomSingle:
                case RangeType.CurrentAndBehindTowColum:
                case RangeType.CurrentAndNearCross:
                    targetPositionId = target.positionId;
                    break;
                case RangeType.CurrentColumn:
                case RangeType.RandomN:
                case RangeType.Cross:
                case RangeType.FirstColum:
                case RangeType.SecondColum:
                case RangeType.ThirdColum:
                case RangeType.SecondRow:
                case RangeType.ExceptMidpoint:
                case RangeType.Midpoint:
                case RangeType.AllAbsolutely:
                case RangeType.BehindTowColum:
                    if (isPlayer)
                        targetPositionId = (uint)FormationPosition.Enemy_Position_2;
                    else
                        targetPositionId = (uint)FormationPosition.Player_Position_2;
                    break;
                case RangeType.FirstRow:
                case RangeType.LeadingDiagonal:
                    if (isPlayer)
                        targetPositionId = (uint)FormationPosition.Enemy_Position_1;
                    else
                        targetPositionId = (uint)FormationPosition.Player_Position_1;
                    break;
                case RangeType.ThirdRow:
                case RangeType.SecondaryDiagonal:
                    if (isPlayer)
                        targetPositionId = (uint)FormationPosition.Enemy_Position_3;
                    else
                        targetPositionId = (uint)FormationPosition.Player_Position_3;
                    break;
            }
            return targetPositionId;
        }

        #region character move trace
        private void CharacterMove(CharacterEntity character, SkillInfo skillInfo, Vector3 startPos, Vector3 endPos, float positionRow, System.Action callback)
        {
#if UNITY_EDITOR
            _startPos = startPos;
            _endPos = endPos;
#endif
            switch (fightStatus)
            {
                case FightStatus.Normal:
                    StartCoroutine(CharacterMoveCoroutine(startPos, endPos, skillInfo, character, positionRow, callback));
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    StartCoroutine(CharacterComboMoveCoroutine(startPos, endPos, skillInfo, character, positionRow, callback));
                    break;
            }
        }

        private void CharacterRun(CharacterEntity character, SkillInfo skillInfo, Vector3 startPos, Vector3 targetPos, float offset, SkillAction skillAction, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, System.Action callback)
        {
#if UNITY_EDITOR
            _startPos = startPos;
            _endPos = targetPos;
#endif
            switch (fightStatus)
            {
                case FightStatus.Normal:
                    StartCoroutine(CharacterRunCoroutine(startPos, targetPos, offset, skillInfo, character, skillAction, timelineList, callback));
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    StartCoroutine(CharacterComboRunCoroutine(startPos, targetPos, offset, skillInfo, character, skillAction, callback));
                    break;
            }
        }

        private IEnumerator CharacterMoveCoroutine(Vector3 startPos, Vector3 endPos, SkillInfo skillInfo, CharacterEntity character, float positionRow, System.Action callback = null)
        {
            if (skillInfo.skillData.pauseTime > 0)
            {
                float delay = skillInfo.skillData.pauseTime / GameSetting.instance.speed;
                float lastTime = Time.realtimeSinceStartup;
                float currentTime = lastTime;
                if (currentTime - lastTime < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            if (skillInfo.animationData.moveTime > 0)
                yield return new WaitForSeconds(skillInfo.animationData.moveTime - skillInfo.skillData.pauseTime);
#if UNITY_EDITOR
            _moveTargets.Clear();
            _backs.Clear();
#endif
            float delta = GameSetting.instance.deltaTimeFight;
            WaitForSeconds waitForSeconds = new WaitForSeconds(delta);
            //float distance = Vector3.Distance(startPos, targetPos);
            //Vector3 endPos = Vector3.Lerp(startPos, targetPos, (distance + offset) / distance);
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 normal = (endPos - startPos).normalized;
            normal.y = 0;
            float moveCost = (skillInfo.animationData.hitTime - skillInfo.animationData.moveTime);
            float time = moveCost * (positionRow * GameConfig.timePercent + 1f);
            float speed = distance / (time / delta) * 2;//游戏帧率为60，动画为30，时间为动画的一半
            Common.Animators.AnimatorSpeedScaler.ScaleSpeed(character.anim, time, moveCost / time);
            Vector3 speedVector3 = speed * normal;
            Vector3 curPos = startPos;
            #region player
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    yield return waitForSeconds;
                    curPos += speedVector3;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    if (character)
                    {
                        character.pos = curPos;
                    }
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                yield return new WaitForSeconds(skillInfo.animationData.backTime - skillInfo.animationData.hitTime);
                normal *= -1;
                float backCostTime = (skillInfo.animationData.endShowTime - skillInfo.animationData.backTime);
                float backTime = backCostTime * (positionRow * GameConfig.timePercent + 1f);
                Common.Animators.AnimatorSpeedScaler.ScaleSpeed(character.anim, backTime, backCostTime / backTime);
                /* 80%距离开始减速到0，回到原地
                 * 推导出数据如下：
                 * S = S1 + S2
                 * S1 = 0.8S
                 * S2 = 0.2S
                 * t = t1 + t2
                 * v1 = 1.2 S/t
                 * 加速度a = -3.6S/t^2
                */
                float a = -3.6f * distance / Mathf.Pow(backTime / delta, 2);
                Vector3 aVector = a * normal;
                float v1 = 1.2f * distance / (backTime / delta);
                Vector3 S1Vector = Vector3.Slerp(endPos, startPos, 0.8f);
                Vector3 v1Vector = v1 * normal;
                while (curPos.x >= S1Vector.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    curPos += v1Vector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (!character.moveBroken)
                    if (character)
                        character.pos = S1Vector;
                curPos = S1Vector;
                int deltaCount = 0;
                Vector3 instantaneous = Vector3.zero;
                while (curPos.x >= startPos.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    deltaCount++;
                    instantaneous = v1Vector + aVector * deltaCount;
                    if (instantaneous.x > 0)
                        break;
                    curPos += instantaneous;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                {
                    character.pos = startPos;
                    Rotate(character.transform, character.pos + Vector3.right * 10);
                }
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            #endregion
            #region enmemy
            else
            {
                while (curPos.x > endPos.x)
                {
                    yield return waitForSeconds;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    curPos += speedVector3;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                yield return new WaitForSeconds(skillInfo.animationData.backTime - skillInfo.animationData.hitTime);
                normal *= -1;
                float backCostTime = (skillInfo.animationData.endShowTime - skillInfo.animationData.backTime);
                float backTime = backCostTime * (positionRow * GameConfig.timePercent + 1f);
                Common.Animators.AnimatorSpeedScaler.ScaleSpeed(character.anim, backTime, backCostTime / backTime);
                /* 80%距离开始减速到0，回到原地
                 * 推导出数据如下：
                 * S = S1 + S2
                 * S1 = 0.8S
                 * S2 = 0.2S
                 * t = t1 + t2
                 * v1 = 1.2 S/t
                 * 加速度a = -3.6S/t^2
                */
                float a = -3.6f * distance / Mathf.Pow(backTime / delta, 2);
                Vector3 aVector = a * normal;
                float v1 = 1.2f * distance / (backTime / delta);
                Vector3 S1Vector = Vector3.Slerp(endPos, startPos, 0.8f);
                Vector3 v1Vector = v1 * normal;
                while (curPos.x <= S1Vector.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    curPos += v1Vector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (!character.moveBroken)
                    if (character)
                        character.pos = S1Vector;
                curPos = S1Vector;
                int deltaCount = 0;
                Vector3 instantaneous = Vector3.zero;
                while (curPos.x <= startPos.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    deltaCount++;
                    instantaneous = v1Vector + aVector * deltaCount;
                    if (instantaneous.x < 0)
                        break;
                    curPos += instantaneous;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                {
                    character.pos = startPos;
                    Rotate(character.transform, character.pos + Vector3.left * 10);
                }
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            if (character)
                character.moveBroken = false;
            #endregion
        }

        private IEnumerator CharacterRunCoroutine(Vector3 startPos, Vector3 targetPos, float offset, SkillInfo skillInfo, CharacterEntity character, SkillAction skillAction, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, System.Action callback = null)
        {
            if (skillInfo.skillData.pauseTime > 0)
            {
                float lastTime = Time.realtimeSinceStartup;
                float delay = skillInfo.skillData.pauseTime / GameSetting.instance.speed;
                float currentTime = lastTime;
                if (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            if (skillInfo.animationData.moveTime > 0)
                yield return new WaitForSeconds(skillInfo.animationData.moveTime - skillInfo.skillData.pauseTime);
#if UNITY_EDITOR
            _moveTargets.Clear();
            _backs.Clear();
#endif
            float delta = GameSetting.instance.deltaTimeFight;
            WaitForSeconds waitForSeconds = new WaitForSeconds(delta);
            float distance = Vector3.Distance(startPos, targetPos);
            Vector3 endPos = Vector3.Lerp(startPos, targetPos, (distance + offset) / distance);
            Vector3 normal = (targetPos - startPos).normalized;
            normal.y = 0;

            AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + AnimatorUtil.RUN), 0, 0f);
            float speed = skillInfo.animationData.runSpeed / (1 / delta);
            Vector3 speedVector3 = speed * normal;
            Vector3 curPos = startPos;
            int layer = (int)LayerType.Fight;
            #region player
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    yield return waitForSeconds;
                    curPos += speedVector3;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + skillInfo.animationData.animName), 0, 0f);

                if (GameSetting.instance.closeupCameraable)
                {
                    if (FightController.instance.fightStatus == FightStatus.Normal)
                    {
                        if (skillInfo.animationData.closeup)
                            layer = (int)LayerType.Closeup;
                    }
                }
                skillAction.PlayCasterEffect(layer);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                yield return new WaitForSeconds(skillInfo.animationData.length);
                normal *= -1;
                if (character)
                    Rotate(character.transform, startPos);
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + AnimatorUtil.RUN), 0, 0f);
                Vector3 backSpeedVector = speed * normal;
                while (curPos.x >= startPos.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    curPos += backSpeedVector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                {
                    character.pos = startPos;
                    Rotate(character.transform, character.pos + Vector3.right * 10);
                }
                AnimatorUtil.Play(character.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                FinishedSkill(character, skillInfo, 0f, true);
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            #endregion
            #region enmemy
            else
            {
                while (curPos.x > endPos.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    curPos += speedVector3;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + skillInfo.animationData.animName), 0, 0f);
                if (GameSetting.instance.closeupCameraable)
                {
                    if (FightController.instance.fightStatus == FightStatus.Normal)
                    {
                        if (skillInfo.animationData.closeup && EnemyController.instance.isBoss(character.characterInfo.instanceID))
                            layer = (int)LayerType.Closeup;
                    }
                }
                skillAction.PlayCasterEffect(layer);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                yield return new WaitForSeconds(skillInfo.animationData.length);
                normal *= -1;
                if (character)
                    Rotate(character.transform, startPos);
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + AnimatorUtil.RUN), 0, 0f);
                Vector3 backSpeedVector = speed * normal;
                while (curPos.x <= startPos.x)
                {
                    if (character.moveBroken)
                        break;
                    yield return waitForSeconds;
                    curPos += backSpeedVector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                {
                    character.pos = startPos;
                    Rotate(character.transform, character.pos + Vector3.left * 10);
                }
                AnimatorUtil.Play(character.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                FinishedSkill(character, skillInfo, 0f, false);
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            character.moveBroken = false;
            #endregion
        }

        private IEnumerator CharacterComboMoveCoroutine(Vector3 startPos, Vector3 endPos, SkillInfo skillInfo, CharacterEntity character, float positionRow, System.Action callback = null)
        {
            float lastTime = Time.realtimeSinceStartup;
            float delay = skillInfo.animationData.moveTime;//include skill puase time;
            delay = delay / GameSetting.instance.speed;
            float currentTime = lastTime;
            while (Time.realtimeSinceStartup - lastTime < delay)
            {
                yield return null;
                if (TimeController.instance.playerPause)
                    lastTime += (Time.realtimeSinceStartup - currentTime);
                currentTime = Time.realtimeSinceStartup;
            }
            lastTime = Time.realtimeSinceStartup;
            float delta = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            //float distance = Vector3.Distance(startPos, targetPos);
            //Vector3 endPos = Vector3.Lerp(startPos, targetPos, (distance + offset) / distance);
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 normal = (endPos - startPos).normalized;
            normal.y = 0;
            float moveCost = (skillInfo.animationData.hitTime - skillInfo.animationData.moveTime);
            float time = moveCost * (positionRow * GameConfig.timePercent + 1f);
            float speed = distance / (time / delta) * 2;
            Vector3 speedVector3 = speed * normal;
            Vector3 curPos = startPos;
            #region player
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += speedVector3;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                lastTime = Time.realtimeSinceStartup;
                delay = skillInfo.animationData.backTime - skillInfo.animationData.hitTime;
                delay /= GameSetting.instance.speed;
                currentTime = lastTime;
                while (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                lastTime = Time.realtimeSinceStartup;
                normal *= -1;
                float backCostTime = (skillInfo.animationData.endShowTime - skillInfo.animationData.backTime);
                float backTime = backCostTime * (positionRow * GameConfig.timePercent + 1f);
                float backSpeed = distance / (backTime / delta) * 2;
                Vector3 backSpeedVector = backSpeed * normal;
                while (curPos.x >= startPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += backSpeedVector;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = startPos;
            }
            #endregion
            #region enemy
            else
            {
                while (curPos.x > endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += speedVector3;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                lastTime = Time.realtimeSinceStartup;
                delay = skillInfo.animationData.backTime - skillInfo.animationData.hitTime;
                delay /= GameSetting.instance.speed;
                currentTime = lastTime;
                while (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                lastTime = Time.realtimeSinceStartup;
                normal *= -1;
                float backCostTime = (skillInfo.animationData.endShowTime - skillInfo.animationData.backTime);
                float backTime = backCostTime * (positionRow * GameConfig.timePercent + 1f);
                float backSpeed = distance / (backTime / delta) * 2;
                Vector3 backSpeedVector = backSpeed * normal;
                while (curPos.x <= startPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += backSpeedVector;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = startPos;
            }
            #endregion
        }

        private IEnumerator CharacterComboRunCoroutine(Vector3 startPos, Vector3 targetPos, float offset, SkillInfo skillInfo, CharacterEntity character, SkillAction skillAction, System.Action callback = null)
        {
            float lastTime = Time.realtimeSinceStartup;
            float delay = skillInfo.animationData.moveTime;//include skill pause time;
            delay /= GameSetting.instance.speed;
            float currentTime = lastTime;
            while (Time.realtimeSinceStartup - lastTime < delay)
            {
                yield return null;
                if (TimeController.instance.playerPause)
                    lastTime += (Time.realtimeSinceStartup - currentTime);
                currentTime = Time.realtimeSinceStartup;
            }
            lastTime = Time.realtimeSinceStartup;
#if UNITY_EDITOR
            _moveTargets.Clear();
            _backs.Clear();
#endif
            float delta = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            float distance = Vector3.Distance(startPos, targetPos);
            Vector3 endPos = Vector3.Lerp(startPos, targetPos, (distance + offset) / distance);
            Vector3 normal = (targetPos - startPos).normalized;
            normal.y = 0;
            AnimatorUtil.Play(character.anim, AnimatorUtil.RUN_ID, 0, 0f);
            float speed = skillInfo.animationData.runSpeed / (1 / delta);
            Vector3 speedVector3 = speed * normal;
            Vector3 curPos = startPos;
            int layer = (int)LayerType.FightCombo;
            #region player
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += speedVector3;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + skillInfo.animationData.animName), 0, 0f);
                skillAction.PlayCasterEffect(layer);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                lastTime = Time.realtimeSinceStartup;
                delay = skillInfo.animationData.backTime - skillInfo.animationData.hitTime;
                delay /= GameSetting.instance.speed;
                currentTime = lastTime;
                while (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                lastTime = Time.realtimeSinceStartup;
                normal *= -1;
                Rotate(character.transform, startPos);
                AnimatorUtil.Play(character.anim, AnimatorUtil.RUN_ID, 0, 0f);
                Vector3 backSpeedVector = speed * normal;
                while (curPos.x >= startPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += backSpeedVector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = startPos;
                Rotate(character.transform, character.pos + Vector3.right * 10);
                AnimatorUtil.Play(character.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                FinishedComboSkill(character, skillInfo, 0f, true);
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            #endregion
            #region enmemy
            else
            {
                while (curPos.x > endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
#if UNITY_EDITOR
                    _moveTargets.Add(curPos);
#endif
                    curPos += speedVector3;
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = endPos;
                curPos = endPos;
#if UNITY_EDITOR
                _moveTargets.Add(endPos);
#endif
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + skillInfo.animationData.animName), 0, 0f);
                skillAction.PlayCasterEffect(layer);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
                lastTime = Time.realtimeSinceStartup;
                delay = skillInfo.animationData.backTime - skillInfo.animationData.hitTime;
                delay /= GameSetting.instance.speed;
                currentTime = lastTime;
                while (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    lastTime += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                lastTime = Time.realtimeSinceStartup;
                normal *= -1;
                Rotate(character.transform, startPos);
                AnimatorUtil.Play(character.anim, AnimatorUtil.RUN_ID, 0, 0f);
                Vector3 backSpeedVector = speed * normal;
                while (curPos.x <= startPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < delta)
                    {
                        yield return null;
                        continue;
                    }
                    else
                    {
                        lastTime = Time.realtimeSinceStartup;
                    }
                    if (TimeController.instance.playerPause) continue;
                    curPos += backSpeedVector;
#if UNITY_EDITOR
                    _backs.Add(curPos);
#endif
                    if (character)
                        character.pos = curPos;
                    else
                        break;
                }
                if (character)
                    character.pos = startPos;
                Rotate(character.transform, character.pos + Vector3.left * 10);
                AnimatorUtil.Play(character.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                FinishedComboSkill(character, skillInfo, 0f, false);
#if UNITY_EDITOR
                _backs.Add(startPos);
#endif
            }
            #endregion
        }
        #endregion

        public void BreakBootSkill(CharacterEntity character)
        {
            DataMessageHandler.DataMessage_BreakBootSkill(character.characterInfo.instanceID);
        }

        private void FinishedSkill(CharacterEntity character, SkillInfo skillInfo, float delay, bool isPlayer)
        {
            if (delay > 0)
                StartCoroutine(FinishedSkillCoroutine(character, skillInfo, delay, isPlayer));
            else
                FinishedSkill(character, skillInfo, isPlayer);
        }

        private IEnumerator FinishedSkillCoroutine(CharacterEntity character, SkillInfo skillInfo, float delay, bool isPlayer)
        {
            yield return new WaitForSeconds(delay);
            FinishedSkill(character, skillInfo, isPlayer);
        }

        public void ForceFinishedSkill(uint id, uint skillId, bool isPlayer)
        {
            CharacterEntity character = null;
            if (isPlayer)
                character = PlayerController.instance[id];
            else
                character = EnemyController.instance[id];
            if (character)
            {
                SkillInfo skillInfo = character.GetSkillInfoById(skillId);
                FinishedSkill(character, skillInfo, isPlayer);
            }
        }

        private void FinishedSkill(CharacterEntity character, SkillInfo skillInfo, bool isPlayer)
        {
            #region fight imitate
#if UNITY_EDITOR
            if (imitate)
            {
                if (hitCounts.Count <= currentIndex)
                    hitCounts.Add(new Dictionary<string, uint>());
                if (skillCounts.Count <= currentIndex)
                    skillCounts.Add(new Dictionary<string, uint>());
                Dictionary<string, uint> hitCountDic = hitCounts[currentIndex];
                Dictionary<string, uint> skillCountDic = skillCounts[currentIndex];
                string key = character.characterInfo.instanceID.ToString() + "_" + character.characterName;
                switch (skillInfo.skillData.skillType)
                {
                    case SkillType.Hit:
                        if (!hitCountDic.ContainsKey(key))
                            hitCountDic[key] = 1;
                        else
                            hitCountDic[key]++;
                        break;
                    case SkillType.Skill:
                    case SkillType.Aeon:
                        if (!skillCountDic.ContainsKey(key))
                            skillCountDic[key] = 1;
                        else
                            skillCountDic[key]++;
                        break;
                }
            }
#endif
            #endregion
            Effect.Controller.EffectController.instance.HideTargetRangeTips(character);
            character.ResetCD(skillInfo);
            character.ResetStatus();
            character.UpdateAttackBuffs();
            if (skillInfo.skillData.skillType == SkillType.Aeon)
                PlayerController.instance.ClearAeons(false);
            if (isPlayer)
                Logic.UI.SkillBar.Controller.SkillBarController.instance.FinishSkill(character.characterInfo.instanceID, skillInfo.skillData.skillId);
            DataMessageHandler.DataMessage_FinishSkill(skillInfo.skillData.skillId, character.characterInfo.instanceID, isPlayer);
            if (finishedSkillHandler != null)
                finishedSkillHandler(skillInfo);
        }

        private void FinishedComboSkill(CharacterEntity character, SkillInfo skillInfo, float delay, bool isPlayer)
        {
            if (delay > 0)
                StartCoroutine(FinishedComboSkillCoroutine(character, skillInfo, delay, isPlayer));
            else
                FinishedComboSkill(character, skillInfo, isPlayer);
        }

        private IEnumerator FinishedComboSkillCoroutine(CharacterEntity character, SkillInfo skillInfo, float delay, bool isPlayer)
        {
            float time = Time.realtimeSinceStartup;
            delay /= GameSetting.instance.speed;
            float currentTime = time;
            while (Time.realtimeSinceStartup - time < delay)
            {
                yield return null;
                if (TimeController.instance.playerPause)
                    time += (Time.realtimeSinceStartup - currentTime);
                currentTime = Time.realtimeSinceStartup;
            }
            FinishedComboSkill(character, skillInfo, isPlayer);
        }

        private void FinishedComboSkill(CharacterEntity character, SkillInfo skillInfo, bool isPlayer)
        {
            #region fight imitate
#if UNITY_EDITOR
            if (imitate)
            {
                if (skillCounts.Count <= currentIndex)
                    skillCounts.Add(new Dictionary<string, uint>());
                Dictionary<string, uint> skillCountDic = skillCounts[currentIndex];
                string key = character.characterInfo.instanceID.ToString() + "_" + character.characterName;
                switch (skillInfo.skillData.skillType)
                {
                    case SkillType.Skill:
                    case SkillType.Aeon:
                        if (!skillCountDic.ContainsKey(key))
                            skillCountDic[key] = 1;
                        else
                            skillCountDic[key]++;
                        break;
                }
            }
#endif
            #endregion
            character.UpdateAttackBuffs();
            if (isPlayer)
            {
                PlayerController.instance.ResetComboSkill(character.characterInfo.instanceID, skillInfo.skillData.skillId);
                Logic.UI.SkillBar.Controller.SkillBarController.instance.FinishSkill(character.characterInfo.instanceID, skillInfo.skillData.skillId);
            }
            else
                EnemyController.instance.ResetComboSkill(character.characterInfo.instanceID, skillInfo.skillData.skillId);
            if (_comboSkillDic.Count == _comboTimes)
            {
                if (Game.GameSetting.instance.speedMode == GameSpeedMode.Normal)
                {
                    FinishedCombo(comboerIsPlayer);
                    if (finishedComboSkillHandler != null)
                        finishedComboSkillHandler(skillInfo);
                }
            }
        }

        public void FinishedComboSkillMechanics(CharacterEntity character, SkillInfo skillInfo, bool isPlayer)
        {
            _comboTimes++;
            if (_comboTimes == _comboSkillDic.Count)//最后一个打击点结束
            {
                ResetComboTargetAnimatorState();
                if (Game.GameSetting.instance.speedMode == GameSpeedMode.Double || Game.GameSetting.instance.speedMode == GameSpeedMode.Triple)
                {
                    FinishedCombo(comboerIsPlayer);
                    if (finishedComboSkillHandler != null)
                        finishedComboSkillHandler(skillInfo);
                }
            }
            if (finishedComboSkillMechanicsHandler != null)
                finishedComboSkillMechanicsHandler(skillInfo);
        }

        public void FinishedSkillMechanics(SkillInfo skillInfo, CharacterEntity character, bool isPlayer)
        {
            DataMessageHandler.DataMessage_FinishedSkillMechanics(skillInfo.skillData.skillId, character.characterInfo.instanceID, isPlayer);
            if (finishedSkillMechanicsHandler != null)
                finishedSkillMechanicsHandler(skillInfo);
        }

        //完成受击
        public void FinishedMechanicsed(uint characterId, bool isPlayer)
        {
            CharacterEntity character = null;
            if (isPlayer)
                character = PlayerController.instance[characterId];
            else
                character = EnemyController.instance[characterId];
            if (character)
                character.ResetStatus();
        }
        #endregion

        #region move skill trace
#if UNITY_EDITOR
        private Vector3 _startPos, _endPos, _normal;
        private List<Vector3> _moveTargets = new List<Vector3>();
        private List<Vector3> _backs = new List<Vector3>();
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_startPos, 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_startPos, _endPos);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_endPos, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_startPos, _endPos);

            foreach (var v in _moveTargets)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }

            Gizmos.color = Color.black;
            foreach (var v in _backs)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }
#endif
        #endregion
    }
}