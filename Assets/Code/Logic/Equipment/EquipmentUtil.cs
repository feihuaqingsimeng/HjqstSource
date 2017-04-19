using UnityEngine;
using System.Collections.Generic;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Equipment.Model;
using Logic.Hero.Model;
using LuaInterface;

namespace Logic.Equipment
{
	public class EquipmentUtil  
	{

		public static GameResData CalculatePutOffCost (EquipmentInfo equipmentInfo)
		{

			GameResData res = equipmentInfo.equipmentData.unloadCost;
			if(res == null)
				res = new GameResData(BaseResType.Diamond,0,0,0);
			return res;
		}

		public static string GetEquipmentStrengthenLevelString (EquipmentInfo equipmentInfo)
		{
			string strengthenLevelString = string.Empty;
			if (equipmentInfo.strengthenLevel > 0)
			{
				strengthenLevelString = string.Format(Localization.Get("common.strengthen_level"), equipmentInfo.strengthenLevel);
			}
			return strengthenLevelString;
		}

		public static void ResetStars (EquipmentInfo equipmentInfo, List<GameObject> starGameObjects)
		{
			int starGameObjectsCount = starGameObjects.Count;
			for (int i = 0; i < starGameObjectsCount; i++)
			{
				if (i < equipmentInfo.equipmentData.star)
				{
					starGameObjects[i].SetActive(true);
				}
				else
				{
					starGameObjects[i].SetActive(false);
				}
			}
		}
		public static int CompareEquipmentQuality(EquipmentInfo aEquipmentInfo, EquipmentInfo bEquipmentInfo)
		{
			return bEquipmentInfo.equipmentData.quality - aEquipmentInfo.equipmentData.quality;
		}
	
		public static int CompareEquipmentWeight(EquipmentInfo aEquipmentInfo, EquipmentInfo bEquipmentInfo,EquipmentType curType,RoleType roleType,bool isAsc)
		{
			EquipmentType aType = aEquipmentInfo.equipmentData.equipmentType;
			EquipmentType bType = bEquipmentInfo.equipmentData.equipmentType;
			
			int aTypeW = curType == aType ? 5 :aType == EquipmentType.PhysicalWeapon ? 4 : aType == EquipmentType.Armor ? 3 : aType == EquipmentType.Accessory ? 2 :1;
			int bTypeW = curType == bType ? 5 :aType == EquipmentType.PhysicalWeapon ? 4 : bType == EquipmentType.Armor ? 3 : bType == EquipmentType.Accessory ? 2 :1;
			if(bTypeW-aTypeW != 0 )
				return bTypeW-aTypeW;
			aTypeW = aEquipmentInfo.equipmentData.equipmentRoleType == roleType ? 2 : 1;
			bTypeW = bEquipmentInfo.equipmentData.equipmentRoleType == roleType ? 2 : 1;
			if(bTypeW-aTypeW != 0 )
				return bTypeW-aTypeW;
			if(isAsc)
				return -CompareEquipmentQuality(aEquipmentInfo,bEquipmentInfo);
			return CompareEquipmentQuality(aEquipmentInfo,bEquipmentInfo);
		}
		public static RoleAttributeType ConvertEquipAttTypeToRoleAttrType(EquipmentAttributeType type)
		{
			return RoleAttributeType.Speed;
		}

		public static Dictionary<RoleAttributeType,RoleAttribute> CalcRoleAttrByEquipAttr(Dictionary<RoleAttributeType,RoleAttribute> roleAttrDic,Dictionary<EquipmentAttributeType,EquipmentAttribute> equipAttrDic)
		{


			Dictionary<RoleAttributeType,RoleAttribute> addAttrDic = new Dictionary<RoleAttributeType, RoleAttribute>();

			if(roleAttrDic == null || equipAttrDic == null)
				return addAttrDic;

			List<EquipmentAttribute> equipAttrList = equipAttrDic.GetValues();
			EquipmentAttribute equipAttr;
			for (int j = 0, count2 = equipAttrList.Count; j < count2; j++) 
			{
				equipAttr =  equipAttrList[j];
				EquipmentAttributeType type = equipAttr.type;
				switch(type)
				{
				case EquipmentAttributeType.HP:                 //生命力
					addAttrDic.Add(RoleAttributeType.HP,new RoleAttribute(RoleAttributeType.HP, equipAttr.value));
					break;
				case EquipmentAttributeType.NormalAtk:          //物理攻击
					addAttrDic.Add(RoleAttributeType.NormalAtk,new RoleAttribute(RoleAttributeType.NormalAtk, equipAttr.value));
					break;
				case EquipmentAttributeType.MagicAtk:        //魔法攻击
					addAttrDic.Add(RoleAttributeType.MagicAtk,new RoleAttribute(RoleAttributeType.MagicAtk, equipAttr.value));
					break;
				case EquipmentAttributeType.Def:               //防御
					addAttrDic.Add(RoleAttributeType.Normal_Def,new RoleAttribute(RoleAttributeType.Normal_Def, equipAttr.value));
					break;
				case EquipmentAttributeType.Speed:              //行动力
					addAttrDic.Add(RoleAttributeType.Speed,new RoleAttribute(RoleAttributeType.Speed, equipAttr.value));
					break;
				case EquipmentAttributeType.Hit :                //命中
					addAttrDic.Add(RoleAttributeType.Hit,new RoleAttribute(RoleAttributeType.Hit, equipAttr.value));
					break;
				case EquipmentAttributeType.Dodge :             //闪避
					addAttrDic.Add(RoleAttributeType.Dodge,new RoleAttribute(RoleAttributeType.Dodge, equipAttr.value));
					break;
				case EquipmentAttributeType.Crit :               //暴击
					addAttrDic.Add(RoleAttributeType.Crit,new RoleAttribute(RoleAttributeType.Crit, equipAttr.value));
					break;
				case EquipmentAttributeType.AntiCrit :          //抗暴击
					addAttrDic.Add(RoleAttributeType.AntiCrit,new RoleAttribute(RoleAttributeType.AntiCrit, equipAttr.value));
					break;
				case EquipmentAttributeType.Block :              //格挡
					addAttrDic.Add(RoleAttributeType.Block,new RoleAttribute(RoleAttributeType.Block, equipAttr.value));
					break;
				case EquipmentAttributeType.AntiBlock :         //破击(破格挡)
					addAttrDic.Add(RoleAttributeType.AntiBlock,new RoleAttribute(RoleAttributeType.AntiBlock, equipAttr.value));
					break;
				case EquipmentAttributeType.CounterAtk :        //反击
					addAttrDic.Add(RoleAttributeType.CounterAtk,new RoleAttribute(RoleAttributeType.CounterAtk, equipAttr.value));
					break;
				case EquipmentAttributeType.CritHurtAdd :       //暴击伤害
					addAttrDic.Add(RoleAttributeType.CritHurtAdd,new RoleAttribute(RoleAttributeType.CritHurtAdd, equipAttr.value));
					break;
				case EquipmentAttributeType.CritHurtDec :       //暴击伤害减免
					addAttrDic.Add(RoleAttributeType.CritHurtDec,new RoleAttribute(RoleAttributeType.CritHurtDec, equipAttr.value));
					break;
				case EquipmentAttributeType.Armor :             //破甲
					addAttrDic.Add(RoleAttributeType.Armor,new RoleAttribute(RoleAttributeType.Armor, equipAttr.value));
					break;
				case EquipmentAttributeType.DamageAdd :        //伤害减免
					addAttrDic.Add(RoleAttributeType.DamageAdd,new RoleAttribute(RoleAttributeType.DamageAdd, equipAttr.value));
					break;
				case EquipmentAttributeType.DamageDec :         //伤害加成
					addAttrDic.Add(RoleAttributeType.DamageDec,new RoleAttribute(RoleAttributeType.DamageDec, equipAttr.value));
					break;
					
				}
				
			}
			float value;
			for(int i = 0,count = equipAttrList.Count;i<count;i++)
			{
				equipAttr =  equipAttrList[i];
				EquipmentAttributeType type = equipAttr.type;
				switch(type)
				{
				case EquipmentAttributeType.HPPercent :         //生命百分比
					
					value = (GetRoleAttrValue(roleAttrDic.GetValue(RoleAttributeType.HP))+GetRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.HP)) )* equipAttr.value/100f;
					addAttrDic[RoleAttributeType.HP] = addRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.HP),RoleAttributeType.HP, value);
					break;
				case EquipmentAttributeType.NormalAtkPercent :  //物理攻击百分比
					value = (GetRoleAttrValue(roleAttrDic.GetValue(RoleAttributeType.NormalAtk))+GetRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.NormalAtk)) )* equipAttr.value/100f;
					addAttrDic[RoleAttributeType.NormalAtk] = addRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.NormalAtk),RoleAttributeType.NormalAtk,value);
					break;
				case EquipmentAttributeType.MagicAtkPercent:   //魔法攻击百分比
					value = (GetRoleAttrValue(roleAttrDic.GetValue(RoleAttributeType.MagicAtk))+GetRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.MagicAtk)) )* equipAttr.value/100f;
					addAttrDic[RoleAttributeType.MagicAtk] = addRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.MagicAtk),RoleAttributeType.MagicAtk, value);
					break;
				case EquipmentAttributeType.DefPercent:        //防御百分比(双防)
					value = (GetRoleAttrValue(roleAttrDic.GetValue(RoleAttributeType.Normal_Def))+GetRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.Normal_Def)) )* equipAttr.value/100f;
					addAttrDic[RoleAttributeType.Normal_Def] = addRoleAttrValue(addAttrDic.GetValue(RoleAttributeType.Normal_Def),RoleAttributeType.Normal_Def, value);
					break;
				}
			}
			return addAttrDic;
		}
		private static float GetRoleAttrValue(RoleAttribute attr)
		{
			if(attr == null)
				return 0f;
			return attr.value;
		}
		private static RoleAttribute addRoleAttrValue(RoleAttribute attr,RoleAttributeType type, float value)
		{
			if(attr == null)
				attr = new RoleAttribute(type,0);
			
			attr.value = attr.value+value;
			return attr;
		}
		public static float CalcEquipPower(EquipmentInfo info)
		{
			if(info == null)
				return 0;

			float power = 0;
			LuaTable equipModel = (LuaTable) LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			LuaTable luaEquipInfo =  (LuaTable)equipModel.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(info.instanceID)[0];
			if(luaEquipInfo != null)
			{
				object o = luaEquipInfo.GetLuaFunction("Power").Call(luaEquipInfo)[0];
				if(o != null)
					power = o.ToString().ToInt32();
			}
			return power;
		}
	}
}
