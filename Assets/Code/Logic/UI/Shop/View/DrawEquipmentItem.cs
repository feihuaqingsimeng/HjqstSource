using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Equipment.Model;
using Logic.UI.CommonEquipment.View;
using Logic.UI.Description.View;
using Common.Util;

namespace Logic.UI.Shop.View
{
	public class DrawEquipmentItem : MonoBehaviour
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
		public Image frameImage;
		
		private Sprite _frameFrontSprite;
		private Sprite _frameBackSprite;
		#endregion

		public void Start ()
		{
			_frameFrontSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/ui_items_02");
			_frameBackSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_head_box_big_back");
			frameImage.SetSprite(_frameBackSprite);
		}
		
		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_equipmentInfo = equipmentInfo;
			frameImage.SetSprite(_frameBackSprite);

		}

		public void TurnOverAfter (float delay)
		{
			TransformUtil.ClearChildren(frameImage.transform,true);
			EquipmentDesButton.Get(gameObject).enabled = false;
			Invoke("TurnOver", delay);
		}
		
		public void TurnOver ()
		{
			LTDescr ltDescr = LeanTween.scaleX(gameObject, 0, 0.15f);
			ltDescr.setOnComplete(OnTurnOverComplete);
		}
		
		private void OnTurnOverComplete ()
		{
			frameImage.SetSprite(_frameFrontSprite);

			LeanTween.scaleX(gameObject, 1, 0.15f);
			CommonEquipmentIcon icon = CommonEquipmentIcon.Create(frameImage.transform);
			icon.SetEquipmentInfo(_equipmentInfo);

		}
	}
}
