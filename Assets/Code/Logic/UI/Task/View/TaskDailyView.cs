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
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using LuaInterface;


namespace Logic.UI.Task.View
{
	public class TaskDailyView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/task/task_daily_view";
		public static TaskDailyView Open()
		{
			TaskDailyView view = UIMgr.instance.Open<TaskDailyView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		#region ui component
		//public Text textTitle;
		public ScrollContentExpand scrollContent;
		public GameObject goFinishedAllTip;
		public Text textHeroCount;
		public Text textEquipCount;
		#endregion

		private bool isFirstEnter = true;

		void Awake()
		{
			BindDelegate();
		}
		void Start()
		{
			Refresh();
		}
		void OnDestroy()
		{
			UnbindDelegate();
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
		private void Refresh()
		{
			TaskProxy.instance.currentTaskBigType = (int)TaskBigType.Task_daliy;
			List<TaskInfo> taskList = TaskProxy.instance.GetTaskListByType();
			scrollContent.Init(taskList.Count,false,0.05f);
			isFirstEnter = false;
			goFinishedAllTip.SetActive( taskList.Count == 0);
			textHeroCount.text = string.Format("{0}/{1}",HeroProxy.instance.GetAllHeroCount(), GameProxy.instance.HeroCellNum);
			textEquipCount.text = string.Format("{0}/{1}",EquipmentProxy.instance.GetFreeEquipmentInfoList().Count,GameProxy.instance.EquipCellNum);
		}

		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
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

