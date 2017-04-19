using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Common.Util;
using Logic.Equipment.Model;
using Logic.Equipment.Controller;
using Logic.UI.CommonEquipment.View;
using Logic.UI.Equipments.View;
using Logic.Equipment;
using Logic.UI.EquipmentsStrengthen.Model;
using Logic.Enums;
using Logic.UI.Tips.View;
using Logic.Game.Model;
using Common.UI.Components;
using Logic.UI.CommonTopBar.View;

namespace Logic.UI.EquipmentsStrengthen.View
{
	public class EquipmentStrengthenView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/equipments/equipment_strengthen_view";
		
		#region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text attributesAfterStrengthenTitleText;
		public Text strengthenTipsText;
		public Text strengthenLevelText;

		public Text startStrengthenText;
		public Text coinText;
		public Text selectStrengthenMaterialTitleText;

		public Text addExpText;
		public Slider sliderAddExpBar;
		public Slider sliderCurrentExpBar;
		public Dropdown dropDownSort; 
		public Transform strengthenEquipmentRoot;
		public Transform[] selectedMaterialRootTran;


		public Transform freeEquipmentsRootTransform;
		public Transform attribute_root;
		public EquipAttributeView attributeViewPrefab;
		public ScrollContentExpand scrollContent;
		public Text noAvailableStrengthenMaterialText;
		#endregion

		private CommonEquipmentIcon _currentEquipIcon;
		private bool _isReachMaxLevel;
		void Awake ()
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

			EquipmentStrengthenProxy.instance.onEquipmentStrengthenSuccessDelegate = OnStrengthenSuccess;
		}

		private void UnbindDelegate ()
		{
			EquipmentStrengthenProxy.instance.onEquipmentStrengthenSuccessDelegate = null;
		}

		private void Init ()
		{
			string title = Localization.Get("ui.equipment_strengthen_view.equipment_strengthen_title");
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, false);

			attributesAfterStrengthenTitleText.text = Localization.Get("ui.equipment_strengthen_view.attributes_after_strengthen_title");
			strengthenTipsText.text = Localization.Get("ui.equipment_strengthen_view.strengthen_tips");
			//strengthenLevelText.text = string.Format(Localization.Get("ui.equipment_strengthen_view.strengthen_level"), 0);

			startStrengthenText.text = Localization.Get("ui.equipment_strengthen_view.start_strengthen");
			coinText.text = string.Format(Localization.Get("ui.equipment_strengthen_view.coin"), 999000);
			selectStrengthenMaterialTitleText.text = Localization.Get("ui.equipment_strengthen_view.select_strengthen_material_title");
			noAvailableStrengthenMaterialText.text = Localization.Get("ui.equipment_strengthen_view.no_available_strengthen_material");
		}

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{


			EquipmentStrengthenProxy.instance.StrengthenEquipInfo = equipmentInfo;
			EquipmentStrengthenProxy.instance.ClearMaterials();
			//cur equip

			CommonEquipmentIcon equipIcon = CommonEquipmentIcon.Create(strengthenEquipmentRoot);
			equipIcon.SetEquipmentInfo(equipmentInfo);
			_currentEquipIcon = equipIcon;
		
			RegenerateEquipmentTable();
			Refresh();
		}

		public void RegenerateEquipmentTable ()
		{
			//EquipmentInfo equipInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo;

			//TransformUtil.ClearChildren(freeEquipmentsRootTransform, true);
			//commonEquipmentIconPrefab.gameObject.SetActive(true);
			List<EquipmentInfo> freeEquipmentInfoList = EquipmentStrengthenProxy.instance.GetEquipmentsByType((EquipmentStrengthenSortType)dropDownSort.value);
			int freeEquipmentCount = freeEquipmentInfoList.Count;
			scrollContent.Init(freeEquipmentCount);
			noAvailableStrengthenMaterialText.gameObject.SetActive(freeEquipmentCount < 1);
//			EquipmentInfo equip ;
//			for (int i = 0; i < freeEquipmentCount; i++)
//			{
//				equip = freeEquipmentInfoList[i];
//				if (equip.instanceID != equipInfo.instanceID)
//				{
//					GameObject equipmentIconGameObject = GameObject.Instantiate(commonEquipmentIconPrefab.gameObject);
//					equipmentIconGameObject.transform.SetParent(freeEquipmentsRootTransform, false);
//					equipmentIconGameObject.transform.localPosition = Vector3.zero;
//					equipmentIconGameObject.name = equip.instanceID.ToString();
//					CommonEquipmentIcon equipmentIconButton = equipmentIconGameObject.GetComponent<CommonEquipmentIcon>();
//					equipmentIconButton.SetEquipmentInfo(equip);
//					equipmentIconButton.onClickHandler = ClickEquipmentIconButton;
//				}
//			}
//			commonEquipmentIconPrefab.gameObject.SetActive(false);
		}
		private void Refresh()
		{
			RefreshEquipmentTable();
			RefreshMaterialEquipment();
			RefreshExp();
		}

		private void RefreshExp()
		{
			EquipmentInfo equipmentInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo ;
			EquipmentInfo[] materials = EquipmentStrengthenProxy.instance.materialsEquipInfo;
			int count = materials.Length;
			int totalExpProvide = 0;
			int selectMaterialCount = 0;
			for(int i = 0;i<count;i++)
			{
				EquipmentInfo info = materials[i];
				if(info!= null)
				{
					EquipmentStrengthenData data = EquipmentStrengthenData.GetStrengthenDataByStar(info.equipmentData.star);
					totalExpProvide += data.exp_provide;
					selectMaterialCount++;
				}

			}
			//current
			int curLvel = equipmentInfo.strengthenLevel;
			EquipmentStrengthenNeedData needData = EquipmentStrengthenNeedData.GetStrengthenNeedDataByLv(curLvel);
			bool isMax = (needData==null);
			//level
			EquipmentStrengthenNeedData nextData = EquipmentStrengthenNeedData.GetStrengthenNeedDataByExp(EquipmentStrengthenNeedData.GetStrengthenTotalExp(curLvel)+equipmentInfo.strengthenExp+ totalExpProvide);
			_isReachMaxLevel = isMax ? true: (nextData == null ? true : false);
			int addLevel = isMax ? 0:(nextData == null ? 1 : nextData.aggr_lv-equipmentInfo.strengthenLevel);

			strengthenLevelText.text = string.Format(Localization.Get("ui.equipment_strengthen_view.strengthen_level"), equipmentInfo.strengthenLevel, addLevel);


			if(isMax)
			{
				addExpText.text = "MAX";
				sliderCurrentExpBar.value = 0;
				sliderAddExpBar.value = 0;
				coinText.text = "0";
			}else{

				sliderCurrentExpBar.value = (equipmentInfo.strengthenExp+0.0f)/needData.exp_need;
				sliderAddExpBar.value = (equipmentInfo.strengthenExp+totalExpProvide+0.0f)/needData.exp_need;
				float money = GetStrengthenMoney(equipmentInfo.strengthenLevel,curLvel+addLevel,totalExpProvide);
				int totalMoney = (int)(money*selectMaterialCount);
				coinText.text = totalMoney.ToString();
			}

			RefreshAttribute(curLvel+addLevel,isMax);
		}
		private float GetStrengthenMoney(int curlevel,int nextLevel,int expTotal){
			EquipmentInfo equipmentInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo ;
			int expTempTotal = expTotal;
			float moneyTotal = 0;
			float partMoney = 0;
			int goldNeed = 0;
			float addExpPercent = 0;
			float totalExpPercent = 0;
			if(expTotal!= 0){
				for(int i = curlevel;i<=nextLevel;i++){
					EquipmentStrengthenNeedData needData = EquipmentStrengthenNeedData.GetStrengthenNeedDataByLv(i);
					if(needData == null){
						
						needData = EquipmentStrengthenNeedData.LastNeedData();
					}
					goldNeed = needData.GetGoldNeedByStar(equipmentInfo.equipmentData.star);
					if(i == nextLevel){
						partMoney = (expTempTotal+0.0f)/expTotal*goldNeed;
						moneyTotal += partMoney;
						addExpPercent = (expTempTotal+0.0f)/needData.exp_need;
						totalExpPercent += addExpPercent;
					}else if(i == curlevel){
						int exp = needData.exp_need-equipmentInfo.strengthenExp;

						partMoney = (exp+0.0f)/expTotal*goldNeed;
						moneyTotal += partMoney;
						expTempTotal -= exp;
						addExpPercent = (exp+0.0f)/needData.exp_need;
						totalExpPercent += addExpPercent;
					}else{
						partMoney = (needData.exp_need+0.0f)/expTotal*goldNeed;
						moneyTotal += partMoney;
						expTempTotal -= needData.exp_need;
						totalExpPercent += 1;
					}
				}
				
			}
			addExpText.text = string.Format("+{0}% EXP",(int)(totalExpPercent*100));
			return moneyTotal;
		}
		private void RefreshAttribute(int nextLevel,bool isMax)
		{
			EquipmentInfo equipmentInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo ;

			TransformUtil.ClearChildren(attribute_root,true);
			//current
			List<EquipmentAttribute> attributes = equipmentInfo.EquipAttribute;
			int count = attributes.Count;
			//next
			EquipmentInfo nextEquipInfo = new EquipmentInfo(equipmentInfo);
			nextEquipInfo.strengthenLevel = nextLevel;
			List<EquipmentAttribute> nextAttributes = nextEquipInfo.EquipAttribute;

			attributeViewPrefab.gameObject.SetActive(true);
			for(int i = 0;i<count;i++){
				GameObject go = Instantiate<GameObject>(attributeViewPrefab.gameObject);
				go.transform.SetParent(attribute_root,false);
				EquipAttributeView attrView = go.GetComponent<EquipAttributeView>();
				int add = (int)(nextAttributes[i].value-attributes[i].value);
				if(isMax)
				{
					attrView.Set(attributes[i]);
				}else {
					if(i == 0)
						attrView.Set(attributes[i],add,false);
					else
						attrView.Set(attributes[i]);
				}

			}
			attributeViewPrefab.gameObject.SetActive(false);
		}
		private void RefreshEquipmentTable()
		{
			scrollContent.RefreshAllContentItems();

		}
		public void RefreshMaterialEquipment ()
		{
			EquipmentInfo[] materials = EquipmentStrengthenProxy.instance.materialsEquipInfo;
			int selectedEquipmentCount = materials.Length;
			for (int i = 0; i < selectedEquipmentCount; i++)
			{
				TransformUtil.ClearChildren( selectedMaterialRootTran[i],true);
				if (materials[i] != null)
				{
					CommonEquipmentIcon equipmentIconButton = CommonEquipmentIcon.CreateSmallIcon(selectedMaterialRootTran[i]);
					equipmentIconButton.SetEquipmentInfo(materials[i]);
					equipmentIconButton.GetEquipmentDesButton().SetDelay(0.3f);
					equipmentIconButton.SetSelect(true);
					equipmentIconButton.HideSelect();
					equipmentIconButton.onClickHandler = ClickEquipmentIconButton;
				}

			}
		}

		private void OnStrengthenSuccess(){
			EquipmentStrengthenProxy.instance.ClearMaterials();
			_currentEquipIcon.Refresh();
			RegenerateEquipmentTable();
			Refresh();
		}

		#region ui event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

	
		public void ClickEquipmentIconButton(CommonEquipmentIcon icon){

			bool refresh = false;
			if(icon.isSelect){
				refresh = EquipmentStrengthenProxy.instance.RemoveMaterialEquipInfo(icon.EquipmentInfo.instanceID);
			}else{
				if(_isReachMaxLevel){
					CommonAutoDestroyTipsView.Open(Localization.Get("ui.equipment_strengthen_view.maxExp"));
					return;
				}

				refresh = EquipmentStrengthenProxy.instance.AddMaterialEquipInfo(icon.EquipmentInfo);
			}
			if(refresh)
				Refresh();
		}
		public void ClickDropDownSort(int index){

			RegenerateEquipmentTable();
			EquipmentStrengthenProxy.instance.ClearMaterials();
			Refresh();
		}
		public void ClickStartStrengthen ()
		{
			EquipmentInfo equipInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo;
			EquipmentInfo[] materials = EquipmentStrengthenProxy.instance.materialsEquipInfo;
			List<int> selectedMaterialIDList = new List<int>();
			int count = materials.Length;
			int selectCount = 0;
			bool hasHighStrengthenLevel = false;
			bool hasHighStar = false;
			for (int i = 0; i < count; i++)
			{
				if (materials[i] != null)
				{
					selectedMaterialIDList.Add(materials[i].instanceID);
					selectCount++;
					if(materials[i].strengthenLevel >= GlobalData.GetGlobalData().equipStrengthenLevelNotice)
					{
						hasHighStrengthenLevel = true;
					}
					if (materials[i].equipmentData.star >= 4)
					{
						hasHighStar = true;
					}
				}
			}
			int money = GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Gold);
			int needMoney = coinText.text.ToInt32();
			if(EquipmentStrengthenNeedData.IsMaxLevel(equipInfo.strengthenLevel))
			{
				CommonErrorTipsView.Open(Localization.Get("ui.hero_strengthen_view.arrival_max_level"));
				return;
			}
			if(selectCount == 0)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.hero_strengthen_view.material_not_enough"));
				return;
			}
			if(money<needMoney)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.hero_strengthen_view.gold_not enough"));
				return;
			}
			if(hasHighStar)
			{
				string confirmTipsStr = Localization.Get("ui.equipment_strengthen_view.strengthen_materials_contains_high_star_equipment");
				Logic.UI.Tips.View.ConfirmTipsView.Open(confirmTipsStr, ConfirmStrengthen);
				return;
			}
			if (hasHighStrengthenLevel)
			{
				string confirmTipsStr = string.Format(Localization.Get("ui.equipment_strengthen_view.strengthen_materials_contains_high_strengthen_level_equipment"), GlobalData.GetGlobalData().equipStrengthenLevelNotice);
				Logic.UI.Tips.View.ConfirmTipsView.Open(confirmTipsStr, ConfirmStrengthen);
				return;
			}
			ConfirmStrengthen();
		}

		private void ConfirmStrengthen ()
		{
			EquipmentInfo equipInfo = EquipmentStrengthenProxy.instance.StrengthenEquipInfo;
			EquipmentInfo[] materials = EquipmentStrengthenProxy.instance.materialsEquipInfo;
			List<int> selectedMaterialIDList = new List<int>();
			int count = materials.Length;
			for (int i = 0; i < count; i++)
			{
				if (materials[i] != null)
				{
					selectedMaterialIDList.Add(materials[i].instanceID);
				}
			}
			EquipmentController.instance.CLIENT2LOBBY_EQUIP_AGGR_REQ(equipInfo.instanceID, selectedMaterialIDList);
		}

		public void OnResetScrollItemHandler(GameObject go,int index)
		{
			CommonEquipmentIcon equipButton = go.GetComponent<CommonEquipmentIcon>();
			if(equipButton!= null)
			{
				EquipmentInfo info = EquipmentStrengthenProxy.instance.currentEqupmentInfoList[index];
				equipButton.SetEquipmentInfo(info);
				equipButton.GetEquipmentDesButton().SetDelay(0.3f);
				List<EquipmentInfo> materialList = EquipmentStrengthenProxy.instance.GetMaterialEquipInfoList();
				if(materialList.Contains(info))
				{
					equipButton.SetSelect(true);
				}else {
					equipButton.SetSelect(false);
				}
				//equipButton.transform.localScale = new Vector3(0.88f,0.88f,1);
				equipButton.onClickHandler = ClickEquipmentIconButton;
			}
		}
		#endregion
	}
}
