using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Equipment.Model;

namespace Logic.UI.Equipments.View
{
	public class EquipmentIconButton : MonoBehaviour
	{
		private EquipmentInfo _equipmentInfo;
		public EquipmentInfo EquipmentInfo
		{
			get
			{
				return _equipmentInfo;
			}
		}

		public bool isSelected = false;

		public Image equipmentIconImage;
		public Image selectedMarkImage;
		public Image[] starImages;

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{
			SetIsSelected(false);
		}

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			_equipmentInfo = equipmentInfo;
			equipmentIconImage.SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetEquipmentIconPath(equipmentInfo.equipmentData.icon)));
		}

		public void SetIsSelected (bool isSelected)
		{
			this.isSelected = isSelected;
			selectedMarkImage.gameObject.SetActive(this.isSelected);
		}
	}
}