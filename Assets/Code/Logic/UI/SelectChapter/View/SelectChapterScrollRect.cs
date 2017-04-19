using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Logic.UI.SelectChapter.View
{
	public class SelectChapterScrollRect : ScrollRect
	{
		private Vector2 _beginDragPosition = Vector2.zero;
		public System.Action onEndDragDelegate;

		public System.Action onDragLeftDelegate;
		public System.Action onDragRightDelegate;

		public override void OnBeginDrag (PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			_beginDragPosition = eventData.position;
		}

		public override void OnDrag (PointerEventData eventData)
		{
		}

		public override void OnEndDrag (PointerEventData eventData) 
		{
			base.OnEndDrag(eventData);
//			if (onEndDragDelegate != null)
//			{
//				onEndDragDelegate();
//			}

			Vector2 dragPositionDelta = eventData.position - _beginDragPosition;
			if (dragPositionDelta.x <= -100)
			{
				if (onDragLeftDelegate != null)
				{
					onDragLeftDelegate();
				}
				Debug.Log("Drag Left");
			}
			else if (dragPositionDelta.x >= 100)
			{
				if (onDragRightDelegate != null)
				{
					onDragRightDelegate();
				}
				Debug.Log("Drag Right");
			}
			Debug.Log("Stopped dragging " + this.name + "!");
		}
	}
}