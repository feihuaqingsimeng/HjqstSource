using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Logic.Dungeon.Model;

namespace Logic.Activity.Model
{
	public class ActivityData
	{
		private static Dictionary<int, ActivityData> _activityDataDictionary;

		public static Dictionary<int, ActivityData> ActivityDataDictionary
		{
			get
			{
				if (_activityDataDictionary == null)
				{
					_activityDataDictionary = CSVUtil.Parse<int, ActivityData>("config/csv/activity", "id");
				}
				return _activityDataDictionary;
			}
		}

		public static ActivityData GetActivityData (int activityDataID)
		{
			ActivityData activityData = null;
			ActivityDataDictionary.TryGetValue(activityDataID, out activityData);
			return activityData;
		}

		public static List<ActivityData> GetAllActivityData ()
		{
			return ActivityDataDictionary.GetValues();
		}

		private List<int> _dungeonIDList;
		public List<int> DungeonIDList
		{
			get
			{
				return _dungeonIDList;
			}
		}

		public int MinDungeonIDIndex
		{
			get
			{
				return 0;
			}
		}

		public int MaxDungeonIDIndex
		{
			get
			{
				return DungeonIDList.Count - 1;
			}
		}

		public int GetDungeonIDOfLevel (int level)
		{
			int dungeonID = -1;
			if (level >= 0 && level <= DungeonIDList.Count - 1)
			{
				dungeonID = DungeonIDList[level];
			}
			return dungeonID;
		}

		public DungeonData GetDungeonDataOfLevel (int level)
		{
			DungeonData dungeonData = null;
			int dungeonID = GetDungeonIDOfLevel(level);
			dungeonData = DungeonData.GetDungeonDataByID(dungeonID);
			return dungeonData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("name")]
		public string name;

		[CSVElement("des")]
		public string des;

		[CSVElement("pic")]
		public string pic;

		[CSVElement("type")]
		public int type;

		public List<int> openDayList;
		[CSVElement("day")]
		public string openDaysStr
		{
			set
			{
				openDayList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
			}
		}

		[CSVElement("reward")]
		public string reward;

		[CSVElement("item_cost")]
		public string item_cost;

		[CSVElement("lv_limit")]
		public int eanbleLevel;

		[CSVElement("level1")]
		public string levelIDString
		{
			set
			{
				_dungeonIDList = value.ToList<int>(';');
			}
		}

		[CSVElement("count")]
		public int count;

		public bool isEnabled = false;
		[CSVElement("open")]
		public int open
		{
			set
			{
				isEnabled = value > 0;
			}
		}
	}
}
