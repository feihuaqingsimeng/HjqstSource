using UnityEngine;
using System.Collections;
using Logic.Enums;
using Common.Localization;

namespace Logic.Hero.Model{

	public class RoleAttribute {
		public RoleAttributeType type;
		private float _value;

		public float value{
			set{
				_value = value;
			}
			get{
				return _value;
			}
		}

        public RoleAttribute() { }

        public RoleAttribute(RoleAttributeType type,float value){
            this.type = type;
            this.value = value;
        }

		public string ValueString{
			get{
				if(IsPercent()){
					return string.Format("{0:F1}%",value*100);
				}else{
					return ((int)value).ToString();
				}
			}
		}
		//属性名
		public string Name{
			get{
				return Localization.Get (string.Format ("attribute_des_{0}", (int)type));
			}

		}
		public bool IsPercent(){
			switch (type)
			{
			case RoleAttributeType.HP:
			case RoleAttributeType.NormalAtk:
			case RoleAttributeType.MagicAtk:
			case RoleAttributeType.Normal_Def:
			case RoleAttributeType.Speed:
				return false;
			default:
				return true;
			}
        }

	}
}

