using UnityEngine;
using Logic.Hero.Model;
using System.Collections.Generic;
using Logic.Enums;
using Common.Localization;
using Logic.Character;
using Logic.Equipment.Model;
using Logic.Game.Model;

namespace Logic.Hero
{
    public class HeroUtil : MonoBehaviour
    {
        private static int CompareHeroAdvanceLevel(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            return (int)aHeroInfo.advanceLevel - (int)bHeroInfo.advanceLevel;
        }

        private static int CompareHeroStrengthenLevel(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            return (int)aHeroInfo.strengthenLevel - (int)bHeroInfo.strengthenLevel;
        }

        private static int CompareHeroLevel(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            return aHeroInfo.level - bHeroInfo.level;
        }

        private static int CompareHeroDataID(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            return (int)aHeroInfo.heroData.id - (int)bHeroInfo.heroData.id;
        }

		public static int CompareHeroByLevelDesc (HeroInfo aHeroInfo, HeroInfo bHeroInfo)
		{
			int result = -CompareHeroLevel(aHeroInfo, bHeroInfo);
			result = result == 0 ? -CompareHeroAdvanceLevel(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroStrengthenLevel(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroDataID(aHeroInfo, bHeroInfo) : result;
			return result;
		}

		public static int CompareHeroByStrengthenLevelDesc (HeroInfo aHeroInfo, HeroInfo bHeroInfo)
		{
			int result = -CompareHeroStrengthenLevel(aHeroInfo, bHeroInfo);
			result = result == 0 ? -CompareHeroAdvanceLevel(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroLevel(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroDataID(aHeroInfo, bHeroInfo) : result;
			return result;
		}

		public static int CompareHeroByAdvanceLevelDesc (HeroInfo aHeroInfo, HeroInfo bHeroInfo)
		{
			int result = -CompareHeroAdvanceLevel(aHeroInfo, bHeroInfo);
			result = result == 0 ? -CompareHeroLevel(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroByStrengthenLevelDesc(aHeroInfo, bHeroInfo) : result;
			result = result == 0 ? -CompareHeroDataID(aHeroInfo, bHeroInfo) : result;
			return result;
		}

        public static int CompareHeroByQualityAsc(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            int result = CompareHeroAdvanceLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = CompareHeroStrengthenLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = CompareHeroLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = CompareHeroDataID(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }
            return 0;
        }

        public static int CompareHeroByQualityDesc(HeroInfo aHeroInfo, HeroInfo bHeroInfo)
        {
            int result = -CompareHeroAdvanceLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = -CompareHeroStrengthenLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = -CompareHeroLevel(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }

            result = -CompareHeroDataID(aHeroInfo, bHeroInfo);
            if (result != 0)
            {
                return result;
            }
            return 0;
        }

        public static float GetHeroExpPercentToNextLevel(HeroInfo heroInfo)
        {
            float heroExpPercentToNextLevel = 0;
            if (HeroExpData.GetHeroExpDataByLv(heroInfo.level).exp > 0)
            {
                heroExpPercentToNextLevel = (float)heroInfo.exp / HeroExpData.GetHeroExpDataByLv(heroInfo.level).exp;
            }
            return heroExpPercentToNextLevel;
        }
		/// <summary>
		/// 计算英雄属性（不包含装备）
		/// </summary>
        public static Dictionary<RoleAttributeType, RoleAttribute> CalcHeroAttributesDic(HeroInfo hero)
        {

            if (hero == null)
                return null;
			return CalcHeroAttributesDic(hero.heroData ,hero.level,hero.advanceLevel,hero.strengthenLevel);

        }
		/// <summary>
		/// 计算英雄属性（不包含装备）
		/// </summary>
		public static Dictionary<RoleAttributeType, RoleAttribute> CalcHeroAttributesDic(HeroData heroData ,int level,int advanceLevel,int strengthenLevel)
		{
			
			if (heroData == null)
				return null;
			Dictionary<RoleAttributeType, RoleAttribute> attributeDic = new Dictionary<RoleAttributeType, RoleAttribute>();
			
			int max = (int)RoleAttributeType.MAX;
			RoleAttributeType type;
			float factor = 0;//成长系数
			bool canGrowUp = false;
			float basic = 0;
			for (int i = 0; i < max; i++)
			{
				type = (RoleAttributeType)i;
				factor = 0;
				canGrowUp = false;
				switch (type)
				{
				case RoleAttributeType.HP:
					basic = heroData.HP;
					factor = heroData.hpAdd / 1000.0f;
					canGrowUp = true;
					break;
				case RoleAttributeType.NormalAtk:
					basic = heroData.normalAtk;
					factor = heroData.normalAtkAdd / 1000.0f;
					canGrowUp = true;
					break;
				case RoleAttributeType.MagicAtk:
					basic = heroData.magicAtk;
					factor = heroData.magicAtkAdd / 1000.0f;
					canGrowUp = true;
					break;
				case RoleAttributeType.Normal_Def:
					basic = heroData.normalDef;
					factor = heroData.normalDefAdd / 1000.0f;
					canGrowUp = true;
					break;
				case RoleAttributeType.Speed:
					basic = heroData.speed;
					break;
				case RoleAttributeType.Hit:
					basic = heroData.hit;
					break;
				case RoleAttributeType.Dodge:
					basic = heroData.dodge;
					break;
				case RoleAttributeType.Crit:
					basic = heroData.crit;
					break;
				case RoleAttributeType.AntiCrit:
					basic = heroData.antiCrit;
					break;
				case RoleAttributeType.Block:
					basic = heroData.block;
					break;
				case RoleAttributeType.AntiBlock:
					basic = heroData.antiBlock;
					break;
				case RoleAttributeType.CounterAtk:
					basic = heroData.counterAtk;
					break;
				case RoleAttributeType.CritHurtAdd:
					basic = heroData.critHurtAdd;
					break;
				case RoleAttributeType.CritHurtDec:
					basic = heroData.critHurtDec;
					break;
				case RoleAttributeType.Armor:
					basic = heroData.armor;
					break;
				case RoleAttributeType.DamageAdd:
					basic = heroData.damageAdd;
					break;
				case RoleAttributeType.DamageDec:
					basic = heroData.damageDec;
					break;
				default:
					basic = 0;
					break;
				}
				int multiple = 2;
				if (level <= 50)
				{
					multiple = 1;
				}
				float total = basic;
				if (canGrowUp)
				{
					HeroStrengthenNeedData strengthenData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByLevel(strengthenLevel-1);
					float aggFactor = 0;
					if(strengthenData != null)
						aggFactor = strengthenData.aggr_value;
					//  total = (basic + (hero.level * multiple - 1) * factor + hero.strengthenLevel * factor * 6) * Mathf.Pow(1.28f, hero.advanceLevel - 1);
					total = (basic + (level * multiple-1-50*(multiple-1))*factor +  aggFactor*factor) * GlobalData.GetGlobalData().starAttr[advanceLevel] * heroData.quality_attr;
					//total = (basic + (level * multiple - 1) * factor + factor*aggFactor) * Mathf.Pow(1.28f,advanceLevel - 1);
				}
				RoleAttribute attri = new RoleAttribute(type, total);
				attributeDic.Add(type, attri);
			}
			return attributeDic;
			
		}
        public static List<RoleAttribute> CalcHeroAttributes(HeroInfo hero)
        {

            Dictionary<RoleAttributeType, RoleAttribute> dic = CalcHeroAttributesDic(hero);
            if (dic == null)
                return null;
            List<RoleAttribute> attributeList = new List<RoleAttribute>(dic.Values);

            return attributeList;
        }
		/// <summary>
		///获取主属性
		/// </summary>

        public static List<RoleAttribute> CalcHeroMainAttributesList(HeroInfo hero)
        {
            List<RoleAttribute> mainAttriList = new List<RoleAttribute>();
            List<RoleAttribute> attriList = CalcHeroAttributes(hero);
            RoleAttribute attribute;
            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(hero.heroData.roleType);


            for (int i = 0, count = attriList.Count; i < count; i++)
            {
                attribute = attriList[i];
                if (attribute.type == RoleAttributeType.HP)
                {
                    mainAttriList.Add(attribute);
                }
                else if (attribute.type == RoleAttributeType.MagicAtk && roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
                {
                    mainAttriList.Add(attribute);
                }
                else if (attribute.type == RoleAttributeType.NormalAtk && roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
                {
                    mainAttriList.Add(attribute);
                }
                else if (attribute.type == RoleAttributeType.Normal_Def)
                {
                    mainAttriList.Add(attribute);
                }
            }
            return mainAttriList;
        }

		/// <summary>
		/// 计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
		/// </summary>
        public static Dictionary<RoleAttributeType, RoleAttribute> CalcHeroAttributesDicByEquip(HeroInfo info)
        {
            List<int> heroEquipIdList = new List<int>();
            heroEquipIdList.Add(info.armorID);
            heroEquipIdList.Add(info.accessoryID);
            heroEquipIdList.Add(info.weaponID);
            int id = 0;
            Equipment.Model.EquipmentInfo equipInfo;
            EquipmentAttribute equipAttr;
            //存放所有的装备总属性
            Dictionary<EquipmentAttributeType, EquipmentAttribute> equipAllAttrDic = new Dictionary<EquipmentAttributeType, EquipmentAttribute>();
            for (int i = 0, count = heroEquipIdList.Count; i < count; i++)
            {
                id = heroEquipIdList[i];
                equipInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(id);
                if (equipInfo != null)
                {
					List<EquipmentAttribute> equipAttrList = equipInfo.EquipAttribute;
                    for (int j = 0, count2 = equipAttrList.Count; j < count2; j++)
                    {
                        equipAttr = equipAttrList[j];
                        EquipmentAttributeType type = equipAttr.type;
                        if (equipAllAttrDic.ContainsKey(type))
                        {
                            equipAllAttrDic[type].value = equipAllAttrDic[type].value + equipAttr.value;
                        }
                        else
                        {
                            equipAllAttrDic.Add(type, equipAttr);
                        }
                    }

                }
            }

            return Equipment.EquipmentUtil.CalcRoleAttrByEquipAttr(CalcHeroAttributesDic(info), equipAllAttrDic);
        }

    }
}
