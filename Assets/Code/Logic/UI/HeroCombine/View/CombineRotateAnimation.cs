using UnityEngine;
using System.Collections;

namespace Logic.UI.HeroCombine.View{

	public class CombineRotateAnimation : MonoBehaviour {


		public float moveTime = 1;
		public float moveDelay = 1;
		public float rotateTime = 2f;
		public float rotateDelay = 0f;

		public bool reset;

		private Vector3 _localPosition;
		Vector3 _zAxis = new Vector3( 0,0,-1);

		void Awake(){
			_localPosition = transform.localPosition;
		}
		void Update(){
			if(reset){
				reset = false;
				init ();
			}
		}
		public void init(){
			transform.localPosition = _localPosition;

			LeanTween.rotateAroundLocal(transform.parent.gameObject, _zAxis, 360, rotateTime).setEase(LeanTweenType.easeInOutQuint).setDelay(rotateDelay);
			LeanTween.rotateAroundLocal(gameObject, -_zAxis, 360, rotateTime).setEase(LeanTweenType.easeInOutQuint).setDelay(rotateDelay);
			LeanTween.moveLocal(gameObject, Vector3.zero, moveTime).setDelay(moveDelay);
			Logic.UI.CommonAnimations.CommonFadeToAnimation fadeto = Logic.UI.CommonAnimations.CommonFadeToAnimation.Get(gameObject);
			fadeto.init(1f, 0, moveTime, moveDelay);
		}
		public void ResetPosition(){
			transform.localPosition = _localPosition;
			Logic.UI.CommonAnimations.CommonFadeToAnimation ani = Logic.UI.CommonAnimations.CommonFadeToAnimation.Get(gameObject);
			ani.init(0, 1, 0.1f, 0);
		}
	}
}

