using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Character;
using Logic.Equipment;
using Logic.Equipment.Model;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero;
using Logic.Hero.Model;
using Logic.UI.ManageHeroes.Model;
using Logic.Role;
using Common.Util;
using Logic.UI.CommonEquipment.View;
using LuaInterface;

namespace Logic.UI.Equipments.View
{
	public class HeroEquipmentsItem : MonoBehaviour
	{
		private uint _playerInstanceID = 0;
		public PlayerInfo PlayerInfo
		{
			set
			{
				if (value == null)
					_playerInstanceID = 0;
				else
					_playerInstanceID = value.instanceID;
			}
			get
			{
				PlayerInfo playerInfo = null;
				if (_playerInstanceID > 0)
					playerInfo = GameProxy.instance.PlayerInfo;
				return playerInfo;
			}
		}
		
		private uint _heroInstanceID;
		public HeroInfo HeroInfo
		{
			set
			{
				if (value == null)
					_heroInstanceID = 0;
				else
					_heroInstanceID = value.instanceID;
			}
			get
			{
				HeroInfo heroInfo = null;
				if (_heroInstanceID > 0)
					heroInfo = HeroProxy.instance.GetHeroInfo(_heroInstanceID);
				return heroInfo;
			}
		}

		#region UI components
		public Transform roleIconRoot;
		public Text heroNameText;

		public RectTransform weaponTran;
		public RectTransform armorTran;
		public RectTransform accessoryTran;
		#endregion

		public void SetPlayerInfo (PlayerInfo playerInfo)
		{
			PlayerInfo = playerInfo;
			HeroInfo = null;
			heroNameText.text = PlayerInfo.playerData.heroData.NameWithQualityColor;
			RefreshRoleInfos();
			RefreshEquipments();
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
			PlayerInfo = null;
			HeroInfo = heroInfo;
			heroNameText.text = HeroInfo.heroData.NameWithQualityColor;
			RefreshRoleInfos();
			RefreshEquipments();
		}

		private void RefreshRoleInfos ()
		{
			TransformUtil.ClearChildren(roleIconRoot, true);
			Logic.UI.CommonHeroIcon.View.CommonHeroIcon commonHeroIcon = Logic.UI.CommonHeroIcon.View.CommonHeroIcon.CreateSmallIcon(roleIconRoot);
			if (PlayerInfo != null)
			{
				commonHeroIcon.SetPlayerInfo(PlayerInfo);
				commonHeroIcon.UsePetIcon();
				commonHeroIcon.SetInFormation(true);
			}
			else if (HeroInfo != null)
			{
				commonHeroIcon.SetHeroInfo(HeroInfo);
				commonHeroIcon.SetInFormation(ManageHeroesProxy.instance.IsHeroInAnyFormation(HeroInfo.instanceID));
			}
		}

		private void RefreshEquipments ()
		{
			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;

			if (PlayerInfo != null)
			{
				characterWeaponID = PlayerInfo.weaponID;
				characterArmorID = PlayerInfo.armorID;
				characterAccessoryID = PlayerInfo.accessoryID;
			}
			else
			{
				characterWeaponID = HeroInfo.weaponID;
				characterArmorID = HeroInfo.armorID;
				characterAccessoryID = HeroInfo.accessoryID;
			}
			TransformUtil.ClearChildren(weaponTran,true);
			TransformUtil.ClearChildren(armorTran,true);
			TransformUtil.ClearChildren(accessoryTran,true);
			if (characterWeaponID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterWeaponID);
				CommonEquipmentIcon icon = CommonEquipmentIcon.Create(weaponTran);
				icon.SetEquipmentInfo(equipmentInfo);
				icon.GetEquipmentDesButton().enabled = false;
				icon.ButtonEnable(false);
				float s = weaponTran.sizeDelta.x/icon.rectTransform.sizeDelta.x;
				icon.transform.localScale = new Vector3(s,s,s);
			}
			
			if (characterArmorID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterArmorID);
				CommonEquipmentIcon icon = CommonEquipmentIcon.Create(armorTran);
				icon.SetEquipmentInfo(equipmentInfo);
				icon.GetEquipmentDesButton().enabled = false;
				icon.ButtonEnable(false);
				float s = armorTran.sizeDelta.x/icon.rectTransform.sizeDelta.x;
				icon.transform.localScale = new Vector3(s,s,s);
			}
			if (characterAccessoryID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterAccessoryID);
				CommonEquipmentIcon icon = CommonEquipmentIcon.Create(accessoryTran);
				icon.SetEquipmentInfo(equipmentInfo);
				icon.GetEquipmentDesButton().enabled = false;
				icon.ButtonEnable(false);
				float s = accessoryTran.sizeDelta.x/icon.rectTransform.sizeDelta.x;
				icon.transform.localScale = new Vector3(s,s,s);
			}
		}

		public void Refresh ()
		{
			RefreshRoleInfos();
			RefreshEquipments();
		}

		#region UI event handlers
		public void ClickHandler ()
		{
//			if (PlayerInfo != null)
//			{
//				RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
//				roleEquipmentsView.SetPlayerInfo(GameProxy.instance.PlayerInfo);
//			}
//			else if (HeroInfo != null)
//			{
//				RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
//				roleEquipmentsView.SetHeroInfo(HeroInfo);
//			}

			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			if (PlayerInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(PlayerInfo.instanceID, true, (int)RoleEquipPos.Weapon);
			else if(HeroInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(HeroInfo.instanceID, false, (int)RoleEquipPos.Weapon);
		}

		public void ClickWeaponHandler ()
		{
//			RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
//			RoleAttackAttributeType roleAttackAttributeType = RoleAttackAttributeType.Invalid;
//			EquipmentType equipmentType = EquipmentType.None;
//			if (PlayerInfo != null)
//			{
//				roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(PlayerInfo.heroData.roleType);
//				roleEquipmentsView.SetPlayerInfo(PlayerInfo);
//			}
//			else if (HeroInfo != null)
//			{
//				roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(HeroInfo.heroData.roleType);
//				roleEquipmentsView.SetHeroInfo(HeroInfo);
//			}
//
//			if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
//			{
//				equipmentType = EquipmentType.PhysicalWeapon;
//			}
//			else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
//			{
//				equipmentType = EquipmentType.MagicalWeapon;
//			}
//			roleEquipmentsView.SetCurrentSelectEquipmentType(equipmentType);

			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			if (PlayerInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(PlayerInfo.instanceID, true, (int)RoleEquipPos.Weapon);
			else if(HeroInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(HeroInfo.instanceID, false, (int)RoleEquipPos.Weapon);
		}

		public void ClickArmorHandler ()
		{
//			RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
//			if (PlayerInfo != null)
//			{
//				roleEquipmentsView.SetPlayerInfo(PlayerInfo);
//			}
//			else if (HeroInfo != null)
//			{
//				roleEquipmentsView.SetHeroInfo(HeroInfo);
//			}
//			roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.Armor);

			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			if (PlayerInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(PlayerInfo.instanceID, true, (int)RoleEquipPos.Armor);
			else if(HeroInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(HeroInfo.instanceID, false, (int)RoleEquipPos.Armor);
		}

		public void ClickAccessoryHandler ()
		{
//			RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
//			if (PlayerInfo != null)
//			{
//				roleEquipmentsView.SetPlayerInfo(PlayerInfo);
//			}
//			else if (HeroInfo != null)
//			{
//				roleEquipmentsView.SetHeroInfo(HeroInfo);
//			}
//			roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.Accessory);

			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			if (PlayerInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(PlayerInfo.instanceID, true, (int)RoleEquipPos.Accessory);
			else if(HeroInfo != null)
				equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(HeroInfo.instanceID, false, (int)RoleEquipPos.Accessory);
		}
		#endregion
	}
}
