using UnityEngine;

namespace Logic.UI.CommonAnimations
{
	public class CommonTopBarAnimation : MonoBehaviour
	{
		public RectTransform rectTransform;
		public Vector2 initialAnchoredPosition;
		public float time = 0.2f;
		public float delay = 0.1f;

		void Awake ()
		{
			if(rectTransform == null)
				rectTransform = GetComponent<RectTransform>();
			rectTransform.anchoredPosition = initialAnchoredPosition;
			StartAction();
		}

		void Start ()
		{

		}
		public void StartAction()
		{
			rectTransform.anchoredPosition = initialAnchoredPosition;
			LTDescr ltDescr = LeanTween.value(gameObject, initialAnchoredPosition, Vector2.zero, time);
			ltDescr.tweenType = LeanTweenType.easeInOutSine;
			ltDescr.setDelay(delay);
			ltDescr.setOnUpdateVector2(OnUpdateAnchoredPosition);
		}
		private void OnUpdateAnchoredPosition (Vector2 anchoredPosition)
		{
			rectTransform.anchoredPosition = anchoredPosition;
		}
	}
}
