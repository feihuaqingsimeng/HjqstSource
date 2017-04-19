using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Common.Util;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Character;
using Logic.Player.Model;
using Logic.Player.Controller;
using Logic.Hero.Model;
using Logic.Hero.Controller;
using Logic.Equipment;
using Logic.Equipment.Model;
using Logic.Equipment.Controller;
using Logic.UI.CommonTopBar.View;
using Logic.UI.EquipmentsStrengthen.View;
using Logic.UI.Tips.View;
using Logic.Role.Model;
using Common.UI.Components;
using Logic.Role;
using System.Collections;
using Logic.ConsumeTip.Model;
using Logic.UI.CommonEquipment.View;

namespace Logic.UI.Equipments.View
{
	public class RoleEquipmentsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/equipments/role_equipments_view";
		
		private PlayerInfo _playerInfo;
		private HeroInfo _heroInfo;
		private RoleInfo _roleInfo;
		private List<EquipmentInfo> _freeEquipmentInfoList;

		private EquipmentInfo _selectedEquipmentInfo = null;

		private int _currentSelectEquipmentIndex;
		private List<EquipmentItem> _freeEquipmentItemList = new List<EquipmentItem>();
		private bool _sortFreeEquipmentItemsAsc = false;

		#region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text weaponTitleText;
		public Text armorTitleText;
		public Text accessoryTitleText;
		public Transform attrRoot;
		public EquipAttributeView attrViewPrefab;

		public Text strengthenEquipmentText;
		public Text removeEquipmentText;

		public Text ownEquipmentsTitleText;
		public Text equipmentCellNumText;
		public Text sortText;
		public GameObject sortTypeRoot;
		public Text sortAscText;
		public Text sortDescText;
		
		public Transform roleHeadIconRoot;

		public EquipmentIcon weaponIcon;
		public EquipmentIcon armorIcon;
		public EquipmentIcon accessoryIcon;

		public Transform freeEquipmentItemsRootTransform;
		public EquipmentItem equipmentItemPrefab;

		public RectTransform currentEquipRoot;
		public Text currentEquipmentNameText;
		
		public Button weaponBotton;
		public Button armorButton;
		public Button accessoryButton;
		public Image selectedEquipmentSlotMask;

		public ScrollContent freeEquipmentsScrollContent;

		public Text _selectedEquipmentNameText;
		public Text mainAttrNameText;
		public Text mainAttrValueText;
		public Text randomAttr1NameText;
		public Text randomAttr1ValueText;
		public Text randomAttr2NameText;
		public Text randomAttr2ValueText;
		public Text gemAttr1NameText;
		public Text gemAttr1ValueText;
		public Text gemAttr2NameText;
		public Text gemAttr2ValueText;
		public Text enchantAttrNameText;
		public Text enchantAttrValueText;
		#endregion

		void Start ()
		{
			Init();
			BindDelegate();
		}

		void OnDestroy ()
		{
			UnbindDelegate();
		}

		private void BindDelegate ()
		{
			GameProxy.instance.onEquipCellNumUpdateDelegate += OnEquipCellNumUpdateHandler;
			PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
			HeroProxy.instance.onHeroInfoUpdateDelegate += UpdateHeroInfoHandler;
			EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate += OnEquipmentInfoListUpdateHandler;
			EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate += OnNewEquipmentMarksChangedHandler;
		}
		
		private void UnbindDelegate ()
		{
			GameProxy.instance.onEquipCellNumUpdateDelegate -= OnEquipCellNumUpdateHandler;
			PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
			HeroProxy.instance.onHeroInfoUpdateDelegate -= UpdateHeroInfoHandler;
			EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate -= OnEquipmentInfoListUpdateHandler;
			EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate -= OnNewEquipmentMarksChangedHandler;
		}

		private void Init ()
		{
			string title = Localization.Get("ui.role_equipments.role_equipments_title");
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, false);

			weaponTitleText.text = Localization.Get("ui.role_equipments.weapon_title");
			armorTitleText.text = Localization.Get("ui.role_equipments.armor_title");
			accessoryTitleText.text = Localization.Get("ui.role_equipments.accessory_title");
			strengthenEquipmentText.text = Localization.Get("ui.role_equipments.strengthen_equipment");
			removeEquipmentText.text = Localization.Get("ui.role_equipments.remove_equipment");
			ownEquipmentsTitleText.text = Localization.Get("ui.role_equipments.own_equipments");
			sortText.text = _sortFreeEquipmentItemsAsc ? Localization.Get("ui.role_equipments.sort_asceding") : Localization.Get("ui.role_equipments.sort_desceding");
			sortAscText.text = Localization.Get("ui.role_equipments.sort_asceding");
			sortDescText.text = Localization.Get("ui.role_equipments.sort_desceding");
			RefreshEquipmentCellNumText();
			SetSelectedEquipmentInfo(null);
			StartCoroutine(ShowSelectMarkCoroutine());
			attrViewPrefab.gameObject.SetActive(false);
		}
		private IEnumerator ShowSelectMarkCoroutine()
		{
			selectedEquipmentSlotMask.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.6f);
			selectedEquipmentSlotMask.gameObject.SetActive(true);
		}
		private void RefreshCharacterEquipmentImages ()
		{
			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;

			if (_playerInfo != null)
			{
				characterWeaponID = _playerInfo.weaponID;
				characterArmorID = _playerInfo.armorID;
				characterAccessoryID = _playerInfo.accessoryID;
			}
			else if (_heroInfo != null)
			{
				characterWeaponID = _heroInfo.weaponID;
				characterArmorID = _heroInfo.armorID;
				characterAccessoryID = _heroInfo.accessoryID;
			}

			weaponIcon.gameObject.SetActive(false);
			armorIcon.gameObject.SetActive(false);
			accessoryIcon.gameObject.SetActive(false);

			weaponTitleText.gameObject.SetActive(true);
			armorTitleText.gameObject.SetActive(true);
			accessoryTitleText.gameObject.SetActive(true);

			if (characterWeaponID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterWeaponID);
				weaponIcon.SetEquipmentInfo(equipmentInfo);
				weaponIcon.gameObject.SetActive(true);
				weaponTitleText.gameObject.SetActive(false);
			}
			if (characterArmorID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterArmorID);
				armorIcon.SetEquipmentInfo(equipmentInfo);
				armorIcon.gameObject.SetActive(true);
				armorTitleText.gameObject.SetActive(false);
			}
			if (characterAccessoryID != 0)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterAccessoryID);
				accessoryIcon.SetEquipmentInfo(equipmentInfo);
				accessoryIcon.gameObject.SetActive(true);
				accessoryTitleText.gameObject.SetActive(false);
			}
		}

		private void RegenerateFreeEquipmentItems (bool playAnimation)
		{
			_freeEquipmentInfoList = EquipmentProxy.instance.GetFreeEquipmentInfoList();
			_freeEquipmentInfoList.Sort(CompareEquipmentInfo);
			freeEquipmentsScrollContent.Init(_freeEquipmentInfoList.Count, playAnimation);
		}

		private void RefreshFreeEquipmentItems ()
		{
			int freeEquipmentItemCount = _freeEquipmentItemList.Count;
			for (int i = 0; i < freeEquipmentItemCount; i++)
			{
				_freeEquipmentItemList[i].SetEquipmentInfo(_freeEquipmentItemList[i].EquipmentInfo);
			}
		}

		private int GetEquipmentTypeWeight (EquipmentInfo equipmentInfo)
		{
			int weight = 0;
			EquipmentType equipmentType = equipmentInfo.equipmentData.equipmentType;
			EquipmentType roleCorrespondingWeaponType = EquipmentType.None;
			if (_playerInfo != null)
			{
				roleCorrespondingWeaponType = RoleUtil.GetRoleCorrespondingWeaponType(_playerInfo);
			}
			else if (_heroInfo != null)
			{
				roleCorrespondingWeaponType = RoleUtil.GetRoleCorrespondingWeaponType(_heroInfo);
			}

			if (_currentSelectEquipmentIndex == 0)
			{
				if (equipmentType == EquipmentType.PhysicalWeapon
				    || equipmentType == EquipmentType.MagicalWeapon)
				{
					if (equipmentType == roleCorrespondingWeaponType)
					{
						weight = 4;
					}
					else
					{
						weight = 1;
					}
				}
				else if (equipmentType == EquipmentType.Armor)
				{
					weight = 3;
				}
				else if (equipmentType == EquipmentType.Accessory)
				{
					weight = 2;
				}
			}
			else if (_currentSelectEquipmentIndex == 1)
			{
				if (equipmentType == EquipmentType.Armor)
				{
					weight = 4;
				}
				else if (equipmentType == EquipmentType.PhysicalWeapon
				    || equipmentType == EquipmentType.MagicalWeapon)
				{
					if (equipmentType == roleCorrespondingWeaponType)
					{
						weight = 2;
					}
					else
					{
						weight = 1;
					}
				}
				else if (equipmentType == EquipmentType.Accessory)
				{
					weight = 3;
				}
			}
			else if (_currentSelectEquipmentIndex == 2)
			{
				if (equipmentType == EquipmentType.Accessory)
				{
					weight = 4;
				}
				else if (equipmentType == EquipmentType.PhysicalWeapon
				    || equipmentType == EquipmentType.MagicalWeapon)
				{
					if (equipmentType == roleCorrespondingWeaponType)
					{
						weight = 3;
					}
					else
					{
						weight = 1;
					}
				}
				else if (equipmentType == EquipmentType.Armor)
				{
					weight = 2;
				}
			}

			return weight;
		}

		private int CompareEquipmentInfo (EquipmentInfo aEquipmentInfo, EquipmentInfo bEquipmentInfo)
		{
			return EquipmentUtil.CompareEquipmentWeight(aEquipmentInfo, bEquipmentInfo,(EquipmentType)(_currentSelectEquipmentIndex+1),_roleInfo.heroData.roleType,_sortFreeEquipmentItemsAsc);
		}

		private void RefreshCurrentEquipment ()
		{
			int currentEquipmentInstanceID = -1;

			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;
			
			if (_playerInfo != null)
			{
				characterWeaponID = _playerInfo.weaponID;
				characterArmorID = _playerInfo.armorID;
				characterAccessoryID = _playerInfo.accessoryID;
			}
			else if (_heroInfo != null)
			{
				characterWeaponID = _heroInfo.weaponID;
				characterArmorID = _heroInfo.armorID;
				characterAccessoryID = _heroInfo.accessoryID;
			}

			if (_currentSelectEquipmentIndex == 0
			    && characterWeaponID > 0)
			{
				currentEquipmentInstanceID = characterWeaponID;
			}
			else if (_currentSelectEquipmentIndex == 1
			    && characterArmorID > 0)
			{
				currentEquipmentInstanceID = characterArmorID;
			}
			else if (_currentSelectEquipmentIndex == 2
			    && characterAccessoryID > 0)
			{
				currentEquipmentInstanceID = characterAccessoryID;
			}

			if (currentEquipmentInstanceID > 0)
			{
				EquipmentInfo currentEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(currentEquipmentInstanceID);

				for(int i = 0,count = currentEquipmentInfo.EquipAttribute.Count;i<count;i++)
				{
					EquipAttributeView view = Instantiate<EquipAttributeView>(attrViewPrefab);
					view.transform.SetParent(attrRoot,false);
					view.gameObject.SetActive(true);
					view.Set(currentEquipmentInfo.EquipAttribute[i]);
				}
			}
		}
		
		private void ResetCurrentSelectEquipmentIndex (int index)
		{
			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;
			
			if (_playerInfo != null)
			{
				characterWeaponID = _playerInfo.weaponID;
				characterArmorID = _playerInfo.armorID;
				characterAccessoryID = _playerInfo.accessoryID;
			}
			else if (_heroInfo != null)
			{
				characterWeaponID = _heroInfo.weaponID;
				characterArmorID = _heroInfo.armorID;
				characterAccessoryID = _heroInfo.accessoryID;
			}

			_currentSelectEquipmentIndex = index;
			EquipmentInfo equipmentInfo = null;
			if (_currentSelectEquipmentIndex == 0)
			{
				selectedEquipmentSlotMask.transform.localPosition = weaponBotton.transform.localPosition;
				if (characterWeaponID != 0)
				{
					equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterWeaponID);

				}
			}
			else if (_currentSelectEquipmentIndex == 1)
			{
				selectedEquipmentSlotMask.transform.localPosition = armorButton.transform.localPosition;
				if (characterArmorID != 0)
				{
					equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterArmorID);
				}
			}
			else if (_currentSelectEquipmentIndex == 2)
			{
				selectedEquipmentSlotMask.transform.localPosition = accessoryButton.transform.localPosition;
				if (characterAccessoryID != 0)
				{
					equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(characterAccessoryID);
				}
			}
			if(equipmentInfo != null)
			{
				TransformUtil.ClearChildren(currentEquipRoot,true);
				CommonEquipmentIcon icon = CommonEquipmentIcon.Create(currentEquipRoot);
				icon.SetEquipmentInfo(equipmentInfo);
				icon.transform.SetParent(currentEquipRoot,false);
				currentEquipmentNameText.text = Localization.Get(equipmentInfo.equipmentData.name);
			}
			RefreshCurrentEquipment();
			RegenerateFreeEquipmentItems(false);
		}

		private void RefreshEquipmentCellNumText ()
		{
			equipmentCellNumText.text = string.Format(Localization.Get("common.value/max"), EquipmentProxy.instance.GetFreeEquipmentInfoList().Count, GameProxy.instance.EquipCellNum);
		}

		private void RefreshSelectedEquipmentInfoPanel ()
		{
			if (_selectedEquipmentInfo != null)
			{
				_selectedEquipmentNameText.text = Localization.Get(_selectedEquipmentInfo.equipmentData.name);
			}
			else
			{
				_selectedEquipmentNameText.text = string.Empty;
			}
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
			_heroInfo = heroInfo;
			_roleInfo = heroInfo;
            //string heroIconImagePath = ResPath.GetCharacterHeadIconPath(_heroInfo.heroData.headIcons[_heroInfo.advanceLevel - 1]);
			CommonHeroIcon.View.CommonHeroIcon commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.CreateBigIcon(roleHeadIconRoot);
			commonHeroIcon.SetRoleInfo(_heroInfo);
			RefreshCharacterEquipmentImages();
			ResetCurrentSelectEquipmentIndex(0);
			RegenerateFreeEquipmentItems(true);
			ClickWeaponHandler();
		}

		public void SetPlayerInfo (PlayerInfo playerInfo)
		{
			_playerInfo = playerInfo;
			_roleInfo = playerInfo;
            //string playerIconImagePath = _playerInfo.HeadIcon;
			CommonHeroIcon.View.CommonHeroIcon commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.CreateBigIcon(roleHeadIconRoot);
			commonHeroIcon.SetRoleInfo(_playerInfo);
			commonHeroIcon.UsePetIcon();
			RefreshCharacterEquipmentImages();
			ResetCurrentSelectEquipmentIndex(0);
			RegenerateFreeEquipmentItems(true);
			ClickWeaponHandler();
		}

		public void SetRoleInfo (RoleInfo roleInfo)
		{
			if (roleInfo is PlayerInfo)
			{
				PlayerInfo playerInfo = roleInfo as PlayerInfo;
				SetPlayerInfo(playerInfo);
			}
			else if (roleInfo is HeroInfo)
			{
				HeroInfo heroInfo = roleInfo as HeroInfo;
				SetHeroInfo(heroInfo);
			}
		}

		public void SetCurrentSelectEquipmentType (EquipmentType equipmentType)
		{
			int currentSelectEquipmentIndex = 0;
			switch (equipmentType)
			{
				case EquipmentType.MagicalWeapon:
				case EquipmentType.PhysicalWeapon:
					currentSelectEquipmentIndex = 0;
					break;
				case EquipmentType.Armor:
					currentSelectEquipmentIndex = 1;
					break;
				case EquipmentType.Accessory:
					currentSelectEquipmentIndex = 2;
					break;
				default:
					currentSelectEquipmentIndex = 0;
				break;
			}
			ResetCurrentSelectEquipmentIndex(currentSelectEquipmentIndex);
		}

		public void SetSelectedEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_selectedEquipmentInfo = equipmentInfo;
			RefreshSelectedEquipmentInfoPanel();
		}

		#region UI event handlers
		public void ClickWeaponHandler ()
		{
			ResetCurrentSelectEquipmentIndex(0);
		}
		
		public void ClickArmorHandler ()
		{
			ResetCurrentSelectEquipmentIndex(1);
		}
		
		public void ClickAccessoryHandler ()
		{
			ResetCurrentSelectEquipmentIndex(2);
		}

		public void ClickSortTypeHandler ()
		{
			sortTypeRoot.SetActive(!sortTypeRoot.activeSelf);
		}

		public void ClickSortAscHandler ()
		{
			_sortFreeEquipmentItemsAsc = true;
			sortText.text = _sortFreeEquipmentItemsAsc ? Localization.Get("ui.role_equipments.sort_asceding") : Localization.Get("ui.role_equipments.sort_desceding");
			RegenerateFreeEquipmentItems(false);
			sortTypeRoot.SetActive(false);
		}

		public void ClickSortDescHandler ()
		{
			_sortFreeEquipmentItemsAsc = false;
			sortText.text = _sortFreeEquipmentItemsAsc ? Localization.Get("ui.role_equipments.sort_asceding") : Localization.Get("ui.role_equipments.sort_desceding");
			RegenerateFreeEquipmentItems(false);
			sortTypeRoot.SetActive(false);
		}

		public void ClickEquipmentButtonHandler (EquipmentItem equipmentItem)
		{
			SetSelectedEquipmentInfo(equipmentItem.EquipmentInfo);
		}

		public void ClickStrengthenCurrentSelectedEquipmentHandler ()
		{
			if(!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.EquipTraining,true))
			{
				return;
			}
			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;
			
			if (_playerInfo != null)
			{
				characterWeaponID = _playerInfo.weaponID;
				characterArmorID = _playerInfo.armorID;
				characterAccessoryID = _playerInfo.accessoryID;
			}
			else if (_heroInfo != null)
			{
				characterWeaponID = _heroInfo.weaponID;
				characterArmorID = _heroInfo.armorID;
				characterAccessoryID = _heroInfo.accessoryID;
			}

			if (_currentSelectEquipmentIndex == 0
			    && characterWeaponID != 0)
			{
				ShowTrainingView(characterWeaponID);

			}
			else if (_currentSelectEquipmentIndex == 1
			         && characterArmorID != 0)
			{
				ShowTrainingView(characterArmorID);

			}
			else if (_currentSelectEquipmentIndex == 2
			         && characterAccessoryID != 0)
			{
				ShowTrainingView(characterAccessoryID);
			}
		}
		private void ShowTrainingView(int equipInstanceId)
		{

		}
		public void ClickPutOffEquipmentHandler ()
		{

			int characterWeaponID = 0;
			int characterArmorID = 0;
			int characterAccessoryID = 0;
			
			if (_playerInfo != null)
			{
				characterWeaponID = _playerInfo.weaponID;
				characterArmorID = _playerInfo.armorID;
				characterAccessoryID = _playerInfo.accessoryID;
			}
			else if (_heroInfo != null)
			{
				characterWeaponID = _heroInfo.weaponID;
				characterArmorID = _heroInfo.armorID;
				characterAccessoryID = _heroInfo.accessoryID;
			}

			int putOffEquipmentInstanceID = 0;
			if (_currentSelectEquipmentIndex == 0
			    && characterWeaponID != 0)
			{
				putOffEquipmentInstanceID = characterWeaponID;
			}
			if (_currentSelectEquipmentIndex == 1
			    && characterArmorID != 0)
			{
				putOffEquipmentInstanceID = characterArmorID;
			}
			if (_currentSelectEquipmentIndex == 2
			    && characterAccessoryID != 0)
			{
				putOffEquipmentInstanceID = characterAccessoryID;
			}


			if (putOffEquipmentInstanceID != 0)
			{
				int roleInstanceID = -1;
				if (_playerInfo != null)
				{
					roleInstanceID = (int)_playerInfo.instanceID;
				}
				else if (_heroInfo != null)
				{
					roleInstanceID = (int)_heroInfo.instanceID;
				}
				EquipmentInfo putOffEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(putOffEquipmentInstanceID);
				ConfirmPutOffEquipmentTipsView confirmPutOffEquipmentTipsView = UIMgr.instance.Open<ConfirmPutOffEquipmentTipsView>(ConfirmPutOffEquipmentTipsView.PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
//				confirmPutOffEquipmentTipsView.SetInfo(putOffEquipmentInfo, roleInstanceID);
			}
		}

		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickPutOnEquipmentHandler (EquipmentItem equipmentItem)
		{
			EquipmentProxy.instance.SetEquipmentAsChecked(equipmentItem.EquipmentInfo.instanceID);

			int oldWeaponID = _roleInfo.weaponID;
			int oldArmorID = _roleInfo.armorID;
			int oldAccessoryID = _roleInfo.accessoryID;
            //bool hasOldEquipment = false;
			EquipmentInfo oldEquip = null;
			EquipmentType type = equipmentItem.EquipmentInfo.equipmentData.equipmentType;
			if (type == EquipmentType.PhysicalWeapon && oldWeaponID > 0)
			{
				oldEquip = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(oldWeaponID);
			}
			else if (type == EquipmentType.Armor && oldArmorID > 0)
			{
				oldEquip = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(oldArmorID);
			}
			else if (type == EquipmentType.Accessory && oldAccessoryID > 0)
			{
				oldEquip = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(oldAccessoryID);
			}
			if(_roleInfo.level < equipmentItem.EquipmentInfo.equipmentData.useLv)
			{
				CommonErrorTipsView.Open(string.Format(Localization.Get("ui.role_equipments.notEnoughLv"),equipmentItem.EquipmentInfo.equipmentData.useLv));
				return;
			}
			if (oldEquip != null)
			{
//				ConfirmSubstituteEquipmentTipsView.Open(_roleInfo,equipmentItem.EquipmentInfo);
			}
			else
			{
				EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(equipmentItem.EquipmentInfo.instanceID, EquipmentWearOffType.Wear, false, (int)_roleInfo.instanceID);
			}
		}
		public void ClickStrengthenEquipmentHandler (EquipmentItem equipmentItem)
		{

		}

		public void ClickSellEquipmentHandler (EquipmentItem equipmentItem)
		{

			EquipmentProxy.instance.SetEquipmentAsChecked(equipmentItem.EquipmentInfo.instanceID);
			ConfirmSellEquipmentView.Open(equipmentItem.EquipmentInfo);
		}

		public void ClickExpandBagHandler ()
		{
			int cost = (GameProxy.instance.EquipCellBuyNum + 1) * GlobalData.GetGlobalData().equipPackageBuyA + GlobalData.GetGlobalData().equipPackageBuyB;
			if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondBuyEquipGrid))
				Logic.UI.Tips.View.CommonExpandBagTipsView.Open(BagType.EquipmentBag, cost, ClickConfirmExpandBagHandler,ConsumeTipType.DiamondBuyEquipGrid);
			else
				ClickConfirmExpandBagHandler();
		}
		
		public void ClickConfirmExpandBagHandler ()
		{
			Logic.Game.Controller.GameController.instance.CLIENT2LOBBY_BUY_PACK_CELL_REQ((int)BagType.EquipmentBag);
		}

		public void OnResetFreeEquipmentItem(GameObject freeEquipmentItemGameObject, int index)
		{
			EquipmentItem equipmentItem = freeEquipmentItemGameObject.GetComponent<EquipmentItem>();
			if (equipmentItem != null)
			{
				EquipmentInfo info = _freeEquipmentInfoList[index];
                //EquipmentType roleCorrespondingWeaponType = EquipmentType.None;

				equipmentItem.SetEquipmentInfo(info);
				if (_roleInfo.heroData.roleType == info.equipmentData.equipmentRoleType)
				{
					equipmentItem.EnablePutOnButton();
				}
				else
				{
					equipmentItem.DisablePutOnButton();
				}
				equipmentItem.SetEquipLevelColor(_roleInfo.level > info.equipmentData.useLv ? Color.green : Color.red);

			}
		}
		#endregion

		#region proxy callback handlers
		public void OnEquipCellNumUpdateHandler ()
		{
			RefreshEquipmentCellNumText();
		}

		public void OnPlayerInfoUpdateHandler ()
		{
			_playerInfo = GameProxy.instance.PlayerInfo;
			RefreshCharacterEquipmentImages();
			ResetCurrentSelectEquipmentIndex(_currentSelectEquipmentIndex);
		}

		public void UpdateHeroInfoHandler (uint heroInstanceID)
		{
			if (_heroInfo.instanceID == heroInstanceID)
			{
				RefreshCharacterEquipmentImages();
				ResetCurrentSelectEquipmentIndex(_currentSelectEquipmentIndex);
			}
		}

		public void OnEquipmentInfoListUpdateHandler ()
		{
			RegenerateFreeEquipmentItems(false);
			ResetCurrentSelectEquipmentIndex(_currentSelectEquipmentIndex);
			RefreshCharacterEquipmentImages();
			RefreshCurrentEquipment();
			RefreshEquipmentCellNumText();
		}

		public void OnNewEquipmentMarksChangedHandler ()
		{
			RefreshFreeEquipmentItems();
		}
		#endregion
	}
}
