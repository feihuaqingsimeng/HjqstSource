using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Equipment;
using Logic.Equipment.Model;
using Common.Util;
using Logic.UI.CommonEquipment.View;

namespace Logic.UI.Equipments.View
{
	public class EquipmentItem : MonoBehaviour
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
		public RectTransform iconRoot;
		public EquipAttributeView attrViewPrefab;
		public RectTransform attrRoot;
		
		public Text equipmentNameText;

		public Button putOnButton;
		public Image newEquipmentHintIconImage;
		#endregion

		private CommonEquipmentIcon _equipIcon;

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{

		}

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_equipmentInfo = equipmentInfo;

			//icon
			_equipIcon = iconRoot.GetComponentInChildren<CommonEquipmentIcon>();
			if(_equipIcon == null)
			{
				_equipIcon = CommonEquipmentIcon.Create(iconRoot);
//				float s = iconRoot.sizeDelta.x/_equipIcon.rectTransform.sizeDelta.x;
//				_equipIcon.transform.localScale = new Vector3(s,s,s);
				_equipIcon.transform.SetAsFirstSibling();
				_equipIcon.GetEquipmentDesButton().enabled = false;
				_equipIcon.ButtonEnable(false);
			}
			_equipIcon.SetEquipmentInfo(_equipmentInfo);

			equipmentNameText.text = Localization.Get(_equipmentInfo.equipmentData.name);

			TransformUtil.ClearChildren(attrRoot,true);
			attrViewPrefab.gameObject.SetActive(true);
			for(int i = 0,count = _equipmentInfo.EquipAttribute.Count;i<count;i++)
			{
				EquipAttributeView view = Instantiate<EquipAttributeView>(attrViewPrefab);
				view.transform.SetParent(attrRoot,false);
				view.Set(equipmentInfo.EquipAttribute[i]);
			}
			attrViewPrefab.gameObject.SetActive(false);

			newEquipmentHintIconImage.gameObject.SetActive(EquipmentProxy.instance.IsNewEquipment(_equipmentInfo.instanceID));
		}
		public void SetEquipLevelColor(Color color)
		{
			if(_equipIcon != null)
				_equipIcon.SetLevelColor(color);
		}
		public void DisablePutOnButton ()
		{
			putOnButton.gameObject.SetActive(false);
		}

		public void EnablePutOnButton ()
		{
			putOnButton.gameObject.SetActive(true);
		}
	}
}
