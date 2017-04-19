using UnityEngine;
using UnityEngine.UI;
using Logic.Formation.Model;
using Common.Util;
using Logic.Enums;
using Common.Localization;
using Logic.UI.TrainFormation.Model;
using Logic.UI.RedPoint.View;
using Common.ResMgr;

namespace Logic.UI.TrainFormation.View
{
	public class FormationButton : MonoBehaviour
	{
		private FormationInfo _formationInfo;
		public FormationInfo FormationInfo
		{
			get
			{
				return _formationInfo;
			}
		}

		#region UI componnents
		public GameObject panel;
		public GameObject levelRoot;
		public Text levelText;
		public Image bgImage;
		public GameObject selectGO;
		public GameObject lockGO;
		public GameObject[] posGO;
		public Button button;
		public RedPointView redPoint;
		#endregion
		private FormationState _state;
		private Sprite _normalBg;
		private Sprite _selectBg;
		void Awake()
		{
			_normalBg = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_team_1");
			_selectBg = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_team_2");
			SetSelect(false);
		}
		public void SetInfo (FormationInfo formationInfo,FormationState state)
		{
			_formationInfo = formationInfo;
			_state = state;
			Refresh();
		}
		public void Refresh()
		{
			if(levelText!= null)
				levelText.text = string.Format(Localization.Get("ui.train_formation_view.formation_level"),_formationInfo.level);
			
			bool value;
			for(int i = 0,count = _formationInfo.formationData.pos.Length;i<count;i++)
			{
				value = _formationInfo.formationData.pos[i];
				posGO[i].SetActive(value);
			}
			if(redPoint!=null)
				redPoint.Set(RedPointType.RedPoint_Formation_specific,_formationInfo.id);
			SetState(_state);
		}
		public void SetButtonEnable(bool value)
		{
			if(button!= null)
				button.enabled = value;
		}
		public void SetState(FormationState state)
		{
			_state = state;
			if(_state == FormationState.InUse)
			{
				bgImage.SetSprite(_selectBg);
			}else 
			{
				bgImage.SetSprite( _normalBg);
			}
			bool isLock = _state == FormationState.Locked;
			if(panel!= null)
				UIUtil.SetGray(panel,isLock);
			if(levelRoot != null)
				levelRoot.SetActive(!isLock);
			if(lockGO != null)
				lockGO.SetActive(isLock);
		}

		public void SetSelect(bool value)
		{
			if(selectGO != null)
				selectGO.SetActive(value);
			if(value)
			{
				TrainFormationProxy.instance.RemoveNewFormationTip(_formationInfo.id);
			}
		}
		public void ShowLevel(bool value)
		{
			if(levelRoot != null)
				levelRoot.SetActive(value);
		}
	}
}