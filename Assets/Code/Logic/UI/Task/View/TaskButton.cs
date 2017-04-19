using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Task.Model;
using Common.Localization;
using Logic.Game.Model;
using Logic.Enums;
using Common.ResMgr;
using Logic.UI.Task.Model;
using Logic.UI.Task.Controller;
using Logic.FunctionOpen.Model;
using Logic.Dungeon.Model;
using Logic.UI.Tips.View;
using Logic.Hero.Model;
using System.Collections.Generic;
using Logic.UI.CommonReward.View;
using Common.Util;

namespace Logic.UI.Task.View
{
	public class TaskButton : MonoBehaviour 
	{
		
		#region ui component
		public Image imgTaskTitle;
		public Image[] imgRewardIcon;
		public Text[] textRewardCount;
		public Transform progressTran;
		public Text textTitle;
		public Text textContent;
		public Button btnSkip;
		
		public Text textProgress;
		public Text textGetRewardBtn;
		public GameObject completeRoot;
		public GameObject progressRoot;
		public GameObject gotRoot;

		#endregion

		private TaskInfo _taskInfo;

		public TaskInfo taskInfo
		{
			get
			{
				return _taskInfo;
			}
		}

		public void SetTaskInfo(TaskInfo info)
		{
			_taskInfo = info;
			_isClickComplete = false;
			Refresh();
		}
		private void Refresh()
		{
			textGetRewardBtn.text = Localization.Get("ui_task_view.get_reward");
			imgTaskTitle.SetSprite(ResMgr.instance.Load<Sprite>(_taskInfo.taskData.GetTaskIconPath()));
			imgTaskTitle.SetNativeSize();
			RefreshTaskContent();
			RefreshReward();
			RefreshProgress();

			TaskConditionData data = TaskConditionData.GetTaskConditionData(taskInfo.taskData.id);
			if (data == null || data.goto_type == 0)
			{
				btnSkip.gameObject.SetActive(false);
			}else{
				btnSkip.gameObject.SetActive(true);
			}
		}
		private void RefreshTaskContent()
		{
			textTitle.text = Localization.Get(taskInfo.taskData.title);
			if(textContent!= null)
				textContent.text = Localization.Get(taskInfo.taskData.description);
		}
		private void RefreshReward()
		{
			for(int i = 0,tcount = imgRewardIcon.Length;i<tcount;i++)
			{
				Transform commonRoot = imgRewardIcon[i].transform.parent.Find("common_root");
				TransformUtil.ClearChildren(commonRoot,true);
				if (i >= taskInfo.taskData.rewardList.Count)
				{
					imgRewardIcon[i].gameObject.SetActive(false);
					textRewardCount[i].gameObject.SetActive(false);
					continue;
				}

				GameResData data =  taskInfo.taskData.rewardList[i];

				imgRewardIcon[i].gameObject.SetActive(true);
				textRewardCount[i].gameObject.SetActive(true);
				
				string path = "";
				bool isShowBox = false;
				Vector3 localScale = Vector3.one;
//				switch(data.type)
//				{
//				case BaseResType.Hero:         //伙伴
//				case BaseResType.Equipment:      //装备
//				case BaseResType.Item:          //道具
//				{
//					isShowBox = true;
//					CommonRewardIcon icon = CommonRewardIcon.Create(commonRoot);
//					icon.SetGameResData(data);
//					icon.HideCount();
//				}
//
//					break;
//				default:
//					path = UIUtil.GetBaseResIconPath(data.type);
//					break;
//				}

				isShowBox = true;
				CommonRewardIcon icon = CommonRewardIcon.Create(commonRoot);
				icon.SetGameResData(data);
				icon.HideCount();
				

				//imgRewardIcon[i].transform.localScale = localScale;
//				imgRewardIcon[i].gameObject.SetActive(!isShowBox);
//				int count = 0;
//				if(isShowBox)
//				{
//					//count = 1;
//				}
//				else
//				{
//					imgRewardIcon[i].SetSprite(ResMgr.instance.Load<Sprite>(path));
//					imgRewardIcon[i].SetNativeSize();
//					//count = data.count;
//				}
				textRewardCount[i].text = string.Format(Localization.Get("common.x_count"),data.count);
			}

		}
		private void RefreshProgress()
		{
			progressRoot.SetActive(false);
			completeRoot.SetActive(false);
			if (gotRoot != null)
				gotRoot.SetActive(false);
			if(taskInfo.isFinished)
			{
				if(taskInfo.isGetReward)
				{
					if (gotRoot != null)
						gotRoot.SetActive(true);
				}else{
					completeRoot.SetActive(true);
				}
			}else{
				TaskConditionInfo conditionData = taskInfo.GetFirstCondition();
				int maxCount = conditionData.maxCount;
				progressRoot.SetActive(true);
				int curCount = maxCount == 1 ? 0 : conditionData.value;
				float scale = curCount/(maxCount+0.0f);
				if (progressTran != null)
				{
					progressTran.localScale = new Vector3(scale,progressTran.localScale.y);
				}
				textProgress.text = string.Format(Localization.Get("common.value/max"),curCount,maxCount);
			}

		}
		private bool _isClickComplete = false;
		public void OnClickCompleteBtnHandler()
		{
			if(_isClickComplete)
				return;


			if(_taskInfo.isFinished)
			{
				TaskProxy.instance.completeTaskInfo = _taskInfo;
				TaskProxy.instance.completeTaskInfoIndexInCurrentTaskList = TaskProxy.instance.GetIndexInCurrentTaskList(_taskInfo.id);
				TaskController.instance.CLIENT2LOBBY_GET_TASK_REWARD_REQ(_taskInfo.id);

				//TaskProxy.instance.UpdateGetDailyTaskRewardByProtocol();

				_isClickComplete = true;
			}else{
				Debugger.LogError("task is not complete!!!");
			}
		}
		public void OnClickTaskIconBtnHandler()
		{
			TaskConditionData data = TaskConditionData.GetTaskConditionData(taskInfo.taskData.id);
			if (data == null)
			{
				Debugger.LogError("taskConditionData 找不到 id:{0}",taskInfo.GetFirstCondition().taskDataId);
				return;
			}
			if(data.goto_type == 0)
			
			{
				Debugger.Log("goto_type is 0");
				return;
			}
				
			FunctionOpenType type = (FunctionOpenType)data.goto_type;
			bool isOpen = FunctionOpenProxy.instance.IsFunctionOpen(type,true);
			if (isOpen)
			{
				FunctionOpenProxy.instance.OpenFunction(type,data.goto_num);
				if (data.goto_num == 0)
					return;
				if (type == FunctionOpenType.MainView_PVE ||type == FunctionOpenType.PVE_Normal ||type == FunctionOpenType.PVE_Hard)
				{
					DungeonInfo info = DungeonProxy.instance.GetDungeonInfo(data.goto_num);

					if(info!= null && !info.isLock)
						FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Detail_View,data.goto_num);
					else if(info == null)
						Debugger.LogError("副本不存在，id"+data.goto_num);
					else if(info.isLock)
					{
						CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.notUnlock"));
					}
				}
				if (type == FunctionOpenType.MainView_Hero)
				{
					List<HeroInfo> heroList = HeroProxy.instance.GetAllHeroInfoList();
					if (heroList.Count == 0){
                        return;
                    }
					int id = (int)heroList[0].instanceID;
					print((FunctionOpenType)data.goto_num);
					FunctionOpenProxy.instance.OpenLuaView((FunctionOpenType)data.goto_num,id);
				}
			}
//			TaskType type = (TaskType)_taskInfo.taskData.type;
//			switch(type)
//			{
//			case TaskType.PassDungeonTimes://通关某个副本
//			case TaskType.clearDungeon:			//通关某个副本
//			case TaskType.assignDungeonStar://指定副本星级
//			{
//				int id = taskInfo.GetFirstCondition().secondParam;
//				DungeonInfo info = DungeonProxy.instance.GetDungeonInfo(id);
//				if(info!= null && !info.isLock)
//					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Detail_View,info,true);
//				if(info == null)
//					Debugger.LogError("副本不存在，id"+id);
//				else if(info.isLock)
//				{
//					CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.notUnlock"));
//				}
//			}
//				break;
//			case TaskType.dailyDungeonFight://参与每日副本
//			{
//				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon,null,false,true);
//			}
//				break;
//			case TaskType.boostNewDungeonCount://推进新副本次数
//			//case TaskType.ProfessionIDAndLevel://职业等级 
//			{
//				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View);
//			}
//				break;
//			case TaskType.pvpFightCount:		//竞技场战斗次数
//			{
//				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Arena,null,false,true);
//			}
//				break;
//
//			}
		}

	}
}

