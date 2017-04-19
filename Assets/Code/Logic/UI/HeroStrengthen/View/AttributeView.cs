using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Hero.Model;
using Logic.Enums;

public class AttributeView : MonoBehaviour {

	public Text text_title;
	public Text text_value;
	public Text text_add;

	public void Set(RoleAttribute curAttri,int add,bool ignoreAdd = false){
		text_title.text = curAttri.Name;
		text_value.text = curAttri.ValueString;
		if(ignoreAdd){
			text_add.text = string.Empty;
		}else{
			text_add.text = string.Format("(+{0})",add);
		}
	}

	public void Set(RoleAttribute curAttri){
		Set(curAttri,0,true);
	}

	public void Set(RoleAttributeType type,int cur,int add){

		RoleAttribute attri = new RoleAttribute(type,cur);
		Set(attri,add,false);
	}
	public void Set(RoleAttributeType type,int cur){
		RoleAttribute attri = new RoleAttribute(type,cur);
		Set(attri,0,true);
	}
}
