using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.CommonAnimations;
using Logic.Audio.Controller;

namespace Logic.UI.FightResult.View{
	public class StarScaleAction : MonoBehaviour {

		#region ui
		public GameObject target;
		public GameObject rootPanelShake;
		#endregion
		public float rootPanelSakeTime = 0.006f;
		public float rootPanelSakeDistance = 6;
		public float time = 0.3f;
		public float fadeInTime = 0.2f;
		public float scaleOverflowTime = 0.07f;
		public float delay = 0.3f;
		public float moveY = -300f;
		public float moveYOverflow = 400f;
		public float scaleOverflow = 0.04f;
		public float moveX = 200;
		public float rotateZ = 90f;

		public float scale = 10;


		public bool reset;

		private GameObject _go ;
		private Transform _tran ;

		CanvasGroup _canvasGroup;
		Canvas _canvas;
		void Awake(){

			_go = target;
			_tran = target.transform;
			_canvas = GetComponent<Canvas>();
			if(_canvas == null)
			{
				Canvas parentCanvas = GetComponentInParent<Canvas>();
				_canvas = gameObject.AddComponent<Canvas>();
				_canvas.overrideSorting = true;
				_canvas.sortingOrder = parentCanvas.sortingOrder+1;
				_canvas.sortingLayerName = parentCanvas.sortingLayerName;
			}
			_canvasGroup = target.GetComponent<CanvasGroup>();
			if(_canvasGroup == null)
				_canvasGroup = target.AddComponent<CanvasGroup>();
			_canvasGroup.alpha = 0;
			_localScale = _tran.localScale;
			_localPosition = _tran.localPosition;
			_localRotate = _tran.eulerAngles;
		}

//		void Update(){
//			if(reset){
//				reset = false;
//				Init(delay);
//			}
//		}
		private Vector3 _localScale;
		private Vector3 _localPosition;
		private Vector3 _localRotate;



		public void StartAction(){

			float y = Mathf.Abs( moveYOverflow*2)+Mathf.Abs(moveY);

			LeanTween.scale(_tran as RectTransform,_localScale,time).setOnComplete(ScaleActionComplete);;
			LeanTween.moveLocalX(_go,_localPosition.x,time);
			//_perMoveYTime*(Mathf.Abs(moveY)+Mathf.Abs(moveYOverflow)
			LeanTween.moveLocalY(_go,_localPosition.y+moveYOverflow,time/2).setOnComplete(MoveYReset);
			LeanTween.rotateAroundLocal(_tran as RectTransform,Vector3.forward,rotateZ,time);
			CommonFadeInAnimation fadeIn = _go.GetComponent< CommonFadeInAnimation>();
			if(fadeIn== null)
				fadeIn = _go.AddComponent<CommonFadeInAnimation>();
			fadeIn.set (fadeInTime,0);


		}
		public void Init()
		{
			Init(delay);
		}
		public void Init(float delayTime){
			this.delay = delayTime;

			_canvasGroup.alpha = 0;

			_tran.localScale = new Vector3(scale,scale,1);
			_tran.localPosition = new Vector3(_localPosition.x+moveX,_localPosition.y+moveY,_localPosition.z);
			_tran.localEulerAngles = new Vector3(_localRotate.x,_localRotate.y,_localRotate.z-rotateZ);

			StartCoroutine(DoInit());

		}
		private IEnumerator DoInit(){
			yield return new WaitForSeconds(delay);

			StartAction();
		}
		private void MoveYReset(){
			//_perMoveYTime*Mathf.Abs(moveYOverflow)
			LeanTween.moveLocalY(_go,_localPosition.y,time/2);
		}
		private void ScaleActionComplete(){
			_canvas.overrideSorting = false;
			LeanTween.scale(_tran as RectTransform,new Vector3(_localScale.x-scaleOverflow,_localScale.y-scaleOverflow,1),scaleOverflowTime).setLoopPingPong(1);
			LeanTween.moveLocalY(rootPanelShake,rootPanelShake.transform.localPosition.y-rootPanelSakeDistance,rootPanelSakeTime).setLoopPingPong(1);
			AudioController.instance.PlayAudio(AudioController.starEvaluate_audio,false);
		}

	}
}

