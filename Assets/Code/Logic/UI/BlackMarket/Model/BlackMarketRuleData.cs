using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.UI.BlackMarket.Model
{
	public class BlackMarketRuleData  
	{
		private static Dictionary<int, BlackMarketRuleData> _blackmarketRuleDataDictionary;
		
		public static Dictionary<int, BlackMarketRuleData> GetBlackMarketRuleDatas()
		{
			if (_blackmarketRuleDataDictionary == null)
			{
				_blackmarketRuleDataDictionary = CSVUtil.Parse<int, BlackMarketRuleData>("config/csv/market_rule", "id");
			}
			return _blackmarketRuleDataDictionary;
		}
		
		public static Dictionary<int, BlackMarketRuleData> BlackMarketDataRuleDictionary
		{
			get
			{
				if (_blackmarketRuleDataDictionary == null)
				{
					GetBlackMarketRuleDatas();
				}
				return _blackmarketRuleDataDictionary;
			}
		}
		public static BlackMarketRuleData GetBlackMarketRuleDataById(int id)
		{
			if(BlackMarketDataRuleDictionary.ContainsKey(id))
			{
				return BlackMarketDataRuleDictionary[id];
			}
			return null;
		}

		public GameResData itemData;
		public List<GameResData> materials = new List<GameResData>();

		[CSVElement("id")]
		public int id;

		[CSVElement("rule_id")]
		public int rule_id;

		[CSVElement("item")]
		public string itemString
		{
			set
			{
				itemData = new GameResData(value);

			}
		}

		[CSVElement("cost1")]
		public string cost1
		{
			set
			{
				if(!value.Equals("-1"))
					materials.Add(new GameResData(value));

			}
		}

		[CSVElement("cost2")]
		public string cost2
		{
			set
			{
				if(!value.Equals("-1"))
					materials.Add(new GameResData(value));
			}
		}


		[CSVElement("cost3")]
		public string cost3
		{
			set
			{
				if(!value.Equals("-1"))
					materials.Add(new GameResData(value));
			}
		}


		[CSVElement("cost4")]
		public string cost4
		{
			set
			{
				if(!value.Equals("-1"))
					materials.Add(new GameResData(value));
			}
		}


	}
}

