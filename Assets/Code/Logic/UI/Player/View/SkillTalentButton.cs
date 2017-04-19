using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Player.Model;
using Logic.UI.Description.View;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Logic.Audio.Controller;

namespace Logic.UI.Player.View
{
	public class SkillTalentButton : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler
	{
		
		public PlayerSkillTalentInfo info;
		
		public Image skillIconImg;
		public Text levelText;
		public GameObject levelGO;
		public GameObject unactiveGO;
		public Image expProgressImg;
		public GameObject SelectGO;
		public UnityEvent OnClick;
		public void SetInfo(PlayerSkillTalentInfo info)
		{
			this.info = info;
			Refresh();
		}
		
		private void Refresh()
		{
			levelGO.SetActive(info.level != 0 );
			unactiveGO.SetActive(info.level == 0);
			levelText.text = string.Format("LV{0}",info.level);
			skillIconImg.SetSprite(info.skillIcon);
			expProgressImg.fillAmount = 1-(info.exp+0.0f)/info.talentData.exp_need;
            SelectGO.SetActive(info.IsCarry);
            
            //SkillDesButton btn = SkillDesButton.Get(gameObject);
            //btn.SetTalenSkillInfo(info,0.15f);
		}
		private bool _isClick;
		public void OnPointerDown(PointerEventData eventData)
		{
			_isClick = true;
			StartCoroutine(ClickCoroutine());
			
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			StopAllCoroutines();
			if(_isClick && OnClick!= null)
			{
				OnClick.Invoke();
				AudioController.instance.PlayAudio(AudioController.SELECT,false);
			}

        }
		private IEnumerator ClickCoroutine()
		{
			yield return new WaitForSeconds(0.15f);
			_isClick = false;
		}
    }
}

