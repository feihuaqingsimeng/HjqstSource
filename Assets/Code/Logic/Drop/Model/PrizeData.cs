using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Drop.Model
{
	public class PrizeData 
	{
		
		private static Dictionary<int, PrizeData> _prizeDataDictionary;
		
		
		public static Dictionary<int, PrizeData> GetPrizeDatas()
		{
			if (_prizeDataDictionary == null)
			{
				_prizeDataDictionary = CSVUtil.Parse<int, PrizeData>("config/csv/prize", "id");
			}
			return _prizeDataDictionary;
		}
		public static Dictionary<int, PrizeData> PrizeDataDictionary
		{
			get
			{
				if (_prizeDataDictionary == null)
				{
					GetPrizeDatas();
				}
				return _prizeDataDictionary;
			}
		}
		public static PrizeData GetPrizeDataByID(int id)
		{
			PrizeData data = null;
			if (PrizeDataDictionary.ContainsKey(id) && PrizeDataDictionary[id] != null)
			{
				data = PrizeDataDictionary[id];
			}
			return data;
		}

		[CSVElement("id")]
		public int id;

		public int[] prize_element_id;
		[CSVElement("prize_id")]
		public string prize_id_string
		{
			set
			{
				prize_element_id =value.ToArray<int>(CSVUtil.SYMBOL_PIPE);
			}
		}
	}
}

