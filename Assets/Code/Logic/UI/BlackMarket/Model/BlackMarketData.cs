using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.UI.BlackMarket.Model
{
	public class BlackMarketData  
	{
		private static Dictionary<int, BlackMarketData> _blackmarketDataDictionary;
		
		public static Dictionary<int, BlackMarketData> GetBlackMarketDatas()
		{
			if (_blackmarketDataDictionary == null)
			{
				_blackmarketDataDictionary = CSVUtil.Parse<int, BlackMarketData>("config/csv/black_market", "id");
			}
			return _blackmarketDataDictionary;
		}
		
		public static Dictionary<int, BlackMarketData> BlackMarketDataDictionary
		{
			get
			{
				if (_blackmarketDataDictionary == null)
				{
					GetBlackMarketDatas();
				}
				return _blackmarketDataDictionary;
			}
		}
		public static BlackMarketData GetBlackMarketDataById(int id)
		{
			if(BlackMarketDataDictionary.ContainsKey(id))
			{
				return BlackMarketDataDictionary[id];
			}
			return null;
		}
		public static string GetBlackMarketTitleNameByType(int type)
		{
			foreach(var value in BlackMarketDataDictionary)
			{
				if (value.Value.item_type == type && !string.IsNullOrEmpty(value.Value.name) )
				{
					return value.Value.name;
				}
			}
			return "";
		}



		[CSVElement("id")]
		public int id;

		[CSVElement("refresh_type")]
		public int refresh_type;//1 24小时刷新 2 固定时间刷新

		[CSVElement("black_tab")]
		public string name;
		[CSVElement("item_type")]
		public int item_type;

		[CSVElement("limit_type")]
		public int limit_type;

		[CSVElement("limit_num")]
		public int limit_num;

		[CSVElement("limit_lv")]
		public int limit_lv;

		[CSVElement("on_sell")]
		public int on_sell;


	}
}

