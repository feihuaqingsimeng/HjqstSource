using UnityEngine;
using System.Collections;
using Logic.Enums;
using Common.Localization;

namespace Logic.Equipment.Model
{
	public class EquipmentAttribute
	{
		
		public EquipmentAttributeType type;
		private float _value;

		public float value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public EquipmentAttribute ()
		{
		}

		public EquipmentAttribute (EquipmentAttributeType type, float value)
		{
			this.type = type;
			this.value = value;
		}

		public string ValueString
		{
			get
			{
				if (IsPercent ())
				{
					return string.Format ("{0:F1}%", value*100);
				}
				else
				{
					return ((int)value).ToString ();
				}
			}
		}
		//属性名
		public string Name
		{
			get
			{

				return Localization.Get (string.Format ("attribute_des_{0}", (int)type));

			}
		}

		public bool IsPercent ()
		{
			switch (type)
			{
				case EquipmentAttributeType.HP:
				case EquipmentAttributeType.NormalAtk:
				case EquipmentAttributeType.MagicAtk:
				case EquipmentAttributeType.Def:
				case EquipmentAttributeType.Speed:
					return false;
				default:
					return true;
			}
		}
	}
}

