using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.ConsumeTip.Model
{
	public class ConsumeTipData 
	{
		private static Dictionary<int, ConsumeTipData> _consumeTipDataDictionary;
		private static Dictionary<int, ConsumeTipData> GetConsumeTipDataDicionary ()
		{
			if (_consumeTipDataDictionary == null)
			{
				_consumeTipDataDictionary = CSVUtil.Parse<int, ConsumeTipData>("config/csv/consumer_tips", "function_id");
			}
			return _consumeTipDataDictionary;
		}
		public static Dictionary<int, ConsumeTipData> ConsumeTipDataDictionary
		{
			get
			{
				return GetConsumeTipDataDicionary();
			}
		}

		public static ConsumeTipData GetConsumeTipDataByType(ConsumeTipType type)
		{
			if(ConsumeTipDataDictionary.ContainsKey((int)type))
			{
				return ConsumeTipDataDictionary[(int)type];
			}
			return null;
		}
		[CSVElement("id")]
		public int id;


		public ConsumeTipType consumeTipType;
		[CSVElement("function_id")]
		public int function_id
		{
			set
			{
				consumeTipType = (ConsumeTipType)value;
			}
		}

		[CSVElement("des")]
		public string des;
	}
}

