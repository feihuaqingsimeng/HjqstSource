using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Hero.Model;
using Logic.Enums;

namespace Logic.UI.Hero.View
{
	public class RoleAttributeView : MonoBehaviour 
	{
		
		public Text text_title;
		public Text text_value;
		public Text text_add;
		
		public void Set(RoleAttribute curAttri,int add = 0,bool ignoreAdd = true)
		{
			if(text_title != null)
				text_title.text = curAttri.Name;
			if(text_value != null)
				text_value.text = curAttri.ValueString;
			if(text_add != null)
			{
				if(ignoreAdd){
					text_add.text = string.Empty;
				}else{
					text_add.text = string.Format("(+{0})",add);
				}
			}

		}

		public void Set(RoleAttribute roleAttribute, string remark)
		{
			if (text_title != null)
			{
				text_title.text = roleAttribute.Name;
			}
			if (text_value != null)
			{
				text_value.text = roleAttribute.ValueString;
			}
			if (text_add != null)
			{
				text_add.text = string.Format("({0})", remark);
			}
		}
	}
}

