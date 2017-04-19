using UnityEngine;
using System.Collections.Generic;
using Logic.Character;
using Logic.UI.ComboBar.View;
using Logic.UI.Damage.View;
using Common.ResMgr;
using PathologicalGames;
using Logic.Skill.Model;
using Common.Util;
using Logic.Enums;
using System.Collections;
using Logic.Character.Controller;
using Logic.Character.Model;
using LuaInterface;
using Logic.Effect.Model;
using Common.GameTime.Controller;
using Logic.Fight.Controller;
using Logic.Pool.Controller;
namespace Logic.Judge.Controller
{
    public class JudgeController : SingletonMono<JudgeController>
    {
        private Dictionary<uint, float> _lastAttackTimeDic = new Dictionary<uint, float>();
        private Dictionary<uint, uint> _comboTimesDic = new Dictionary<uint, uint>();
        private Transform _damageBarViewPool;
        private SpawnPool _spawnPool;
        private PrefabPool _prefabPool;
        private GameObject _prefab;
        private const string POOL_NAME = "damageBarViews";
        private uint _totalDamage = 0;
        private uint _comboCount = 0;
        #region fight imitate
#if UNITY_EDITOR
        public List<Dictionary<string, uint>> hitDamages = new List<Dictionary<string, uint>>();
        public List<Dictionary<string, uint>> skillDamages = new List<Dictionary<string, uint>>();
        public void InitImitateData()
        {
            hitDamages.Clear();
            skillDamages.Clear();
        }

        public void ClaerImitateStatisticData()
        {
            InitImitateData();
        }
        private int currentIndex
        {
            get
            {
                return Fight.Controller.FightController.instance.currentIndex;
            }
        }
#endif
        #endregion
        public uint totalDamage
        {
            get { return _totalDamage; }
        }

        public uint comboCount
        {
            get { return _comboCount; }
        }
        void Awake()
        {
            instance = this;
        }

        public void InitFightData()
        {
            _comboCount = 0;
            _totalDamage = 0;
            InitDamageBarViewPool();
            Logic.UI.Buff.Controller.BuffTipsController.instance.InitBuffTipsViewPool();
            StartUpdateContinuousBuffs();
            OpenComboBarView();
        }

        public void DestroyFightData()
        {
            ResetDamageBarViewPool();
            Logic.UI.Buff.Controller.BuffTipsController.instance.ResetBuffTipsViewPool();
            StopUpdateContinuousBuffs();
        }

        #region object pool
        private void InitDamageBarViewPool()
        {
            if (!_damageBarViewPool)
            {
                //DestoryDamageBarViewPool();
                GameObject go = new GameObject(POOL_NAME);
                _damageBarViewPool = go.transform;
                _damageBarViewPool.SetParent(UI.UIMgr.instance.basicCanvas.transform, false);
                //create _spawnPoll
                _spawnPool = PoolController.instance.CreatePool(POOL_NAME, DamageBarView.PREFAB_PATH, true);
                _spawnPool.matchPoolScale = true;

                _prefab = ResMgr.instance.Load<GameObject>(DamageBarView.PREFAB_PATH);
                _prefabPool = new PrefabPool(_prefab.transform);
                _prefabPool.preloadAmount = 5;//默认初始化5个Prefab
                _prefabPool.limitInstances = true;//开启限制
                _prefabPool.limitFIFO = true;//开启无限取Prefab
                _prefabPool.limitAmount = 20; //限制池子里最大的Prefab数量
                _prefabPool.preloadTime = true;//开启预加载
                _prefabPool.preloadFrames = 2;//每帧加载个数
                _prefabPool.preloadDelay = 2;//延迟几秒开始预加载
                _prefabPool.cullDespawned = true;//开启自动清理
                _prefabPool.cullAbove = 5;//缓存池自动清理，但是始终保存几个对象不清理
                _prefabPool.cullDelay = 10;//每过多久执行一次清理(销毁)，单位秒
                _prefabPool.cullMaxPerPass = 2;//每次自动清理个数
                //初始化内存池
                //_spawnPool._perPrefabPoolOptions.Add(_prefabPool);
                _spawnPool.CreatePrefabPool(_prefabPool);
            }
        }

        private void ResetDamageBarViewPool()
        {
            foreach (var kvp in _spawnPool.prefabPools)
            {
                for (int i = 0, count = kvp.Value.despawned.Count; i < count; i++)
                {
                    kvp.Value.despawned[i].SetParent(_spawnPool.transform, false);
                }
            }
        }

        private void OnDestroy()
        {
            if (_damageBarViewPool)
                GameObject.Destroy(_damageBarViewPool.gameObject);
            _damageBarViewPool = null;
            _spawnPool = null;
            _prefabPool = null;
            _prefab = null;
        }

        public void DespawnDamageBarView(Transform prefab)
        {
            if (_spawnPool)
                _spawnPool.Despawn(prefab);
        }

        public void ShowDamageBarViewPool(bool show)
        {
            //if (_damageBarViewPool)
            //    _damageBarViewPool.gameObject.SetActive(show);
        }
        #endregion

        /// <summary>
        /// 收益计算
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <param name="skillInfo"></param>
        /// <param name="mechanics"></param>
        /// <param name="mechanicsValue"></param>
        public void Judge(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            if (character.isPlayer != target.isPlayer)//敌方
            {
                if (judgeType <= 0)//命中
                {
                    SetDodge(target, skillInfo);
                    return;
                }
            }

            foreach (var kvp in target.characterInfo.passiveIdDic)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.JUDGE_MECHANICS, kvp.Key);
                if (func != null)
                {
                    object[] rs = func.Call(mechanics.mechanicsType, kvp.Value);
                    if (rs != null && rs.Length > 0)
                    {
                        int r = 0;
                        int.TryParse(rs[0].ToString(), out r);
                        if (r <= 0)
                            return;
                    }
                }
            }
            switch (mechanics.mechanicsType)
            {
                case MechanicsType.Damage:
                case MechanicsType.DrainDamage:
                case MechanicsType.IgnoreDefenseDamage:
                case MechanicsType.ImmediatePercentDamage:
                case MechanicsType.SwimmyExtraDamage:
                case MechanicsType.LandificationExtraDamage:
                case MechanicsType.BleedExtraDamage:
                case MechanicsType.FrozenExtraDamage:
                case MechanicsType.PoisoningExtraDamage:
                case MechanicsType.TagExtraDamage:
                case MechanicsType.IgniteExtraDamage:
                    foreach (var kvp in character.characterInfo.passiveIdDic)
                    {
                        LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.DAMAGE_2_FLOAT, kvp.Key);
                        if (func != null)
                        {
                            object[] rs = func.Call(skillInfo, mechanics, kvp.Value);
                            if (rs != null && rs.Length > 0)
                            {
                                int r = 0;
                                int.TryParse(rs[0].ToString(), out r);
                                if (r == 1)
                                {
                                    FloatAndTumble(character, target, skillInfo, mechanics, MechanicsType.Float);
                                    return;
                                }
                                if (r == 2)
                                {
                                    FloatAndTumble(character, target, skillInfo, mechanics, MechanicsType.Tumble);
                                    return;
                                }
                            }
                        }
                    }
                    Damage(character, target, skillInfo, mechanics, mechanicsValue, judgeType, false);
                    break;
                case MechanicsType.Treat:
                    Treat(BuffType.Treat, character, target, skillInfo, mechanics, mechanicsValue, judgeType, false);
                    break;
                case MechanicsType.TreatPercent:
                    Treat(BuffType.TreatPercent, character, target, skillInfo, mechanics, mechanicsValue, judgeType, false);
                    break;
                case MechanicsType.Poisoning:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Poisoning, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.Ignite:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Ignite, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.Bleed:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Bleed, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.LastTreat:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Treat, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.Swimmy:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Swimmy, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Frozen:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Frozen, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Sleep:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Sleep, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Landification:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Landification, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Tieup:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Tieup, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Reborn:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        float result = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(mechanicsValue.a, character.characterInfo.dlevel);
                        if (target.isDead)
                            Reborn(character, target, skillInfo, mechanics, result);
                        else
                        {
                            float hpRate = target.HP / (float)target.maxHP;
                            float diff = result - hpRate;
                            if (diff > 0)
                            {
                                float rateHP = (target.maxHP * result);
                                float targetHP = rateHP - target.HP;
                                SetTreatValue(target, targetHP, false);
                            }
                        }
                    }
                    break;
                case MechanicsType.Disperse: //驱散
                    Disperse(character, target, skillInfo, mechanics);
                    break;
                case MechanicsType.Float:
                case MechanicsType.Tumble:
                    {
                        float probability = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(mechanicsValue.b, character.characterInfo.dlevel);
                        if (JudgeUtil.GetRandom(probability))//浮空，倒地概率
                        {
                            FloatAndTumble(character, target, skillInfo, mechanics);
                        }
                    }
                    break;
                case MechanicsType.Immune://免疫特殊状态(时间随等级提升)
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Immune, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ReboundTime://反伤(时间随等级提升)
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Rebound, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ReboundValue://反伤(数值随等级提升)                     
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Rebound, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.RandomMechanics://随机效果
                    if (JudgeUtil.GetRandom(mechanicsValue.c))
                        RandomMechanics(character, target, skillInfo, mechanics, mechanicsValue, judgeType);
                    break;
                case MechanicsType.Invincible:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Invincible, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    }
                    break;
                case MechanicsType.Silence:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Silence, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        Fight.Controller.FightController.instance.BreakBootSkill(target);
                    }
                    break;
                case MechanicsType.Blind:
                    if (JudgeUtil.GetRandom(mechanicsValue.b, true))
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Blind, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    }
                    break;
                case MechanicsType.ShieldTime:
                    {
                        float shieldValue = 0f;
                        RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(character.characterInfo.roleType);
                        switch (roleAttackAttributeType)
                        {
                            case RoleAttackAttributeType.PhysicalAttack:
                                shieldValue = character.physicsAttack * mechanicsValue.b;
                                break;
                            case RoleAttackAttributeType.MagicalAttack:
                                shieldValue = character.magicAttack * mechanicsValue.b;
                                break;
                        }
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Shield, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, shieldValue, skillInfo.currentLevel, judgeType);
                    }
                    break;
                case MechanicsType.ShieldValue:
                    {
                        float shieldValue = 0f;
                        RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(character.characterInfo.roleType);
                        switch (roleAttackAttributeType)
                        {
                            case RoleAttackAttributeType.PhysicalAttack:
                                shieldValue = character.physicsAttack * mechanicsValue.b;
                                break;
                            case RoleAttackAttributeType.MagicalAttack:
                                shieldValue = character.magicAttack * mechanicsValue.b;
                                break;
                        }
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.Shield, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, shieldValue, skillInfo.currentLevel, judgeType);
                    }
                    break;
                case MechanicsType.DrainTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Drain, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DrainValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Drain, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ImmunePhysicsAttack:

                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.ImmunePhysicsAttack, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ImmuneMagicAttack:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.ImmuneMagicAttack, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.Tag:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Tag, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.GeneralSkillHit:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.GeneralSkillHit, SkillLevelBuffAddType.Count, BuffAddType.None, (int)mechanicsValue.a, 0, 0f, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.GeneralSkillCrit:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.GeneralSkillCrit, SkillLevelBuffAddType.Count, BuffAddType.None, (int)mechanicsValue.a, 0, 0f, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.AccumulatorTag:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.AccumulatorTag, SkillLevelBuffAddType.Time, BuffAddType.None, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ForceKill:
                    {
                        if (JudgeUtil.GetRandom(mechanicsValue.b, true))//几率死亡
                            StartCoroutine(SetDamageValueCoroutine(character, target, skillInfo, (uint)target.HP, false, mechanicsValue.a, judgeType));//即死不回血
                        Effect.Controller.EffectController.instance.PlayForceKillEffect(target);
                    }
                    break;
                case MechanicsType.PhysicsDefensePercentTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsDefense, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsDefensePercentValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsDefense, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicDefensePercentTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicDefense, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicDefensePercentValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicDefense, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsAttackPercentTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsAttack, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsAttackPercentValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsAttack, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicAttackPercentTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicAttack, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicAttackPercentValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicAttack, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.HPLimitPercentTime:
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.HPLimit, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        float hpValue = mechanicsValue.b * target.characterInfo.maxHP;
                        SetTreatValue(target, hpValue, false);
                    }
                    break;
                case MechanicsType.HPLimitPercentValue:
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.HPLimit, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        float hpRate = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(mechanicsValue.b, character.characterInfo.dlevel);
                        hpRate = BuffUtil.GetBuffValue(hpRate, skillInfo.currentLevel);
                        float hpValue = hpRate * target.characterInfo.maxHP;
                        SetTreatValue(target, hpValue, false);
                    }
                    break;
                case MechanicsType.SpeedPercentTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Speed, SkillLevelBuffAddType.Time, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.SpeedPercentValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Speed, SkillLevelBuffAddType.Value, BuffAddType.Percent, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsDefenseFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsDefense, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsDefenseFixedValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsDefense, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicDefenseFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicDefense, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicDefenseFixedValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicDefense, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsAttackFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsAttack, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.PhysicsAttackFixedValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.PhysicsAttack, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicAttackFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicAttack, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.MagicAttackFixedValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.MagicAttack, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.HPLimitFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.HPLimit, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    SetTreatValue(target, mechanicsValue.b, false);
                    break;
                case MechanicsType.HPLimitFixedValue:
                    {
                        target.AddBuff(character, target, skillInfo, mechanics, BuffType.HPLimit, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                        float hpValue = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(mechanicsValue.b, character.characterInfo.dlevel);
                        hpValue = BuffUtil.GetBuffValue(hpValue, skillInfo.currentLevel);
                        SetTreatValue(target, hpValue, false);
                    }
                    break;
                case MechanicsType.SpeedFixedTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Speed, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.SpeedFixedValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Speed, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.HitTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Hit, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.HitValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Hit, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DodgeTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Dodge, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DodgeValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Dodge, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Crit, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Crit, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.AntiCritTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.AntiCrit, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.AntiCritValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.AntiCrit, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.BlockTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Block, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.BlockValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Block, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.AntiBlockTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.AntiBlock, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.AntiBlockValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.AntiBlock, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CounterAtkTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CounterAtk, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CounterAtkValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CounterAtk, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritHurtAddTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CritHurtAdd, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritHurtAddValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CritHurtAdd, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritHurtDecTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CritHurtDec, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.CritHurtDecValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.CritHurtDec, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ArmorTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Armor, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.ArmorValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.Armor, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DamageDecTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.DamageDec, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DamageDecValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.DamageDec, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DamageAddTime:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.DamageAdd, SkillLevelBuffAddType.Time, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.DamageAddValue:
                    target.AddBuff(character, target, skillInfo, mechanics, BuffType.DamageAdd, SkillLevelBuffAddType.Value, BuffAddType.Fixed, mechanicsValue.a, mechanicsValue.b, skillInfo.currentLevel, judgeType);
                    break;
                case MechanicsType.Transform:
                    //do nothing
                    break;
            }
        }

        private void RandomMechanics(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_RANDOM_BUFFS);
            if (func != null)
            {
                object[] rs = func.Call(character, (int)mechanicsValue.a);
                if (rs == null) return;
                List<RandomBuff> list = new List<RandomBuff>();
                for (int i = 0, length = rs.Length; i < length; i++)
                {
                    LuaTable lt = rs[i] as LuaTable;
                    RandomBuff randomBuff = new RandomBuff();
                    randomBuff.mechanicsType = (MechanicsType)lt[1];
                    float time;
                    float.TryParse(lt[2].ToString(), out time);
                    randomBuff.time = time;
                    float value;
                    float.TryParse(lt[3].ToString(), out value);
                    randomBuff.value = value;
                    float probability;
                    float.TryParse(lt[4].ToString(), out probability);
                    randomBuff.probability = probability;
                    list.Add(randomBuff);
                }
                float randomValue = JudgeUtil.GetRandom();
                float currentValue = 0;
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    RandomBuff rb = list[i];
                    currentValue += rb.probability;
                    if (randomValue <= currentValue)
                    {
                        MechanicsData md = new MechanicsData()
                        {
                            mechanicsId = mechanicsData.mechanicsId,
                            mechanicsType = rb.mechanicsType,
                            effectIds = mechanicsData.effectIds,
                            audioDelay = mechanicsData.audioDelay,
                            audioType = mechanicsData.audioType,
                            maxLayer = mechanicsData.maxLayer,
                            targetType = mechanicsData.targetType
                        };
                        Judge(character, target, skillInfo, md, new Triple<float, float, float>(rb.time, rb.value * mechanicsValue.b, mechanicsValue.c), judgeType);
                        break;
                    }
                }
            }
        }

        private void FloatAndTumble(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData)
        {
            Logic.Fight.Controller.MechanicsController.instance.PlayMechanicsAction(skillInfo, mechanicsData, character, target, true);
            Fight.Controller.FightController.instance.BreakBootSkill(target);
        }


        private void FloatAndTumble(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, MechanicsType mechanicsType)
        {
            Logic.Fight.Controller.MechanicsController.instance.PlayMechanicsAction(skillInfo, mechanicsData, character, target, true, mechanicsType);
            Fight.Controller.FightController.instance.BreakBootSkill(target);
        }

        //复活
        private void Reborn(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, float hpRate)
        {
            if (target.isPlayer)
                PlayerController.instance.Reborn(target, hpRate, 0);
            else
                EnemyController.instance.Reborn(target, hpRate, 0);
        }

        //驱散
        private void Disperse(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData)
        {
            if (target.isPlayer == character.isPlayer)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_DISPERSE_FRIENDS_BUFFS);
                if (func != null)
                {
                    object[] rs = func.Call(character);
                    if (rs != null)
                    {
                        for (int i = 0, count = rs.Length; i < count; i++)
                        {
                            target.DisperseBuff((BuffType)rs[i], false);
                        }
                    }
                }
            }
            else
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_DISPERSE_ENEMIES_BUFFS);
                if (func != null)
                {
                    object[] rs = func.Call(character);
                    if (rs != null)
                    {
                        for (int i = 0, count = rs.Length; i < count; i++)
                        {
                            target.DisperseBuff((BuffType)rs[i], true);
                        }
                    }
                }
            }
        }

        private void Damage(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, Triple<float, float, float> mechanicsValue, int judgeType, bool isContinuous)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    if (character.Blind)//致盲
                        SetDodge(target, skillInfo);
                    else
                        CalcDamage(character, target, skillInfo, mechanics, judgeType, mechanicsValue, isContinuous);
                    break;
                case FightType.ConsortiaFight:
                    {
                        KeyValuePair<int, int> damage = Fight.Model.FightProxy.instance.GetConsortiaMechanicsValue((int)character.characterInfo.instanceID, (int)skillInfo.skillData.skillId, skillInfo.mechanicsIndex, (int)target.characterInfo.instanceID);
                        Debugger.LogError("current HP:{0} damaged HP:{1}", target.HP, damage.Value);
                        target.HP = damage.Value;
                        SetDamageValue(character, target, skillInfo, (uint)damage.Key, false, judgeType);
                    }
                    break;
                case FightType.FirstFight:
                    {
                        int damage = Logic.Tutorial.Model.TutorialProxy.instance.CurrentTutorialStepData.damage;
                        SetDamageValue(character, target, skillInfo, (uint)damage, false, judgeType);
                    }
                    break;
            }
        }

        private void Treat(BuffType buffType, CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, Triple<float, float, float> mechanicsValue, int judgeType, bool isContinuous)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    CalcTreat(buffType, character, target, judgeType, mechanicsValue, skillInfo, isContinuous);
                    break;
                case FightType.ConsortiaFight:
                    {
                        KeyValuePair<int, int> treat = Fight.Model.FightProxy.instance.GetConsortiaMechanicsValue((int)character.characterInfo.instanceID, (int)skillInfo.skillData.skillId, skillInfo.mechanicsIndex, (int)target.characterInfo.instanceID);
                        SetTreatValue(target, treat.Key, isContinuous);
                        Debugger.LogError("current HP:{0} damaged HP:{1}", target.HP, treat.Value);
                        target.HP = treat.Value;
                    }
                    break;
                case FightType.FirstFight:

                    break;
            }
        }

        private uint CalcDamage(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, int judgeType, Triple<float, float, float> mechanicsValue, bool isContinuous)
        {
            uint result = 0;
            float extraDamageRate = 1f;
            if (judgeType > 0)//命中
            {
                if (!isContinuous)
                {
                    foreach (var kvp in character.characterInfo.passiveIdDic)
                    {
                        if(skillInfo.skillData.skillId==1121)
                            Debug.Log(skillInfo);
                        LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ATTACK_BUFF, kvp.Key);
                        if (func != null)
                        {
                            object[] rs = func.Call(character, target, skillInfo, judgeType, kvp.Value);
                            if (rs != null && rs.Length > 0)
                            {
                                float r = 0;
                                float.TryParse(rs[0].ToString(), out r);
                                extraDamageRate += r;
                            }
                        }
                    }
                }
                uint damage = 0;
                float factor = 0f;
                RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(character.characterInfo.roleType);
                if (mechanics != null)
                {
                    switch (mechanics.mechanicsType)
                    {
                        //物理伤害：伤害值=｛[攻击方物理攻击-受击方物理防御×(1-攻击方破甲)]×90%+攻击方物理攻击×10%｝×（1+我方伤害加成）*（1-敌方伤害减免）×技能伤害百分比															
                        //魔法伤害：伤害值=｛[攻击方魔法攻击-受击方魔法防御×(1-攻击方破甲)]×90%+攻击方魔法攻击×10%｝×（1+我方伤害加成）*（1-敌方伤害减免）×技能伤害百分比															
                        case MechanicsType.Damage:
                        case MechanicsType.DrainDamage:
                        case MechanicsType.Poisoning:
                        case MechanicsType.Ignite:
                        case MechanicsType.Bleed:
                        case MechanicsType.SwimmyExtraDamage:
                        case MechanicsType.LandificationExtraDamage:
                        case MechanicsType.BleedExtraDamage:
                        case MechanicsType.FrozenExtraDamage:
                        case MechanicsType.PoisoningExtraDamage:
                        case MechanicsType.TagExtraDamage:
                        case MechanicsType.IgniteExtraDamage:
                            switch (roleAttackAttributeType)
                            {
                                case RoleAttackAttributeType.PhysicalAttack:
                                    {
                                        int r = 0;
                                        foreach (var kvp in character.characterInfo.passiveIdDic)
                                        {
                                            int t = 0;
                                            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.IGNORE_DEFENSE, kvp.Key);
                                            if (func != null)
                                            {
                                                object[] rs = func.Call(character, skillInfo, kvp.Value);
                                                int.TryParse(rs[0].ToString(), out t);
                                                r += t;
                                            }
                                        }
                                        if (r > 0)
                                            factor = character.physicsAttack;
                                        else
                                            factor = character.physicsAttack - target.physicsDefense * (1 - character.armor);
                                        if (factor < 0)
                                            factor = 0;
                                        damage = (uint)((factor * 0.8f + character.physicsAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                                    }
                                    break;
                                case RoleAttackAttributeType.MagicalAttack:
                                    {
                                        int r = 0;
                                        foreach (var kvp in character.characterInfo.passiveIdDic)
                                        {
                                            int t = 0;
                                            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.IGNORE_DEFENSE, kvp.Key);
                                            if (func != null)
                                            {
                                                object[] rs = func.Call(character, skillInfo, kvp.Value);
                                                int.TryParse(rs[0].ToString(), out t);
                                                r += t;
                                            }
                                        }
                                        if (r > 0)
                                            factor = character.magicAttack;
                                        else
                                            factor = character.magicAttack - target.magicDefense * (1 - character.armor);
                                        if (factor < 0)
                                            factor = 0;
                                        damage = (uint)((factor * 0.8f + character.magicAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                                        break;
                                    }
                            }
                            break;
                        case MechanicsType.IgnoreDefenseDamage://无视防御
                            switch (roleAttackAttributeType)
                            {
                                case RoleAttackAttributeType.PhysicalAttack:
                                    factor = character.physicsAttack;
                                    if (factor < 0)
                                        factor = 0;
                                    damage = (uint)((factor * 0.8f + character.physicsAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                                    break;
                                case Enums.RoleAttackAttributeType.MagicalAttack:
                                    factor = character.magicAttack;
                                    if (factor < 0)
                                        factor = 0;
                                    damage = (uint)((factor * 0.8f + character.magicAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                                    break;
                            }
                            break;
                        case MechanicsType.ImmediatePercentDamage:
                            damage = (uint)(target.maxHP * mechanicsValue.a);
                            break;
                    }
                }
                else
                {
                    switch (roleAttackAttributeType)
                    {
                        case RoleAttackAttributeType.PhysicalAttack:
                            factor = character.physicsAttack - target.physicsDefense * (1 - character.armor);
                            if (factor < 0)
                                factor = 0;
                            damage = (uint)((factor * 0.8f + character.physicsAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                            break;
                        case Enums.RoleAttackAttributeType.MagicalAttack:
                            factor = character.magicAttack - target.magicDefense * (1 - character.armor);
                            if (factor < 0)
                                factor = 0;
                            damage = (uint)((factor * 0.8f + character.magicAttack * 0.2f) * (1 + character.damageAdd) * (1 - target.damageDec) * mechanicsValue.a);
                            break;
                    }
                }
                //Debugger.Log("damge:" + damage);
                if (character.ExistBuff(BuffType.GeneralSkillCrit))
                    judgeType |= 1 << 1;
                if ((judgeType & 1 << 1) > 0)//暴击伤害=伤害总值×（我方暴击伤害-敌方暴击伤害减免）
                {
                    float critRate = character.critHurtAdd - target.critHurtDec;//最小为1,100%
                    if (critRate < 0)
                        critRate = 0;
                    damage = (uint)(damage * (1 + critRate));
                }
                else if ((judgeType & 1 << 2) > 0)//格挡，伤害=伤害总值×50%
                    damage = (uint)(damage * 0.5f);
                if (!isContinuous)//持续伤害已经计算过星级加成，避免重复计算
                    damage = (uint)Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(damage, character.characterInfo.dlevel);
                Const.Model.ConstData constData = Const.Model.ConstData.GetConstData();
                #region skill level no use
                /* switch (skillInfo.currentLevel)
                 {
                     case 1:
                         //damage = (uint)(damage);
                         break;
                     case 2:
                         damage = (uint)(damage * constData.skillHurtB1);
                         break;
                     case 3:
                         damage = (uint)(damage * constData.skillHurtB2);
                         break;
                 }
                 */
                #endregion
                #region float and tumble skill damage add
                if (Fight.Controller.FightController.instance.isComboing)
                    damage = (uint)(damage * (1 + constData.floatHurtAdd));
                #endregion
                #region combo attack damage add
                uint comboCount = MaxComboCount();
                float comboRate = comboCount * constData.comboHurtAdd;
                if (comboRate > constData.comboHurtMax)
                    comboRate = constData.comboHurtMax;
                //Debugger.Log(string.Format("damage original is {0}", damage));
                damage = (uint)(damage * (1 + comboRate));
                //Debugger.Log(string.Format("damage add rate {0},add is {1}", comboRate, damage));
                #endregion
                damage = (uint)(damage * extraDamageRate);
                #region extra damage
                if (mechanics != null)
                {
                    switch (mechanics.mechanicsType)
                    {
                        case MechanicsType.SwimmyExtraDamage:
                            if (target.Swimmy)
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                            break;
                        case MechanicsType.LandificationExtraDamage:
                            if (target.Landification)
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                            break;
                        case MechanicsType.BleedExtraDamage:
                            if (target.ExistBuff(BuffType.Bleed))
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                            break;
                        case MechanicsType.FrozenExtraDamage:
                            if (target.Frozen)
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                            break;
                        case MechanicsType.PoisoningExtraDamage:
                            if (target.ExistBuff(BuffType.Poisoning))
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                            break;
                        case MechanicsType.TagExtraDamage:
                            if (target.Tag)
                            {
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                                //target.RemoveBuff(BuffType.Tag);
                            }
                            break;
                        case MechanicsType.IgniteExtraDamage:
                            if (target.ExistBuff(BuffType.Ignite))
                            {
                                damage = (uint)(damage * (1 + mechanicsValue.b));
                                target.RemoveBuff(BuffType.Tag);
                            }
                            break;
                    }
                }
                #endregion
                if (damage <= 0)
                    damage = 1;//最低值为1
                result = damage;
                if ((judgeType & 1 << 2) > 0)
                {
                    PlayEffect(Effect.Controller.EffectController.BLOCK_EFFECT_ID, target);
                    SetBlock(target, skillInfo);
                }
                uint actualHPValue = SetDamageValue(character, target, skillInfo, damage, isContinuous, judgeType);
                if (!isContinuous)
                {
                    switch (mechanics.mechanicsType)//伤害并吸血
                    {
                        case MechanicsType.DrainDamage:
                            float drainValue = actualHPValue * mechanicsValue.b;
                            if (drainValue > 0)
                                SetTreatValue(character, drainValue, false);
                            break;
                    }
                    foreach (var kvp in character.characterInfo.passiveIdDic)
                    {
                        LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ATTACK_FINISH_BUFF, kvp.Key);
                        if (func != null)
                            func.Call(character, target, skillInfo, judgeType, actualHPValue, kvp.Value);
                    }
                }
                foreach (var kvp in target.characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ATTACKED_FINISH_BUFF, kvp.Key);
                    if (func != null)
                        func.Call(character, target, skillInfo, judgeType, actualHPValue, kvp.Value);
                }
                foreach (var kvp in target.characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ATTACKED_BUFF, kvp.Key);
                    if (func != null)
                        func.Call(character, target, skillInfo, judgeType, actualHPValue, kvp.Value);
                }
            }
            else
                SetDodge(target, skillInfo);
            return result;
        }

        private void SetDodge(CharacterEntity target, SkillInfo skillInfo)
        {
            if (target)
            {
                PlayEffect(Effect.Controller.EffectController.DODGE_EFFECT_ID, target);
                if (_prefab)
                {
                    GameObject go = _spawnPool.Spawn(_prefab.transform, _damageBarViewPool).gameObject;
                    DamageBarView db = go.GetComponent<DamageBarView>();
                    db.SetDodge(skillInfo);
                    db.worldPos = target.pos + new Vector3(0f, target.height, 0f);
                }
            }
        }

        private void SetBlock(CharacterEntity target, SkillInfo skillInfo)
        {
            if (target)
            {
                if (_prefab)
                {
                    GameObject go = _spawnPool.Spawn(_prefab.transform, _damageBarViewPool).gameObject;
                    DamageBarView db = go.GetComponent<DamageBarView>();
                    db.SetBlock(skillInfo);
                    db.worldPos = target.pos + new Vector3(0f, target.height, 0f);
                }
            }
        }

        public void CalcDamage(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, int judgeType, float mechanicsValueA, float mechanicsValueB, float mechanicsValueC, float delay)
        {
            Triple<float, float, float> mechanicsValue = new Triple<float, float, float>();
            mechanicsValue.a = mechanicsValueA;
            mechanicsValue.b = mechanicsValueB;
            mechanicsValue.c = mechanicsValueC;
            StartCoroutine(CalcDamageCoroutine(character, target, skillInfo, mechanics, judgeType, mechanicsValue, delay));
        }

        private IEnumerator CalcDamageCoroutine(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, int judgeType, Triple<float, float, float> mechanicsValue, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            CalcDamage(character, target, skillInfo, mechanics, judgeType, mechanicsValue, false);
        }

        public void SetDamageValue(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, uint HPValue, bool isContinuous, float delay, int judgeType)
        {
            StartCoroutine(SetDamageValueCoroutine(character, target, skillInfo, (uint)target.HP, isContinuous, delay, judgeType));
        }

        private IEnumerator SetDamageValueCoroutine(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, uint HPValue, bool isContinuous, float delay, int judgeType)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            SetDamageValue(character, target, skillInfo, HPValue, isContinuous, judgeType);
        }

        private uint SetDamageValue(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, uint HPValue, bool isContinuous, int judgeType)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.FirstFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                case FightType.WorldBoss:
                case FightType.DailyPVE:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                    if (target.damageImmuneTime)
                        HPValue = 0;
                    else if (target.damageImmuneCount)
                    {
                        HPValue = 0;
                        target.UpdateDamageBuffs();
                    }
                    break;
                case FightType.ConsortiaFight:
                    if (target.damageImmuneCount)
                        target.UpdateDamageBuffs();
                    break;
            }
            uint actualHPValue = HPValue;
            if (target)
            {
                #region fight imitate
#if UNITY_EDITOR
                if (Fight.Controller.FightController.imitate)
                {
                    if (hitDamages.Count <= currentIndex)
                        hitDamages.Add(new Dictionary<string, uint>());
                    if (skillDamages.Count <= currentIndex)
                        skillDamages.Add(new Dictionary<string, uint>());
                    Dictionary<string, uint> hitDamageDic = hitDamages[currentIndex];
                    Dictionary<string, uint> skillDamageDic = skillDamages[currentIndex];
                    string key = character.characterInfo.instanceID.ToString() + "_" + character.characterName;
                    switch (skillInfo.skillData.skillType)
                    {
                        case SkillType.Hit:
                            if (!hitDamageDic.ContainsKey(key))
                                hitDamageDic[key] = HPValue;
                            else
                                hitDamageDic[key] += HPValue;
                            break;
                        case SkillType.Skill:
                        case SkillType.Aeon:
                            if (!skillDamageDic.ContainsKey(key))
                                skillDamageDic[key] = HPValue;
                            else
                                skillDamageDic[key] += HPValue;
                            break;
                    }
                }
#endif
                #endregion
                switch (Fight.Controller.FightController.instance.fightType)
                {
                    case FightType.PVE:
                    case FightType.Arena:
                    case FightType.Expedition:
                    case FightType.WorldTree:
                    //case FightType.DailyPVE:
                    case FightType.FirstFight:
                    case FightType.PVP:
                    case FightType.FriendFight:
                    case FightType.MineFight:
#if UNITY_EDITOR
                    case FightType.Imitate:
#endif
                        if (HPValue > 0)
                            actualHPValue = target.SetDamageValue(character, skillInfo, HPValue);
                        break;
                    case FightType.ConsortiaFight:
                        target.SetDamageValue(character, skillInfo, 0);//假伤害
                        break;
                    case FightType.WorldBoss:
                        if (!target.isPlayer)
                            target.SetDamageValue(character, skillInfo, 0);//假伤害
                        else
                        {
                            if (HPValue > 0)
                                actualHPValue = target.SetDamageValue(character, skillInfo, HPValue);
                        }
                        break;
                    case FightType.DailyPVE:
                        if (Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.ActitityFloat)
                        {
                            if (!target.isPlayer)
                            {
                                if (!Fight.Controller.FightController.instance.isComboing)
                                {
                                    HPValue = 0;
                                    actualHPValue = 0;
                                }
                            }
                        }
                        if (HPValue > 0)
                            actualHPValue = target.SetDamageValue(character, skillInfo, HPValue);
                        break;
                    case FightType.SkillDisplay:

                        break;
                }
                switch (Fight.Controller.FightController.instance.fightType)
                {
                    case FightType.PVE:
                    case FightType.Arena:
                    case FightType.Expedition:
                    case FightType.WorldTree:
                    case FightType.WorldBoss:
#if UNITY_EDITOR
                    case FightType.Imitate:
#endif
                    case FightType.DailyPVE:
                    case FightType.PVP:
                    case FightType.FriendFight:
                    case FightType.MineFight:
                        if (actualHPValue > 0)
                        {
                            if (character.isPlayer)
                                _totalDamage += actualHPValue;
                            //吸血
                            float drainValue = character.GetBuffsValue(BuffType.Drain, actualHPValue);
                            if (drainValue > 0)
                                SetTreatValue(character, drainValue, false);
                            float reboundValue = target.GetBuffsValue(BuffType.Rebound, 1);//反伤，以1为基数
                            if (reboundValue > 0)
                            {
                                GameObject go = _spawnPool.Spawn(_prefab.transform, _damageBarViewPool).gameObject;
                                DamageBarView db = go.GetComponent<DamageBarView>();
                                uint reboundHp = (uint)(reboundValue * actualHPValue);
                                db.SetDemageValue(null, reboundHp, false);
                                db.worldPos = character.pos + new Vector3(0f, character.height, 0f);
                                character.SetDamageValue(target, null, reboundHp);
                            }
                        }
                        break;
                    case FightType.FirstFight:
                    case FightType.SkillDisplay:
                    case FightType.ConsortiaFight:
                        break;
                }

                if (_prefab)
                {
                    if (Fight.Controller.FightController.instance.isCloseup && isContinuous)
                    {
                        //do not show last damage on close up camera
                    }
                    else
                    {
                        GameObject go = _spawnPool.Spawn(_prefab.transform, _damageBarViewPool).gameObject;
                        DamageBarView db = go.GetComponent<DamageBarView>();
                        if (Fight.Controller.FightController.instance.isComboing || (judgeType & 1 << 1) > 0)//暴击
                            db.SetCriticalValue((uint)actualHPValue);
                        else
                            db.SetDemageValue(skillInfo, (uint)actualHPValue, isContinuous);
                        db.worldPos = target.pos + new Vector3(0f, target.height, 0f);
                    }
                }
                if (!isContinuous)
                {
                    if (!target.isPlayer)
                    {
                        CalcCombo(target, skillInfo);
                    }
                    else
                    {
                        float angry = Logic.Game.Model.GlobalData.GetGlobalData().angryValueEverytime;
                        foreach (var kvp in target.characterInfo.passiveIdDic)
                        {
                            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ANGRY, kvp.Key);
                            if (func != null)
                            {
                                object[] rs = func.Call(kvp.Value);
                                int r = 0;
                                int.TryParse(rs[0].ToString(), out r);
                                angry += r;
                            }
                        }
                        Fight.Controller.FightController.instance.SetAngry(angry);
                    }
                }
            }
            return actualHPValue;
        }

        private uint CalcTreat(BuffType buffType, CharacterEntity character, CharacterEntity target, int judgeType, Triple<float, float, float> mechanicsValue, SkillInfo skillInfo, bool isContinuous)
        {
            uint treat = 0;
            switch (buffType)
            {
                case BuffType.Treat:
                    RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(character.characterInfo.roleType);
                    switch (roleAttackAttributeType)
                    {
                        //治疗值=攻击方攻击×（1+我方伤害加成）×技能伤害百分比
                        case RoleAttackAttributeType.PhysicalAttack:
                            treat = (uint)(character.physicsAttack * (1 + character.damageAdd) * mechanicsValue.a);
                            break;
                        case RoleAttackAttributeType.MagicalAttack:
                            treat = (uint)(character.magicAttack * (1 + character.damageAdd) * mechanicsValue.a);
                            break;
                    }
                    if ((judgeType & 1 << 1) > 0)//暴击伤害=伤害总值×（我方暴击伤害-敌方暴击伤害减免）
                    {
                        float critRate = character.critHurtAdd - target.critHurtDec;//最少为1
                        if (critRate < 1)
                            critRate = 1;
                        treat = (uint)(treat * critRate);
                    }
                    if (!isContinuous)//持续回血已经计算过星级加成，避免重复计算
                        treat = (uint)Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(treat, character.characterInfo.dlevel);
                    //Const.Model.ConstData constData = Const.Model.ConstData.GetConstData();
                    //switch (skillInfo.currentLevel)
                    //{
                    //    case 1:
                    //        //treat = (uint)(treat * (1 + constData.skillHurtA * (level - 1)));
                    //        break;
                    //    case 2:
                    //        treat = (uint)(treat * constData.skillHurtB1);
                    //        break;
                    //    case 3:
                    //        treat = (uint)(treat * constData.skillHurtB2);
                    //        break;
                    //}
                    break;
                case BuffType.TreatPercent:
                    treat = (uint)(target.maxHP * mechanicsValue.a);
                    break;
            }
            SetTreatValue(target, treat, isContinuous);
            return treat;
        }

        public void SetTreatValue(CharacterEntity target, float HPValue, bool isContinuous)
        {
            if (target)
            {
                switch (FightController.instance.fightType)
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
                        float hp = target.GetBuffsValue(BuffType.TreatAdd, HPValue);
                        HPValue += hp;
                        float extraTreatRate = 1f;
                        foreach (var kvp in target.characterInfo.passiveIdDic)
                        {
                            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.TREAT_BUFF, kvp.Key);
                            if (func != null)
                            {
                                object[] rs = func.Call(target, kvp.Value);
                                if (rs != null && rs.Length > 0)
                                {
                                    float r = 0;
                                    float.TryParse(rs[0].ToString(), out r);
                                    extraTreatRate += r;
                                }
                            }
                        }
                        HPValue = (uint)(HPValue * extraTreatRate);
                        target.SetTreatValue((uint)HPValue);
                        break;
                    case FightType.ConsortiaFight:
                        break;
                }
                if (_prefab)
                {
                    if (Fight.Controller.FightController.instance.isCloseup && isContinuous)
                    {
                        //do not show last treat on close up camera
                    }
                    else
                    {
                        GameObject go = _spawnPool.Spawn(_prefab.transform, _damageBarViewPool).gameObject;
                        DamageBarView db = go.GetComponent<DamageBarView>();
                        db.SetTreatValue((uint)HPValue);
                        db.worldPos = target.pos + new Vector3(0f, target.height, 0f);
                    }
                }
                if (!isContinuous)
                {
                    if (!target.isPlayer)
                    {
                        float angry = Logic.Game.Model.GlobalData.GetGlobalData().angryValueEverytime;
                        foreach (var kvp in target.characterInfo.passiveIdDic)
                        {
                            LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ANGRY, kvp.Key);
                            if (func != null)
                            {
                                object[] rs = func.Call(kvp.Value);
                                int r = 0;
                                int.TryParse(rs[0].ToString(), out r);
                                angry += r;
                            }
                        }
                        Fight.Controller.FightController.instance.SetAngry(angry);
                        //Fight.Controller.FightController.instance.SetAngry(Logic.Game.Model.GlobalData.GetGlobalData().angryValueEverytime);
                    }
                }
            }
        }

        #region ContinuousBuffs
        public void StartUpdateContinuousBuffs()
        {
            StopCoroutine("StartUpdateContinuousBuffsCoroutine");
            StartCoroutine("StartUpdateContinuousBuffsCoroutine");
        }

        private IEnumerator StartUpdateContinuousBuffsCoroutine()
        {
            float time = TimeController.instance.fightSkillTime;
            while (true)
            {
                if (Fight.Controller.FightController.instance.fightStatus == FightStatus.GameOver)
                    break;
                if (TimeController.instance.fightSkillTime - time >= 1f)
                {
                    List<HeroEntity> heros = PlayerController.instance.heros;
                    for (int i = 0, count = heros.Count; i < count; i++)
                    {
                        HeroEntity hero = heros[i];
                        if (hero.isDead) continue;
                        switch (FightController.instance.fightType)
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
                                UpdateContinuousBuffs(hero);
                                break;
                            case FightType.ConsortiaFight:
                                UpdateConsortiaContinuousBuffs(hero);
                                break;
                        }
                    }

                    List<EnemyEntity> enemys = EnemyController.instance.enemies;
                    for (int i = 0, count = enemys.Count; i < count; i++)
                    {
                        EnemyEntity enemy = enemys[i];
                        if (enemy.isDead) continue;
                        switch (FightController.instance.fightType)
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
                                UpdateContinuousBuffs(enemy);
                                break;
                            case FightType.ConsortiaFight:
                                UpdateConsortiaContinuousBuffs(enemy);
                                break;
                        }
                    }
                    time = TimeController.instance.fightSkillTime;
                }
                yield return null;
            }
        }

        private void UpdateContinuousBuffs(CharacterEntity target)
        {
            List<BuffInfo> treatBuffs = target.GetBuffs(BuffType.Treat);
            if (treatBuffs != null)
            {
                for (int i = 0, count = treatBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = treatBuffs[i];
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        if (target.HP < target.maxHP)
                            Treat(BuffType.Treat, buffInfo.character, buffInfo.target, buffInfo.skillInfo, buffInfo.mechanics, new Triple<float, float, float>(buffInfo.value, 0f, 0f), buffInfo.judgeType, true);
                    }
                }
            }

            List<BuffInfo> recoverHPPercentBuffs = target.GetBuffs(BuffType.TreatPercent);
            if (recoverHPPercentBuffs != null)
            {
                for (int i = 0, count = recoverHPPercentBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = recoverHPPercentBuffs[i];
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        if (target.HP < target.maxHP)
                            Treat(BuffType.TreatPercent, buffInfo.character, buffInfo.target, buffInfo.skillInfo, buffInfo.mechanics, new Triple<float, float, float>(buffInfo.value, 0f, 0f), buffInfo.judgeType, true);
                    }
                }
            }

            List<BuffInfo> poisoningBuffs = target.GetBuffs(BuffType.Poisoning);
            if (poisoningBuffs != null)
            {
                for (int i = 0, count = poisoningBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = poisoningBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        Damage(buffInfo.character, buffInfo.target, buffInfo.skillInfo, buffInfo.mechanics, new Triple<float, float, float>(buffInfo.value, 0f, 0f), buffInfo.judgeType, true);//triple 的a为伤害
                    }
                }
            }

            List<BuffInfo> igniteBuffs = target.GetBuffs(BuffType.Ignite);
            if (igniteBuffs != null)
            {
                for (int i = 0, count = igniteBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = igniteBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        Damage(buffInfo.character, buffInfo.target, buffInfo.skillInfo, buffInfo.mechanics, new Triple<float, float, float>(buffInfo.value, 0f, 0f), buffInfo.judgeType, true);//triple 的a为伤害
                    }
                }
            }

            List<BuffInfo> bleedBuffs = target.GetBuffs(BuffType.Bleed);
            if (bleedBuffs != null)
            {
                for (int i = 0, count = bleedBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = bleedBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        Damage(buffInfo.character, buffInfo.target, buffInfo.skillInfo, buffInfo.mechanics, new Triple<float, float, float>(buffInfo.value, 0f, 0f), buffInfo.judgeType, true);//triple 的a为伤害
                    }
                }
            }
        }

        private void UpdateConsortiaContinuousBuffs(CharacterEntity target)
        {
            List<BuffInfo> treatBuffs = target.GetBuffs(BuffType.Treat);
            if (treatBuffs != null)
            {
                for (int i = 0, count = treatBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = treatBuffs[i];
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        if (target.HP < target.maxHP)
                            SetTreatValue(target, buffInfo.value, true);
                    }
                }
            }

            List<BuffInfo> recoverHPPercentBuffs = target.GetBuffs(BuffType.TreatPercent);
            if (recoverHPPercentBuffs != null)
            {
                for (int i = 0, count = recoverHPPercentBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = recoverHPPercentBuffs[i];
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        if (target.HP < target.maxHP)
                            SetTreatValue(target, buffInfo.value, true);
                    }
                }
            }

            List<BuffInfo> poisoningBuffs = target.GetBuffs(BuffType.Poisoning);
            if (poisoningBuffs != null)
            {
                for (int i = 0, count = poisoningBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = poisoningBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        SetDamageValue(null, target, null, (uint)buffInfo.value, false, buffInfo.judgeType);
                    }
                }
            }

            List<BuffInfo> igniteBuffs = target.GetBuffs(BuffType.Ignite);
            if (igniteBuffs != null)
            {
                for (int i = 0, count = igniteBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = igniteBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        SetDamageValue(null, target, null, (uint)buffInfo.value, false, buffInfo.judgeType);
                    }
                }
            }

            List<BuffInfo> bleedBuffs = target.GetBuffs(BuffType.Bleed);
            if (bleedBuffs != null)
            {
                for (int i = 0, count = bleedBuffs.Count; i < count; i++)
                {
                    BuffInfo buffInfo = bleedBuffs[i];
                    //Debugger.Log("持续伤害：" + buffInfo.value);
                    buffInfo.nextTime--;
                    if (buffInfo.nextTime == 0)
                    {
                        buffInfo.nextTime = buffInfo.interval;
                        SetDamageValue(null, target, null, (uint)buffInfo.value, false, buffInfo.judgeType);
                    }
                }
            }
        }

        public void StopUpdateContinuousBuffs()
        {
            StopCoroutine("StartUpdateContinuousBuffsCoroutine");
        }
        #endregion

        private void PlayEffect(uint effectId, CharacterEntity character)
        {
            EffectInfo effectInfo = new EffectInfo(effectId);
            if (effectInfo.effectData == null) return;
            effectInfo.character = character;
            switch (effectInfo.effectData.effectType)
            {
                case EffectType.LockTarget:
                    effectInfo.target = character;
                    break;
                case EffectType.LockPart:
                    effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, character.transform);
                    effectInfo.target = character;
                    break;
            }
            effectInfo.delay = effectInfo.effectData.delay;
            Effect.Controller.EffectController.instance.PlayEffect(effectInfo);
        }

        public void OpenComboBarView()
        {
            Logic.UI.ComboBar.View.ComboBarView comboBarView = UI.UIMgr.instance.Open<Logic.UI.ComboBar.View.ComboBarView>(Logic.UI.ComboBar.View.ComboBarView.PREFAB_PATH, Logic.UI.EUISortingLayer.MainUI, Logic.UI.UIOpenMode.Overlay);
            comboBarView.onComboOver += OnComboOver;
        }

        private void CalcCombo(CharacterEntity character, SkillInfo skillInfo)
        {
            uint id = character.characterInfo.instanceID;
            if (!_lastAttackTimeDic.ContainsKey(id))
                _lastAttackTimeDic[id] = Time.realtimeSinceStartup;
            float delay = Const.Model.ConstData.GetConstData().comboHurtDelay / Game.GameSetting.instance.speed;
            delay += skillInfo.skillData.pauseTime;
            if (Time.realtimeSinceStartup - _lastAttackTimeDic[id] <= delay)
            {
                if (!_comboTimesDic.ContainsKey(id))
                    _comboTimesDic[id] = 1;
                else
                    _comboTimesDic[id]++;
            }
            else
            {
                _comboTimesDic[id] = 1;
            }
            _lastAttackTimeDic[id] = Time.realtimeSinceStartup;
            if (_comboTimesDic[id] > 1)
            {
                ComboBarView comboBarView = Logic.UI.UIMgr.instance.Get<ComboBarView>(ComboBarView.PREFAB_PATH);
                if (comboBarView)
                {
                    KeyValuePair<uint, uint> kvp = _comboTimesDic.MaxValue<uint, uint>();
                    comboBarView.SetComboCount(kvp.Key, kvp.Value);
                }
            }
        }

        private uint MaxComboCount()
        {
            if (_comboTimesDic.Count > 0)
            {
                KeyValuePair<uint, uint> kvp = _comboTimesDic.MaxValue<uint, uint>();
                return kvp.Value;
            }
            return 0;
        }

        private void OnComboOver(uint id, uint count)
        {
            _comboTimesDic.Clear();
            if (count != 0)
            {
                _comboTimesDic.Add(id, count);
            }
            else
                _comboCount++;
        }
    }

    struct RandomBuff
    {
        public MechanicsType mechanicsType;
        public float time;
        public float value;
        public float probability;
    }
}