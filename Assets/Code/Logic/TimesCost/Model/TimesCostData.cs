using Common.Util;
using System.Collections.Generic;

namespace Logic.TimesCost.Model
{
	public class TimesCostData
	{
		private static List<TimesCostData> _timesCostDataList;
		public static List<TimesCostData> TimeCostDataList
		{
			get
			{
				if (_timesCostDataList == null)
				{
					_timesCostDataList = CSVUtil.Parse<TimesCostData>("config/csv/timescost");
				}
				return _timesCostDataList;
			}
		}

		public static TimesCostData GetTimesCostData(int type, int times)
		{
			TimesCostData timesCostData = null;
			for (int i = 0, count = TimeCostDataList.Count; i < count; i++)
			{
				if (TimeCostDataList[i].type == type)
				{
					timesCostData = TimeCostDataList[i];
					if (TimeCostDataList[i].times == times)
					{
						break;
					}
				}
			}
			return timesCostData;
		}

		[CSVElement("type")]
		public int type;

		[CSVElement("times")]
		public int times;

		[CSVElement("cost")]
		public int cost;
	}
}
