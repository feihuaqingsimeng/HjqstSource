using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Equipment;
using Logic.Equipment.Model;
using Logic.Equipment.Controller;

namespace Logic.UI.Tips.View
{
	public class ConfirmPutOffEquipmentTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/confirm_put_off_equipment_tips_view";

		private GameResData _costGameResData;
		private EquipmentInfo _equipmentInfo;
		private int _roleInstanceID;

		public Text tipsText;
		public Image costResourceImage;
		public Text costResourceCountText;
		public Text cancelText;
		public Text okText;

		public static ConfirmPutOffEquipmentTipsView Open (int equipID, int roleID, BaseResType costBaseResType, int costResID, int costResCount)
		{
			ConfirmPutOffEquipmentTipsView confirmPutOffEquipmentTipsView = UIMgr.instance.Open<ConfirmPutOffEquipmentTipsView>(PREFAB_PATH, EUISortingLayer.Tips, UIOpenMode.Overlay);
			GameResData costGameResData = new GameResData(costBaseResType, costResID, costResCount, 0);
			confirmPutOffEquipmentTipsView.SetInfo(EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipID), roleID, costGameResData);
			return confirmPutOffEquipmentTipsView;
		}

		public void Awake ()
		{
			tipsText.text = Localization.Get("ui.confirm_put_off_equipment_tips_view.tips");
			cancelText.text = Localization.Get("ui.confirm_put_off_equipment_tips_view.cancel");
			okText.text = Localization.Get("ui.confirm_put_off_equipment_tips_view.ok");
		}

		public void SetInfo (EquipmentInfo equipmentInfo, int roleInstanceID, GameResData costGameResData)
		{
			_equipmentInfo = equipmentInfo;
			_roleInstanceID = roleInstanceID;
			costResourceImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type)));
			costResourceCountText.text = costGameResData.count.ToString();
		}

		#region UI event handlers
		public void ClickCancleHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickOKHandler ()
		{
			GameResData costGameResData = EquipmentUtil.CalculatePutOffCost(_equipmentInfo);
			if (GameProxy.instance.BaseResourceDictionary[costGameResData.type] >= costGameResData.count)
			{
				EquipmentController.instance.CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ(_equipmentInfo.instanceID, EquipmentWearOffType.Off, false, _roleInstanceID);
				UIMgr.instance.Close(PREFAB_PATH);
			}
			else
			{
				CommonErrorTipsView.Open("Not enough diamond.");
			}
		}
		#endregion UI event handlers
	}
}