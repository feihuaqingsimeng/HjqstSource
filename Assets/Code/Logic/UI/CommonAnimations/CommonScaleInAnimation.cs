using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	public class CommonScaleInAnimation : MonoBehaviour 
	{
		
		public static CommonScaleInAnimation Get(GameObject go) {
			CommonScaleInAnimation scaleIn = go.GetComponent<CommonScaleInAnimation>();
			if (scaleIn == null) {
				scaleIn = go.AddComponent<CommonScaleInAnimation>();
			}
			return scaleIn;
		}


		public float time;
		public float delay;
		public Vector3 startScale;
		public Vector3 endScale;
		void Start () 
		{
			StartCoroutine(StartActionCoroutine());
		}

		public void Set(float time ,float delay,Vector3 startScale,Vector3 endScale)
		{
			this.time = time;
			this.delay = delay;
			this.startScale = startScale;
			this.endScale = endScale;
			transform.localScale = startScale;
		}

		private IEnumerator StartActionCoroutine()
		{
			transform.localScale = startScale;
			yield return new WaitForSeconds(delay);
			LeanTween.scale(gameObject,endScale,time);
		}

	}

}
