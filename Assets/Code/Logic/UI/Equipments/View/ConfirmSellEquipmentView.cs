using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Equipment.Model;
using Logic.Equipment.Controller;
using Logic.UI.CommonEquipment.View;
using Common.Util;

namespace Logic.UI.Equipments.View
{
	public class ConfirmSellEquipmentView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/equipments/confirm_sell_equipment_view";

		private EquipmentInfo _equipmentInfo;

		#region UI components
		public Text titleText;
		public Transform equipIconRoot;
		public EquipAttributeView attrViewPrefab;
		public Transform attrRoot;

		public Text equipmentNameText;
		public Text priceTitleText;
		public Text priceText;
		public Image priceResourceIconImage;
		public Text sellText;
		public Text cancelText;
		#endregion UI components

		void Start ()
		{
			titleText.text = Localization.Get("ui.confirm_sell_equipment_view.title");
			priceTitleText.text = Localization.Get("ui.confirm_sell_equipment_view.price_title");
			sellText.text = Localization.Get("ui.confirm_sell_equipment_view.sell");
			cancelText.text = Localization.Get("ui.confirm_sell_equipment_view.cancel");
		}

		public static ConfirmSellEquipmentView Open(EquipmentInfo equipmentInfo)
		{
			ConfirmSellEquipmentView confirmSellEquipmentView = UIMgr.instance.Open<ConfirmSellEquipmentView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
			confirmSellEquipmentView.SetEquipmentInfo(equipmentInfo);
			return confirmSellEquipmentView;
		}

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_equipmentInfo = equipmentInfo;
			TransformUtil.ClearChildren(equipIconRoot,true);
			CommonEquipmentIcon icon = CommonEquipmentIcon.Create(equipIconRoot);
			icon.SetEquipmentInfo(_equipmentInfo);
			icon.GetEquipmentDesButton().enabled = false;
			icon.ButtonEnable(false);



			equipmentNameText.text = Localization.Get(_equipmentInfo.equipmentData.name);

			attrViewPrefab.gameObject.SetActive(true);
			for(int i = 0,count = equipmentInfo.EquipAttribute.Count;i<count;i++)
			{
				EquipAttributeView v = Instantiate<EquipAttributeView>(attrViewPrefab);
				v.transform.SetParent(attrRoot,false);
				v.Set(equipmentInfo.EquipAttribute[i]);
			}
			attrViewPrefab.gameObject.SetActive(false);
			priceText.text = _equipmentInfo.equipmentData.price.ToString();
		}

		#region UI event handlers
		public void ClickSellHandler ()
		{
			EquipmentController.instance.CLIENT2LOBBY_EQUIP_SELL_REQ(_equipmentInfo.instanceID);
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickCancelHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion UI event handlers
	}
}
