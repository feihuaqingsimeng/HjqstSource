using UnityEngine;
using UnityEngine.UI;

namespace Logic.UI.HealthAdvice.View
{
	public class HealthAdviceView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/health_advice/health_advice_view";

        public Text healthAdviceText;
        public Text copyrightText;
		public float crossFadeDelay;
		public float crossFadeDuration;

		public delegate void OnTimeOutDelegate ();
		public OnTimeOutDelegate onTimeOutDelegate;

		public static HealthAdviceView Open ()
        {
			return UIMgr.instance.Open<HealthAdviceView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
		}

		public void SetOnTimeOutDelegate (OnTimeOutDelegate onTimeOutDelegate)
		{
			this.onTimeOutDelegate = onTimeOutDelegate;
		}

		void Awake ()
		{
			LTDescr ltDescr = LeanTween.delayedCall(crossFadeDelay + crossFadeDuration, OnTimeOut);
			ltDescr.setIgnoreTimeScale(true);
			LTDescr crossFadeLTDescr = LeanTween.delayedCall(crossFadeDelay, StartCrossFade);
			crossFadeLTDescr.setIgnoreTimeScale(true);
		}

		private void StartCrossFade ()
		{
            healthAdviceText.CrossFadeAlpha(0, crossFadeDuration, true);
            copyrightText.CrossFadeAlpha(0, crossFadeDuration, true);
		}

		private void OnTimeOut ()
		{
			if (onTimeOutDelegate != null)
				onTimeOutDelegate();
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}
