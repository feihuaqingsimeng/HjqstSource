using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CommonFadeInAnimation : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		public float time = 0.2f;
		public float delay = 0;

		public static CommonFadeInAnimation Get(GameObject go) {
			CommonFadeInAnimation fade = go.GetComponent<CommonFadeInAnimation>();
			if (fade == null) {
				fade = go.AddComponent<CommonFadeInAnimation>();
			}
			return fade;
		}

		void Awake ()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0f;
		}

		void Start ()
		{
			StartAction();
		}
		public void StartAction(){
			_canvasGroup.alpha = 0f;
			LTDescr ltDescr = LeanTween.value(gameObject, 0, 1, time);
			ltDescr.setDelay(delay);
			ltDescr.setEase(LeanTweenType.easeInOutSine);
			ltDescr.setOnUpdate(OnUpdateFloat);
			ltDescr.setIgnoreTimeScale(true);
		}
		public void set(float time,float delay){
			this.time = time;
			this.delay = delay;
			_canvasGroup.alpha = 0f;
		}
		public void Init(float time,float delay = 0)
		{
			set(time,delay);
		}

		public void OnUpdateFloat (float alpha)
		{
			_canvasGroup.alpha = alpha;
		}
	}
}
