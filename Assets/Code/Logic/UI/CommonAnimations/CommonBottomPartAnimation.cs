using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	public class CommonBottomPartAnimation : MonoBehaviour 
	{
		public static CommonBottomPartAnimation Get(GameObject go) {
			CommonBottomPartAnimation scaleIn = go.GetComponent<CommonBottomPartAnimation>();
			if (scaleIn == null) {
				scaleIn = go.AddComponent<CommonBottomPartAnimation>();
			}
			return scaleIn;
		}
		void Awake()
		{
			_localpos = transform.localPosition;
			StartAction();
		}
		void Start () {

		}
		public void StartAction()
		{
			transform.localPosition = _localpos-new Vector3(0, (transform as RectTransform).sizeDelta.y,0);
			StopCoroutine("StartActionCoroutine");
			StartCoroutine("StartActionCoroutine");
		}

		public void Set(float time , float delay)
		{
			this.time = time;
			this.delay = delay;
			_localpos = transform.localPosition;
			StartAction();
		}
		public float time;
		public float delay;

		private Vector3 _localpos;

		public IEnumerator StartActionCoroutine()
		{
			if(delay > 0)
				yield return new WaitForSeconds(delay);

			LeanTween.moveLocalY(gameObject,_localpos.y,time);
		}
	}
}

