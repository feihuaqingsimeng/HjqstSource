using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	public class CommonPunchInAnimation : MonoBehaviour
	{
		RectTransform _rectTransform;

		void Awake ()
		{
			_rectTransform = GetComponent<RectTransform>();
			if (_rectTransform == null)
			{
				Debugger.LogError("can't find RectTransform component on " + gameObject.name);
				return;
			}
			_rectTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
		}

		// Use this for initialization
		void Start ()
		{
			if (_rectTransform == null)
			{
				return;
			}
			LTDescr ltDescr = LeanTween.scale(_rectTransform, new Vector3(1f, 1f, 1f), 0.3f);
			ltDescr.setEase(LeanTweenType.easeSpring);
		}
	}
}
