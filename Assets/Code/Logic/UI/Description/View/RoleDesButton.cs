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
using Logic.Audio.Controller;


namespace Logic.UI.Description.View
{
	public class RoleDesButton : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
	{
		public static RoleDesButton Get(GameObject go)
		{
			RoleDesButton btn = go.GetComponent<RoleDesButton>();
			if(btn == null)
				btn = go.AddComponent<RoleDesButton>();
			return btn;
		}

		private CommonRoleDesView _roleView;
		
		private RoleInfo _roleInfo;
		private float _showDelay;
		private ShowDescriptionType _type;
		public void SetRoleInfo(GameResData data,ShowDescriptionType type = ShowDescriptionType.longPress,float showDelay = 0.15f)
		{
			HeroInfo info = new HeroInfo(0,data.id,1,0,data.star);
			SetRoleInfo(info,type,showDelay);
		}

		public void SetRoleInfo(RoleInfo roleInfo ,ShowDescriptionType type = ShowDescriptionType.longPress,float showDelay = 0.15f)
		{
			_roleInfo = roleInfo;
			_showDelay = showDelay;
			_type = type;
		}
		public void SetType(ShowDescriptionType type)
		{
			_type = type;
		}
		void Awake()
		{

		}
		
		public void OnPointerDown(PointerEventData eventData)
		{
			if(_type != ShowDescriptionType.click)
			{
				if(_roleInfo == null)
					return;
				StartCoroutine(showTipsCoroutine(_showDelay));
                Debugger.Log("OnPointerDown");
			}
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			if(_type != ShowDescriptionType.click)
			{
				StopAllCoroutines();
				if(_roleView!= null)
					_roleView.Close();
				_roleView = null;
                //Debugger.Log("OnPointerUp");
			}
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if(_type == ShowDescriptionType.click)
			{
				StartCoroutine(showTipsCoroutine(0));
                //Debugger.Log("OnPointerClick");
				AudioController.instance.PlayAudio(AudioController.CLICK,false);
			}
		}

		private IEnumerator showTipsCoroutine(float delay)
		{

			yield return new WaitForSeconds(delay);
			if(_roleInfo != null)
			{
				RectTransform rectTran = transform as RectTransform;
				Vector2 sizeDelta = new Vector2(100,100);
				if(rectTran != null)
					sizeDelta = (transform as RectTransform).sizeDelta;
				_roleView = CommonRoleDesView.Open(_roleInfo,transform.position,sizeDelta);
			}

		}
	}
}

