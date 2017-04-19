using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using System.Collections;
using Logic.UI.CommonAnimations;

namespace Logic.UI.Tips.View
{
	public class CommonAutoDestroyTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/common_auto_destroy_tips_view";

		public static CommonAutoDestroyTipsView Open(string tipsString){
			CommonAutoDestroyTipsView tips = UIMgr.instance.Open<CommonAutoDestroyTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
			tips.SetTips(tipsString);
			return tips;
		}
		public Text tipsText;
		private float _delay = 0.2f;

		public void SetTips (string tipsString)
		{
			tipsText.text = tipsString;
			StartAction();
		}
		void Start(){

		}
		private void StartAction()
		{
			LeanTween.cancel(gameObject);
			CommonFadeToAnimation fadeTo = CommonFadeToAnimation.Get(gameObject);
			fadeTo.init(0,1,_delay,0);
			StopCoroutine("DestroyDelayCoroutine");
			StartCoroutine("DestroyDelayCoroutine");
		}
		private IEnumerator DestroyDelayCoroutine(){
			yield return new WaitForSeconds(1.5f);

			CommonFadeToAnimation fadeTo = CommonFadeToAnimation.Get(gameObject);
			fadeTo.init(1,0,_delay,0);
			yield return new WaitForSeconds(_delay);

			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}