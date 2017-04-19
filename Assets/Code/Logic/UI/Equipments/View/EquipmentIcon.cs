using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using System.Collections.Generic;
using Logic.Equipment;
using Logic.Equipment.Model;

namespace Logic.UI.Equipments.View
{
	public class EquipmentIcon : MonoBehaviour
	{
		public Image equipmentIconImage;

		public void SetEquipmentInfo (EquipmentInfo equipmentInfo)
		{
			equipmentIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetEquipmentIconPath(equipmentInfo.equipmentData.icon)));
		}
	}
}
