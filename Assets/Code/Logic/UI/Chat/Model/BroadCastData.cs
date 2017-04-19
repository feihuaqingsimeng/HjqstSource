using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.UI.Chat.Model
{
	public class BroadCastData 
	{
		
		private static Dictionary<int, BroadCastData> _broadcastDataDictionary;
		
		public static Dictionary<int, BroadCastData> GetBroadcastDatas()
		{
			if (_broadcastDataDictionary == null)
			{
				_broadcastDataDictionary = CSVUtil.Parse<int, BroadCastData>("config/csv/broadcast", "id");
			}
			return _broadcastDataDictionary;
		}
		
		public static Dictionary<int, BroadCastData> BroadcastDataDictionary
		{
			get
			{
				if (_broadcastDataDictionary == null)
				{
					GetBroadcastDatas();
				}
				return _broadcastDataDictionary;
			}
		}
		public static BroadCastData GetBroadcastDataById(int id)
		{
			if(BroadcastDataDictionary.ContainsKey(id))
			{
				return BroadcastDataDictionary[id];
			}
			return null;
		}
		[CSVElement("id")]
		public int id;

		[CSVElement("des")]
		public string des;

		[CSVElement("repeat_times")]
		public int repeat_times;

		[CSVElement("repeat_every_time")]
		public int repeat_every_time;

	}
}

