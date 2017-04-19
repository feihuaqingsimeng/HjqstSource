using UnityEngine;
using System.Collections;
using Logic.Role.Model;
using Logic.Equipment.Model;

namespace Logic.UI.IllustratedHandbook.Model
{
	public class IllustrationInfo
	{
		public RoleInfo roleInfo;
		public EquipmentInfo equipInfo;

		public IllustrationInfo( RoleInfo info)
		{
			roleInfo = info;
		}
		public IllustrationInfo(EquipmentInfo info)
		{
			equipInfo = info;
		}
	}
}

