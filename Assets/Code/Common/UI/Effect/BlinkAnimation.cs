using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Effect
{
	public class BlinkAnimation : MonoBehaviour
	{
		private Image _image;

		public float showDuration = 0.5f;
		public float hideDuration = 0.5f;

		void Awake ()
		{
			_image = GetComponent<Image>();
			Show();
		}

		private void Show ()
		{
			if (_image != null)
			{
				_image.CrossFadeAlpha(1, 0, true);
				LeanTween.delayedCall(showDuration, Hide).setIgnoreTimeScale(true);
			}
		}

		private void Hide ()
		{
			if (_image != null)
			{
				_image.CrossFadeAlpha(0, 0, true);
				LeanTween.delayedCall(hideDuration, Show).setIgnoreTimeScale(true);
			}
		}
	}
}
