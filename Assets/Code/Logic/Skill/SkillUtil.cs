using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using System.Collections.Generic;
using Logic.Enums;
using LuaInterface;


namespace Logic.Skill
{
    public static class SkillUtil
    {
        [NoToLua]
        public static MechanicsType GetSkillMechanicsType(SkillInfo skillInfo)
        {
            List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
            for (int i = 0, count = timelineKeys.Count; i < count; i++)
            {
                List<uint> mechanicses = skillInfo.skillData.timeline[timelineKeys[i]];
                for (int j = 0, jCount = mechanicses.Count; j < jCount; j++)
                {
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicses[j]);
                    if (mechanicsData.mechanicsType == MechanicsType.Float)
                        return MechanicsType.Float;
                    if (mechanicsData.mechanicsType == MechanicsType.Tumble)
                        return MechanicsType.Tumble;
                }
            }
            return MechanicsType.None;
        }

        [NoToLua]
        public static MechanicsType GetSkillMechanicsType(uint skillId)
        {
            SkillData skillData = SkillData.GetSkillDataById(skillId);
            if (skillData == null)
                return MechanicsType.None;
            List<float> timelineKeys = skillData.timeline.GetKeys();
            for (int i = 0, count = timelineKeys.Count; i < count; i++)
            {
                List<uint> mechanicses = skillData.timeline[timelineKeys[i]];
                for (int j = 0, jCount = mechanicses.Count; j < jCount; j++)
                {
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicses[j]);
                    if (mechanicsData.mechanicsType == MechanicsType.Float)
                        return MechanicsType.Float;
                    if (mechanicsData.mechanicsType == MechanicsType.Tumble)
                        return MechanicsType.Tumble;
                }
            }
            return MechanicsType.None;
        }

        [NoToLua]
        public static string GetAttackableTypeIconPath(SkillInfo skillInfo)
        {
            string path = string.Empty;
            switch (skillInfo.skillData.attackableType)
            {
                case AttackableType.Normal:
                    MechanicsType mechanicsType = GetSkillMechanicsType(skillInfo);
                    switch (mechanicsType)
                    {
                        case MechanicsType.Float:
                            path = "sprite/main_ui/icon_skill_type_1";

                            break;
                        case MechanicsType.Tumble:
                            path = "sprite/main_ui/icon_skill_type_3";

                            break;
                        case MechanicsType.None:

                            break;
                    }
                    break;
                case AttackableType.Float:
                case AttackableType.FloatAndNormal:
                    path = "sprite/main_ui/icon_skill_type_2";
                    break;
                case AttackableType.Tumble:
                case AttackableType.TumbleAndNormal:
                    path = "sprite/main_ui/icon_skill_type_4";
                    break;
            }
            return path;
        }
        [NoToLua]
        public static string GetDesTypeIcon(SkillInfo skillInfo)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(skillInfo.skillData.desTypeIconName))
                path = string.Format("sprite/main_ui/{0}", skillInfo.skillData.desTypeIconName);
            return path;
        }
        [NoToLua]
        public static string GetDesTypeIcon(SkillData skillData)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(skillData.desTypeIconName))
                path = string.Format("sprite/main_ui/{0}", skillData.desTypeIconName);
            return path;
        }
        [NoToLua]
        public static string GetDesTypeIcon2(SkillInfo skillInfo)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(skillInfo.skillData.desTypeIconName2))
                path = string.Format("sprite/main_ui/{0}", skillInfo.skillData.desTypeIconName2);
            return path;
        }
        [NoToLua]
        public static string GetDesTypeIcon2(SkillData skillData)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(skillData.desTypeIconName2))
                path = string.Format("sprite/main_ui/{0}", skillData.desTypeIconName2);
            return path;
        }

        [NoToLua]
        public static float GetMechanicsValueByAdvanceLevel(float value, uint dlevel)
        {
            float result = 0f;
            result = value * (1 + dlevel * Const.Model.ConstData.GetConstData().skillHurtA);
            return result;
        }
        public static List<SkillDesInfo> GetMechanicsValueType(int skillId)
        {
            SkillData skillData = SkillData.GetSkillDataById((uint)skillId);
            List<SkillDesInfo> result = new List<SkillDesInfo>();
            Dictionary<float, List<uint>>.Enumerator e = skillData.timeline.GetEnumerator();
            int k = 0;
            while (e.MoveNext())
            {
                List<uint> ms = e.Current.Value;
                for (int i = 0, count = ms.Count; i < count; i++)
                {
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(ms[i]);
                    if (mechanicsData != null)
                    {
                        List<SkillDesInfo> skillDesInfos = GetMechanicsDesInfo(mechanicsData, skillData.mechanicsValues[k][i]);
                        CombineSameSkillDesInfos(result, skillDesInfos);
                    }
                }
                k++;
            }
            e.Dispose();
            return result;
        }

        private static void CombineSameSkillDesInfos(List<SkillDesInfo> parent, List<SkillDesInfo> sons)
        {
            for (int i = 0, count = sons.Count; i < count; i++)
            {
                SkillDesInfo skillDesInfo = sons[i];
                bool flag = false;
                for (int j = 0, jCount = parent.Count; j < jCount; j++)
                {
                    SkillDesInfo sdi = parent[j];
                    if (sdi.mechanicsType == skillDesInfo.mechanicsType && sdi.target == skillDesInfo.target && sdi.mechanicsValueType == skillDesInfo.mechanicsValueType)
                    {
						if(skillDesInfo.mechanicsValueType != MechanicsValueType.Extra)
						{
							sdi.mechanicsValue1 += skillDesInfo.mechanicsValue1;
						}
                        flag = true;
                    }
                }
                if (!flag)
                    parent.Add(skillDesInfo);
            }
        }

        [NoToLua]
        public static List<SkillDesInfo> GetMechanicsDesInfo(MechanicsData mechanicsData, Triple<float, float, float> mechancisValue)
        {
            List<SkillDesInfo> result = new List<SkillDesInfo>();
            #region 效果数值
            switch (mechanicsData.mechanicsType)
            {
                case MechanicsType.Damage:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.DrainDamage:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        skillDesInfo.mechanicsValue2 = mechancisValue.b;
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.Treat:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.Poisoning:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
						SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
						SkillDesInfo skillDesInfo3 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.c, false);
                        result.Add(skillDesInfo1);
						result.Add(skillDesInfo2);
						result.Add(skillDesInfo3);
                    }
                    break;
                case MechanicsType.Ignite:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
						SkillDesInfo skillDesInfo3 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.c, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
						result.Add(skillDesInfo3);
                    }
                    break;
                case MechanicsType.Bleed:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.LastTreat:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.TreatPercent:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.IgnoreDefenseDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Swimmy:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Frozen:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Sleep:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Landification:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Tieup:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Reborn:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Float:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.Tumble:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.Invincible:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Silence:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Blind:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Disperse:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.Immune:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ReboundTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ReboundValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.RandomMechanics:

                    break;
                case MechanicsType.ImmunePhysicsAttack:
                    break;
                case MechanicsType.ImmuneMagicAttack:
                    break;
                case MechanicsType.Tag:
                    break;
                case MechanicsType.GeneralSkillHit:
					{
						SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.a, false);
						SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
						result.Add(skillDesInfo1);
						result.Add(skillDesInfo2);
					}
                    break;
                case MechanicsType.GeneralSkillCrit:
					{
						SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.a, false);
						SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
						result.Add(skillDesInfo1);
						result.Add(skillDesInfo2);
					}
                    break;
                case MechanicsType.AccumulatorTag:
                    break;
                case MechanicsType.ImmediatePercentDamage:
                    {
                        SkillDesInfo skillDesInfo = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        result.Add(skillDesInfo);
                    }
                    break;
                case MechanicsType.SwimmyExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.LandificationExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.BleedExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.FrozenExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PoisoningExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.TagExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.IgniteExtraDamage:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Extra, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ShieldTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ShieldValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DrainTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DrainValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ForceKill:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsDefensePercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsDefensePercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
						SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
						SkillDesInfo skillDesInfo3 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Probabiblity, mechancisValue.c, false);
						result.Add(skillDesInfo1);
						result.Add(skillDesInfo2);
						result.Add(skillDesInfo3);
                    }
                    break;
                case MechanicsType.MagicDefensePercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicDefensePercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsAttackPercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsAttackPercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicAttackPercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicAttackPercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HPLimitPercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HPLimitPercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.SpeedPercentTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.SpeedPercentValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.PercentValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsDefenseFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsDefenseFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicDefenseFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicDefenseFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsAttackFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.PhysicsAttackFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicAttackFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.MagicAttackFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HPLimitFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HPLimitFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.SpeedFixedTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.SpeedFixedValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HitTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.HitValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DodgeTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DodgeValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.AntiCritTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.AntiCritValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.BlockTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.BlockValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.AntiBlockTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.AntiBlockValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CounterAtkTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CounterAtkValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritHurtAddTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritHurtAddValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritHurtDecTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.CritHurtDecValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ArmorTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.ArmorValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DamageDecTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DamageDecValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DamageAddTime:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, true);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, false);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.DamageAddValue:
                    {
                        SkillDesInfo skillDesInfo1 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.Time, mechancisValue.a, false);
                        SkillDesInfo skillDesInfo2 = new SkillDesInfo(mechanicsData.mechanicsType, mechanicsData.targetType, MechanicsValueType.FixedValue, mechancisValue.b, true);
                        result.Add(skillDesInfo1);
                        result.Add(skillDesInfo2);
                    }
                    break;
                case MechanicsType.Transform:

                    break;
            }
            #endregion

            return result;
        }

        public static bool AttackableNormal(SkillInfo skillInfo)
        {
            if (skillInfo == null) return false;
            return AttackableNormal(skillInfo.skillData);
        }

        public static bool AttackableNormal(SkillData skillData)
        {
            if (skillData == null) return false;
            bool result = skillData.attackableType == AttackableType.Normal || skillData.attackableType == AttackableType.FloatAndNormal || skillData.attackableType == AttackableType.TumbleAndNormal || skillData.attackableType == AttackableType.All;
            return result;
        }

        public static bool AttackableFloat(SkillInfo skillInfo)
        {
            if (skillInfo == null) return false;
            return AttackableFloat(skillInfo.skillData);
        }

        public static bool AttackableFloat(SkillData skillData)
        {
            if (skillData == null) return false;
            bool result = skillData.attackableType == AttackableType.Float || skillData.attackableType == AttackableType.FloatAndNormal || skillData.attackableType == AttackableType.All;
            return result;
        }

        public static bool AttackableTumble(SkillInfo skillInfo)
        {
            if (skillInfo == null) return false;
            return AttackableTumble(skillInfo.skillData);
        }

        public static bool AttackableTumble(SkillData skillData)
        {
            if (skillData == null) return false;
            bool result = skillData.attackableType == AttackableType.Tumble || skillData.attackableType == AttackableType.TumbleAndNormal || skillData.attackableType == AttackableType.All;
            return result;
        }
    }
}