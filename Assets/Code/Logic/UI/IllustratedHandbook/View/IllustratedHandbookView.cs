using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.UI.IllustratedHandbook.Model;
using Common.Util;
using Logic.UI.IllustratedHandbook.Controller;
using Logic.UI.CommonTopBar.View;
using Common.Localization;
using LuaInterface;

namespace Logic.UI.IllustratedHandbook.View
{
	public class IllustratedHandbookView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/illustrated_handbook/illustrated_handbook_view";
		//是否使用上次的页签和scroll的位置
		public static IllustratedHandbookView Open(bool useSaveState = false)
		{
			//IllustratedHandbookView view = UIMgr.instance.Open<IllustratedHandbookView>(PREFAB_PATH);
			//view.SetState(useSaveState);
			//return view;
			LuaTable illustration_ctrl = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","illustration_ctrl")[0];
			illustration_ctrl.GetLuaFunction("OpenIllustrationView").Call(useSaveState);
			return null;
		}

		#region ui component
		public Text textCount;
		public ScrollContentExpand scrollContent;
		public Scrollbar scrollBar;
		public Transform toggleRoot;
		public Toggle togglePrefab;

		public Transform panel;
		public Dropdown dropdownSelect;
		#endregion

		private Toggle _currentToggle;
		private bool useSaveState = false;
		private bool _isfirstEnter = true;
		void Awake()
		{
			CommonTopBarView view = CommonTopBarView.CreateNewAndAttachTo(panel);
			view.SetAsCommonStyle(Localization.Get("ui.illustration_view.illustation"),OnClickCloseBtnHandler,true,true,true,false);
		}

		public void SetState(bool state)
		{
			useSaveState = state;
			init();

		}
		private void init()
		{
			IllustratedHandbookProxy.instance.filterIndex = 0;
			IllustratedHandbookProxy.instance.Clear();
			IllustratedHandbookProxy.instance.InitDictionary();
			InitToggles();
			BindDelegate();
			//if(IllustratedHandbookProxy.instance.IllustrationDictionary.Count == 0)
				IllustrationController.instance.CLIENT2LOBBY_Illustration_REQ();
			//else
			//	RefreshAll();

			List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
			for(int i = 0;i<6;i++)
			{
				optionList.Add(new Dropdown.OptionData(Localization.Get("ui.illustration_view.show"+i.ToString())));
			}
			dropdownSelect.options = optionList;
		}
		private void OnDestroy()
		{

			UnbindDelegate();
			IllustratedHandbookProxy.instance.selectToggleId = _currentToggle.GetComponent<ToggleContent>().id;
			IllustratedHandbookProxy.instance.scrollPercent = scrollBar.value;
		}
		private void BindDelegate()
		{
			IllustratedHandbookProxy.instance.UpdateIllustrationDelegate += Refresh;
			IllustratedHandbookProxy.instance.InitIllustrationDelegate += RefreshAll;
		}
		private void UnbindDelegate()
		{
			IllustratedHandbookProxy.instance.UpdateIllustrationDelegate -= Refresh;
			IllustratedHandbookProxy.instance.InitIllustrationDelegate -= RefreshAll;
		}
		private void InitToggles()
		{
			TransformUtil.ClearChildren(toggleRoot,true);
			togglePrefab.gameObject.SetActive(true);
			Toggle toggle;
			Dictionary<int ,string> titleDic = IllustratedHandbookProxy.instance.bigTitleStringDictionary;
			int index = 0;
			foreach(var value in titleDic)
			{
				toggle = Instantiate<Toggle>(togglePrefab);
				toggle.transform.SetParent(toggleRoot,false);
				toggle.GetComponent<ToggleContent>().Set(value.Key,value.Value);

				if (useSaveState)
				{
					bool isOn = value.Key == IllustratedHandbookProxy.instance.selectToggleId;
					if(isOn)
						_currentToggle = toggle;
					toggle.isOn =  isOn;
				}else
				{
					if(index == 0)
						_currentToggle = toggle;
					toggle.isOn = index == 0 ? true : false;
				}


				index ++;

			}
			togglePrefab.gameObject.SetActive(false);
		}
		private void Refresh()
		{
			int count = IllustratedHandbookProxy.instance.currentSelectRoleList.Count;

			if (useSaveState)
			{
				scrollContent.Init(count,false,0);
				scrollContent.ScrollToPosition(IllustratedHandbookProxy.instance.scrollPercent);
			}else
			{
				scrollContent.Init(count,_isfirstEnter,0.2f);
			}
			_isfirstEnter = false;
			useSaveState = false;
			UpdateCountText();
		}
		private void RefreshAll()
		{
			int id = 0;
			if (useSaveState)
			{
				id = IllustratedHandbookProxy.instance.selectToggleId;
			}else
			{
				id = _currentToggle.GetComponent<ToggleContent>().id;
				
			}

			IllustratedHandbookProxy.instance.UpdateSelectToggleRoleDic(id);
			Refresh();
		}
		private void UpdateCountText()
		{
			List<IllustrationInfo > roleList ;
			int totalCount = 0;
			int hasCount = 0;
			RoleInfo info;

			for(int i = 0,count = IllustratedHandbookProxy.instance.currentSelectRoleList.Count;i<count;i++)
			{
				roleList = IllustratedHandbookProxy.instance.currentSelectRoleList[i];
				int count2 = roleList.Count;
				totalCount += count2;
				for(int j = 0;j<count2;j++)
				{
					info = roleList[j].roleInfo;
					if(IllustratedHandbookProxy.instance.isHeroGotInIllustration(info.modelDataId,info.advanceLevel))
					{
						hasCount ++;
					}
				}
			}
			textCount.text = string.Format("{0}/{1}",hasCount,totalCount);
		}
		#region ui event handler
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnClickToggleBtnHandler(Toggle toggle)
		{
			if(_currentToggle == toggle)
				return;
			if(toggle.isOn)
			{
				_currentToggle = toggle;
				int id = toggle.GetComponent<ToggleContent>().id;
				IllustratedHandbookProxy.instance.UpdateSelectToggleRoleDic(id);
				Refresh();
			}
		}
		public void ClickDropDownSort(){
			
			int type = dropdownSelect.value;
			if(type != IllustratedHandbookProxy.instance.filterIndex)
			{
				IllustratedHandbookProxy.instance.filterIndex = type;
				RefreshAll();
			}
		}
		public void OnResetItemHandler(GameObject go,int index)
		{
			IllustratedContentView view = go.GetComponent<IllustratedContentView>();
			List<IllustrationInfo> illustrationList = IllustratedHandbookProxy.instance.currentSelectRoleList[index];
			int titleId = IllustratedHandbookProxy.instance.currentSelectTitleList[index];
			view.SetData(IllustratedHandbookProxy.instance.smallTitleStringDictionary[titleId],illustrationList,illustrationList.Count != 0);
		}

		#endregion
	}
}

