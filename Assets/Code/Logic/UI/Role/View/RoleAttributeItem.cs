using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Enums;

namespace Logic.UI.Role.View
{
	public class RoleAttributeItem : MonoBehaviour
	{
		private RoleAttributeValueType _roleAttributeValueType = RoleAttributeValueType.RealValue;

		public Text titleText;
		public Text valueText;
		public Text addValueText;

		public void SetRoleAttributeValueType (RoleAttributeValueType roleAttributeValueType)
		{
			_roleAttributeValueType = roleAttributeValueType;
		}

		public void SetTitle (string title)
		{
			titleText.text = title;
		}

		public void SetValue (int value)
		{
			if (_roleAttributeValueType == RoleAttributeValueType.RealValue)
			{
				valueText.text = value.ToString();
			}
			else
			{
				valueText.text = string.Format(Localization.Get("common.percent"), value);
			}
		}

		public void SetAddValue (int addValue)
		{
			if (_roleAttributeValueType == RoleAttributeValueType.RealValue)
			{
				addValueText.text = addValue == 0 ? string.Empty : string.Format(Localization.Get("common.attribute_add_by_equipment"), addValue);
			}
			else
			{
				addValueText.text = addValue == 0 ? string.Empty : string.Format(Localization.Get("common.attribute_add_by_equipment"), string.Format(Localization.Get("common.percent"), addValue));
			}
		}
	}
}
