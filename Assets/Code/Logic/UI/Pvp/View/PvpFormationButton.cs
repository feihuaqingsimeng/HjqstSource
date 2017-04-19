using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Expedition.Model;
using Common.Localization;
using Logic.Role.Model;
using Logic.Game.Model;

namespace Logic.UI.Pvp.View
{
	public class PvpFormationButton : MonoBehaviour 
	{
		private  RoleInfo _roleInfo;
		public RoleInfo RoleInfo
		{
			get
			{
				return _roleInfo;
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
		public delegate void OnClickHandler(PvpFormationButton btn);
		public OnClickHandler onClickHandler;

		#region ui component
		public GameObject SelectMarkGO;
		public GameObject inTeamMarkGO;
		public Text textPlayerTitle;
		#endregion

		private CommonHeroIcon.View.CommonHeroIcon _commonHeroIcon;

		void Awake()
		{
			if(textPlayerTitle!=null)
				textPlayerTitle.text = Localization.Get("ui.expedition_formation_view.playerTitle");
			SetSelect(false);
			SetInTeam(false);
			HidePlayerTitle();
		}

		public void SetRoleInfo(RoleInfo info,bool isSmall = false)
		{

			_roleInfo = info;

			if(_commonHeroIcon == null)
			{
				if(isSmall)
					_commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.CreateSmallIcon(transform);
				else
					_commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.Create(transform);
				_commonHeroIcon.transform.SetAsFirstSibling();
				_commonHeroIcon.SetButtonEnable(false);
			}
			_commonHeroIcon.SetRoleInfo(_roleInfo);

			if(_roleInfo.instanceID == GameProxy.instance.PlayerInfo.instanceID)
			{
				//_commonHeroIcon.HideStar();
				ShowPlayerTitle();
			}else{
				//_commonHeroIcon.ShowStar();
				HidePlayerTitle();
			}
		}

		public void Refresh (bool isInTeam, bool isSelected)
		{
			inTeamMarkGO.SetActive(isInTeam);
			SelectMarkGO.SetActive(isSelected);
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
			if(SelectMarkGO != null)
				SelectMarkGO.SetActive(select);
		}
		public void HideSelect()
		{
			if(SelectMarkGO != null)
				SelectMarkGO.SetActive(false);
		}
		public void ShowSelect()
		{
			if(SelectMarkGO != null)
				SelectMarkGO.SetActive(true);
		}
		public void SetInTeam(bool inTeam)
		{
			if(inTeamMarkGO!= null)
				inTeamMarkGO.SetActive(inTeam);
		}
		public void HidePlayerTitle()
		{
			if(textPlayerTitle!=null)
				textPlayerTitle.gameObject.SetActive(false);
		}
		public void ShowPlayerTitle()
		{
			if(textPlayerTitle!=null)
				textPlayerTitle.gameObject.SetActive(true);
		}
		#region UI event handlers
		public void ClickHandler (PvpFormationButton btn)
		{
			if (onClickHandler != null)
			{
				onClickHandler(btn);
			}
		}
		#endregion UI event handlers
	}
}

