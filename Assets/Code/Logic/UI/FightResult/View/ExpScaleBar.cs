using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Logic.UI.FightResult.View{
	public class ExpScaleBar : MonoBehaviour {
		
		#region ui
		public Image Image_Fill;
		#endregion
		
		private float _from;
		private float _to;
		private float _duringTime;


		public void ChangeValue(float value){
			if(value>1)
				value = 1;
			else if(value<0)
				value = 0;
			Vector3 scale = Image_Fill.transform.localScale;
			Image_Fill.transform.localScale = new Vector3( value,scale.y,scale.z);
		}
		public LTDescr ChangeValue(float to,float DuringTime,float delay = 0){
			if(to>1)
				to = 1;
			else if(to<0)
				to = 0;
			return LeanTween.scaleX(Image_Fill.gameObject,to,DuringTime).setDelay(delay);

		}
		public float Value{
			get{
				return Image_Fill.transform.localScale.x;
			}

		}
	}
}

