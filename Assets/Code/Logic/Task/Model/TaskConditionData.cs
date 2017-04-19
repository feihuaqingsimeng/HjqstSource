using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.Task.Model
{
	public class TaskConditionData
	{
		private static Dictionary<int, TaskConditionData> _taskConditionDataDictionary;
		public static Dictionary<int, TaskConditionData> GetTaskConditionDataDictionary ()
		{
			if (_taskConditionDataDictionary == null)
			{
				_taskConditionDataDictionary = CSVUtil.Parse<int, TaskConditionData>("config/csv/task_condition", "id");
			}
			return _taskConditionDataDictionary;
		}

		public static TaskConditionData GetTaskConditionData (int id)
		{
			TaskConditionData taskConditionData = null;
			GetTaskConditionDataDictionary().TryGetValue(id, out taskConditionData);
			return taskConditionData;
		}

		[CSVElement("id")]
		public int id;

		public TaskType taskType;
		[CSVElement("type")]
		public int type
		{
			set
			{
				taskType = (TaskType)value;
			}
		}

		[CSVElement("parameter1")]
		public string parameter1;

		[CSVElement("parameter2")]
		public string parameter2;

		[CSVElement("desc")]
		public string description;

		[CSVElement("goto_type")]
		public int goto_type;

		[CSVElement("goto_num")]
		public int goto_num;
	}
}
