using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.Const.Model;

namespace Logic.Character
{
    public class BuffUtil
    {
        public static bool Judge(BuffType buffType, float value)
        {
            bool result = false;
            switch (buffType)
            {
                case BuffType.Swimmy:
                    result = false;
                    break;
                case BuffType.Invincible:
                    result = true;
                    break;
                case BuffType.Silence:
                    result = false;
                    break;
                case BuffType.Blind:
                    result = false;
                    break;
                case BuffType.Poisoning:
                    result = false;
                    break;
                case BuffType.Treat:
                    result = true;
                    break;
                case BuffType.Speed:
                    result = value > 0;
                    break;
                case BuffType.Shield:
                    result = true;
                    break;
                case BuffType.Drain:
                    result = true;
                    break;
                case BuffType.PhysicsDefense:
                    result = value > 0;
                    break;
                case BuffType.MagicDefense:
                    result = value > 0;
                    break;
                case BuffType.PhysicsAttack:
                    result = value > 0;
                    break;
                case BuffType.MagicAttack:
                    result = value > 0;
                    break;
                case BuffType.HPLimit:
                    result = value > 0;
                    break;
                case BuffType.Hit:
                    result = value > 0;
                    break;
                case BuffType.Dodge:
                    result = value > 0;
                    break;
                case BuffType.Crit:
                    result = value > 0;
                    break;
                case BuffType.AntiCrit:
                    result = value > 0;
                    break;
                case BuffType.Block:
                    result = value > 0;
                    break;
                case BuffType.AntiBlock:
                    result = value > 0;
                    break;
                case BuffType.CounterAtk:
                    result = value > 0;
                    break;
                case BuffType.CritHurtAdd:
                    result = value > 0;
                    break;
                case BuffType.CritHurtDec:
                    result = value > 0;
                    break;
                case BuffType.Armor:
                    result = value > 0;
                    break;
                case BuffType.DamageDec:
                    result = value > 0;
                    break;
                case BuffType.DamageAdd:
                    result = value > 0;
                    break;
                case BuffType.Frozen:
                    result = false;
                    break;
                case BuffType.TreatPercent:
                    result = value > 0;
                    break;
                case BuffType.Ignite:
                    result = false;
                    break;
                case BuffType.Bleed:
                    result = false;
                    break;
                case BuffType.Sleep:
                    result = false;
                    break;
                case BuffType.Landification:
                    result = false;
                    break;
                case BuffType.Tieup:
                    result = false;
                    break;
                case BuffType.GeneralSkillPhysicsAttack:
                    result = value > 0;
                    break;
                case BuffType.GeneralSkillMagicAttack:
                    result = value > 0;
                    break;
                case BuffType.TargetSkillPhysicsAttack:
                    result = value > 0;
                    break;
                case BuffType.TargetSkillMagicAttack:
                    result = value > 0;
                    break;
                case BuffType.Immune:
                    result = true;
                    break;
                case BuffType.Rebound:
                    result = true;
                    break;
                case BuffType.DamageImmuneCount:
                    result = true;
                    break;
                case BuffType.DamageImmuneTime:
                    result = true;
                    break;
                case BuffType.Weakness:
                    result = true;
                    break;
                case BuffType.TreatAdd:
                    result = value > 0;
                    break;
                case BuffType.ImmunePhysicsAttack:
                    result = true;
                    break;
                case BuffType.ImmuneMagicAttack:
                    result = true;
                    break;
                case BuffType.Tag:
                    result = false;
                    break;
                case BuffType.GeneralSkillHit:
                    result = true;
                    break;
                case BuffType.GeneralSkillCrit:
                    result = true;
                    break;
                case BuffType.AccumulatorTag:
                    result = true;
                    break;
            }
            return result;
        }

        public static float GetBuffValue(float value, uint level)
        {
            float result = 0;
            ConstData constData = ConstData.GetConstData();
            switch (level)
            {
                case 1:
                    result = value;
                    break;
                //case 2:
                //    result = value * constData.skillHurtB1;
                //    break;
                //case 3:
                //    result = value * constData.skillHurtB2;
                //    break;
                default:
                    result = value;
                    break;
            }
            return result;
        }
    }
}
