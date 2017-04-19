using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Enums;
using Logic.Equipment.Model;

namespace Logic.UI.Equipments.View{
	public class EquipAttributeView : MonoBehaviour {
		
		public Text text_title;
		public Text text_value;
		public Text text_add;
		
		public void Set(EquipmentAttribute curAttri,int add = 0,bool ignoreAdd = true)
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
		public void Set(string name,string value,string add = "")
		{
			if(text_title != null)
				text_title.text = name;
			
			if(text_value != null)
				text_value.text = value;
			
			if(text_add != null)
				text_add.text = add;
		}

		public void SetNameColor(Color color)
		{
			if(text_title != null)
				text_title.color = color;
		}
		public void SetValueColor(Color color)
		{
			if(text_value != null)
				text_value.color = color;
		}
		public void SetAddColor(Color color)
		{
			if(text_add != null)
				text_add.color = color;
		}
	}
}

