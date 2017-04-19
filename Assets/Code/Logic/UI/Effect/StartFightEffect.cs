using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Logic.UI.Effect
{
	public class StartFightEffect : MonoBehaviour 
	{
		public GameObject rootPanel;
		public Transform bgTransform;
		public Transform tipTransform;
		public Transform leftArrow;
		public Transform rightArrow;
		public GameObject blackBG;


		public float bgStartScaleY = 0.5f;
		public float bgScaleTime = 0.04f;

		public float tipStartScaleX = 0.5f;
		public float tipScaleTime = 0.04f;

		public float arrowTranslate = 3;
		public float arrowTranslateTime = 0.04f;

		public float duringTime = 0.8f;

		public float disappearTime = 0.04f;

		public bool refresh;

		private Vector3 _leftArrowOriginPos;
		private Vector3 _rightArrowOriginPos;
		void  Awake()
		{
			gameObject.GetComponent<Canvas> ().worldCamera = UIMgr.instance.uiCamera;
			_leftArrowOriginPos = leftArrow.localPosition;
			_rightArrowOriginPos = rightArrow.localPosition;
			Refresh();
		}
		private void Refresh()
		{
			rootPanel.SetActive(false);
			if(blackBG!= null)
				blackBG.SetActive(true);
			StartCoroutine(RefreshCoroutine());
		}

		private IEnumerator RefreshCoroutine()
		{
			rootPanel.SetActive(true);
			rootPanel.transform.localScale = Vector3.one;

			tipTransform.gameObject.SetActive(false);

			bgTransform.localScale  = new Vector3(1,bgStartScaleY,1);
			LeanTween.scaleY(bgTransform.gameObject,1,bgScaleTime);
			yield return new WaitForSeconds(0.066f);

			tipTransform.gameObject.SetActive(true);
			leftArrow.localPosition = _leftArrowOriginPos;
			rightArrow.localPosition = _rightArrowOriginPos;
			tipTransform.localScale = new Vector3(tipStartScaleX,1,1);
			LeanTween.scaleX(tipTransform.gameObject,1,tipScaleTime).onComplete = tipScaleComplete;
			yield return new WaitForSeconds(duringTime);

			LeanTween.scaleY(rootPanel,0,disappearTime).onComplete = disappearComplete;
		}
		private void tipScaleComplete()
		{
			LeanTween.moveLocalX(leftArrow.gameObject,_leftArrowOriginPos.x-arrowTranslate,arrowTranslateTime);
			LeanTween.moveLocalX(rightArrow.gameObject,_rightArrowOriginPos.x+arrowTranslate,arrowTranslateTime);
		}
		private void disappearComplete()
		{
			if(blackBG!=null)
				blackBG.SetActive(false);
		}
		void Update()
		{
			if(refresh)
			{
				refresh = false;
				Refresh();
			}
		}
	}
}

