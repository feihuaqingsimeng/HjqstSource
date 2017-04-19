using Logic.Enums;
using Logic.Character.Controller;
using System.Collections.Generic;
using Logic.Skill.Model;
using Common.Util;
using Logic.Effect.Controller;
using Logic.Skill;
using Logic.Character.Model;
namespace Logic.Character
{
    public static class CharacterUtil
    {
        /// <summary>
        /// 查找攻击目标范围
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mechanicsData"></param>
        /// <returns></returns>
        public static List<uint> GetTargetRangePositionIds(CharacterEntity target, MechanicsData mechanicsData, bool isPlayer)
        {
            List<uint> result = null;
            switch (mechanicsData.rangeType)
            {
                case RangeType.CurrentRow:
                    result = Position.Model.PositionData.GetRows((int)target.positionData.rowNum, isPlayer);
                    break;
                case RangeType.CurrentColumn:
                    result = Position.Model.PositionData.GetColumns((int)target.positionData.columnNum, isPlayer);
                    break;
                case RangeType.All:
                case RangeType.AllAbsolutely:
                    result = new List<uint>();
                    for (int i = 1; i <= 9; i++)
                    {
                        if (isPlayer)
                            result.Add((uint)i);
                        else
                            result.Add((uint)(100 + i));
                    }
                    break;
                case RangeType.CurrentAndBehindFirst:
                    result = new List<uint>();
                    result.Add(target.positionData.positionId);
                    if (target.positionData.columnNum <= 2)
                        result.Add(target.positionData.positionId + 3);
                    break;
                case RangeType.CurrentAndNearCross:
                    result = new List<uint>();
                    result.Add(target.positionData.positionId);
                    if (target.positionData.rowNum > 1)
                        result.Add(target.positionData.positionId - 1);
                    if (target.positionData.rowNum <= 2)
                        result.Add(target.positionData.positionId + 1);
                    if (target.positionData.columnNum <= 2)
                        result.Add(target.positionData.positionId + 3);
                    break;
                case RangeType.Cross:
                    result = new List<uint>();
                    for (int i = 1; i <= 9; i++)
                    {
                        if (i % 2 == 0 || i == 5)
                        {
                            if (isPlayer)
                                result.Add((uint)i);
                            else
                                result.Add((uint)(100 + i));
                        }
                    }
                    break;
                case RangeType.FirstColum:
                    result = Position.Model.PositionData.GetColumns(1, isPlayer);
                    break;
                case RangeType.SecondColum:
                    result = Position.Model.PositionData.GetColumns(2, isPlayer);
                    break;
                case RangeType.ThirdColum:
                    result = Position.Model.PositionData.GetColumns(3, isPlayer);
                    break;
                case RangeType.FirstRow:
                    result = Position.Model.PositionData.GetRows(1, isPlayer);
                    break;
                case RangeType.SecondRow:
                    result = Position.Model.PositionData.GetRows(2, isPlayer);
                    break;
                case RangeType.ThirdRow:
                    result = Position.Model.PositionData.GetRows(3, isPlayer);
                    break;
                case RangeType.ExceptMidpoint:
                    result = new List<uint>();
                    for (int i = 1; i <= 9; i++)
                    {
                        if (i != 5)
                        {
                            if (isPlayer)
                                result.Add((uint)i);
                            else
                                result.Add((uint)(100 + i));
                        }
                    }
                    break;
                case RangeType.Midpoint:
                    result = new List<uint>();
                    if (isPlayer)
                        result.Add((uint)5);
                    else
                        result.Add((uint)105);
                    break;
                case RangeType.LeadingDiagonal:
                    result = new List<uint>();
                    if (isPlayer)
                    {
                        result.Add((uint)1);
                        result.Add((uint)5);
                        result.Add((uint)9);
                    }
                    else
                    {
                        result.Add((uint)101);
                        result.Add((uint)105);
                        result.Add((uint)109);
                    }
                    break;
                case RangeType.SecondaryDiagonal:
                    result = new List<uint>();
                    if (isPlayer)
                    {
                        result.Add((uint)3);
                        result.Add((uint)5);
                        result.Add((uint)7);
                    }
                    else
                    {
                        result.Add((uint)103);
                        result.Add((uint)105);
                        result.Add((uint)107);
                    }
                    break;
                case RangeType.CurrentAndBehindTowColum:
                    result = new List<uint>();
                    result.Add(target.positionData.positionId);
                    if (target.positionData.columnNum < 2)
                    {
                        if (target.isPlayer)
                        {
                            result.Add((uint)4);
                            result.Add((uint)5);
                            result.Add((uint)6);
                            result.Add((uint)7);
                            result.Add((uint)8);
                            result.Add((uint)9);
                        }
                        else
                        {
                            result.Add((uint)104);
                            result.Add((uint)105);
                            result.Add((uint)106);
                            result.Add((uint)107);
                            result.Add((uint)108);
                            result.Add((uint)109);
                        }
                    }
                    else if (target.positionData.columnNum < 3)
                    {
                        if (target.isPlayer)
                        {
                            result.Add((uint)7);
                            result.Add((uint)8);
                            result.Add((uint)9);
                        }
                        else
                        {
                            result.Add((uint)107);
                            result.Add((uint)108);
                            result.Add((uint)109);
                        }
                    }
                    break;
                case RangeType.BehindTowColum:
                    result = new List<uint>();
                    if (target.isPlayer)
                    {
                        result.Add((uint)4);
                        result.Add((uint)5);
                        result.Add((uint)6);
                        result.Add((uint)7);
                        result.Add((uint)8);
                        result.Add((uint)9);
                    }
                    else
                    {
                        result.Add((uint)104);
                        result.Add((uint)105);
                        result.Add((uint)106);
                        result.Add((uint)107);
                        result.Add((uint)108);
                        result.Add((uint)109);
                    }
                    break;
            }
            return result;
        }

        public static List<KeyValuePair<uint, uint>> FindAllTargets(TargetType targetType, bool isPlayer)
        {
            List<KeyValuePair<uint, uint>> result = new List<KeyValuePair<uint, uint>>();
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                    {
                        uint[] keys = PlayerController.instance.heroDic.GetKeyArray();
                        for (int i = 0, count = keys.Length; i < count; i++)
                        {
                            result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                        }
                    }
                    else
                    {
                        uint[] keys = EnemyController.instance.enemyDic.GetKeyArray();
                        for (int i = 0, count = keys.Length; i < count; i++)
                        {
                            result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                        }
                    }
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                    {
                        uint[] keys = EnemyController.instance.enemyDic.GetKeyArray();
                        for (int i = 0, count = keys.Length; i < count; i++)
                        {
                            result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                        }
                    }
                    else
                    {
                        uint[] keys = PlayerController.instance.heroDic.GetKeyArray();
                        for (int i = 0, count = keys.Length; i < count; i++)
                        {
                            result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                        }
                    }
                    break;
            }
            return result;
        }

        public static List<KeyValuePair<uint, uint>> FindDeadTargets(RangeType rangeType, TargetType targetType, bool isPlayer)
        {
            List<KeyValuePair<uint, uint>> result = new List<KeyValuePair<uint, uint>>();
            switch (rangeType)
            {
                case RangeType.CurrentSingle:
                    switch (targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                            {
                                if (PlayerController.instance.deadHeroDic.Count > 0)
                                {
                                    uint id = PlayerController.instance.deadHeroDic.First().Key;
                                    result.Add(new KeyValuePair<uint, uint>(id, id));
                                }
                            }
                            else
                            {
                                if (EnemyController.instance.deadEnemyDic.Count > 0)
                                {
                                    uint id = EnemyController.instance.deadEnemyDic.First().Key;
                                    result.Add(new KeyValuePair<uint, uint>(id, id));
                                }
                            }
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                            {
                                if (EnemyController.instance.deadEnemyDic.Count > 0)
                                {
                                    uint id = EnemyController.instance.deadEnemyDic.First().Key;
                                    result.Add(new KeyValuePair<uint, uint>(id, id));
                                }
                            }
                            else
                            {
                                if (PlayerController.instance.deadHeroDic.Count > 0)
                                {
                                    uint id = PlayerController.instance.deadHeroDic.First().Key;
                                    result.Add(new KeyValuePair<uint, uint>(id, id));
                                }
                            }
                            break;
                    }
                    break;
                case RangeType.CurrentRow:
                    break;
                case RangeType.CurrentColumn:
                    break;
                case RangeType.All:
                case RangeType.AllAbsolutely:
                    switch (targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                            {
                                uint[] keys = PlayerController.instance.deadHeroDic.GetKeyArray();
                                for (int i = 0, count = keys.Length; i < count; i++)
                                {
                                    result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                                }
                            }
                            else
                            {
                                uint[] keys = EnemyController.instance.deadEnemyDic.GetKeyArray();
                                for (int i = 0, count = keys.Length; i < count; i++)
                                {
                                    result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                                }
                            }
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                            {
                                uint[] keys = EnemyController.instance.deadEnemyDic.GetKeyArray();
                                for (int i = 0, count = keys.Length; i < count; i++)
                                {
                                    result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                                }
                            }
                            else
                            {
                                uint[] keys = PlayerController.instance.deadHeroDic.GetKeyArray();
                                for (int i = 0, count = keys.Length; i < count; i++)
                                {
                                    result.Add(new KeyValuePair<uint, uint>(keys[i], keys[i]));
                                }
                            }
                            break;
                    }
                    break;
                case RangeType.CurrentAndBehindFirst:
                    break;
                case RangeType.CurrentAndNearCross:
                    break;
                case RangeType.CurrentBehindFirst:
                    break;
                case RangeType.CurrentBehindSecond:
                    break;
                case RangeType.CurrentIntervalOne:
                    break;
                case RangeType.CurrentAndRandomTwo:
                    break;
                case RangeType.RandomN:
                    break;
                case RangeType.Cross:
                    break;
                case RangeType.FirstColum:
                    break;
                case RangeType.SecondColum:
                    break;
                case RangeType.ThirdColum:
                    break;
                case RangeType.FirstRow:
                    break;
                case RangeType.SecondRow:
                    break;
                case RangeType.ThirdRow:
                    break;
                case RangeType.ExceptMidpoint:
                    break;
                case RangeType.Midpoint:
                    break;
                case RangeType.LeadingDiagonal:
                    break;
                case RangeType.SecondaryDiagonal:
                    break;
                case RangeType.Weakness:
                    break;
                case RangeType.LowestHP:
                    break;
                case RangeType.RandomSingle:
                    break;
                case RangeType.CurrentAndBehindTowColum:
                    break;
                case RangeType.BehindTowColum:
                    break;
            }
            return result;
        }

        public static CharacterEntity FindTarget(MechanicsType mechanicsType, TargetType targetType, bool isPlayer, uint tid)
        {
            CharacterEntity result = null;
            if (mechanicsType == MechanicsType.Reborn)
            {
                switch (targetType)
                {
                    case TargetType.Ally:
                        if (isPlayer)
                            result = PlayerController.instance.GetDeadHeroById(tid);
                        else
                            result = EnemyController.instance.GetDeadHeroById(tid);
                        break;
                    case TargetType.Enemy:
                        if (isPlayer)
                            result = EnemyController.instance.GetDeadHeroById(tid);
                        else
                            result = PlayerController.instance.GetDeadHeroById(tid);
                        break;
                }
                if (result == null)
                    result = FindTarget(targetType, isPlayer, tid);
            }
            else
                result = FindTarget(targetType, isPlayer, tid);
            return result;
        }

        public static CharacterEntity FindTarget(TargetType targetType, bool isPlayer, uint tid)
        {
            CharacterEntity result = null;
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                        result = PlayerController.instance[tid];
                    else
                        result = EnemyController.instance[tid];
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                        result = EnemyController.instance[tid];
                    else
                        result = PlayerController.instance[tid];
                    break;
            }
            return result;
        }

        public static CharacterEntity FindTarget(uint tid)
        {
            CharacterEntity result = null;
            result = PlayerController.instance[tid];
            if (!result)
                result = EnemyController.instance[tid];
            return result;
        }

        public static int PlayerFirst(CharacterEntity x, CharacterEntity y)
        {
            if (x is PlayerEntity && y is HeroEntity)
                return -1;
            else if (x is HeroEntity && y is PlayerEntity)
                return 1;
            else if (x.positionId < y.positionId)
                return -1;
            else if (x.positionId > y.positionId)
                return 1;
            return 0;
        }

        public static RoleAttackAttributeType GetRoleAttackAttributeType(RoleType roleType)
        {
            RoleAttackAttributeType roleAttackAttributeType = RoleAttackAttributeType.Invalid;
            switch (roleType)
            {
                case RoleType.Defence:
                case RoleType.Offence:
                case RoleType.Mighty:
                    roleAttackAttributeType = RoleAttackAttributeType.PhysicalAttack;
                    break;
                case RoleType.Mage:
                case RoleType.Support:
                    roleAttackAttributeType = RoleAttackAttributeType.MagicalAttack;
                    break;
                default:
                    break;
            }
            return roleAttackAttributeType;
        }

        public static List<CharacterEntity> SwitchCharacterLayer(SkillInfo skillInfo, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, bool isPlayer, int layer)
        {
            List<CharacterEntity> result = new List<CharacterEntity>();
            for (int j = 0, count = timelineList.Count; j < count; j++)
            {
                Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[j];
                List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                //List<Triple<float, float, float>> mechanicsValues = skillInfo.skillData.mechanicsValues[j];
                for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                {
                    uint mechanicsId = mechanicsKeys[k];
                    List<KeyValuePair<uint, uint>> tids = mechanicsDic[mechanicsId];
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                    for (int m = 0, mCount = tids.Count; m < mCount; m++)
                    {
                        KeyValuePair<uint, uint> tid = tids[m];
                        CharacterEntity target1 = CharacterUtil.FindTarget(mechanicsData.targetType, isPlayer, tid.Key);
                        //CharacterEntity target2 = CharacterUtil.FindTarget(mechanicsData.targetType, isPlayer, tid.Value);
                        if (target1)
                        {
                            TransformUtil.SwitchLayer(target1.transform, layer);
                            result.Add(target1);
                        }
                        //TransformUtil.SwitchLayer(target2.transform, layer);
                    }
                }
            }
            return result;
        }

        public static uint GetEffectIdByBuffType(BuffInfo buffInfo)
        {
            uint effectId = 0;
            switch (buffInfo.buffType)
            {
                case BuffType.Swimmy:
                    effectId = EffectController.SWIMMY_EFFECT_ID;
                    break;
                case BuffType.Invincible:
                    effectId = EffectController.INVINCIBLE_ID;
                    break;
                case BuffType.Silence:
                    effectId = EffectController.SILENCE_EFFECT_ID;
                    break;
                case BuffType.Blind:
                    break;
                case BuffType.Frozen:
                    break;
                case BuffType.Sleep:
                    break;
                case BuffType.Landification:
                    break;
                case BuffType.Tieup:
                    break;
                case BuffType.Rebound:
                    break;
                case BuffType.Float:
                    break;
                case BuffType.Tumble:
                    break;
                case BuffType.Poisoning:
                    effectId = EffectController.POISONING_EFFECT_ID;
                    break;
                case BuffType.Ignite:
                    effectId = EffectController.IGNITE_EFFECT_ID;
                    break;
                case BuffType.Bleed:
                    effectId = EffectController.BLEED_EFFECT_ID;
                    break;
                case BuffType.Treat:

                    break;
                case BuffType.TreatPercent:

                    break;
                case BuffType.Speed:
                    effectId = EffectController.SLEEP_EFFECT_ID;
                    break;
                case BuffType.Shield:
                    effectId = EffectController.SHIELD_EFFECT_ID;
                    break;
                case BuffType.Drain:
                    break;
                case BuffType.PhysicsDefense:
                    if (buffInfo.kindness)
                        effectId = EffectController.PHYSICS_DEFENSE_ADD_EFFECT_ID;
                    else
                        effectId = EffectController.PHYSICS_DEFENSE_REDUCE_EFFECT_ID;
                    break;
                case BuffType.MagicDefense:
                    if (buffInfo.kindness)
                        effectId = EffectController.MAGIC_DEFENSE_ADD_EFFECT_ID;
                    else
                        effectId = EffectController.MAGIC_DEFENSE_REDUCE_EFFECT_ID;
                    break;
                case BuffType.PhysicsAttack:
                    if (buffInfo.kindness)
                        effectId = EffectController.PHYSICS_ATTACK_ADD_EFFECT_ID;
                    else
                        effectId = EffectController.PHYSICS_ATTACK_REDUCE_EFFECT_ID;
                    break;
                case BuffType.MagicAttack:
                    if (!buffInfo.kindness)
                        effectId = EffectController.MAGIC_ATTACK_REDUCE_EFFECT_ID;
                    break;
                case BuffType.HPLimit:
                    break;
                case BuffType.Hit:
                    break;
                case BuffType.Dodge:
                    break;
                case BuffType.Crit:
                    break;
                case BuffType.AntiCrit:
                    break;
                case BuffType.Block:
                    break;
                case BuffType.AntiBlock:
                    break;
                case BuffType.CounterAtk:
                    break;
                case BuffType.CritHurtAdd:
                    break;
                case BuffType.CritHurtDec:
                    break;
                case BuffType.Armor:
                    break;
                case BuffType.DamageDec:
                    if (buffInfo.kindness)
                        effectId = EffectController.DAMAGE_DESC_ADD_EFFECT_ID;
                    else
                        effectId = EffectController.DAMAGE_DESC_DESC_EFFECT_ID;
                    break;
                case BuffType.DamageAdd:
                    if (buffInfo.kindness)
                        effectId = EffectController.DAMAGE_ADD_ADD_EFFECT_ID;
                    else
                        effectId = EffectController.DAMAGE_ADD_DESC_EFFECT_ID;
                    break;
                case BuffType.GeneralSkillMagicAttack:
                    break;
                case BuffType.GeneralSkillPhysicsAttack:
                    break;
                case BuffType.TargetSkillMagicAttack:
                    break;
                case BuffType.TargetSkillPhysicsAttack:
                    break;
                case BuffType.DamageImmuneCount:
                    break;
                case BuffType.DamageImmuneTime:
                    break;
                case BuffType.Weakness:
                    break;
                case BuffType.TreatAdd:
                    break;
                case BuffType.ForceKill:
                    break;
                case BuffType.ImmunePhysicsAttack:
                    break;
                case BuffType.ImmuneMagicAttack:
                    break;
                case BuffType.Tag:
                    break;
                case BuffType.GeneralSkillHit:
                    break;
                case BuffType.GeneralSkillCrit:
                    break;
                case BuffType.AccumulatorTag:
                    break;
            }
            return effectId;
        }

        public static int SortSkill(SkillInfo x, SkillInfo y)
        {
            MechanicsType mtX = SkillUtil.GetSkillMechanicsType(x);
            MechanicsType mtY = SkillUtil.GetSkillMechanicsType(y);
            if ((mtX == MechanicsType.Float || mtX == MechanicsType.Tumble) && (mtY != MechanicsType.Float && mtY != MechanicsType.Tumble))
                return -1;
            if ((mtX != MechanicsType.Float && mtX != MechanicsType.Tumble) && (mtY == MechanicsType.Float || mtY == MechanicsType.Tumble))
                return 1;
            return 0;
        }
    }
}
