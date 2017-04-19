using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.Character.Controller;
using Logic.Character;
using Logic.Skill.Model;
using Logic.Action;
using Logic.Effect.Controller;
using Logic.Judge.Controller;
using System.Collections.Generic;
using Logic.Net.Controller;
using Logic.Effect.Model;
using Logic.Position.Model;
using Common.Util;
using Logic.Judge;
using Common.Animators;
using Common.Components.Trans;
using Common.ResMgr;
using Common.GameTime.Controller;
using PathologicalGames;
namespace Logic.Fight.Controller
{
    public class MechanicsController : SingletonMono<MechanicsController>
    {
        void Awake()
        {
            instance = this;
        }

        //我方移动攻击效果
        [LuaInterface.NoToLua]
        public void PlayerMechanics(SkillInfo skillInfo, CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList)
        {
            switch (FightController.instance.fightStatus)
            {
                case FightStatus.Normal:
                    StartCoroutine(MechanicsCoroutine(skillInfo, character, timelineList, true));
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    StartCoroutine(ComboMechanicsCoroutine(skillInfo, character, timelineList, true));
                    break;
            }
        }

        [LuaInterface.NoToLua]
        //我方远程效果
        public void PlayerMechanics(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, float delay, int judgeType, bool isLastTarget)
        {
            if (delay > 0)
                StartCoroutine(PlayerMechanicsCoroutine(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget));
            else
                PlayerMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType, isLastTarget);
        }

        private IEnumerator PlayerMechanicsCoroutine(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, float delay, int judgeType, bool isLastTarget)
        {
            switch (FightController.instance.fightStatus)
            {
                case FightStatus.Normal:
                    yield return new WaitForSeconds(delay);
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    float lastTime = Time.realtimeSinceStartup;
                    delay /= Game.GameSetting.instance.speed;
                    float currentTime = lastTime;
                    while (Time.realtimeSinceStartup - lastTime < delay)
                    {
                        yield return null;
                        if (TimeController.instance.playerPause)
                            lastTime += (Time.realtimeSinceStartup - currentTime);
                        currentTime = Time.realtimeSinceStartup;
                    }
                    break;
            }
            PlayerMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType, isLastTarget);
        }

        private void PlayerMechanics(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, int judgeType, bool isLastTarget)
        {
            ReleaseMechanics(skillInfo, character, mechanicsValue, mechanicsData, target1, target2, judgeType, true);
            if (isLastTarget)
            {
                switch (FightController.instance.fightStatus)
                {
                    case FightStatus.Normal:
                        FightController.instance.FinishedSkillMechanics(skillInfo, character, true);
                        break;
                    case FightStatus.FloatComboing:
                    case FightStatus.TumbleComboing:
                        FightController.instance.FinishedComboSkillMechanics(character, skillInfo, true);
                        break;
                }
            }
        }

        [LuaInterface.NoToLua]
        //敌方移动攻击效果
        public void EnemyMechanics(SkillInfo skillInfo, CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList)
        {
            switch (FightController.instance.fightStatus)
            {
                case FightStatus.Normal:
                    StartCoroutine(MechanicsCoroutine(skillInfo, character, timelineList, false));
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    StartCoroutine(ComboMechanicsCoroutine(skillInfo, character, timelineList, false));
                    break;

            }
        }

        [LuaInterface.NoToLua]
        //敌方远程效果
        public void EnemyMechanics(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, float delay, int judgeType, bool isLastTarget)
        {
            if (delay > 0)
                StartCoroutine(EnemyMechanicsCoroutine(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget));
            else
                EnemyMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType, isLastTarget);
        }

        private IEnumerator EnemyMechanicsCoroutine(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, float delay, int judgeType, bool isLastTarget)
        {
            switch (FightController.instance.fightStatus)
            {
                case FightStatus.Normal:
                    yield return new WaitForSeconds(delay);
                    break;
                case FightStatus.FloatComboing:
                case FightStatus.TumbleComboing:
                    float lastTime = Time.realtimeSinceStartup;
                    delay /= Game.GameSetting.instance.speed;
                    float currentTime = lastTime;
                    while (Time.realtimeSinceStartup - lastTime < delay)
                    {
                        yield return null;
                        if (TimeController.instance.playerPause)
                            lastTime += (Time.realtimeSinceStartup - currentTime);
                        currentTime = Time.realtimeSinceStartup;
                    }
                    break;
            }
            EnemyMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType, isLastTarget);
        }

        private void EnemyMechanics(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, int judgeType, bool isLastTarget)
        {
            ReleaseMechanics(skillInfo, character, mechanicsValue, mechanicsData, target1, target2, judgeType, false);
            if (isLastTarget)
            {
                switch (FightController.instance.fightStatus)
                {
                    case FightStatus.Normal:
                        FightController.instance.FinishedSkillMechanics(skillInfo, character, false);
                        break;
                    case FightStatus.FloatComboing:
                    case FightStatus.TumbleComboing:
                        FightController.instance.FinishedComboSkillMechanics(character, skillInfo, false);
                        break;
                }
            }
        }

        private IEnumerator MechanicsCoroutine(SkillInfo skillInfo, CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, bool isPlayer)
        {
            List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
            Dictionary<CharacterEntity, int> judgeTypeDic = new Dictionary<CharacterEntity, int>();
            float delay = 0f;
            for (int i = 0, count = timelineKeys.Count; i < count; i++)
            {
                if (i == 0)
                    delay = timelineKeys[i] - skillInfo.animationData.hitTime;
                else
                    delay = timelineKeys[i] - timelineKeys[i - 1];//取时间差
                if (delay > 0)
                    yield return new WaitForSeconds(delay);
                Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[i];
                List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                List<Triple<float, float, float>> mechanicsValues = skillInfo.skillData.mechanicsValues[i];
                for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                {
                    uint mechanicsId = mechanicsKeys[k];
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                    List<KeyValuePair<uint, uint>> tids;
                    if (mechanicsData.mechanicsType == MechanicsType.Reborn)
                    {
                        tids = CharacterUtil.FindDeadTargets(mechanicsData.rangeType, mechanicsData.targetType, isPlayer);
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids.AddRange(CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer));
                    }
                    else
                    {
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids = CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer);
                        else
                            tids = mechanicsDic[mechanicsId];
                    }
                    Triple<float, float, float> mechanicsValue = mechanicsValues[k];
                    for (int m = 0, mCount = tids.Count; m < mCount; m++)
                    {
                        KeyValuePair<uint, uint> tid = tids[m];
                        CharacterEntity target1 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Key);
                        CharacterEntity target2 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Value);
                        if (target1 == null) continue;
                        int judgeType = 0;
                        if (!judgeTypeDic.ContainsKey(target1))
                        {
                            judgeType = JudgeUtil.GetJudgeResult(character, target1, target2, (int)skillInfo.skillData.skillId);
                            judgeTypeDic[target1] = judgeType;
                            if (judgeType == 0)
                                EffectController.instance.RemoveEffectByPrefabName(EffectController.SPEED_LINE);
                        }
                        else
                            judgeType = judgeTypeDic[target1];
                        ReleaseMechanics(skillInfo, character, mechanicsValue, mechanicsData, target1, target2, judgeType, isPlayer);
                    }
                }
            }
            judgeTypeDic.Clear();
            judgeTypeDic = null;
            FightController.instance.FinishedSkillMechanics(skillInfo, character, isPlayer);
        }

        private IEnumerator ComboMechanicsCoroutine(SkillInfo skillInfo, CharacterEntity character, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, bool isPlayer)
        {
            float time = Time.realtimeSinceStartup;
            List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
            Dictionary<CharacterEntity, int> judgeTypeDic = new Dictionary<CharacterEntity, int>();
            float delay = 0f;
            for (int i = 0, count = timelineKeys.Count; i < count; i++)
            {
                if (i == 0)
                    delay = timelineKeys[i] - skillInfo.animationData.hitTime;
                else
                    delay = timelineKeys[i] - timelineKeys[i - 1];//取时间差
                delay /= Game.GameSetting.instance.speed;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
                time = Time.realtimeSinceStartup;
                Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[i];
                List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                List<Triple<float, float, float>> mechanicsValues = skillInfo.skillData.mechanicsValues[i];
                for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                {
                    uint mechanicsId = mechanicsKeys[k];
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                    List<KeyValuePair<uint, uint>> tids;
                    if (mechanicsData.mechanicsType == MechanicsType.Reborn)
                    {
                        tids = CharacterUtil.FindDeadTargets(mechanicsData.rangeType, mechanicsData.targetType, isPlayer);
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids.AddRange(CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer));
                    }
                    else
                    {
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids = CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer);
                        else
                            tids = mechanicsDic[mechanicsId];
                    }
                    Triple<float, float, float> mechanicsValue = mechanicsValues[k];
                    for (int m = 0, mCount = tids.Count; m < mCount; m++)
                    {
                        KeyValuePair<uint, uint> tid = tids[m];
                        CharacterEntity target1 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Key);
                        CharacterEntity target2 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Value);
                        if (target1 == null) continue;
                        int judgeType = 0;
                        if (!judgeTypeDic.ContainsKey(target1))
                        {
                            judgeType = JudgeUtil.GetJudgeResult(character, target1, target2, (int)skillInfo.skillData.skillId);
                            judgeTypeDic[target1] = judgeType;
                            if (judgeType == 0)
                                EffectController.instance.RemoveEffectByPrefabName(EffectController.SPEED_LINE);
                        }
                        else
                            judgeType = judgeTypeDic[target1];
                        ReleaseMechanics(skillInfo, character, mechanicsValue, mechanicsData, target1, target2, judgeType, isPlayer);
                    }
                }
            }
            judgeTypeDic.Clear();
            judgeTypeDic = null;
            FightController.instance.FinishedComboSkillMechanics(character, skillInfo, isPlayer);
        }

        private void ReleaseMechanics(SkillInfo skillInfo, CharacterEntity character, Triple<float, float, float> mechanicsValue, MechanicsData mechanicsData, CharacterEntity target1, CharacterEntity target2, int judgeType, bool isPlayer)
        {
            if (character.ExistBuff(BuffType.GeneralSkillHit))
                judgeType |= 1;
            switch (mechanicsData.mechanicsType)
            {
                //攻击
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
                    if (target2.Invincible) //无敌
                    {
                        EffectController.instance.PlayInvincibleEffect(target2);
                    }
                    else if (target2.ImmunePhysicsAttack || target2.ImmuneMagicAttack)
                    {
                        Debugger.Log("ImmunePhysicsAttack  or ImmuneMagicAttack");
                    }
                    else
                    {
                        if (isPlayer)
                            PlayerAttack(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType);
                        else
                            EnemyAttack(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, judgeType);
                    }
                    break;
                //治疗
                case MechanicsType.Treat:
                case MechanicsType.TreatPercent:
                    if (isPlayer)
                        PlayerTreat(skillInfo, mechanicsData, character, target2, mechanicsValue, judgeType);
                    else
                        EnemyTreat(skillInfo, mechanicsData, character, target2, mechanicsValue, judgeType);
                    break;
                //控制
                case MechanicsType.Float:
                case MechanicsType.Tumble:
                //即死
                case MechanicsType.ForceKill:
                //复活
                case MechanicsType.Reborn:
                //随机效果
                case MechanicsType.RandomMechanics:
                //驱散
                case MechanicsType.Disperse:
                    ReleaseMechanics(skillInfo, mechanicsData, character, target2, mechanicsValue, judgeType);
                    break;
                //持续伤害
                case MechanicsType.Poisoning:
                case MechanicsType.Ignite:
                case MechanicsType.Bleed:
                case MechanicsType.LastTreat:
                //控制buff
                case MechanicsType.Swimmy:
                case MechanicsType.Invincible:
                case MechanicsType.Silence:
                case MechanicsType.Blind:
                case MechanicsType.Frozen:
                case MechanicsType.Sleep:
                case MechanicsType.Landification:
                case MechanicsType.Tieup:
                //buff
                case MechanicsType.ShieldTime:
                case MechanicsType.ShieldValue:
                case MechanicsType.DrainTime:
                case MechanicsType.DrainValue:
                case MechanicsType.PhysicsDefensePercentTime:
                case MechanicsType.PhysicsDefensePercentValue:
                case MechanicsType.MagicDefensePercentTime:
                case MechanicsType.MagicDefensePercentValue:
                case MechanicsType.PhysicsAttackPercentTime:
                case MechanicsType.PhysicsAttackPercentValue:
                case MechanicsType.MagicAttackPercentTime:
                case MechanicsType.MagicAttackPercentValue:
                case MechanicsType.HPLimitPercentTime:
                case MechanicsType.HPLimitPercentValue:
                case MechanicsType.SpeedPercentTime:
                case MechanicsType.SpeedPercentValue:
                case MechanicsType.PhysicsDefenseFixedTime:
                case MechanicsType.PhysicsDefenseFixedValue:
                case MechanicsType.MagicDefenseFixedTime:
                case MechanicsType.MagicDefenseFixedValue:
                case MechanicsType.PhysicsAttackFixedTime:
                case MechanicsType.PhysicsAttackFixedValue:
                case MechanicsType.MagicAttackFixedTime:
                case MechanicsType.MagicAttackFixedValue:
                case MechanicsType.HPLimitFixedTime:
                case MechanicsType.HPLimitFixedValue:
                case MechanicsType.SpeedFixedTime:
                case MechanicsType.SpeedFixedValue:
                case MechanicsType.HitTime:
                case MechanicsType.HitValue:
                case MechanicsType.DodgeTime:
                case MechanicsType.DodgeValue:
                case MechanicsType.CritTime:
                case MechanicsType.CritValue:
                case MechanicsType.AntiCritTime:
                case MechanicsType.AntiCritValue:
                case MechanicsType.BlockTime:
                case MechanicsType.BlockValue:
                case MechanicsType.AntiBlockTime:
                case MechanicsType.AntiBlockValue:
                case MechanicsType.CounterAtkTime:
                case MechanicsType.CounterAtkValue:
                case MechanicsType.CritHurtAddTime:
                case MechanicsType.CritHurtAddValue:
                case MechanicsType.CritHurtDecTime:
                case MechanicsType.CritHurtDecValue:
                case MechanicsType.ArmorTime:
                case MechanicsType.ArmorValue:
                case MechanicsType.DamageDecTime:
                case MechanicsType.DamageDecValue:
                case MechanicsType.DamageAddTime:
                case MechanicsType.DamageAddValue:
                case MechanicsType.Immune:
                case MechanicsType.ReboundTime:
                case MechanicsType.ReboundValue:
                case MechanicsType.ImmunePhysicsAttack:
                case MechanicsType.ImmuneMagicAttack:
                case MechanicsType.Tag:
                case MechanicsType.GeneralSkillHit:
                case MechanicsType.GeneralSkillCrit:
                case MechanicsType.AccumulatorTag:
                    if (isPlayer)
                        PlayerBuff(skillInfo, mechanicsData, character, target2, mechanicsValue, judgeType);
                    else
                        EnemyBuff(skillInfo, mechanicsData, character, target2, mechanicsValue, judgeType);
                    break;
                case MechanicsType.Transform:
                    Transform(character, target2, mechanicsValue);
                    break;
            }
            if (Fight.Controller.FightController.instance.fightType == FightType.ConsortiaFight)
            {
                //buff
                if (skillInfo.mechanicsIndex == 0)
                {
                    ConsortiaBuffs(character, skillInfo);
                }
                skillInfo.mechanicsIndex++;
            }
        }

        #region attack
        private void PlayerAttack(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            if (target1 != null)
            {
                if (Fight.Controller.FightController.instance.fightType == FightType.FirstFight)
                    judgeType = 1;
                //if (Fight.Controller.FightController.instance.fightType == FightType.DailyPVE)
                //{
                //    if (Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.ActitityFloat)
                //    {
                //        if (!Fight.Controller.FightController.instance.isComboing)
                //            judgeType = 0;
                //    }
                //}
                JudgeController.instance.Judge(character, target2, skillInfo, mechanicsData, mechanicsValue, judgeType);
                if (FightController.instance.isComboing)
                {
                    if (target1.anim.speed == Game.GameSetting.instance.slowSpeed)
                        target1.anim.speed = 1;
                }
                if (judgeType > 0)
                {
                    PlayMechanicsAction(skillInfo, mechanicsData, character, target1, false);
                    EffectController.instance.SetAttackRootTips(target1.positionId);
                }
            }
        }

        private void EnemyAttack(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target1, CharacterEntity target2, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            if (target1 != null)
            {
                if (Fight.Controller.FightController.instance.fightType == FightType.FirstFight)
                    judgeType = 1;
                JudgeController.instance.Judge(character, target2, skillInfo, mechanicsData, mechanicsValue, judgeType);
                if (FightController.instance.isComboing)
                {
                    if (target1.anim.speed == Game.GameSetting.instance.slowSpeed)
                        target1.anim.speed = 1;
                }
                if (judgeType > 0)
                {
                    PlayMechanicsAction(skillInfo, mechanicsData, character, target1, false);
                    EffectController.instance.SetAttackRootTips(target1.positionId);
                }
            }
        }

        [LuaInterface.NoToLua]
        public void PlayMechanicsAction(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, bool breakable)
        {
            if (character == target) return;//自己给自己造成伤害，没有动画
            MechanicsAction mechanicsAction = new MechanicsAction();
            mechanicsAction.mechanicsMaker = character;
            mechanicsAction.character = target;
            mechanicsAction.mechanicsData = mechanicsData;
            mechanicsAction.skillInfo = skillInfo;
            mechanicsAction.breakable = breakable;
            mechanicsAction.Execute();
        }

        [LuaInterface.NoToLua]
        public void PlayMechanicsAction(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, bool breakable, MechanicsType mechanicsType)
        {
            if (character == target) return;//自己给自己造成伤害，没有动画
            MechanicsAction mechanicsAction = new MechanicsAction();
            mechanicsAction.mechanicsMaker = character;
            mechanicsAction.character = target;
            mechanicsAction.mechanicsData = mechanicsData;
            mechanicsAction.skillInfo = skillInfo;
            mechanicsAction.breakable = breakable;
            mechanicsAction.mechanicsType = mechanicsType;
            mechanicsAction.Execute();
        }

        public void SetDamageValue(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, uint HPValue, bool isContinuous, float delay, int judgeType)
        {
            JudgeController.instance.SetDamageValue(character, target, skillInfo, HPValue, isContinuous, delay, judgeType);
        }

        public void CalcDamage(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, int judgeType, float mechanicsValueA, float mechanicsValueB, float mechanicsValueC, float delay)
        {
            JudgeController.instance.CalcDamage(character, target, skillInfo, mechanics, judgeType, mechanicsValueA, mechanicsValueB, mechanicsValueC, delay);
        }
        #endregion

        #region treat
        private void PlayerTreat(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            PlayBuffEffect(target, mechanicsData);
            JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
        }

        private void EnemyTreat(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            PlayBuffEffect(target, mechanicsData);
            JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
        }

        public void SetTreatValue(CharacterEntity target, float HPValue, bool isContinuous)
        {
            JudgeController.instance.SetTreatValue(target, HPValue, isContinuous);
        }
        #endregion

        #region buff
        private void PlayerBuff(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.FirstFight:
                    break;
                case FightType.ConsortiaFight:
                    switch (mechanicsData.mechanicsType)
                    {
                        case MechanicsType.Float:
                        case MechanicsType.Tumble:
                        //即死
                        case MechanicsType.ForceKill:
                        //复活
                        case MechanicsType.Reborn:
                        case MechanicsType.RandomMechanics:
                            JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
                            break;
                    }
                    break;
                default:
                    JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
                    break;
            }
            //PlayBuffEffect(target, mechanicsData);
            //JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
        }

        private void EnemyBuff(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.FirstFight:
                    break;
                case FightType.ConsortiaFight:
                    switch (mechanicsData.mechanicsType)
                    {
                        case MechanicsType.Float:
                        case MechanicsType.Tumble:
                        //即死
                        case MechanicsType.ForceKill:
                        //复活
                        case MechanicsType.Reborn:
                        case MechanicsType.RandomMechanics:
                            JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
                            break;
                    }
                    break;
                default:
                    JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
                    break;
            }
        }

        private static void ConsortiaBuffs(CharacterEntity character, SkillInfo skillInfo)
        {
            List<Logic.Protocol.Model.Buff> buffs = Fight.Model.FightProxy.instance.GetConsortiaBuffList((int)character.characterInfo.instanceID, (int)skillInfo.skillData.skillId);
            if (buffs != null)
            {
                for (int i = 0, count = buffs.Count; i < count; i++)
                {
                    Logic.Protocol.Model.Buff buff = buffs[i];
                    CharacterEntity target = CharacterUtil.FindTarget((uint)buff.desId);
                    if (target)
                    {
                        MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById((uint)buff.mechanicsId);
                        target.AddBuff(target, mechanicsData, (BuffType)buff.type, buff.time, buff.intervalTime, buff.count, buff.effectValue);
                    }
                }
            }
            List<Logic.Protocol.Model.Buff> delBuffs = Fight.Model.FightProxy.instance.GetConsortiaDelBuffList((int)character.characterInfo.instanceID, (int)skillInfo.skillData.skillId);
            if (delBuffs != null)
            {
                for (int i = 0, count = buffs.Count; i < count; i++)
                {
                    Logic.Protocol.Model.Buff buff = buffs[i];
                    CharacterEntity target = CharacterUtil.FindTarget((uint)buff.desId);
                    if (target)
                    {
                        BuffType buffType = (BuffType)buff.type;
                        target.RemoveBuff(buffType);
                        bool kindness = BuffUtil.Judge(buffType, buff.effectValue);
                        string path = Fight.Model.FightProxy.instance.GetIconPath(buffType, kindness);
                        target.RemoveBuffIcon(path);
                    }
                }
            }
        }

        public void PlayBuffEffect(CharacterEntity target, MechanicsData mechanicsData)
        {
            if (!Logic.Game.GameSetting.instance.effectable) return;
            for (int i = 0, count = mechanicsData.effectIds.Length; i < count; i++)
            {
                EffectInfo effectInfo = new EffectInfo(mechanicsData.effectIds[i]);
                if (effectInfo.effectData == null) continue;
                //Debugger.Log(effectInfo.effectData.effectType);
                effectInfo.character = target;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        effectInfo.pos = PositionData.GetPostionDataById(target.positionId).position + effectInfo.effectData.offset;
                        effectInfo.target = target;
                        break;
                    case EffectType.LockTarget:
                        effectInfo.target = target;
                        break;
                    case EffectType.ChangeColor:
                        effectInfo.target = target;
                        break;
                    case EffectType.LockPart:
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, target.transform);
                        effectInfo.target = target;
                        break;
                }
                effectInfo.delay = effectInfo.effectData.delay;
                EffectController.instance.PlayNoSkillEffect(effectInfo);
            }
        }
        #endregion

        #region angry
        public void SetAngry(float angry)
        {
            FightController.instance.SetAngry(angry);
        }
        #endregion

        #region transform
        public void Transform(CharacterEntity target, int tranformId, int mechanicsId)
        {
            FightController.instance.FightHangupOrder(() =>
            {
                Transform(null, target, new Triple<float, float, float>(tranformId, mechanicsId, 0f));
            });
        }
        private void Transform(CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue)
        {
            int tranformId = (int)mechanicsValue.a;
            Transforms.Model.HeroTransformData heroTransformData = Transforms.Model.HeroTransformData.GetHeroTransformDataByID(tranformId);
            if (heroTransformData == null)
            {
                FightController.instance.FightRegainOrder();
                return;
            }
            uint mechanicsId = (uint)mechanicsValue.b;
            MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
            if (mechanicsData != null)
                PlayBuffEffect(target, mechanicsData);
            AnimatorUtil.Play(target.anim, AnimatorUtil.TRANSFORM_ID, 0, 0f);
            switch (heroTransformData.heroTransformType)
            {
                case HeroTransformType.ModelAndSkill:
                    StartCoroutine(TransformModelCoroutine(heroTransformData, target));
                    break;
                case HeroTransformType.AnimationAndSkill:
                    StartCoroutine(TransformAnimationCoroutine(heroTransformData, target));
                    break;
                case HeroTransformType.Scale:
                    StartCoroutine(TransformScaleCoroutine(heroTransformData, target));
                    break;
            }
        }

        private IEnumerator TransformModelCoroutine(Transforms.Model.HeroTransformData heroTransformData, CharacterEntity target)
        {
            yield return new WaitForSeconds(heroTransformData.changeTime);
            bool transform = false;
            Hero.Model.HeroData heroData = Hero.Model.HeroData.GetHeroDataByID((int)heroTransformData.heroId);
            if (heroData != null)
            {
                if (target is PlayerEntity)
                {
                    PlayerEntity player = target as PlayerEntity;
                    Player.Model.PlayerInfo playerInfo = PlayerController.instance.GetPlayerInfo();
                    if (playerInfo != null)
                    {
                        Player.Model.PlayerInfo playerInfoCopy = playerInfo.GetPlayerInfoCopy();
                        playerInfoCopy.heroData = heroData;
                        PlayerEntity playerCopy = PlayerEntity.CreatePlayerEntity(playerInfoCopy, null);
                        playerCopy.gameObject.SetActive(false);
                        yield return null;
                        //UnityEngine.Object.Destroy(playerCopy.petEntiy.gameObject);
                        Map.Controller.MapController.instance.RemoveTarget(player.rootNode.transform);

                        Animator animTemplete = player.anim;
                        player.anim = playerCopy.anim;
                        playerCopy.anim = animTemplete;

                        GameObject rootNodeTemplete = player.rootNode;
                        player.rootNode = playerCopy.rootNode;
                        playerCopy.rootNode = rootNodeTemplete;

                        player.anim.transform.SetParent(playerCopy.transform, false);
                        playerCopy.anim.transform.SetParent(player.transform, false);

                        LockTransform lockTransform = player.petEntity.gameObject.GetComponent<LockTransform>();
                        lockTransform.trans = player.rootNode.transform;
                        Vector2 size = HeroEntity.GetShadowSize(playerInfoCopy.heroData.shadowType);
                        Map.Controller.MapController.instance.AddTarget(player.rootNode.transform, size);
                        //UnityEngine.Object.Destroy(player.anim.gameObject);
                        //playerCopy.anim.transform.SetParent(player.transform, false);
                        //UnityEngine.Object.Destroy(playerCopy.gameObject);
                        Pool.Controller.PoolController.instance.Despawn(playerCopy.name, playerCopy);
                        //Debugger.Log("pool name:{0}  {1}", playerCopy.name, (spawnPool == null));
                        //spawnPool.Despawn(playerCopy.transform);

                        player.characterInfo = player.characterInfo.UpdateCharacterBaseInfo(heroData);
                        DataMessageHandler.DataMessage_BreakSkill(player.characterInfo.instanceID);
                        target.SetOrderable();
                        player.skillItemBoxView.InitSkill();
                        PlayerController.instance.SortSkillInfos();
                        transform = true;
                    }
                }
                else if (target is HeroEntity)
                {
                    Hero.Model.HeroInfo heroInfo = PlayerController.instance.GetHeroInfo(target.characterInfo.instanceID);
                    if (heroInfo != null)
                    {
                        //Hero.Model.HeroInfo heroInfoCopy = heroInfo.GetHeroInfoCopy();

                        transform = true;
                    }
                }
                else if (target is EnemyPlayerEntity)
                {
                    Player.Model.PlayerInfo playerInfo = EnemyController.instance.GetEnemyPlayerInfo();
                    if (playerInfo != null)
                    {
                        //Player.Model.PlayerInfo playerInfoCopy = playerInfo.GetPlayerInfoCopy();
                        //EnemyPlayerEntity.CreatePlayerEntity(target as EnemyPlayerEntity, playerInfo);
                        transform = true;
                    }
                }
                else if (target is EnemyEntity)
                {
                    Hero.Model.HeroInfo heroInfo = EnemyController.instance.GetEnemyHeroInfo(target.characterInfo.instanceID);
                    if (heroInfo != null)
                    {
                        //Hero.Model.HeroInfo heroInfoCopy = heroInfo.GetHeroInfoCopy();
                        transform = true;
                    }
                }
            }
            FightController.instance.FightRegainOrder();
            if (transform)
            {
                if (heroTransformData.scale != Vector3.zero)
                    target.scale = heroTransformData.scale;
                if (heroTransformData.color.a != 0)
                    EffectController.instance.SetColor(target, heroTransformData.color);
                if (heroTransformData.duration > 0)
                {
                    Debugger.Log("duration:{0}", heroTransformData.duration);
                    float time = Time.time;
                    float lastTime = time;
                    float delay = heroTransformData.duration;
                    while (Time.time - time < delay)
                    {
                        if (!target.tickCD)
                            time += (Time.time - lastTime);
                        lastTime = Time.time;
                        yield return null;
                    }
                }
            }
        }

        private IEnumerator TransformAnimationCoroutine(Transforms.Model.HeroTransformData heroTransformData, CharacterEntity target)
        {
            PlayCloseupCamera(target);
            yield return new WaitForSeconds(heroTransformData.changeTime);
            FinishCloseupCamera(target);
            string path = string.Empty;
            if (target is PlayerEntity || target is EnemyPlayerEntity)
                path = ResPath.GetPlayerAnimatorControllerPath(heroTransformData.animationName);
            else
                path = ResPath.GetHeroAnimatorControllerPath(heroTransformData.animationName);
            target.anim.runtimeAnimatorController = ResMgr.instance.Load<Object>(path) as RuntimeAnimatorController;
            Vector3 scale = target.scale;
            if (heroTransformData.scale != Vector3.zero)
                target.scale = heroTransformData.scale;
            if (heroTransformData.color.a != 0)
                EffectController.instance.SetColor(target, heroTransformData.color);
            for (int i = 0, count = heroTransformData.skillIds.Length; i < count; i++)
            {
                target.characterInfo.UpdateCharacterBaseInfo(heroTransformData.backSkillIds[i], heroTransformData.skillIds[i]);
            }
            DataMessageHandler.DataMessage_BreakSkill(target.characterInfo.instanceID);
            target.SetOrderable();
            if (target is HeroEntity)
            {
                HeroEntity hero = target as HeroEntity;
                if (hero.skillItemBoxView)
                    hero.skillItemBoxView.InitSkill();
            }
            if (target.isPlayer)
                PlayerController.instance.SortSkillInfos();
            else
                EnemyController.instance.SortSkillInfos();
            FightController.instance.FightRegainOrder();
            if (heroTransformData.backable)
            {
                float time = Time.time;
                float lastTime = time;
                float delay = heroTransformData.duration;
                while (Time.time - time < delay)
                {
                    if (!target.tickCD)
                        time += (Time.time - lastTime);
                    lastTime = Time.time;
                    yield return null;
                }
                //yield return new WaitForSeconds();
                if (target)
                {
                    while (true)
                    {
                        if (target.status == Status.Idle)
                            break;
                        yield return null;
                    }
                    if (target)
                    {
                        if (target is PlayerEntity || target is EnemyPlayerEntity)
                            path = ResPath.GetPlayerAnimatorControllerPath(heroTransformData.backAnimationName);
                        else
                            path = ResPath.GetHeroAnimatorControllerPath(heroTransformData.backAnimationName);
                        if (target.anim)
                            target.anim.runtimeAnimatorController = ResMgr.instance.Load<Object>(path) as RuntimeAnimatorController;
                        target.scale = scale;
                        for (int i = 0, count = heroTransformData.skillIds.Length; i < count; i++)
                        {
                            target.characterInfo.UpdateCharacterBaseInfo(heroTransformData.skillIds[i], heroTransformData.backSkillIds[i]);
                        }
                        DataMessageHandler.DataMessage_BreakSkill(target.characterInfo.instanceID);
                        target.SetOrderable();
                        if (target is HeroEntity)
                        {
                            HeroEntity hero = target as HeroEntity;
                            if (hero.skillItemBoxView != null)
                                hero.skillItemBoxView.InitSkill();
                        }
                        if (target.isPlayer)
                            PlayerController.instance.SortSkillInfos();
                        else
                            EnemyController.instance.SortSkillInfos();
                    }
                }
            }
        }

        private IEnumerator TransformScaleCoroutine(Transforms.Model.HeroTransformData heroTransformData, CharacterEntity target)
        {
            yield return new WaitForSeconds(heroTransformData.changeTime);
            Vector3 scale = target.scale;
            if (heroTransformData.scale != Vector3.zero)
                target.scale = heroTransformData.scale;
            if (heroTransformData.color.a != 0)
                EffectController.instance.SetColor(target, heroTransformData.color);
            FightController.instance.FightRegainOrder();
            if (heroTransformData.backable)
            {
                float time = Time.time;
                float lastTime = time;
                float delay = heroTransformData.duration;
                while (Time.time - time < delay)
                {
                    if (!target.tickCD)
                        time += (Time.time - lastTime);
                    lastTime = Time.time;
                    yield return null;
                }
                target.scale = scale;
            }
        }

        private void PlayCloseupCamera(CharacterEntity character)
        {
            //return;
            if (Game.GameSetting.instance.closeupCameraable)
            {
                if (!character.isPlayer && EnemyController.instance.isBoss(character.characterInfo.instanceID))
                {
                    FightController.instance.isCloseup = true;
                    //layer = (int)LayerType.Closeup;
                    EnemyController.instance.ShowHPBarViews(false);
                    PlayerController.instance.ShowHPBarViews(false);
                    //if (Skill.SkillUtil.IsComboStartSkill(skillInfo) == MechanicsType.None)
                    Logic.UI.SkillBar.Controller.SkillBarController.instance.Show(false);
                    Judge.Controller.JudgeController.instance.ShowDamageBarViewPool(false);
                    Cameras.Controller.CameraController.instance.ShowMainCamera(false);
                    Transform parent = TransformUtil.Find(Logic.Cameras.Controller.CameraController.CAMERA_NODE, character.transform, true);
                    Cameras.Controller.CameraController.instance.ShowCloseupCamera(character, parent);
                    TransformUtil.SwitchLayer(character.transform, (int)LayerType.Closeup);
                }
            }
        }

        private void FinishCloseupCamera(CharacterEntity character)
        {
            if (Game.GameSetting.instance.closeupCameraable)
            {
                //Cameras.Controller.CameraController.instance.ShowCloseupCamera(false, false, null);
                Cameras.Controller.CameraController.instance.ResetCloseupCamera(character);
                Cameras.Controller.CameraController.instance.ShowMainCamera(true);
                Common.Util.TransformUtil.SwitchLayer(character.transform, (int)LayerType.Fight);
                //CharacterUtil.SwitchCharacterLayer(skillInfo, timelineList, isPlayer, (int)LayerType.Fight);
                EnemyController.instance.ShowHPBarViews(true);
                PlayerController.instance.ShowHPBarViews(true);
                Logic.UI.SkillBar.Controller.SkillBarController.instance.Show(true);
                Judge.Controller.JudgeController.instance.ShowDamageBarViewPool(true);
            }
        }
        #endregion

        #region mechanics
        private void ReleaseMechanics(SkillInfo skillInfo, MechanicsData mechanicsData, CharacterEntity character, CharacterEntity target, Triple<float, float, float> mechanicsValue, int judgeType)
        {
            if (Fight.Controller.FightController.instance.fightType == FightType.FirstFight)
                judgeType = 1;
            PlayMechanicsEffect(target, mechanicsData);
            JudgeController.instance.Judge(character, target, skillInfo, mechanicsData, mechanicsValue, judgeType);
        }

        private void PlayMechanicsEffect(CharacterEntity target, MechanicsData mechanicsData)
        {
            for (int i = 0, count = mechanicsData.effectIds.Length; i < count; i++)
            {
                EffectInfo effectInfo = new EffectInfo(mechanicsData.effectIds[i]);
                if (effectInfo.effectData == null) continue;
                //Debugger.Log(effectInfo.effectData.effectType);
                effectInfo.character = target;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        effectInfo.pos = PositionData.GetPostionDataById(target.positionId).position + effectInfo.effectData.offset;
                        effectInfo.target = target;
                        break;
                    case EffectType.LockTarget:
                        effectInfo.target = target;
                        break;
                    case EffectType.ChangeColor:
                        effectInfo.target = target;
                        break;
                    case EffectType.LockPart:
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, target.transform);
                        effectInfo.target = target;
                        break;
                }
                effectInfo.delay = effectInfo.effectData.delay;
                EffectController.instance.PlayEffect(effectInfo);
            }
        }

        #endregion
    }
}
