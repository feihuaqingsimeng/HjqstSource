using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Task.Model;
using Common.Localization;
using System.Collections.Generic;
using Logic.UI.CommonAnimations;
using Logic.Enums;

namespace Logic.UI.Task.View
{
	public class TaskCompleteTipView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/task/task_complete_tip_view";
		public static TaskCompleteTipView Open(TaskInfo info)
		{

			TaskCompleteTipView view = UIMgr.instance.Open<TaskCompleteTipView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetTaskInfo(info);
			return view;
		}
		public static TaskCompleteTipView Open(List<TaskInfo> taskList)
		{
			
			TaskCompleteTipView view = UIMgr.instance.Open<TaskCompleteTipView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetTaskInfoList(taskList);
			return view;
		}
		public static void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#region ui component
		public Text textTitle;
		public Text textTaskName;
		public Text textTips;
		public GameObject root;
		#endregion

		private List<TaskInfo> _taskList = new List<TaskInfo>();
		private bool _isStartCoroutine = false;
		public void SetTaskInfo(TaskInfo info)
		{
			_taskList.Add(info);
			if(!_isStartCoroutine)
			{
				StartCoroutine(RefreshCoroutine());
				_isStartCoroutine = true;
			}

		}
		public void SetTaskInfoList(List<TaskInfo> taskList)
		{
			_taskList.AddRange(taskList);
			if(!_isStartCoroutine)
            {
				_isStartCoroutine = true;
				StartCoroutine(RefreshCoroutine());
			}
		}
		private IEnumerator RefreshCoroutine()
		{
			for(int i = 0;i<_taskList.Count;i++)
			{
				TaskInfo info = _taskList[i];
				Refresh(info);
				yield return new WaitForSeconds(2.5f);
				root.SetActive(false);
				yield return new WaitForSeconds(0.1f);
			}
			Destroy();
		}

		private void Refresh(TaskInfo info)
		{
			root.SetActive(true);
			CommonFadeToAnimation.Get(root).init(0,1,0.2f);
			if(info.taskData.task == (int)TaskBigType.Task_achievement)
			{
				textTitle.text =  Localization.Get("ui_task_view.task_achievement_complete");
				textTaskName.text = string.Format(Localization.Get("ui_task_view.task_congratulation_achieve"),Localization.Get( info.taskData.description));
			}else{
				textTitle.text =  Localization.Get("ui_task_view.task_complete");
				textTaskName.text = string.Format(Localization.Get("ui_task_view.task_congratulation"),Localization.Get( info.taskData.description));
			}

			textTips.text = Localization.Get("ui_task_view.task_tip");
		}
		private void Destroy()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

