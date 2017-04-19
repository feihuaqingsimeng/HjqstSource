using UnityEngine;
using System.Collections;

namespace Logic.UI.Launch.View
{
	public class SimpleIntroductionItem : MonoBehaviour
	{
		public Animator animator;

		public delegate void OnAnimationCompleteDelegate(int index);
		private OnAnimationCompleteDelegate _onAnimationCompleteDelegate;

		public void SetOnAnimationEndCompleteDelegate (OnAnimationCompleteDelegate onAnimationEndCompleteDelegate)
		{
			_onAnimationCompleteDelegate = onAnimationEndCompleteDelegate;
		}

		void OnAnimationComplete (int index)
		{
			if (_onAnimationCompleteDelegate != null)
				_onAnimationCompleteDelegate(index);
		}
	}
}
