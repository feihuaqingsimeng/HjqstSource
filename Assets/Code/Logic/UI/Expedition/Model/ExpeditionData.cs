using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.UI.Expedition.Model
{
	public class ExpeditionData 
	{
		private static Dictionary<int, ExpeditionData> _expeditionDataDictionary;
		
		public static Dictionary<int, ExpeditionData> GetExpeditionDatas()
		{
			if (_expeditionDataDictionary == null)
			{
				_expeditionDataDictionary = CSVUtil.Parse<int, ExpeditionData>("config/csv/faraway_expedition", "id");
			}
			return _expeditionDataDictionary;
		}
		
		public static Dictionary<int, ExpeditionData> ExpeditionDataDictionary
		{
			get
			{
				if (_expeditionDataDictionary == null)
				{
					GetExpeditionDatas();
				}
				return _expeditionDataDictionary;
			}
		}
		public static ExpeditionData GetExpeditionDataByID(int id)
		{
			if(ExpeditionDataDictionary.ContainsKey(id))
				return ExpeditionDataDictionary[id];
			return null;
		}
		public static ExpeditionData GetNextExpeditionData(int curId)
		{
			List<int> keys = ExpeditionDataDictionary.GetKeys();
			ExpeditionData data = null;
			bool isNext = false;
			int id;
			for(int i = 0,count = keys.Count;i<count;i++)
			{
				id = keys[i];
				if(isNext)
				{
					data = ExpeditionDataDictionary[id];
					break;
				}

				if(curId == id)
				{
					isNext = true;
				}
			}
			return data;
		}
		[CSVElement("id")]
		public int id;
		//1 怪物  2 资源点  3 最后大点
		[CSVElement("type")]
		public int type;

		public Vector3 position;
		[CSVElement("position")]
		public string positionString
		{
			set
			{
				string[] vec = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				position = new Vector3(vec[0].ToFloat(),vec[1].ToFloat(),0);
			}
		}

		[CSVElement("chapter")]
		public int chapter;
		[CSVElement("prefab")]
		public string prefabPath;

		public List<GameResData> rewardList;
		[CSVElement("reward_show")]
		public string reward_show
		{
			set
			{
				rewardList = new List<GameResData>();
				string[] data = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = data.Length;i<count;i++)
				{
					rewardList.Add(new GameResData(data[i]));
				}
			}
		}
	}
}

