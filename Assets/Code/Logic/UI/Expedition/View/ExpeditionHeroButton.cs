using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Expedition.Model;
using Common.Localization;
namespace Logic.UI.Expedition.View
{
	public class ExpeditionHeroButton : MonoBehaviour 
	{
		private ExpeditionHeroInfo _expeditionHeroInfo;
		public ExpeditionHeroInfo ExpeditionHeroInfo
		{
			get
			{
				return _expeditionHeroInfo;
			}
		}
		private bool _isSelect;
		public bool IsSelect
		{
			get
			{
				return _isSelect;
			}
		}
		public delegate void OnClickHandler(ExpeditionHeroButton btn);
		public OnClickHandler onClickHandler;

		#region ui component
		public Slider hpBarSlider;
		public Text textPlayerTitle;
		public Text textDie;
		public GameObject selectGO;
		#endregion

		private CommonHeroIcon.View.CommonHeroIcon _commonHeroIcon;

		void Awake()
		{
			textPlayerTitle.text = Localization.Get("ui.expedition_formation_view.playerTitle");
			if(textDie!=null)
				textDie.text = Localization.Get("ui.expedition_formation_view.die");
			SetSelect(false);
			HidePlayerTitle();
		}

		public void SetExpeditionHeroInfo(ExpeditionHeroInfo info,bool isSmall = false)
		{
			bool needGrayRefresh = _expeditionHeroInfo == null ?false:_expeditionHeroInfo.IsDead;
			_expeditionHeroInfo = info;

			if(_commonHeroIcon == null)
			{
				if(isSmall)
					_commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.CreateSmallIcon(transform);
				else
					_commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.Create(transform);
				_commonHeroIcon.transform.SetAsFirstSibling();
				_commonHeroIcon.SetButtonEnable(false);
			}
			_commonHeroIcon.SetRoleInfo(_expeditionHeroInfo.roleInfo);

			hpBarSlider.value = _expeditionHeroInfo.hpRate;
			if(_expeditionHeroInfo.IsPlayer)
			{
				//_commonHeroIcon.HideStar();
				HidePlayerTitle();
			}else{
				//_commonHeroIcon.ShowStar();
				HidePlayerTitle();
			}
			if(textDie!=null)
				textDie.gameObject.SetActive(_expeditionHeroInfo.IsDead);
			if(needGrayRefresh ^ _expeditionHeroInfo.IsDead)
			{
				if(_expeditionHeroInfo.IsDead)
				{
					SetChildGray(_commonHeroIcon.transform,true);
				}else
				{

					SetChildGray(_commonHeroIcon.transform,false);
				}
			}

		}
		public void SetInFormation(bool inFormation)
		{
			_commonHeroIcon.SetInFormation(inFormation);
		}
		public void Refresh (bool isInTeam, bool isSelected)
		{

		}
		private void SetChildGray(Transform tran,bool gray)
		{
			Image img = tran.GetComponent<Image>();
			if(img!=null)
				img.SetGray(gray);
			int count = tran.childCount;
			Transform child;

			for(int i = 0;i<count;i++)
			{
				child = tran.GetChild(i);
				img = child.GetComponent<Image>();
				if(img!=null)
					img.SetGray(gray);
				Text text = child.GetComponent<Text>();
				if(text)
					text.SetGray(gray);
				SetChildGray(child,gray);
			}
		}
		public void SetButtonEnable(bool enable)
		{
			Button b = transform.GetComponent<Button>();
			if(b!= null)
				b.enabled = enable;
		}

		public void SetSelect(bool select)
		{
			_isSelect = select;
			selectGO.SetActive(select);
		}
		public void HideSelect()
		{

		}
		public void ShowSelect()
		{

		}
		public void HidePlayerTitle()
		{
			textPlayerTitle.gameObject.SetActive(false);
		}
		public void ShowPlayerTitle()
		{
			textPlayerTitle.gameObject.SetActive(true);
		}
		#region UI event handlers
		public void ClickHandler (ExpeditionHeroButton btn)
		{
			if (onClickHandler != null)
			{
				onClickHandler(btn);
			}
		}
		#endregion UI event handlers
	}
}

