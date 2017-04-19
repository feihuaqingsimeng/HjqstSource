using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	public class CommonScaleUpAnimation : MonoBehaviour
	{
		public float time = 0.2f;
		public float delay = 0.15f;

		void Start ()
		{
			Execute();
		}

		public void Execute ()
		{
			transform.localScale = Vector3.zero;
			LeanTween.scale(gameObject, Vector3.one, time).setEase(LeanTweenType.easeOutBack).setDelay(delay);
		}
	}
}
