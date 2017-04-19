using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.Character;
using Common.Localization;
using Logic.Hero;


namespace Logic.Player
{
    public class PlayerUtil : MonoBehaviour
    {
        public static float GetPlayerExpPercentToNextLevel(PlayerInfo playerInfo)
        {
            float playerExpPercentToNextLevel = 0;
            if (HeroExpData.GetPlayerExpDataByLv(playerInfo.level).exp > 0)
            {
                playerExpPercentToNextLevel = (float)playerInfo.exp / HeroExpData.GetHeroExpDataByLv(playerInfo.level).exp;
            }
            return playerExpPercentToNextLevel;
        }
		/// <summary>
		/// 计算英雄属性（不包含装备）
		/// </summary>
        public static Dictionary<RoleAttributeType, RoleAttribute> CalcPlayerAttributesDic(PlayerInfo playerInfo)
        {

            if (playerInfo == null)
            {
                return null;
            }
			return HeroUtil.CalcHeroAttributesDic(playerInfo.heroData,playerInfo.level,playerInfo.advanceLevel,playerInfo.strengthenLevel);

        }
		/// <summary>
		/// 计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
		/// </summary>
        public static Dictionary<RoleAttributeType, RoleAttribute> CalcPlayerAttributesDicByEquip(PlayerInfo info)
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

            return Equipment.EquipmentUtil.CalcRoleAttrByEquipAttr(CalcPlayerAttributesDic(info), equipAllAttrDic);
        }
		/// <summary>
		///获取主属性()
		/// </summary>
		
		public static List<RoleAttribute> CalcPlayerMainAttributesList(PlayerInfo player)
		{
			List<RoleAttributeType> typeList = new List<RoleAttributeType>();
			typeList.Add(RoleAttributeType.HP);
			typeList.Add(RoleAttributeType.MagicAtk);
			typeList.Add(RoleAttributeType.NormalAtk);
			typeList.Add(RoleAttributeType.Normal_Def);
			return CalcPlayerAttributesList(player,typeList);
//			List<RoleAttribute> mainAttriList = new List<RoleAttribute>();
//			List<RoleAttribute> attriList = CalcPlayerAttributesDic(player).GetValues();
//			RoleAttribute attribute;
//			RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(player.heroData.roleType);
//			
//			
//			for (int i = 0, count = attriList.Count; i < count; i++)
//			{
//				attribute = attriList[i];
//				if (attribute.type == RoleAttributeType.HP)
//				{
//					mainAttriList.Add(attribute);
//				}
//				else if (attribute.type == RoleAttributeType.MagicAtk && roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
//				{
//					attribute.Name = Localization.Get("attribute_des_2_1");
//					mainAttriList.Add(attribute);
//				}
//				else if (attribute.type == RoleAttributeType.NormalAtk && roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
//				{
//					attribute.Name = Localization.Get("attribute_des_2_1");
//					mainAttriList.Add(attribute);
//				}
//				else if (attribute.type == RoleAttributeType.Normal_Def)
//				{
//					attribute.Name = Localization.Get("attribute_des_3_1");
//					mainAttriList.Add(attribute);
//				}
//			}
//			return mainAttriList;
		}
		/// <summary>
		///获取属性(给定type列表)
		/// </summary>

		public static List<RoleAttribute> CalcPlayerAttributesList(PlayerInfo player,List<RoleAttributeType> needTypes)
		{
			List<RoleAttribute> mainAttriList = new List<RoleAttribute>();
			List<RoleAttribute> attriList = CalcPlayerAttributesDic(player).GetValues();
			RoleAttribute attribute;
			RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(player.heroData.roleType);
			
			for (int i = 0, count = attriList.Count; i < count; i++)
			{
				attribute = attriList[i];
				if(needTypes.Contains(attribute.type))
				{
					if (attribute.type == RoleAttributeType.MagicAtk)
					{
						if(roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
						{
							mainAttriList.Add(attribute);
						}
					}
					else if (attribute.type == RoleAttributeType.NormalAtk)
					{
						if(roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
						{
							mainAttriList.Add(attribute);
						}

					}
					else if (attribute.type == RoleAttributeType.Normal_Def)
					{
						mainAttriList.Add(attribute);
					}else
					{
						mainAttriList.Add(attribute);
					}
				}
			}
			return mainAttriList;
		}
    }
}