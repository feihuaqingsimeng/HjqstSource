using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace Common.UI.Components
{
	[Serializable]
	public class UnityEventGameObject : UnityEvent<GameObject>
	{
		
    }
    
	public class EventTriggerDelegate : MonoBehaviour ,IPointerClickHandler,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
	{

		public UnityEventGameObject onClick = new UnityEventGameObject();
		public UnityEventGameObject onDown = new UnityEventGameObject();
		public UnityEventGameObject onEnter = new UnityEventGameObject();
		public UnityEventGameObject onExit = new UnityEventGameObject();
		public UnityEventGameObject onUp = new UnityEventGameObject();

		public void OnPointerClick(PointerEventData eventData)
		{
			onClick.Invoke(gameObject);
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			onDown.Invoke(gameObject);
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			onEnter.Invoke(gameObject);
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			onExit.Invoke(gameObject);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			onUp.Invoke(gameObject);
		}
	}
}

