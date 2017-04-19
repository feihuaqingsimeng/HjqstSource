using UnityEngine;
using System.Collections;
using Logic.Game.Model;
using Logic.Protocol.Model;
using System.Collections.Generic;
using Logic.Enums;
using Common.Localization;
namespace Logic.Task.Model
{
	public class TaskConditionInfo
	{
		public int id;
		public int value;
		public int taskDataId;
		public int maxCount
		{
			get
			{
				int max = 0;
				TaskConditionData data = TaskConditionData.GetTaskConditionData(id);
				if(data == null)
				{
					Debugger.LogError("task condition data is not found,id:"+id);
					return max;
				}
//				switch((int)data.taskType)
//				{
//				case 1:
//				case 5:
//				case 6:
//				case 7:
//				case 10:
//				case 11:
//				case 12:
//				case 13:
//				case 17:
//				case 20:
//				case 21:
//				case 23:
//				case 24:
//				case 25:
//				case 26:
//				case 28:
//				case 29:
//				case 30:
//				case 31:
//				case 50:
//					max = data.parameter1.ToInt32();
//					break;
//				case 2:
//				case 4:
//				case 14:
//				case 15:
//				case 16:
//				case 18:
//				case 19:
//				case 22:
//					max = data.parameter2.ToInt32();
//					break;
//				}
				TaskData taskData = TaskData.GetTaskDataByID(taskDataId);
				if(taskData!= null)
				{
					if(taskData.count == 0)
						max = 1;
					else if(taskData.count == 1)
						max = data.parameter1.ToInt32();
					else if(taskData.count == 2)
						max = data.parameter2.ToInt32();
				}

                return max;
                
            }
		}
		public int secondParam
		{
			get
			{
				int max = 0;
				TaskConditionData data = TaskConditionData.GetTaskConditionData(id);
				if(data == null)
				{
					Debugger.LogError("task condition data is not found,id:"+id);
					return max;
				}
				switch((int)data.taskType)
				{
				case 1:
				case 5:
				case 6:
				case 7:
				case 10:
				case 11:
				case 12:
				case 13:
				case 17:
				case 20:
				case 21:
				case 23:
				case 24:
				case 25:
				case 26:
				case 28:
				case 29:
				case 30:
				case 31:
				case 50:
					max = data.parameter2.ToInt32();
					break;
				default:
					max = data.parameter1.ToInt32();
					break;
				}
				return max;
				
			}
		}
	}
	public class TaskInfo  
	{
		public TaskInfo()
		{

		}
		public TaskInfo(TaskProtoData data)
		{
			UpdateTaskInfo(data);
		}

		public int id;
		public TaskData taskData;

		public bool isFinished;// 是否已完成
		public bool isGetReward;// 奖励是否已领取
		List<TaskConditionInfo> taskConditionDataList;

		public TaskConditionInfo GetFirstCondition()
		{
			return taskConditionDataList.First();
		}
		public void UpdateTaskInfo(TaskProtoData data)
		{
			id = data.id;
			taskData = TaskData.GetTaskDataByID(id);
			if(taskData == null)
			{
				Debugger.LogError("taskData is null ,task ID :"+id);
			}

			isFinished = data.completed;
			isGetReward = data.getReward;
			taskConditionDataList = new List<TaskConditionInfo>();
			
			List<TaskConditionProtoData> dataList = data.conditions;
			TaskConditionProtoData conditionProtoData;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				conditionProtoData = dataList[i];
				TaskConditionInfo taskConditionData = new TaskConditionInfo();
				taskConditionData.id = conditionProtoData.id;
				taskConditionData.taskDataId = data.id;
				taskConditionData.value = conditionProtoData.value < 0 ? 0 : conditionProtoData.value;
                taskConditionDataList.Add(taskConditionData);
            }
		}
		public override string ToString ()
		{
			TaskConditionInfo condition = GetFirstCondition();
			string value = "null";
			string maxCount = "null";
			if(condition != null)
			{
				value = condition.value.ToString();
				maxCount = condition.maxCount.ToString();
			}
			return string.Format ("[任务]id:{0},title:{1},isFinished:{2},isGetReward:{3},condition[value:{4},MaxCount:{5},task:{6}]",id, Localization.Get( taskData.description),isFinished,isGetReward,value,maxCount,taskData.task);
		}
	}
}

