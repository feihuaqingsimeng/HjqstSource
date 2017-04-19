using UnityEngine;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
using Logic.Game.Model;

namespace Logic.UI.Pvp.Model
{
	public class PvpArenaPrizeData  
	{
		private static Dictionary<int, PvpArenaPrizeData> _arenaPrizeDataDictionary;
		
		public static Dictionary<int, PvpArenaPrizeData> GetArenaPrizeDatas()
		{
			if (_arenaPrizeDataDictionary == null)
			{
				_arenaPrizeDataDictionary = CSVUtil.Parse<int, PvpArenaPrizeData>("config/csv/arena_prize", "id");
			}
			return _arenaPrizeDataDictionary;
		}
		
		public static Dictionary<int, PvpArenaPrizeData> arenaPrizeDataDictionary
		{
			get
			{
				if (_arenaPrizeDataDictionary == null)
				{
					GetArenaPrizeDatas();
				}
				return _arenaPrizeDataDictionary;
			}
		}
		public static PvpArenaPrizeData GetDataByID (int prizeID)
		{
			PvpArenaPrizeData data = null;
			if (arenaPrizeDataDictionary.ContainsKey(prizeID))
			{
				data = arenaPrizeDataDictionary[prizeID];
			}
			return data;
		}
		public static PvpArenaPrizeData GetDataByRank(int rankLevel)
		{
			PvpArenaPrizeData data ;

			List<PvpArenaPrizeData> dataList = arenaPrizeDataDictionary.GetValues();

			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				if(rankLevel >= data.beginInterval && data.endInterval == -1)
				{
					return data;
				}
				if(rankLevel >= data.beginInterval && rankLevel <= data.endInterval)
				{
					return data;
				}

			}
			return null;
		}
		
		[CSVElement("id")]
		public int id;

		[CSVElement("beginInterval")]
		public int beginInterval;

		[CSVElement("endInterval")]
		public int endInterval;

		public List<GameResData> bonusList = new List<GameResData>();
		[CSVElement("dailybonus")]
		public string dailybonus
		{
			set
			{
				string[] bonus = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = bonus.Length;i<count;i++)
				{
					bonusList.Add(new GameResData(bonus[i]));
				}

			}
		}
		
	}
}

