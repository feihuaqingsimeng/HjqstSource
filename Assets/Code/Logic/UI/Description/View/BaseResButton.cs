using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.UI.Description.View;
using Logic.Hero.Model;
using Logic.Player.Model;
using Logic.Game.Model;
using Logic.Role.Model;
using Logic.Enums;


namespace Logic.UI.Description.View
{
	public class BaseResButton : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler
	{
		

		public static BaseResButton Get(GameObject go)
		{
			BaseResButton btn = go.GetComponent<BaseResButton>();
			if(btn == null)
				btn = go.AddComponent<BaseResButton>();
			return btn;
		}

		public BaseResType type;

		private CommonBaseResDesView _baseResView;

		private float _showDelay;

		public void SetBaseRes(BaseResType type,float delay)
		{
			this.type = type;
			_showDelay = delay;
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if(type == BaseResType.None)
				return;
			StartCoroutine(showTipsCoroutine());
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			StopAllCoroutines();
			if(_baseResView!= null)
				_baseResView.Close();
			_baseResView = null;
		}
		private IEnumerator showTipsCoroutine()
		{
			yield return new WaitForSeconds(_showDelay);
			RectTransform rectTran = transform as RectTransform;
			Vector2 sizeDelta = new Vector2(100,100);
			if(rectTran != null)
			sizeDelta = (transform as RectTransform).sizeDelta;
			_baseResView = CommonBaseResDesView.Open(type,transform.position,sizeDelta);
		}
	}
}

