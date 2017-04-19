using UnityEngine;
using System.Collections;
using Logic.UI.CommonAnimations;
namespace Logic.UI.Shop.View
{
	public class WhiteFrameFadeInOutAnimation : MonoBehaviour 
	{
		private float fadeInTime = 0.2f;
		private float FadeOutTime = 0.2f;
		private float DuringTime = 0.1f;


		public void StartAction()
		{
			StartCoroutine( StartActionCoroutine());

		}
		private IEnumerator StartActionCoroutine()
		{
			CommonFadeToAnimation fadeIn = CommonFadeToAnimation.Get(gameObject);
			fadeIn.init(0,1,fadeInTime);
			yield return new WaitForSeconds(fadeInTime+DuringTime);
			CommonFadeToAnimation fadeOut = CommonFadeToAnimation.Get(gameObject);
			fadeOut.init(1,0,FadeOutTime);
		}
		
	}

}
