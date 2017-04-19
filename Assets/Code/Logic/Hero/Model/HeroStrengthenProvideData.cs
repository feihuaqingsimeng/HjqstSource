using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Hero.Model{
	public class HeroStrengthenProvideData {
		
		private static Dictionary<int, HeroStrengthenProvideData> _heroDataDictionary;
		
		public static Dictionary<int, HeroStrengthenProvideData> GetStrengthProvideDatas()
		{
			if (_heroDataDictionary == null)
			{
				_heroDataDictionary = CSVUtil.Parse<int, HeroStrengthenProvideData>("config/csv/aggr", "star");
			}
			return _heroDataDictionary;
		}

		public static List<HeroStrengthenProvideData> GetProvideDataList(){
			return new List<HeroStrengthenProvideData>(HeroStrengthenProvideDataDictionary.Values);
		}

		public static Dictionary<int, HeroStrengthenProvideData> HeroStrengthenProvideDataDictionary
		{
			get
			{
				if (_heroDataDictionary == null)
				{
					GetStrengthProvideDatas();
				}
				return _heroDataDictionary;
			}
		}
		
		public static HeroStrengthenProvideData GetHeroStrengthenProvideDataByID(int star)
		{
			HeroStrengthenProvideData data = null;
			if (HeroStrengthenProvideDataDictionary.ContainsKey(star) && HeroStrengthenProvideDataDictionary[star] != null)
			{
				data = HeroStrengthenProvideDataDictionary[star];
			}
			return data;
		}
		
		public static HeroStrengthenProvideData GetHeroStrengthenProvideDataByExp(int expTotal){

			List<HeroStrengthenProvideData> dataList = GetProvideDataList();
			int exp = 0;

			int count = dataList.Count;
			HeroStrengthenProvideData data = null;
			for(int i = 0;i<count;i++){
				data = dataList[i];
				exp += data.exp_provide;
				if(expTotal<exp){
					break;
				}
			}
			return data;
		}
		
		[CSVElement("star")]
		public int star;
		[CSVElement("exp_provide")]
		public int exp_provide;
		[CSVElement("crit")]
		public int crit;

	}
}

