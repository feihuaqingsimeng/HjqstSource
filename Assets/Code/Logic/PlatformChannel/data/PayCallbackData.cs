using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;



	public class PayCallbackData {

		private static Dictionary<int, PayCallbackData> _serverDataDictionary;
		
		public static Dictionary<int, PayCallbackData> GetPayCallbackData()
		{
			if (_serverDataDictionary == null)
			{
				_serverDataDictionary = CSVUtil.Parse<int, PayCallbackData>("config/csv/paycallback", "id");
			}
			return _serverDataDictionary;
		}
		
		public static Dictionary<int, PayCallbackData> PayCallbackDataDictionary
		{
			get
			{
				if (_serverDataDictionary == null)
				{
					GetPayCallbackData();
				}
				return _serverDataDictionary;
			}
		}
		public static PayCallbackData GetPayCallbackDataByID (int id)
		{
			PayCallbackData data = null;
			if (PayCallbackDataDictionary.ContainsKey(id))
			{
				data = PayCallbackDataDictionary[id];
			}
			return data;
		}
		[CSVElement("id")]
		public int sever_id;
		
		[CSVElement("callback")]
		public string callback;

	}

