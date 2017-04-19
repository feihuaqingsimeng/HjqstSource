using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Localization;
using Logic.Equipment;
using Logic.Equipment.Model;
using Logic.UI.EquipmentsStrengthen.View;
using Common.Util;
using Logic.UI.CommonEquipment.View;

namespace Logic.UI.Equipments.View
{
	public class FreeEquipmentItemButton : MonoBehaviour
	{
		private EquipmentInfo _equipmentInfo;
		public EquipmentInfo EquipmentInfo
		{
			get
			{
				return _equipmentInfo;
			}
		}

		#region UI components
		public Transform iconRoot;
		public Text equipmentNameText;
		public Transform attrRoot;
		public EquipAttributeView attrViewPrefab;


		public Text strengthenText;
		public Text sellText;
		public Image isNewEquipmentHintImage;
		#endregion

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_equipmentInfo = equipmentInfo;
			TransformUtil.ClearChildren(iconRoot,true);
			CommonEquipmentIcon icon = CommonEquipmentIcon.Create(iconRoot);
			icon.SetEquipmentInfo(_equipmentInfo);
			icon.GetEquipmentDesButton().enabled = false;
			float w = (iconRoot as RectTransform ).sizeDelta.x;
			float s = w/icon.rectTransform.sizeDelta.x;
			icon.transform.localScale = new Vector3(s,s,s);

			equipmentNameText.text = Localization.Get(equipmentInfo.equipmentData.name);

			attrViewPrefab.gameObject.SetActive(true);
			TransformUtil.ClearChildren(attrRoot,true);
			for(int i = 0,count = _equipmentInfo.EquipAttribute.Count;i<count;i++)
			{
				EquipAttributeView view = Instantiate<EquipAttributeView>(attrViewPrefab);
				view.transform.SetParent(attrRoot,false);
				view.Set(_equipmentInfo.EquipAttribute[i]);

			}
			attrViewPrefab.gameObject.SetActive(false);


			strengthenText.text = Localization.Get("ui.equipments_browse_view.training");
			sellText.text = Localization.Get("ui.equipments_browse_view.sell");
			isNewEquipmentHintImage.gameObject.SetActive(EquipmentProxy.instance.IsNewEquipment(_equipmentInfo.instanceID));
		}

		#region UI event handlers
		public void ClickHandler ()
		{
//			EquipmentStrengthenView equipmentStrengthenView = UIMgr.instance.Open<EquipmentStrengthenView>(EquipmentStrengthenView.PREFAB_PATH);
//			equipmentStrengthenView.SetEquipmentInfo(_equipmentInfo);
		}

		public void ClickStrengthenHandler ()
		{
			if(!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.EquipTraining,true))
			{
				return;
			}
			EquipmentProxy.instance.SetEquipmentAsChecked(_equipmentInfo.instanceID);
			//EquipmentStrengthenView equipmentStrengthenView = UIMgr.instance.Open<EquipmentStrengthenView>(EquipmentStrengthenView.PREFAB_PATH);
			//equipmentStrengthenView.SetEquipmentInfo(_equipmentInfo);
			EquipmentProxy.EquipModelLuaTable.GetLuaFunction("OpenTrainingViewByBag").Call(_equipmentInfo.instanceID);
		}

		public void ClickSellHandler ()
		{
		
			EquipmentProxy.instance.SetEquipmentAsChecked(_equipmentInfo.instanceID);
			ConfirmSellEquipmentView.Open(_equipmentInfo);
		}
		#endregion
	}
}
