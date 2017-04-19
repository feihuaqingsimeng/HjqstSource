using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.Task.Model
{
	public class TaskData
	{
		private static Dictionary<int, TaskData> _taskDataDictionary;
		public static Dictionary<int, TaskData> GetTaskDataDicionary ()
		{
			if (_taskDataDictionary == null)
			{
				_taskDataDictionary = CSVUtil.Parse<int, TaskData>("config/csv/task", "id");
			}
			return _taskDataDictionary;
		}
		public static Dictionary<int, TaskData> TaskDataDictionary
		{
			get
			{
				return GetTaskDataDicionary();
			}
		}

		public static TaskData GetTaskDataByID(int id)
		{
			if(TaskDataDictionary!= null && TaskDataDictionary.ContainsKey(id))
			{
				return TaskDataDictionary[id];
			}
			return null;
		}
		public bool isFirstTaskOpen()
		{
			if(pre_task == 0)
				return true;
			return false;
		}
		public string GetTaskIconPath()
		{
			return  Common.ResMgr.ResPath.GetTaskIconPath(pic);
		}
		[CSVElement("id")]
		public int id;

		[CSVElement("task")]
		public int task;//每日任务、成就

		[CSVElement("type")]
		public int type;

		[CSVElement("title")]
		public string title;

		[CSVElement("des")]
		public string description;

		[CSVElement("count")]
		public int count;

		[CSVElement("transfer")]
		public int transfer;

		public List<GameResData> rewardList = new List<GameResData>();
		[CSVElement("reward")]
		public string rewardString
		{
			set
			{
				string[] rewards = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = rewards.Length;i<count;i++)
				{
					rewardList.Add(new GameResData(rewards[i]));
				}
			}
		}
		[CSVElement("pre_task")]
		public int pre_task;

		[CSVElement("lv_limit")]
		public string lv_limit;
		[CSVElement("pic")]
		public string pic ;
		
	}
}
