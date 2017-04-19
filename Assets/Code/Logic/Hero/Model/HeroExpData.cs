using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Hero.Model{
	public class HeroExpData  {
		
		private static Dictionary<int, HeroExpData> _heroExpDataDictionary;
		
		public static Dictionary<int,HeroExpData> GetHeroExpDatas(){
			
			if(_heroExpDataDictionary == null)
				_heroExpDataDictionary = CSVUtil.Parse<int,HeroExpData>("config/csv/exp_hero","lv");
			return _heroExpDataDictionary;
		}
		public static Dictionary<int,HeroExpData> HeroExpDataDictionary{
			get{
				if(_heroExpDataDictionary == null)
					GetHeroExpDatas();
				return _heroExpDataDictionary;
			}
		}

		public static HeroExpData GetPlayerExpDataByLv(int lv){
			HeroExpData data = null;
			if(HeroExpDataDictionary.ContainsKey(lv) && HeroExpDataDictionary[lv] != null)
				data = _heroExpDataDictionary[lv];
			return data;
		}

		public static HeroExpData GetHeroExpDataByLv(int lv){
			HeroExpData data = null;


			if(lv == 0){
				data = new HeroExpData();
				data.lv = data.exp = data.exp_total = 0;
				
			}else{
				if(HeroExpDataDictionary.ContainsKey(lv) && HeroExpDataDictionary[lv] != null)
					data = _heroExpDataDictionary[lv];
			}
			return data;
		}

		public static HeroExpData GetHeroExpData(int oldExp,int addExp = 0){
			
			int totalExp = oldExp+addExp;
			List<HeroExpData> dataList = GetHeroExpDataList();
			HeroExpData data = null;
			for(int i = 0,count = dataList.Count;i<count;i++){
				data = dataList[i];
				if(totalExp<data.exp_total){
					return data;
				}
			}
			return null;
		}

		public static List<HeroExpData> GetHeroExpDataList(){
			return new List<HeroExpData>(HeroExpDataDictionary.Values);
		}

		[CSVElement("lv")]
		public int lv;
		
		[CSVElement("exp")]
		public int exp;
		
		[CSVElement("exp_total")]
		public int exp_total;
	}
}

