using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic.UI.CommonEffect
{
	[RequireComponent(typeof(Animator))]
	public class CommonButtonStateEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private Animator _animator;

		public string normalAnimationName = "Normal";
		public string pressdAnimationName = "Pressed";

		void Awake ()
		{
			_animator = GetComponent<Animator>();
		}

		public void OnPointerDown (PointerEventData eventData)
		{
			Button b = gameObject.GetComponent<Button>();
			if(b!=null && b.interactable)
			{
				_animator.Play(pressdAnimationName);
			}

		}

		public void OnPointerUp (PointerEventData eventData)
		{
			Button b = gameObject.GetComponent<Button>();
			if(b!=null && b.interactable)
			{
				_animator.Play(normalAnimationName);
			}
		}
	}
}
