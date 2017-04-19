using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CommonFadeToAnimation : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		public float time = 1f;
		public float delay = 0f;
        public float fromAlpha = 1f;
        public float toAlpha = 0f;


        public System.Action onComplete;

        public static CommonFadeToAnimation Get(GameObject go) {
            CommonFadeToAnimation fadeto = go.GetComponent<CommonFadeToAnimation>();
            if (fadeto == null) {
                fadeto = go.AddComponent<CommonFadeToAnimation>();
            }
            return fadeto;
        }

		void Awake ()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}
        public void init(float from = 1f, float to = 0f, float time = 1f, float delay = 0f) {
            fromAlpha = from;
            toAlpha = to;
            this.time = time;
            this.delay = delay;

            startAction();
        }
        public void setComplete(System.Action completeAction) {
            onComplete = completeAction;
        }
		void Start ()
		{
           
		}
        public void startAction(){
            _canvasGroup.alpha = fromAlpha;
            LTDescr ltDescr = LeanTween.value(gameObject, fromAlpha, toAlpha, time);
			ltDescr.setDelay(delay);
			ltDescr.setEase(LeanTweenType.easeInOutSine);
			ltDescr.setOnUpdate(OnUpdateFloat);
        }
		public void OnUpdateFloat (float alpha)
		{
			_canvasGroup.alpha = alpha;
            if (alpha == toAlpha && onComplete != null)
            {
				Debugger.Log("complete fade to");
                onComplete();
              
            }
                
		}
	}
}
