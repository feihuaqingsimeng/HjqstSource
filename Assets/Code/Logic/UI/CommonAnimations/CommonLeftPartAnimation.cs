using UnityEngine;

namespace Logic.UI.CommonAnimations
{
	public class CommonLeftPartAnimation : MonoBehaviour
	{
		public float time = 0.2f;
		public float delay = 0.05f;

		public bool useAbsolutePos;

		void Awake()
		{
			StartAction();
		}
		void Start ()
		{

		}
		public void StartAction()
		{
			Vector3 fromLocalPosition = Vector3.zero;
			if(useAbsolutePos)
			{
				fromLocalPosition = transform.localPosition - new Vector3(1136/2,0,0);
			}else
			{
				fromLocalPosition = transform.localPosition - new Vector3((transform as RectTransform).sizeDelta.x, 0, 0);
			}

			Vector3 toLocalPosition = transform.localPosition;
			transform.localPosition = fromLocalPosition;
			LeanTween.moveLocalX(gameObject, toLocalPosition.x, time).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
	}
}
