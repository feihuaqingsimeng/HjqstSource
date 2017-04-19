using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Equipment;
using Logic.Equipment.Model;
using Logic.Equipment.Controller;
using Logic.Role.Model;

namespace Logic.UI.Tips.View
{
	public class ConfirmSubstituteEquipmentTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/confirm_substitute_equipment_tips_view";

//		private PlayerInfo _playerInfo;
//		private HeroInfo _heroInfo;
//		private EquipmentInfo _equipmentInfo;
//		private EquipmentInfo _oldEquipmentInfo;

		private int _roleInstanceID;
		private int _equipmentInstanceID;

		public Text tipsText;
		public Image costResourceImage;
		public Text costResourceCountText;

		public Text destroyAndEquipText;
		public Text substituteText;

		void Awake ()
		{
			tipsText.text = Localization.Get("ui.confirm_substitute_equipment_tips_view.tips");
			destroyAndEquipText.text = Localization.Get("ui.confirm_substitute_equipment_tips_view.destory_and_equip");
			substituteText.text = Localization.Get("ui.confirm_substitute_equipment_tips_view.substitute");
		}
//		public static ConfirmSubstituteEquipmentTipsView Open(RoleInfo role,EquipmentInfo equipmentInfo, GameResData costGameResData)
//		{
//			ConfirmSubstituteEquipmentTipsView view = UIMgr.instance.Open<ConfirmSubstituteEquipmentTipsView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
//			if(role is PlayerInfo)
//				view.SetInfo(role as PlayerInfo ,equipmentInfo, costGameResData);
//			else 
//				view.SetInfo(role as HeroInfo ,equipmentInfo, costGameResData);
//			return view;
//		}
//		public void SetInfo (PlayerInfo playerInfo, EquipmentInfo equipmentInfo, GameResData costGameResData)
//		{
//			_playerInfo = playerInfo;
//			_equipmentInfo = equipmentInfo;
//			EquipmentType equipmentType = _equipmentInfo.equipmentData.equipmentType;
//			if (equipmentType == EquipmentType.PhysicalWeapon ||
//			    equipmentType == EquipmentType.MagicalWeapon)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.weaponID);
//			}
//			else if (equipmentType == EquipmentType.Armor)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.armorID);
//			}
//			else if (equipmentType == EquipmentType.Accessory)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.accessoryID);
//			}
//			costResourceImage.sprite = ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type));
//			costResourceCountText.text = costGameResData.count.ToString();
//		}
//
//		public void SetInfo (HeroInfo heroInfo, EquipmentInfo equipmentInfo, GameResData costGameResData)
//		{
//			_heroInfo = heroInfo;
//			_equipmentInfo = equipmentInfo;
//			EquipmentType equipmentType = _equipmentInfo.equipmentData.equipmentType;
//			if (equipmentType == EquipmentType.PhysicalWeapon ||
//			    equipmentType == EquipmentType.MagicalWeapon)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_heroInfo.weaponID);
//			}
//			else if (equipmentType == EquipmentType.Armor)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_heroInfo.armorID);
//			}
//			else if (equipmentType == EquipmentType.Accessory)
//			{
//				_oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_heroInfo.accessoryID);
//			}
//			costResourceImage.sprite = ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type));
//			costResourceCountText.text = costGameResData.count.ToString();
//		}

		public static ConfirmSubstituteEquipmentTipsView Open(int roleInstanceID, int equipmentID, GameResData costGameResData)
		{
			ConfirmSubstituteEquipmentTipsView view = UIMgr.instance.Open<ConfirmSubstituteEquipmentTipsView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetInfo(roleInstanceID, equipmentID, costGameResData);
			return view;
		}

		public void SetInfo (int roleInstanceID, int equipmentID, GameResData costGameResData)
		{
			_roleInstanceID = roleInstanceID;
			_equipmentInstanceID = equipmentID;
			costResourceImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type)));
			costResourceCountText.text = costGameResData.count.ToString();
		}

		#region ui event handlers
		public void ClickCancelHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickDestroyAndEquipHandler ()
		{
//			int roleInstanceID = -1;
//			if (_playerInfo != null)
//			{
//				roleInstanceID = (int)_playerInfo.instanceID;
//			}
//			else if (_heroInfo != null)
//			{
//				roleInstanceID = (int)_heroInfo.instanceID;
//			}
//			EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(_equipmentInfo.instanceID, EquipmentWearOffType.Wear, true, roleInstanceID);
//			UIMgr.instance.Close(PREFAB_PATH);

			EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(_equipmentInstanceID, EquipmentWearOffType.Wear, true, _roleInstanceID);
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickSubstituteHandler ()
		{
//			int roleInstanceID = -1;
//			if (_playerInfo != null)
//			{
//				roleInstanceID = (int)_playerInfo.instanceID;
//			}
//			else if (_heroInfo != null)
//			{
//				roleInstanceID = (int)_heroInfo.instanceID;
//			}
//
//			GameResData costGameResData = EquipmentUtil.CalculatePutOffCost(_oldEquipmentInfo);
//			if (GameProxy.instance.BaseResourceDictionary[costGameResData.type] < costGameResData.count)
//			{
//				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.not_enough_diamond"));
//				return;
//			}
//
//			EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(_equipmentInfo.instanceID, EquipmentWearOffType.Wear, false, roleInstanceID);
//			UIMgr.instance.Close(PREFAB_PATH);

			EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(_equipmentInstanceID, EquipmentWearOffType.Wear, false, _roleInstanceID);
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion ui event handlers
	}
}
