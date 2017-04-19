using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Localization;
using Logic.Enums;
using Logic.Role.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Player;
using Logic.Hero;
using Logic.Game.Model;
using Logic.Equipment.Model;
using Logic.Equipment;
using Logic.Character;

namespace Logic.Role
{
	public class RoleUtil
	{
		private static int CompareRoleAdvanceLevel(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			return (int)aHeroInfo.advanceLevel - (int)bHeroInfo.advanceLevel;
		}
		
		private static int CompareRoleStrengthenLevel(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			return (int)aHeroInfo.strengthenLevel - (int)bHeroInfo.strengthenLevel;
		}
		
		private static int CompareRoleLevel(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			return aHeroInfo.level - bHeroInfo.level;
		}
		
		private static int CompareRoleDataID(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			return (int)aHeroInfo.modelDataId - (int)bHeroInfo.modelDataId;
		}
		public static int CompareRoleDataPower(RoleInfo aInfo,RoleInfo bInfo)
		{
			return (int)(aInfo.Power-bInfo.Power);
		}
		public static int CompareRoleByQualityDesc(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			int result = -CompareRoleAdvanceLevel(aHeroInfo, bHeroInfo);
			if (result != 0)
			{
				return result;
			}
			result = -CompareRoleStrengthenLevel(aHeroInfo, bHeroInfo);
			if (result != 0)
			{
				return result;
			}
			
			result = -CompareRoleLevel(aHeroInfo, bHeroInfo);
			if (result != 0)
			{
				return result;
			}
			
			result = -CompareRoleDataID(aHeroInfo, bHeroInfo);
			if (result != 0)
			{
				return result;
			}
			return 0;
		}
		public static int CompareRoleByQualityAsc(RoleInfo aHeroInfo, RoleInfo bHeroInfo)
		{
			return -CompareRoleByQualityDesc(aHeroInfo,bHeroInfo);
		}
		/// <summary>
		/// 计算英雄属性（不包含装备）
		/// </summary>
		public static Dictionary<RoleAttributeType, RoleAttribute> CalcRoleAttributesDic(RoleInfo roleInfo)
		{
			PlayerInfo player = roleInfo as PlayerInfo;
			HeroInfo hero = roleInfo as HeroInfo;
			if(player!= null)
			{
				return PlayerUtil.CalcPlayerAttributesDic(player);
			}
			if(hero != null)
			{
				return HeroUtil.CalcHeroAttributesDic(hero);
			}
			return null;
		}
		/// <summary>
		/// 计算英雄总属性（包含装备）
		/// </summary>
		public static Dictionary<RoleAttributeType,RoleAttribute> CalcRoleAttributesDicContainsEquip(RoleInfo roleInfo)
		{
			Dictionary<RoleAttributeType,RoleAttribute> roleAttrDic = CalcRoleAttributesDic(roleInfo);
			Dictionary<RoleAttributeType,RoleAttribute> equipAttrDic = CalcRoleAttributesDicByEquip(roleInfo);

			List<RoleAttributeType> keyList = roleAttrDic.GetKeys();
			RoleAttributeType key;
			for(int i = 0,count = keyList.Count;i<count;i++)
			{
				key = keyList[i];
				if(equipAttrDic.ContainsKey(key))
				{
					roleAttrDic[key].value = roleAttrDic[key].value+equipAttrDic[key].value;
				}
			}
			return roleAttrDic;
		}
		/// <summary>
		/// 计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
		/// </summary>
		public static Dictionary<RoleAttributeType, RoleAttribute> CalcRoleAttributesDicByEquip(RoleInfo roleInfo)
		{
			PlayerInfo player = roleInfo as PlayerInfo;
			HeroInfo hero = roleInfo as HeroInfo;
			if(player!= null)
			{
				return PlayerUtil.CalcPlayerAttributesDicByEquip(player);
			}
			if(hero != null)
			{
				return HeroUtil.CalcHeroAttributesDicByEquip(hero);
			}
			return null;
		}
		public static List<RoleAttribute> CalcRoleMainAttributesList(RoleInfo roleInfo)
		{
			PlayerInfo player = roleInfo as PlayerInfo;
			HeroInfo hero = roleInfo as HeroInfo;
			if(player!= null)
			{
				return PlayerUtil.CalcPlayerMainAttributesList(player);
			}
			if(hero != null)
			{
				return HeroUtil.CalcHeroMainAttributesList(hero);
			}
			return null;
		}


		public static float CalcRolePower(RoleInfo roleInfo)
		{
			PlayerInfo player = roleInfo as PlayerInfo;
			HeroInfo hero = roleInfo as HeroInfo;
			int correction = 0;
			if(player != null)
				correction = player.heroData.correction;
			else if(hero!= null)
				correction = hero.heroData.correction;
			int powerBasic = GlobalData.GetGlobalData().powerBasic;
			int levelFactor = 0;
			if(roleInfo.level<=50)
				levelFactor = roleInfo.level-1;
			else 
				levelFactor = 2*roleInfo.level-51;

			HeroStrengthenNeedData strengthenData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByLevel(roleInfo.strengthenLevel-1);
			float aggFactor = 0;
			if(strengthenData != null)
				aggFactor = strengthenData.aggr_value;

			float rolePower = correction/100.0f*15*((powerBasic+levelFactor+aggFactor)*Mathf.Pow( 1.28f,roleInfo.advanceLevel-1));

			rolePower += EquipmentUtil.CalcEquipPower( EquipmentProxy.instance.GetEquipmentInfoByInstanceID(roleInfo.armorID));
			rolePower += EquipmentUtil.CalcEquipPower( EquipmentProxy.instance.GetEquipmentInfoByInstanceID(roleInfo.weaponID));
			rolePower += EquipmentUtil.CalcEquipPower( EquipmentProxy.instance.GetEquipmentInfoByInstanceID(roleInfo.accessoryID));

			return rolePower;
		}

		private static string GetColorFormatStringOfStrengthenStage (RoleStrengthenStage roleStrengthenStage)
		{
			string colorFormatString = string.Empty;
			switch (roleStrengthenStage)
			{
				case RoleStrengthenStage.White:
					colorFormatString = "<color=#D3D3D3>{0}</color>";
					break;
				case RoleStrengthenStage.Green:
					colorFormatString = "<color=#57D313>{0}</color>";
					break;
				case RoleStrengthenStage.Blue:
					colorFormatString = "<color=#208EFF>{0}</color>";
					break;
				case RoleStrengthenStage.Purple:
					colorFormatString = "<color=#DC53D5>{0}</color>";
					break;
				case RoleStrengthenStage.Orange:
					colorFormatString = "<color=#FF7B06>{0}</color>";
					break;
				default:
					break;
			}
			return colorFormatString;
		}

		public static string GetStrengthenAddShowValueString(RoleInfo roleInfo)
		{
			string colorFormatStringOfStrengthenStage = GetColorFormatStringOfStrengthenStage(roleInfo.RoleStrengthenStage);
			string strengthenAddShowValueString = roleInfo.StrengthenAddShowValue > 0 ? string.Format(Localization.Get("common.strengthen_level"), roleInfo.StrengthenAddShowValue) : string.Empty;
			return string.Format(colorFormatStringOfStrengthenStage, strengthenAddShowValueString);
		}

		public static string GetRoleNameWithColor (RoleInfo roleInfo)
		{
			string roleNameStr = string.Empty;
			if (roleInfo is PlayerInfo)
			{
//				PlayerInfo playerInfo = roleInfo as PlayerInfo;
//				roleNameStr = playerInfo.name;
				roleNameStr = GameProxy.instance.AccountName;
			}
			else
			{
				HeroInfo heroInfo = roleInfo as HeroInfo;
				roleNameStr = Localization.Get(heroInfo.heroData.name);
			}
			return string.Format(GetColorFormatStringOfStrengthenStage(roleInfo.RoleStrengthenStage), roleNameStr);
		}

		public static string GetRoleNameWithStrengthenLevel (RoleInfo roleInfo)
		{
			string roleNameStr = string.Empty;
			string strengthenLevelStr = string.Empty;
			if (roleInfo is PlayerInfo)
			{
				PlayerInfo playerInfo = roleInfo as PlayerInfo;
				roleNameStr = Localization.Get(playerInfo.heroData.name);
			}
			else
			{
				HeroInfo heroInfo = roleInfo as HeroInfo;
				roleNameStr = Localization.Get(heroInfo.heroData.name);
			}
			strengthenLevelStr = GetStrengthenAddShowValueString(roleInfo);
			return string.Format(GetColorFormatStringOfStrengthenStage(roleInfo.RoleStrengthenStage), roleNameStr + strengthenLevelStr);
		}
		public static int GetStrengthenAddShowValue(int strengthenLv)
		{
			int strengthenAddShowValue = 0;
			int correspondingHeroStrengthenNeedDataID = strengthenLv - 1;
			HeroStrengthenNeedData heroStrengthenNeedData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByID(correspondingHeroStrengthenNeedDataID);
			if (heroStrengthenNeedData != null)
			{
				strengthenAddShowValue = heroStrengthenNeedData.strengthenAddShowValue;
			}
			return strengthenAddShowValue;
		}
		public static string GetStrengthenLevelColorName(int strengthenLv )
		{
			int correspondingHeroStrengthenNeedDataID = strengthenLv - 1;
			HeroStrengthenNeedData heroStrengthenNeedData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByID(correspondingHeroStrengthenNeedDataID);
			RoleStrengthenStage color = RoleStrengthenStage.White;
			if (heroStrengthenNeedData != null)
			{
				color = heroStrengthenNeedData.roleStrengthenStage;
			}
			string roleNameColorString = string.Empty;
			switch (color)
			{
			case RoleStrengthenStage.White:
				roleNameColorString = Localization.Get("common.color_White");
				break;
			case RoleStrengthenStage.Green:
				roleNameColorString = Localization.Get("common.color_Green");
				break;
			case RoleStrengthenStage.Blue:
				roleNameColorString = Localization.Get("common.color_Blue");
				break;
			case RoleStrengthenStage.Purple:
				roleNameColorString = Localization.Get("common.color_Purple");
				break;
			case RoleStrengthenStage.Orange:
				roleNameColorString = Localization.Get("common.color_Orange");
				break;
			default:
				break;
			}
			return roleNameColorString;
		}
		public static EquipmentType GetRoleCorrespondingWeaponType (RoleInfo roleInfo)
		{
			RoleAttackAttributeType roleAttackAttributeType = RoleAttackAttributeType.Invalid;
			EquipmentType equipmentType = EquipmentType.None;
			if (roleInfo is PlayerInfo)
			{
				PlayerInfo playerInfo = roleInfo as PlayerInfo;
				roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(playerInfo.heroData.roleType);
			}
			else if (roleInfo is HeroInfo)
			{
				HeroInfo heroInfo = roleInfo as HeroInfo;
				roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(heroInfo.heroData.roleType);
			}

			if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
			{
				equipmentType = EquipmentType.PhysicalWeapon;
			}
			else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
			{
				equipmentType = EquipmentType.MagicalWeapon;
			}
			return equipmentType;
		}
	}
}

