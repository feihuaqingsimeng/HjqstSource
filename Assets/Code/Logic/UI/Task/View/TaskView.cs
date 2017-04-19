using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Task.Model;
using System.Collections.Generic;
using Logic.Enums;
using Logic.UI.Task.Model;
using Common.Util;
using Logic.UI.Task.Controller;
using Common.Localization;
using Common.UI.Components;
using LuaInterface;


namespace Logic.UI.Task.View
{
	public class TaskView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/task/task_view";
		public static TaskView Open()
		{
			TaskView view = UIMgr.instance.Open<TaskView>(PREFAB_PATH);
			return view;
		}

		#region ui component
		//public Text textTitle;

		public Toggle togglePrefab;
		public Transform toggleRoot;
		public ScrollContentExpand scrollContent;
		public Transform panel;
		#endregion
		private Toggle[] toggles;
		private Toggle _currentToggle;

		private bool isFirstEnter = true;

		void Awake()
		{


			BindDelegate();
		}
		void Start()
		{
			Init();
			LTDescr ltDescr = LeanTween.delayedCall(0.6f, OnViewReady);
		}
		void OnDestroy()
		{
			UnbindDelegate();
		}

		private void OnViewReady ()
		{
			LuaTable audio_model = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","audio_model")[0];
			audio_model.GetLuaFunction("PlayRandomAudioInView").Call((int)AudioViewType.taskView,0);
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}
		private void OnScrollReady ()
		{
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnScrollReady"));
		}
		private void BindDelegate()
		{
			TaskProxy.instance.onUpdateUIRereshByProtocolDelegate += Refresh;
			TaskProxy.instance.onUpdateGetTaskRewardByProtocolDelegate += onUpdateGainRewardByProtocol;
		}
		private void UnbindDelegate()
		{
			TaskProxy.instance.onUpdateUIRereshByProtocolDelegate -= Refresh;
			TaskProxy.instance.onUpdateGetTaskRewardByProtocolDelegate -= onUpdateGainRewardByProtocol;
		}
		private void Init()
		{
			InitText();
			CommonTopBar.View.CommonTopBarView view = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(panel);
			view.SetAsCommonStyle(Localization.Get("ui_task_view.task_title"),OnClickCloseBtnHandler,true,true,true,false);
			InitToggles();
		//	Refresh(true);

		}

		private void InitText()
		{


		}
		private void InitToggles()
		{
			TransformUtil.ClearChildren(toggleRoot,true);
			togglePrefab.gameObject.SetActive(true);
			int end = (int)TaskBigType.Task_achievement;
			int start = (int)TaskBigType.Task_Max-1;
			toggles = new Toggle[start-end+1];
			int index = 0;
			for(int i = start;i >= end;i--)
			{
				if (i ==(int)TaskBigType.Task_daliy)
					continue;
				Toggle toggle = Instantiate<Toggle>(togglePrefab);
				toggle.transform.SetParent(toggleRoot,false);

				toggle.GetComponent<ToggleContent>().Set(i,Localization.Get("ui_task_view.task_toggle"+i.ToString()));

				Logic.UI.RedPoint.View.RedPointView redPointView = toggle.GetComponentInChildren<Logic.UI.RedPoint.View.RedPointView>();
				if(i == (int)TaskBigType.Task_achievement)
					redPointView.type = RedPointType.RedPoint_TaskAchievement;
				else if(i == (int)TaskBigType.Task_daliy)
					redPointView.type = RedPointType.RedPoint_TaskDaily;
				else if(i == (int)TaskBigType.Task_main)
					redPointView.type = RedPointType.RedPoint_TaskMain;
				toggles[i-1] = toggle;
				toggle.gameObject.name = index.ToString();
				index++;

			}
			toggles[start-1].isOn = true;

			togglePrefab.gameObject.SetActive(false);

		}

		private void Refresh()
		{
			Refresh(false);
		}
		private void Refresh(bool playInitAni)
		{
			List<TaskInfo> taskList = TaskProxy.instance.GetTaskListByType();


			if(!isFirstEnter)
			{
//				scrollContent.RefreshCount(taskList.Count,TaskProxy.instance.completeTaskInfoIndexInCurrentTaskList);
				scrollContent.Init(taskList.Count);
			}else
			{
				scrollContent.Init(taskList.Count,playInitAni,0.05f);
			}
			isFirstEnter = false;
		}

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
				TaskProxy.instance.currentTaskBigType = toggle.GetComponent<ToggleContent>().id;
				Refresh(true);
				LeanTween.delayedCall(0.8f, OnScrollReady);
			}
		}
		public void OnResetScrollItemHandler(GameObject go,int index)
		{
			TaskButton taskButton = go.GetComponent<TaskButton>();

			if(taskButton!= null)
			{
				TaskInfo info = TaskProxy.instance.currentTaskInfoList[index];
				taskButton.SetTaskInfo(info);
				go.name = info.id.ToString();
			}
		}
		#region server
		private void onUpdateGainRewardByProtocol()
		{
			//Logic.UI.Tips.View.CommonRewardAutoDestroyTipsView.Open(TaskProxy.instance.completeTaskInfo.taskData.rewardList);
			LuaTable tips_model = LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","tips_model")[0] as LuaTable;
			LuaTable tip_view =tips_model.GetLuaFunction("GetTipView").Call("common_reward_tips_view")[0] as LuaTable;
			tip_view.GetLuaFunction("CreateByCSharpGameResDataList").Call(TaskProxy.instance.completeTaskInfo.taskData.rewardList);
		}
		#endregion
	}
}

