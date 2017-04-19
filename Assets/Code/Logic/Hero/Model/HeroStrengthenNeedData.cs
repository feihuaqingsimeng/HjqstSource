using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Hero.Model{
	public class HeroStrengthenNeedData  {
		
		private static Dictionary<int, HeroStrengthenNeedData> _heroDataDictionary;


		public static Dictionary<int, HeroStrengthenNeedData> GetStrengthenNeedDatas()
		{
			if (_heroDataDictionary == null)
			{
				_heroDataDictionary = CSVUtil.Parse<int, HeroStrengthenNeedData>("config/csv/aggr_need", "aggr_lv");
			}
			return _heroDataDictionary;
		}
		public static Dictionary<int, HeroStrengthenNeedData> HeroStrengthenNeedDataDictionary
		{
			get
			{
				if (_heroDataDictionary == null)
				{
					GetStrengthenNeedDatas();
				}
				return _heroDataDictionary;
			}
		}
		public static List<HeroStrengthenNeedData> GetNeedDataList(){
			return new List<HeroStrengthenNeedData>(HeroStrengthenNeedDataDictionary.Values);
		}

		public static HeroStrengthenNeedData GetHeroStrengthenNeedDataByID (int id)
		{
			HeroStrengthenNeedData heroStrengthenNeedData = null;
			HeroStrengthenNeedDataDictionary.TryGetValue(id, out heroStrengthenNeedData);
			return heroStrengthenNeedData;
		}

		public static HeroStrengthenNeedData GetHeroStrengthenNeedDataByLevel(int lv)
		{
			HeroStrengthenNeedData data = null;
			if (HeroStrengthenNeedDataDictionary.ContainsKey(lv) && HeroStrengthenNeedDataDictionary[lv] != null)
			{
				data = HeroStrengthenNeedDataDictionary[lv];
			}
			return data;
		}

		public static HeroStrengthenNeedData LastNeedData(){

			HeroStrengthenNeedData data = HeroStrengthenNeedDataDictionary.Last().Value;
			return data;
		}

		public static bool IsMaxLevel(int level){
			int lv = MaxLevel();
			if(level>=lv)
				return true;
			return false;

		}
		public static int MaxLevel(){
			return LastNeedData().aggr_lv+1;

		}
		public static HeroStrengthenNeedData GetHeroStrengthenNeedDataByExp(int expTotal){
			
			List<HeroStrengthenNeedData> dataList = GetNeedDataList();
			int exp = 0;
			
			int count = dataList.Count;
			HeroStrengthenNeedData data = null;
			for(int i = 0;i<count;i++){
				data = dataList[i];
				exp += data.exp_need;
				if(expTotal<exp){
					return data;
				}
			}
			return null;
		}
		public static int GetStrengthenTotalExp(int level){
			List<HeroStrengthenNeedData >  dataList = GetNeedDataList();
			HeroStrengthenNeedData data;
			int needTotal = 0;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				if(level<= data.aggr_lv)
					break;
				needTotal += data.exp_need;
				
			}
			return needTotal;
		}

		[CSVElement("aggr_lv")]
		public int aggr_lv;
		[CSVElement("exp_need")]
		public int exp_need;
		[CSVElement("gold_need")]
		public int gold_need;
		[CSVElement("aggr_value")]
		public int aggr_value;//强化属性系数

		public Logic.Enums.RoleStrengthenStage roleStrengthenStage;
		[CSVElement("colour")]
		public int color
		{
			set
			{
				roleStrengthenStage = (Logic.Enums.RoleStrengthenStage)value;
			}
		}

		[CSVElement("aggr_des")]
		public int strengthenAddShowValue;
	}
}

