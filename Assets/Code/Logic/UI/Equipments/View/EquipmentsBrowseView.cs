using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Common.UI.Components;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.Tips.View;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Equipments.View
{
    public class EquipmentsBrowseView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/equipments/equipments_browse_view";

		private RoleType _selectedRoleType = RoleType.Invalid;
		private RoleType _selectedEquipmentRoleType = RoleType.Invalid;

		private List<HeroInfo> _cachedHeroInfoList = new List<HeroInfo>();
		private List<EquipmentInfo> _cachedFreeEquipmentInfoList = new List<EquipmentInfo>();

        #region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		private Toggle _currentSelectToggle = null;

		public Toggle allHeroesToggle;
        public Toggle allEquipmentsToggle;

        public GameObject heroEquipmentsRoot;
        public GameObject freeEquipmentsRoot;

		public Text equipmentCellNumTitleText;
		public Text equipmentCellNumText;

		public Dropdown roleTypeDropDown;
		public Dropdown equipmentRoleTypeDropDown;

		public ScrollContent heroesScrollContent;
		public ScrollContent equipmentsScrollContent;

		public Image hasNewEquipmentHintImage;
        #endregion

        void Start()
        {
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			string title = Localization.Get("ui.equipments_browse_view.equipments_browse_title");
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, false);
			
			equipmentCellNumTitleText.text = Localization.Get("ui.equipments_browse_view.own_equipments_title");
			
			hasNewEquipmentHintImage.gameObject.SetActive(EquipmentProxy.instance.HasNewEquipment());

			allHeroesToggle.GetComponent<ToggleContent>().Set(1, Localization.Get("ui.equipments_browse_view.roles_equipments_title"));
			allEquipmentsToggle.GetComponent<ToggleContent>().Set(2, Localization.Get("ui.equipments_browse_view.free_equipments_title"));

			roleTypeDropDown.options[0].text = Localization.Get("ui.equipments_browse_view.equipment_type.all");
			roleTypeDropDown.options[1].text = Localization.Get("ui.equipments_browse_view.equipment_type.defence");
			roleTypeDropDown.options[2].text = Localization.Get("ui.equipments_browse_view.equipment_type.offence");
			roleTypeDropDown.options[3].text = Localization.Get("ui.equipments_browse_view.equipment_type.mage");
			roleTypeDropDown.options[4].text = Localization.Get("ui.equipments_browse_view.equipment_type.support");
			roleTypeDropDown.options[5].text = Localization.Get("ui.equipments_browse_view.equipment_type.mighty");
			roleTypeDropDown.captionText.text = roleTypeDropDown.options[0].text;

			equipmentRoleTypeDropDown.options[0].text = Localization.Get("ui.equipments_browse_view.equipment_type.all");
			equipmentRoleTypeDropDown.options[1].text = Localization.Get("ui.equipments_browse_view.equipment_type.defence");
			equipmentRoleTypeDropDown.options[2].text = Localization.Get("ui.equipments_browse_view.equipment_type.offence");
			equipmentRoleTypeDropDown.options[3].text = Localization.Get("ui.equipments_browse_view.equipment_type.mage");
			equipmentRoleTypeDropDown.options[4].text = Localization.Get("ui.equipments_browse_view.equipment_type.support");
			equipmentRoleTypeDropDown.options[5].text = Localization.Get("ui.equipments_browse_view.equipment_type.mighty");
			equipmentRoleTypeDropDown.captionText.text = equipmentRoleTypeDropDown.options[0].text;
			BindDelegate();

			Invoke("Init", 0.01f);
        }

		void OnDestroy ()
		{
			UnbindDelegate();
		}

		private void BindDelegate ()
		{
			GameProxy.instance.onEquipCellNumUpdateDelegate += OnEquipCellNumUpdateHandler;
			EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate += OnEquipmentInfoListUpdateHandler;
			EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate += OnNewEquipmentMarksChangedHandler;
			PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
			HeroProxy.instance.onHeroInfoUpdateDelegate += OnHeroInfoUpdateHandler;
		}

		private void UnbindDelegate ()
		{
			GameProxy.instance.onEquipCellNumUpdateDelegate -= OnEquipCellNumUpdateHandler;
			EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate -= OnEquipmentInfoListUpdateHandler;
			EquipmentProxy.instance.onNewEquipmentMarksChangedDelegate -= OnNewEquipmentMarksChangedHandler;
			PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
			HeroProxy.instance.onHeroInfoUpdateDelegate -= OnHeroInfoUpdateHandler;
		}

        private void Init()
        {
			RegenerateHeroEquipmentsItems(true);
			RegenerateFreeEquipmentItemButtons();
			RefreshEquipmentCellNumText();
			ClickToggleHandler(allHeroesToggle);
        }	

		private void RegenerateHeroEquipmentsItems (bool playerShowAnimation = false)
		{
			_cachedHeroInfoList.Clear();
			List<HeroInfo> allHeroInfoList = HeroProxy.instance.GetAllHeroInfoList();
			if (_selectedRoleType == RoleType.Invalid)
			{
				_cachedHeroInfoList.AddRange(allHeroInfoList);
			}
			else
			{
				for (int index = 0, count = allHeroInfoList.Count; index < count; index++)
				{
					if (allHeroInfoList[index].heroData.roleType == _selectedRoleType)
						_cachedHeroInfoList.Add(allHeroInfoList[index]);
				}
			}
			_cachedHeroInfoList.Sort(HeroUtil.CompareHeroByQualityDesc);
			int roleItemCount = _cachedHeroInfoList.Count;
			if (_selectedRoleType == RoleType.Invalid || GameProxy.instance.PlayerInfo.heroData.roleType == _selectedRoleType)
			{
				roleItemCount += 1; // count the player(as the first item)
			}
			heroesScrollContent.Init(roleItemCount, playerShowAnimation);
		}

		private void RefreshHeroEquipmentsItems ()
		{
			heroesScrollContent.RefreshAllContentItems();
		}

		private void RegenerateFreeEquipmentItemButtons ()
		{
			_cachedFreeEquipmentInfoList.Clear();
			List<EquipmentInfo> allFreeEquipmentInfoList = EquipmentProxy.instance.GetFreeEquipmentInfoList();
			if (_selectedEquipmentRoleType == RoleType.Invalid)
			{
				_cachedFreeEquipmentInfoList.AddRange(allFreeEquipmentInfoList);
			}
			else
			{
				for (int index = 0, count = allFreeEquipmentInfoList.Count; index < count; index++)
				{
					if (allFreeEquipmentInfoList[index].equipmentData.equipmentRoleType == _selectedEquipmentRoleType)
						_cachedFreeEquipmentInfoList.Add(allFreeEquipmentInfoList[index]);
				}
			}
			equipmentsScrollContent.Init(_cachedFreeEquipmentInfoList.Count,true);
		}

		private void RefreshFreeEquipmentItemButtons ()
		{
			equipmentsScrollContent.RefreshAllContentItems();
		}

		private void RefreshEquipmentCellNumText ()
		{
			equipmentCellNumText.text = string.Format(Localization.Get("common.value/max"), EquipmentProxy.instance.GetFreeEquipmentInfoList().Count, GameProxy.instance.EquipCellNum);
		}

		private void RefreshNewEquipmentMarks ()
		{
			hasNewEquipmentHintImage.gameObject.SetActive(EquipmentProxy.instance.HasNewEquipment());
		}

		#region proxy callback handlers
		private void OnEquipCellNumUpdateHandler ()
		{
			RefreshEquipmentCellNumText();
		}

		private void OnEquipmentInfoListUpdateHandler ()
		{
			RegenerateFreeEquipmentItemButtons();
			RefreshEquipmentCellNumText();
			RefreshNewEquipmentMarks();
		}

		private void OnNewEquipmentMarksChangedHandler ()
		{
			equipmentsScrollContent.RefreshAllContentItems();
			RefreshNewEquipmentMarks();
		}

		public void OnPlayerInfoUpdateHandler ()
		{
            //PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
			heroesScrollContent.RefreshAllContentItems();
		}

		private void OnHeroInfoUpdateHandler (uint heroInstanceID)
		{
            //HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceID);
			heroesScrollContent.RefreshAllContentItems();
		}
		#endregion

        #region ui event handlers
        public void ClickToggleHandler(Toggle toggle)
        {
			if (toggle.isOn)
			{
				if (_currentSelectToggle == toggle)
					return;

				_currentSelectToggle = toggle;
	            heroEquipmentsRoot.SetActive(false);
	            freeEquipmentsRoot.SetActive(false);

				if (toggle == allHeroesToggle)
					heroEquipmentsRoot.SetActive(true);
				else if (toggle == allEquipmentsToggle)
					freeEquipmentsRoot.SetActive(true);

				if (_currentSelectToggle == allEquipmentsToggle && toggle != allEquipmentsToggle)
					EquipmentProxy.instance.ClearNewEquipmentMarks();
			}
        }

		public void ClickExpandBagHandler ()
		{
			if (GameProxy.instance.EquipCellNum >= GlobalData.GetGlobalData().equipPackageMaxNum)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.equipment_bag_reach_max"));
				return;
			}

			int cost = (GameProxy.instance.EquipCellBuyNum + 1) * GlobalData.GetGlobalData().equipPackageBuyA + GlobalData.GetGlobalData().equipPackageBuyB;
			//if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondBuyEquipGrid))
				Logic.UI.Tips.View.CommonExpandBagTipsView.Open(BagType.EquipmentBag, cost, ClickConfirmExpandBagHandler,ConsumeTipType.None);
			//else
			//	ClickConfirmExpandBagHandler();
		}
		
		public void ClickConfirmExpandBagHandler ()
		{
			Logic.Game.Controller.GameController.instance.CLIENT2LOBBY_BUY_PACK_CELL_REQ((int)BagType.EquipmentBag);
		}

		public void OnEquipmentTypeValueChanged (int index)
		{
			RoleType equipmentRoleType = (RoleType)equipmentRoleTypeDropDown.value;
			_selectedEquipmentRoleType = equipmentRoleType;
			RegenerateFreeEquipmentItemButtons();
		}

		public void OnRoleTypeValueChanged (int index)
		{
			RoleType roleType = (RoleType)roleTypeDropDown.value;
			_selectedRoleType = roleType;
			RegenerateHeroEquipmentsItems();
		}

        public void ClickCloseHandler()
        {
			if (_currentSelectToggle == allEquipmentsToggle)
				EquipmentProxy.instance.ClearNewEquipmentMarks();
            UIMgr.instance.Close(PREFAB_PATH);
        }

		public void ResetHeroEquipmentsItemHandler (GameObject gameObject, int index)
		{
			HeroEquipmentsItem heroEquipmentsItem = gameObject.GetComponent<HeroEquipmentsItem>();
			if (heroEquipmentsItem != null)
			{
				PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
				if (_selectedRoleType == RoleType.Invalid || playerInfo.heroData.roleType == _selectedRoleType)
				{
					if (index == 0)
						heroEquipmentsItem.SetPlayerInfo(playerInfo);
					else
						heroEquipmentsItem.SetHeroInfo(_cachedHeroInfoList[index - 1]);
				}
				else
				{
					heroEquipmentsItem.SetHeroInfo(_cachedHeroInfoList[index]);
				}
			}
		}

		public void ResetFreeEquipmentItemButtonHandler (GameObject gameObject, int index)
		{
			FreeEquipmentItemButton freeEquipmentItemButton = gameObject.GetComponent<FreeEquipmentItemButton>();
			freeEquipmentItemButton.SetEquipmentInfo(_cachedFreeEquipmentInfoList[index]);
		}
        #endregion
    }
}
