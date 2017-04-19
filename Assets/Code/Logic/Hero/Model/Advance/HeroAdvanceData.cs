using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Logic.Game.Model;
namespace Logic.Hero.Model.Advance
{
	public class HeroAdvanceData
	{
		
		private static Dictionary<int, HeroAdvanceData> _heroAdvanceDataDictionary;
		
		public static Dictionary<int,HeroAdvanceData> GetHeroAdvanceDatas(){
			
			if(_heroAdvanceDataDictionary == null)
				_heroAdvanceDataDictionary = CSVUtil.Parse<int,HeroAdvanceData>("config/csv/advance","star");
			return _heroAdvanceDataDictionary;
		}
		public static Dictionary<int,HeroAdvanceData> HeroAdvanceDataDictionary
		{
			get{
				if(_heroAdvanceDataDictionary == null)
					GetHeroAdvanceDatas();
				return _heroAdvanceDataDictionary;
			}
		}
		public static HeroAdvanceData GetHeroAdvanceDataByStar(int star)
		{
			HeroAdvanceData data = null;
			if(HeroAdvanceDataDictionary.ContainsKey(star))
			{
				data = HeroAdvanceDataDictionary[star];
			}
			return data;
		}

		public List<GameResData> GetItemIdByHeroType(RoleType type)
		{
			List<GameResData> itemList = new List<GameResData>();
			string s = "";
			switch(type)
			{
			case RoleType.Defence :       //
				s = hero_type_1;
				break;
			case RoleType.Offence :       //
				s = hero_type_2;
				break;
			case RoleType.Mage:          //
				s = hero_type_3;
				break;
			case RoleType.Support:      //
				s = hero_type_4;
				break;
			case RoleType.Mighty:        //
				s = hero_type_5;
				break;
			}
			string[] itemString = s.Split(CSVUtil.SYMBOL_SEMICOLON);
			for(int i = 0,count = itemString.Length;i<count;i++)
			{
				int[] values = itemString[i].ToArray<int>(CSVUtil.SYMBOL_COLON);
				GameResData data = new GameResData(BaseResType.Item,values[0],values[1],0);
				itemList.Add(data);
			}

			return itemList;
		}
		public List<GameResData> GetBasicItemId()
		{
			List<GameResData> itemList = new List<GameResData>();
			string[] itemString = hero_type_all.Split(CSVUtil.SYMBOL_SEMICOLON);
			for(int i = 0,count = itemString.Length;i<count;i++)
			{
				int[] values = itemString[i].ToArray<int>(CSVUtil.SYMBOL_COLON);
				GameResData data = new GameResData(BaseResType.Item,values[0],values[1],0);
				itemList.Add(data);
			}
			
			return itemList;
		}

		[CSVElement("star")]
		public int star;

		[CSVElement("lv_limit")]
		public int lv_limit;

		[CSVElement("gold")]
		public int gold;

		[CSVElement("hero_type_1")]
		public string hero_type_1;

		[CSVElement("hero_type_2")]
		public string hero_type_2;

		[CSVElement("hero_type_3")]
		public string hero_type_3;

		[CSVElement("hero_type_4")]
		public string hero_type_4;

		[CSVElement("hero_type_5")]
		public string hero_type_5;

		[CSVElement("hero_type_all")]
		public string hero_type_all;

	}
}

