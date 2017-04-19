using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Drop.Model
{
	public class PrizeElementData 
	{
		
		private static List<PrizeElementData> _prizeElementDataList;
		
		
		public static List<PrizeElementData> GetPrizeElementDatas()
		{
			if (_prizeElementDataList == null)
			{
				_prizeElementDataList = CSVUtil.Parse<PrizeElementData>("config/csv/prize_element");
			}
			return _prizeElementDataList;
		}
		public static List<PrizeElementData> PrizeElementDataList
		{
			get
			{
				if (_prizeElementDataList == null)
				{
					GetPrizeElementDatas();
				}
				return _prizeElementDataList;
			}
		}
		public static List<PrizeElementData> GetPrizeElementDataByID(int id)
		{
			PrizeElementData data = null;
			List<PrizeElementData> tempList = new List<PrizeElementData>();
			List<PrizeElementData> dataList = PrizeElementDataList;

			bool isFind = false;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				if(data.prize_id == id)
				{
					isFind = true;
					tempList.Add(data);
				}else
				{
					if(isFind)
						break;
				}
			}

			return tempList;
		}

		[CSVElement("prize_id")]
		public int prize_id;

		[CSVElement("type")]
		public int type;

		[CSVElement("item_id")]
		public int item_id;

		[CSVElement("star")]
		public int star;

		[CSVElement("count_min")]
		public int count_min;
	}
}

